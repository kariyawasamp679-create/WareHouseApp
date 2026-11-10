using WareHouseApp.Exceptions;

namespace WareHouseApp.Models
{
    /// <summary>An inventory item stored in the warehouse.</summary>
    public class Material : Entity
    {
        private int quantity;

        public string Name { get; set; }
        public string Category { get; set; }
        public decimal UnitPrice { get; set; }
        public int ReorderLevel { get; set; }
        public string Location { get; set; }

        // Encapsulation: quantity can never be set negative.
        public int Quantity
        {
            get { return quantity; }
            set
            {
                if (value < 0) throw new ValidationException("Quantity cannot be negative.");
                quantity = value;
            }
        }

        public bool IsLowStock { get { return Quantity <= ReorderLevel; } }
        public decimal TotalValue { get { return Quantity * UnitPrice; } }

        public override void Validate()
        {
            Require(Name, "Item name");
            if (UnitPrice < 0) throw new ValidationException("Unit price cannot be negative.");
            if (ReorderLevel < 0) throw new ValidationException("Reorder level cannot be negative.");
        }
    }
}
