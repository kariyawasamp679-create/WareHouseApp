using System;
using System.Windows.Forms;
using WareHouseApp.Exceptions;
using WareHouseApp.People;
using WareHouseApp.Resources.AppStrings;
using WareHouseApp.Services;

namespace WareHouseApp
{
    public partial class Form1 : Form
    {
        private readonly AuthService auth = new AuthService();
        private readonly LoginPage loginPage = new LoginPage();

        /// <summary>The authenticated user after a successful login (DialogResult.OK).</summary>
        public Person CurrentUser { get; private set; }

        public Form1()
        {
            InitializeComponent();
            AcceptButton = butLogin;

            // Field captions (the starter design had none).
            var captionFont = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            Controls.Add(new Label { Text = "Username", Font = captionFont, AutoSize = true, Location = new System.Drawing.Point(303, 138) });
            Controls.Add(new Label { Text = "Password", Font = captionFont, AutoSize = true, Location = new System.Drawing.Point(303, 241) });
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { }

        private void label1_Click(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void butLogin_Click(object sender, EventArgs e)
        {
            try
            {
                CurrentUser = auth.Login(txtName.Text, txtPassword.Text);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (AuthenticationException ex)
            {
                MessageBox.Show(ex.Message, loginPage.LoginErrorTitleEn, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (DataAccessException ex)
            {
                MessageBox.Show(ex.Message, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                txtPassword.Text = "";
            }
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            using (var form = new SignUpForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    txtName.Text = form.CreatedUserName;
                    txtPassword.Focus();
                }
            }
        }
    }
}
