using System;
using System.Windows.Forms;
using WareHouseApp.Data;

namespace WareHouseApp
{
    static class Program
    {
        /// <summary>
        /// Entry point: prepare the database, then loop Login -> Dashboard until the user exits.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Database.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cannot start", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            while (true)
            {
                using (var login = new Form1())
                {
                    if (login.ShowDialog() != DialogResult.OK) break;
                    using (var dash = new DashBoard(login.CurrentUser))
                    {
                        dash.ShowDialog();
                        if (!dash.LoggedOut) break;
                    }
                }
            }
        }
    }
}
