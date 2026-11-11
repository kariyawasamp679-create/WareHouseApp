using System;
using System.Data;
using System.Data.SqlClient;
using WareHouseApp.Exceptions;
using WareHouseApp.Security;

namespace WareHouseApp.Data
{
    /// <summary>
    /// Central ADO.NET gateway. Finds a reachable SQL Server instance, creates the
    /// WarehouseDB database and tables on first run, and runs parameterised commands.
    /// </summary>
    public static class Database
    {
        private const string DbName = "WarehouseDB";
        private static string connectionString;

        // Tried in order. Set env var WAREHOUSE_DB_SERVER to force a specific server.
        private static readonly string[] CandidateServers =
        {
            @"(localdb)\MSSQLLocalDB",
            @"(localdb)\WarehouseLocal",
            @".\SQLEXPRESS",
            @"."
        };

        public static string ServerInUse { get; private set; }

        public static void Initialize()
        {
            string forced = Environment.GetEnvironmentVariable("WAREHOUSE_DB_SERVER");
            string[] servers = string.IsNullOrWhiteSpace(forced) ? CandidateServers : new[] { forced };
            Exception last = null;
            var details = new System.Text.StringBuilder();

            // Two passes: LocalDB can be slow to wake up the first time it is used.
            for (int attempt = 1; attempt <= 2; attempt++)
            {
                foreach (string server in servers)
                {
                    try
                    {
                        string master = Build(server, "master");
                        using (var con = new SqlConnection(master))
                        {
                            con.Open();
                            CreateDatabaseIfMissing(con, server);
                        }
                        connectionString = Build(server, DbName);
                        ServerInUse = server;
                        CreateSchema();
                        return;
                    }
                    catch (Exception ex)
                    {
                        last = ex;
                        if (attempt == 2) details.AppendLine(server + ": " + ex.Message);
                    }
                }
                System.Threading.Thread.Sleep(2000);
            }
            throw new DataAccessException(
                "Could not connect to SQL Server (tried LocalDB and SQLEXPRESS). " +
                "Install SQL Server Express/LocalDB or set WAREHOUSE_DB_SERVER.\n\n" + details, last);
        }

        /// <summary>
        /// Creates WarehouseDB if it does not exist. On LocalDB the files are placed in
        /// %LOCALAPPDATA%\WareHouseApp so they never clash with other files in the user's profile.
        /// </summary>
        private static void CreateDatabaseIfMissing(SqlConnection con, string server)
        {
            var exists = new SqlCommand("SELECT DB_ID('" + DbName + "')", con).ExecuteScalar();
            if (exists != null && exists != DBNull.Value) return;

            if (server.StartsWith("(localdb)", StringComparison.OrdinalIgnoreCase))
            {
                string dir = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WareHouseApp");
                System.IO.Directory.CreateDirectory(dir);
                string mdf = System.IO.Path.Combine(dir, DbName + ".mdf");
                string ldf = System.IO.Path.Combine(dir, DbName + "_log.ldf");

                if (System.IO.File.Exists(mdf))
                    // A database file from an earlier run exists: re-attach it (rebuilding the log if needed).
                    Exec(con, "CREATE DATABASE [" + DbName + "] ON (FILENAME = N'" + mdf + "') FOR ATTACH_REBUILD_LOG");
                else
                    Exec(con, "CREATE DATABASE [" + DbName + "] ON (NAME = N'" + DbName + "', FILENAME = N'" + mdf +
                              "') LOG ON (NAME = N'" + DbName + "_log', FILENAME = N'" + ldf + "')");
            }
            else
            {
                Exec(con, "CREATE DATABASE [" + DbName + "]");
            }
        }

        private static string Build(string server, string db)
        {
            return "Data Source=" + server + ";Initial Catalog=" + db +
                   ";Integrated Security=True;Connect Timeout=30";
        }

