using System;
using System.Data;
using System.Data.SqlClient;
using WareHouseApp.Data;
using WareHouseApp.People;

namespace WareHouseApp.Repositories
{
    public class UserRepository
    {
        /// <summary>Returns the user plus stored credentials, or null when the username is unknown.</summary>
        public Person FindByUserName(string username, out string hash, out string salt)
        {
            hash = salt = null;
            DataTable t = Database.ExecuteTable(
                "SELECT UserId,Username,PasswordHash,Salt,FullName,Email,Role FROM Users WHERE Username=@u",
                new SqlParameter("@u", username.Trim()));
            if (t.Rows.Count == 0) return null;

            DataRow r = t.Rows[0];
            Person p = Person.Create(Convert.ToString(r["Role"]));
            p.Id = Convert.ToInt32(r["UserId"]);
            p.UserName = Convert.ToString(r["Username"]);
            p.FullName = Convert.ToString(r["FullName"]);
            p.Email = r["Email"] == DBNull.Value ? null : Convert.ToString(r["Email"]);
            hash = Convert.ToString(r["PasswordHash"]);
            salt = Convert.ToString(r["Salt"]);
            return p;
        }

        public void Add(Person p, string hash, string salt)
        {
            Database.ExecuteNonQuery(
                @"INSERT INTO Users(Username,PasswordHash,Salt,FullName,Email,Role)
                  VALUES(@u,@h,@s,@f,@e,@r)",
                new SqlParameter("@u", p.UserName.Trim()),
                new SqlParameter("@h", hash),
                new SqlParameter("@s", salt),
                new SqlParameter("@f", p.FullName.Trim()),
                new SqlParameter("@e", (object)p.Email ?? DBNull.Value),
                new SqlParameter("@r", p.Role));
        }

        public void UpdatePassword(int userId, string hash, string salt)
        {
            Database.ExecuteNonQuery("UPDATE Users SET PasswordHash=@h,Salt=@s WHERE UserId=@id",
                new SqlParameter("@h", hash), new SqlParameter("@s", salt), new SqlParameter("@id", userId));
        }
    }
}
