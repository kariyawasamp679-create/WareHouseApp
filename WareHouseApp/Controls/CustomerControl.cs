using System.Data;
using System.Windows.Forms;
using WareHouseApp.Models;
using WareHouseApp.Repositories;

namespace WareHouseApp.Controls
{
    public class CustomerControl : ManagementControl
    {
        private readonly CustomerRepository repo = new CustomerRepository();
        private readonly TextBox txtName = new TextBox(), txtCompany = new TextBox(), txtPhone = new TextBox(),
                                 txtEmail = new TextBox(), txtAddress = new TextBox();

        public CustomerControl()
        {
            AddField("Customer name *", txtName);
            AddField("Company", txtCompany);
            AddField("Phone *", txtPhone);
            AddField("Email", txtEmail);
            AddField("Address", txtAddress);
        }

        protected override string ScreenTitle { get { return "Customer Management"; } }
        protected override DataTable LoadData(string keyword) { return repo.Search(keyword); }

        private Customer Build()
        {
            return new Customer
            {
                Id = SelectedId,
                Name = txtName.Text,
                Company = txtCompany.Text.Trim(),
                Phone = txtPhone.Text,
                Email = txtEmail.Text.Trim(),
                Address = txtAddress.Text.Trim()
            };
        }

        protected override void AddRecord() { repo.Add(Build()); }
        protected override void UpdateRecord() { repo.Update(Build()); }
        protected override void DeleteRecord(int id) { repo.Delete(id); }

        protected override void FillFields(DataGridViewRow r)
        {
            txtName.Text = Cell(r, "Name");
            txtCompany.Text = Cell(r, "Company");
            txtPhone.Text = Cell(r, "Phone");
            txtEmail.Text = Cell(r, "Email");
            txtAddress.Text = Cell(r, "Address");
        }

        protected override void ClearFields()
        {
            txtName.Clear(); txtCompany.Clear(); txtPhone.Clear(); txtEmail.Clear(); txtAddress.Clear();
        }
    }
}