        private static void CreateSchema()
        {
            using (var con = Open())
            {
                Exec(con, @"IF OBJECT_ID('dbo.Users','U') IS NULL CREATE TABLE dbo.Users(
                    UserId INT IDENTITY(1,1) PRIMARY KEY,
                    Username NVARCHAR(50) NOT NULL UNIQUE,
                    PasswordHash NVARCHAR(200) NOT NULL,
                    Salt NVARCHAR(100) NOT NULL,
                    FullName NVARCHAR(100) NOT NULL,
                    Email NVARCHAR(100) NULL,
                    Role NVARCHAR(30) NOT NULL)");
                Exec(con, @"IF OBJECT_ID('dbo.Materials','U') IS NULL CREATE TABLE dbo.Materials(
                    MaterialId INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    Category NVARCHAR(50) NULL,
                    Quantity INT NOT NULL DEFAULT 0,
                    UnitPrice DECIMAL(18,2) NOT NULL DEFAULT 0,
                    ReorderLevel INT NOT NULL DEFAULT 0,
                    Location NVARCHAR(50) NULL,
                    LastUpdated DATETIME NOT NULL DEFAULT GETDATE())");
                Exec(con, @"IF OBJECT_ID('dbo.Customers','U') IS NULL CREATE TABLE dbo.Customers(
                    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    Phone NVARCHAR(20) NOT NULL,
                    Email NVARCHAR(100) NULL,
                    Address NVARCHAR(200) NULL,
                    Company NVARCHAR(100) NULL)");
                Exec(con, @"IF OBJECT_ID('dbo.Employees','U') IS NULL CREATE TABLE dbo.Employees(
                    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    NIC NVARCHAR(20) NOT NULL UNIQUE,
                    Phone NVARCHAR(20) NOT NULL,
                    Email NVARCHAR(100) NULL,
                    Address NVARCHAR(200) NULL,
                    Position NVARCHAR(50) NOT NULL,
                    Salary DECIMAL(18,2) NOT NULL DEFAULT 0,
                    HireDate DATE NOT NULL)");

                // Seed the default administrator on first run: admin / admin123
                var count = Convert.ToInt32(new SqlCommand("SELECT COUNT(*) FROM dbo.Users", con).ExecuteScalar());
                if (count == 0)
                {
                    string salt = PasswordHasher.GenerateSalt();
                    var cmd = new SqlCommand(@"INSERT INTO dbo.Users(Username,PasswordHash,Salt,FullName,Email,Role)
                        VALUES('admin',@h,@s,'System Administrator','admin@warehouse.local','Admin')", con);
                    cmd.Parameters.AddWithValue("@h", PasswordHasher.Hash("admin123", salt));
                    cmd.Parameters.AddWithValue("@s", salt);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static SqlConnection Open()
        {
            if (connectionString == null) throw new DataAccessException("Database not initialised.", null);
            var con = new SqlConnection(connectionString);
            con.Open();
            return con;
        }

        private static void Exec(SqlConnection con, string sql)
        {
            using (var cmd = new SqlCommand(sql, con)) cmd.ExecuteNonQuery();
        }

        private static SqlCommand Command(SqlConnection con, string sql, params SqlParameter[] ps)
        {
            var cmd = new SqlCommand(sql, con);
            if (ps != null) cmd.Parameters.AddRange(ps);
            return cmd;
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] ps)
        {
            try { using (var con = Open()) using (var cmd = Command(con, sql, ps)) return cmd.ExecuteNonQuery(); }
            catch (SqlException ex) { throw Wrap(ex); }
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] ps)
        {
            try { using (var con = Open()) using (var cmd = Command(con, sql, ps)) return cmd.ExecuteScalar(); }
            catch (SqlException ex) { throw Wrap(ex); }
        }

        public static DataTable ExecuteTable(string sql, params SqlParameter[] ps)
        {
            try
            {
                using (var con = Open())
                using (var cmd = Command(con, sql, ps))
                using (var da = new SqlDataAdapter(cmd))
                {
                    var table = new DataTable();
                    da.Fill(table);
                    return table;
                }
            }
            catch (SqlException ex) { throw Wrap(ex); }
        }

        private static Exception Wrap(SqlException ex)
        {
            // 2601/2627 = unique constraint violation
            if (ex.Number == 2601 || ex.Number == 2627)
                return new ValidationException("A record with the same unique value (username / NIC) already exists.");
            return new DataAccessException("Database error: " + ex.Message, ex);
        }
    }
}
