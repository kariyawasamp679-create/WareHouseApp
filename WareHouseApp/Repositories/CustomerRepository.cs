using System;
using System.Data;
using System.Data.SqlClient;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;

namespace WareHouseApp.Repositories
{
    public class CustomerRepository : IRepository<Customer>
    {
        public DataTable Search(string keyword)
        {
            return Database.ExecuteTable(
                @"SELECT CustomerId AS [ID], Name, Company, Phone, Email, Address FROM Customers
                  WHERE Name LIKE @k OR Company LIKE @k OR Phone LIKE @k OR Email LIKE @k
                  ORDER BY Name",
                new SqlParameter("@k", "%" + (keyword ?? "").Trim() + "%"));
        }

        public void Add(Customer c)
        {
            c.Validate();
            Database.ExecuteNonQuery(
                "INSERT INTO Customers(Name,Phone,Email,Address,Company) VALUES(@n,@ph,@e,@a,@co)", Params(c));
        }

        public void Update(Customer c)
        {
            c.Validate();
            int rows = Database.ExecuteNonQuery(
                "UPDATE Customers SET Name=@n,Phone=@ph,Email=@e,Address=@a,Company=@co WHERE CustomerId=@id", Params(c));
            if (rows == 0) throw new ValidationException("Customer no longer exists.");
        }

        public void Delete(int id)
        {
            Database.ExecuteNonQuery("DELETE FROM Customers WHERE CustomerId=@id", new SqlParameter("@id", id));
        }

        public int Count() { return Convert.ToInt32(Database.ExecuteScalar("SELECT COUNT(*) FROM Customers")); }

        private static SqlParameter[] Params(Customer c)
        {
            return new[]
            {
                new SqlParameter("@id", c.Id),
                new SqlParameter("@n", c.Name.Trim()),
                new SqlParameter("@ph", c.Phone.Trim()),
                new SqlParameter("@e", (object)c.Email ?? DBNull.Value),
                new SqlParameter("@a", (object)c.Address ?? DBNull.Value),
                new SqlParameter("@co", (object)c.Company ?? DBNull.Value)
            };
        }
    }
}
