using System;
using WareHouseApp.Exceptions;

namespace WareHouseApp.Models
{
    public class Employee : Contact
    {
        public string NIC { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }

        public override void Validate()
        {
            base.Validate();
            Require(NIC, "NIC");
            Require(Position, "Position");
            if (Salary < 0) throw new ValidationException("Salary cannot be negative.");
            if (HireDate.Date > DateTime.Today) throw new ValidationException("Hire date cannot be in the future.");
        }
    }
}
