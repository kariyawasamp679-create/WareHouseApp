using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WareHouseApp.Repositories;

namespace WareHouseApp
{
    /// <summary>Home screen with live summary figures read from the database.</summary>
    public partial class MainDash : UserControl
    {
        private readonly Label lblInventory = new Label(), lblPeople = new Label(), lblLow = new Label();

        public MainDash()
        {
            InitializeComponent();
            Setup(panel1, "Inventory", lblInventory);
            Setup(panel2, "People", lblPeople);
            Setup(panel3, "Low stock alerts", lblLow);
        }

        private static void Setup(Panel panel, string heading, Label body)
        {
            panel.Controls.Add(new Label
            {
                Text = heading,
                Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold),
                ForeColor = Color.RoyalBlue,
                AutoSize = true,
                Location = new Point(12, 10)
            });
            body.Font = new Font("Microsoft Sans Serif", 11F);
            body.Location = new Point(14, 48);
            body.Size = new Size(panel.Width - 28, panel.Height - 58);
            panel.Controls.Add(body);
        }

        public void RefreshStats()
        {
            if (DesignMode) return;
            try
            {
                var materials = new MaterialRepository();
                lblInventory.Text = "Item types: " + materials.Count() +
                                    "\nTotal units in stock: " + materials.TotalUnits() +
                                    "\nStock value: " + materials.TotalValue().ToString("N2");
                lblPeople.Text = "Customers: " + new CustomerRepository().Count() +
                                 "\nEmployees: " + new EmployeeRepository().Count();

                DataTable low = materials.LowStock();
                if (low.Rows.Count == 0) lblLow.Text = "All items are above their reorder level.";
                else
                {
                    var sb = new StringBuilder();
                    for (int i = 0; i < low.Rows.Count && i < 6; i++)
                        sb.AppendLine(low.Rows[i]["Name"] + ": " + low.Rows[i]["Quantity"] +
                                      " left (reorder at " + low.Rows[i]["ReorderLevel"] + ")");
                    lblLow.Text = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                lblInventory.Text = "Could not load statistics:\n" + ex.Message;
            }
        }
    }
}
