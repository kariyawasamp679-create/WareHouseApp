namespace WareHouseApp.Models
{
    /// <summary>Shared details of any real-world party (customer, employee) - inheritance.</summary>
    public abstract class Contact : Entity
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public override void Validate()
        {
            Require(Name, "Name");
            CheckPhone(Phone);
            CheckEmail(Email);
        }
    }
}
