using System.Data;
using System.Windows.Forms;
using WareHouseApp.Models;
using WareHouseApp.Repositories;

namespace WareHouseApp.Controls
{
    public class InventoryControl : ManagementControl
    {
        private readonly MaterialRepository repo = new MaterialRepository();
        private readonly TextBox txtName = new TextBox(), txtCategory = new TextBox(), txtQty = new TextBox(),
                                 txtPrice = new TextBox(), txtReorder = new TextBox(), txtLocation = new TextBox();

        public InventoryControl()
        {
            AddField("Item name *", txtName);
            AddField("Category", txtCategory);
            AddField("Quantity *", txtQty);
            AddField("Unit price *", txtPrice);
            AddField("Reorder level", txtReorder);
            AddField("Storage location", txtLocation);
        }

        protected override string ScreenTitle { get { return "Inventory Management"; } }
        protected override DataTable LoadData(string keyword) { return repo.Search(keyword); }

        private Material Build()
        {
            return new Material
            {
                Id = SelectedId,
                Name = txtName.Text,
                Category = txtCategory.Text.Trim(),
                Quantity = ParseInt(txtQty.Text, "Quantity"),
                UnitPrice = ParseDecimal(txtPrice.Text, "Unit price"),
                ReorderLevel = string.IsNullOrWhiteSpace(txtReorder.Text) ? 0 : ParseInt(txtReorder.Text, "Reorder level"),
                Location = txtLocation.Text.Trim()
            };
        }

        protected override void AddRecord() { repo.Add(Build()); }
        protected override void UpdateRecord() { repo.Update(Build()); }
        protected override void DeleteRecord(int id) { repo.Delete(id); }

        protected override void FillFields(DataGridViewRow r)
        {
            txtName.Text = Cell(r, "Name");
            txtCategory.Text = Cell(r, "Category");
            txtQty.Text = Cell(r, "Quantity");
            txtPrice.Text = Cell(r, "Unit Price");
            txtReorder.Text = Cell(r, "Reorder Level");
            txtLocation.Text = Cell(r, "Location");
        }

        protected override void ClearFields()
        {
            txtName.Clear(); txtCategory.Clear(); txtQty.Clear();
            txtPrice.Clear(); txtReorder.Clear(); txtLocation.Clear();
        }
    }
}
