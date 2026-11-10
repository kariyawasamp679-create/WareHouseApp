using System;
using System.Data;
using System.Data.SqlClient;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;

namespace WareHouseApp.Repositories
{
    public class EmployeeRepository : IRepository<Employee>
    {
        public DataTable Search(string keyword)
        {
            return Database.ExecuteTable(
                @"SELECT EmployeeId AS [ID], Name, NIC, Position, Phone, Email, Address,
                         Salary, HireDate AS [Hire Date]
                  FROM Employees
                  WHERE Name LIKE @k OR NIC LIKE @k OR Position LIKE @k OR Phone LIKE @k
                  ORDER BY Name",
                new SqlParameter("@k", "%" + (keyword ?? "").Trim() + "%"));
        }

        public void Add(Employee e)
        {
            e.Validate();
            Database.ExecuteNonQuery(
                @"INSERT INTO Employees(Name,NIC,Phone,Email,Address,Position,Salary,HireDate)
                  VALUES(@n,@nic,@ph,@e,@a,@pos,@s,@h)", Params(e));
        }

        public void Update(Employee e)
        {
            e.Validate();
            int rows = Database.ExecuteNonQuery(
                @"UPDATE Employees SET Name=@n,NIC=@nic,Phone=@ph,Email=@e,Address=@a,
                  Position=@pos,Salary=@s,HireDate=@h WHERE EmployeeId=@id", Params(e));
            if (rows == 0) throw new ValidationException("Employee no longer exists.");
        }

        public void Delete(int id)
        {
            Database.ExecuteNonQuery("DELETE FROM Employees WHERE EmployeeId=@id", new SqlParameter("@id", id));
        }

        public int Count() { return Convert.ToInt32(Database.ExecuteScalar("SELECT COUNT(*) FROM Employees")); }

        private static SqlParameter[] Params(Employee e)
        {
            return new[]
            {
                new SqlParameter("@id", e.Id),
                new SqlParameter("@n", e.Name.Trim()),
                new SqlParameter("@nic", e.NIC.Trim()),
                new SqlParameter("@ph", e.Phone.Trim()),
                new SqlParameter("@e", (object)e.Email ?? DBNull.Value),
                new SqlParameter("@a", (object)e.Address ?? DBNull.Value),
                new SqlParameter("@pos", e.Position.Trim()),
                new SqlParameter("@s", e.Salary),
                new SqlParameter("@h", e.HireDate.Date)
            };
        }
    }
}
