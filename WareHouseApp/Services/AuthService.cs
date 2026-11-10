using WareHouseApp.Exceptions;
using WareHouseApp.People;
using WareHouseApp.Repositories;
using WareHouseApp.Security;

namespace WareHouseApp.Services
{
    /// <summary>Business rules for logging in, signing up and changing passwords.</summary>
    public class AuthService
    {
        private readonly UserRepository users = new UserRepository();

        public Person Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password))
                throw new AuthenticationException("Enter both username and password.");

            string hash, salt;
            Person p = users.FindByUserName(username, out hash, out salt);
            if (p == null || !PasswordHasher.Verify(password, salt, hash))
                throw new AuthenticationException("Username or Password is incorrect.");
            return p;
        }

        public Person SignUp(string username, string fullName, string email, string password, string confirm)
        {
            var p = new ShippingOperator { UserName = username, FullName = fullName, Email = email };
            p.Validate();
            CheckPassword(password);
            if (password != confirm) throw new ValidationException("Passwords do not match.");

            string h, s;
            if (users.FindByUserName(username, out h, out s) != null)
                throw new ValidationException("That username is already taken.");

            string salt = PasswordHasher.GenerateSalt();
            users.Add(p, PasswordHasher.Hash(password, salt), salt);
            return p;
        }

        public void ChangePassword(Person user, string current, string newPassword)
        {
            Login(user.UserName, current);
            CheckPassword(newPassword);
            string salt = PasswordHasher.GenerateSalt();
            users.UpdatePassword(user.Id, PasswordHasher.Hash(newPassword, salt), salt);
        }

        private static void CheckPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
                throw new ValidationException("Password must be at least 6 characters.");
        }
    }
}
