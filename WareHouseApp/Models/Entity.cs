using System.Text.RegularExpressions;
using WareHouseApp.Exceptions;

namespace WareHouseApp.Models
{
    /// <summary>Abstract root of every persisted object. Enforces self-validation (abstraction).</summary>
    public abstract class Entity
    {
        public int Id { get; set; }

        /// <summary>Throws <see cref="ValidationException"/> when the object breaks a business rule.</summary>
        public abstract void Validate();

        protected static void Require(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(fieldName + " is required.");
        }

        protected static void CheckEmail(string email)
        {
            if (!string.IsNullOrWhiteSpace(email) &&
                !Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ValidationException("Email address is not valid.");
        }

        protected static void CheckPhone(string phone)
        {
            Require(phone, "Phone");
            if (!Regex.IsMatch(phone.Trim(), @"^\+?[0-9]{7,15}$"))
                throw new ValidationException("Phone must be 7-15 digits (optional leading +).");
        }
    }
}
