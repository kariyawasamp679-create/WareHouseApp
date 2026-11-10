namespace WareHouseApp.People
{
    /// <summary>Day-to-day staff account created through Sign Up. Cannot manage employees.</summary>
    public class ShippingOperator : Person
    {
        public override string Role { get { return "Operator"; } }
        public override bool CanManageEmployees { get { return false; } }
    }
}
