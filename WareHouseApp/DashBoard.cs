using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Controls;
using WareHouseApp.People;

namespace WareHouseApp
{
    public partial class DashBoard : Form
    {
        private readonly Person user;
        private Control current;

        /// <summary>True when the user chose Log out (so Program shows the login screen again).</summary>
        public bool LoggedOut { get; private set; }

        public DashBoard() : this(new Admin { UserName = "admin", FullName = "Designer" }) { }

        public DashBoard(Person user)
        {
            this.user = user;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

            button1.Text = "Dashboard";
            button2.Text = "Inventory";
            button3.Text = "Customers";
            button4.Text = "Employees";
            button5.Text = "Log out";
            button6.Text = "Exit";
            button7.Text = "Profile";
            button8.Visible = false;   // Settings: not part of this coursework scope
            button9.Text = "Welcome, " + user.FullName + " (" + user.Role + ")";
            button9.Enabled = false;

            // Role based access: only admins may open Employee management.
            button4.Enabled = user.CanManageEmployees;

            button1.Click += delegate { ShowHome(); };
            button2.Click += delegate { ShowScreen(new InventoryControl()); };
            button3.Click += delegate { ShowScreen(new CustomerControl()); };
            button4.Click += delegate { ShowScreen(new EmployeeControl()); };
            button5.Click += delegate { LoggedOut = true; Close(); };
            button6.Click += delegate { Close(); };
            button7.Click += delegate
            {
                MessageBox.Show("Name: " + user.FullName + "\nUsername: " + user.UserName +
                                "\nEmail: " + user.Email + "\nRole: " + user.Role,
                                "Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            current = mainDash1;
            Load += delegate { mainDash1.RefreshStats(); };
        }

        private void ShowHome()
        {
            ShowScreen(mainDash1);
            mainDash1.RefreshStats();
        }

        private void ShowScreen(Control next)
        {
            if (current != null && current != mainDash1) { Controls.Remove(current); current.Dispose(); }
            if (current == mainDash1) mainDash1.Visible = false;
            next.Location = new Point(277, 79);
            next.Size = new Size(806, 510);
            if (next == mainDash1) mainDash1.Visible = true;
            else Controls.Add(next);
            next.BringToFront();
            current = next;
        }
    }
}
