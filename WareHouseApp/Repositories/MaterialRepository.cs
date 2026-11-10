using System;
using System.Data;
using System.Data.SqlClient;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;

namespace WareHouseApp.Repositories
{
    public class MaterialRepository : IRepository<Material>
    {
        public DataTable Search(string keyword)
        {
            return Database.ExecuteTable(
                @"SELECT MaterialId AS [ID], Name, Category, Quantity, UnitPrice AS [Unit Price],
                         ReorderLevel AS [Reorder Level], Location, LastUpdated AS [Last Updated]
                  FROM Materials
                  WHERE Name LIKE @k OR Category LIKE @k OR Location LIKE @k
                  ORDER BY Name",
                new SqlParameter("@k", "%" + (keyword ?? "").Trim() + "%"));
        }

        public void Add(Material m)
        {
            m.Validate();
            Database.ExecuteNonQuery(
                @"INSERT INTO Materials(Name,Category,Quantity,UnitPrice,ReorderLevel,Location,LastUpdated)
                  VALUES(@n,@c,@q,@p,@r,@l,GETDATE())", Params(m));
        }

        public void Update(Material m)
        {
            m.Validate();
            int rows = Database.ExecuteNonQuery(
                @"UPDATE Materials SET Name=@n,Category=@c,Quantity=@q,UnitPrice=@p,
                  ReorderLevel=@r,Location=@l,LastUpdated=GETDATE() WHERE MaterialId=@id", Params(m));
            if (rows == 0) throw new ValidationException("Item no longer exists.");
        }

        public void Delete(int id)
        {
            Database.ExecuteNonQuery("DELETE FROM Materials WHERE MaterialId=@id", new SqlParameter("@id", id));
        }

        public int Count() { return Convert.ToInt32(Database.ExecuteScalar("SELECT COUNT(*) FROM Materials")); }

        public int TotalUnits()
        {
            return Convert.ToInt32(Database.ExecuteScalar("SELECT ISNULL(SUM(Quantity),0) FROM Materials"));
        }

        public decimal TotalValue()
        {
            return Convert.ToDecimal(Database.ExecuteScalar("SELECT ISNULL(SUM(Quantity*UnitPrice),0) FROM Materials"));
        }

        public DataTable LowStock()
        {
            return Database.ExecuteTable(
                "SELECT Name, Quantity, ReorderLevel FROM Materials WHERE Quantity <= ReorderLevel ORDER BY Quantity");
        }

        private static SqlParameter[] Params(Material m)
        {
            return new[]
            {
                new SqlParameter("@id", m.Id),
                new SqlParameter("@n", m.Name.Trim()),
                new SqlParameter("@c", (object)m.Category ?? DBNull.Value),
                new SqlParameter("@q", m.Quantity),
                new SqlParameter("@p", m.UnitPrice),
                new SqlParameter("@r", m.ReorderLevel),
                new SqlParameter("@l", (object)m.Location ?? DBNull.Value)
            };
        }
    }
}
