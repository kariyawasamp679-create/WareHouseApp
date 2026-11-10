using WareHouseApp.Exceptions;
using WareHouseApp.Models;

namespace WareHouseApp.People
{
    /// <summary>A system user (someone who can log in). Subclasses define role specific rights.</summary>
    public abstract class Person : Entity
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public abstract string Role { get; }
        public abstract bool CanManageEmployees { get; }

        public override void Validate()
        {
            Require(UserName, "Username");
            if (UserName.Trim().Length < 3) throw new ValidationException("Username must be at least 3 characters.");
            Require(FullName, "Full name");
            CheckEmail(Email);
        }

        /// <summary>Factory: builds the right subclass from the role stored in the database.</summary>
        public static Person Create(string role)
        {
            if (role == "Admin") return new Admin();
            return new ShippingOperator();
        }
    }
}
