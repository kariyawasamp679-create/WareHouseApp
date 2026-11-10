using System;
using System.Data;
using System.Windows.Forms;
using WareHouseApp.Models;
using WareHouseApp.Repositories;

namespace WareHouseApp.Controls
{
    public class EmployeeControl : ManagementControl
    {
        private readonly EmployeeRepository repo = new EmployeeRepository();
        private readonly TextBox txtName = new TextBox(), txtNic = new TextBox(), txtPosition = new TextBox(),
                                 txtPhone = new TextBox(), txtEmail = new TextBox(), txtAddress = new TextBox(),
                                 txtSalary = new TextBox();
        private readonly DateTimePicker dtHire = new DateTimePicker { Format = DateTimePickerFormat.Short };

        public EmployeeControl()
        {
            AddField("Full name *", txtName);
            AddField("NIC *", txtNic);
            AddField("Position *", txtPosition);
            AddField("Phone *", txtPhone);
            AddField("Email", txtEmail);
            AddField("Address", txtAddress);
            AddField("Salary", txtSalary);
            AddField("Hire date", dtHire);
        }

        protected override string ScreenTitle { get { return "Employee Management"; } }
        protected override DataTable LoadData(string keyword) { return repo.Search(keyword); }

        private Employee Build()
        {
            return new Employee
            {
                Id = SelectedId,
                Name = txtName.Text,
                NIC = txtNic.Text,
                Position = txtPosition.Text,
                Phone = txtPhone.Text,
                Email = txtEmail.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Salary = string.IsNullOrWhiteSpace(txtSalary.Text) ? 0 : ParseDecimal(txtSalary.Text, "Salary"),
                HireDate = dtHire.Value
            };
        }

        protected override void AddRecord() { repo.Add(Build()); }
        protected override void UpdateRecord() { repo.Update(Build()); }
        protected override void DeleteRecord(int id) { repo.Delete(id); }

        protected override void FillFields(DataGridViewRow r)
        {
            txtName.Text = Cell(r, "Name");
            txtNic.Text = Cell(r, "NIC");
            txtPosition.Text = Cell(r, "Position");
            txtPhone.Text = Cell(r, "Phone");
            txtEmail.Text = Cell(r, "Email");
            txtAddress.Text = Cell(r, "Address");
            txtSalary.Text = Cell(r, "Salary");
            DateTime d;
            if (DateTime.TryParse(Cell(r, "Hire Date"), out d)) dtHire.Value = d;
        }

        protected override void ClearFields()
        {
            txtName.Clear(); txtNic.Clear(); txtPosition.Clear(); txtPhone.Clear();
            txtEmail.Clear(); txtAddress.Clear(); txtSalary.Clear(); dtHire.Value = DateTime.Today;
        }
    }
}
