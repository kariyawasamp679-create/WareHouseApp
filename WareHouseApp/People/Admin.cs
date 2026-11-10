namespace WareHouseApp.People
{
    public class Admin : Person
    {
        public override string Role { get { return "Admin"; } }
        public override bool CanManageEmployees { get { return true; } }
    }
}
