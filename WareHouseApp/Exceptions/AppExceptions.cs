using System;

namespace WareHouseApp.Exceptions
{
    /// <summary>Thrown when user supplied data fails a business rule.</summary>
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    /// <summary>Thrown when login fails or a sign-up cannot be completed.</summary>
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message) { }
    }

    /// <summary>Wraps low-level database failures so the UI never deals with SqlException directly.</summary>
    public class DataAccessException : Exception
    {
        public DataAccessException(string message, Exception inner) : base(message, inner) { }
    }
}
