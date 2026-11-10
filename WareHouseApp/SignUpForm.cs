using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Exceptions;
using WareHouseApp.Services;

namespace WareHouseApp
{
    /// <summary>Registration dialog. Creates an Operator account in the database.</summary>
    public class SignUpForm : Form
    {
        private readonly AuthService auth = new AuthService();
        private readonly TextBox txtUser = new TextBox(), txtFull = new TextBox(), txtEmail = new TextBox(),
                                 txtPass = new TextBox { UseSystemPasswordChar = true },
                                 txtConfirm = new TextBox { UseSystemPasswordChar = true };

        public string CreatedUserName { get; private set; }

        public SignUpForm()
        {
            Text = "Sign Up";
            ClientSize = new Size(380, 430);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;
            BackColor = Color.White;

            var heading = new Label
            {
                Text = "Create Account",
                Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Bold),
                ForeColor = Color.FromArgb(5, 146, 18),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            Controls.Add(heading);

            int y = 65;
            AddRow("Username *", txtUser, ref y);
            AddRow("Full name *", txtFull, ref y);
            AddRow("Email", txtEmail, ref y);
            AddRow("Password * (min 6)", txtPass, ref y);
            AddRow("Confirm password *", txtConfirm, ref y);

            var btnCreate = new Button
            {
                Text = "SIGN UP",
                BackColor = Color.FromArgb(155, 236, 0),
                Bounds = new Rectangle(20, y + 10, 160, 40)
            };
            var btnCancel = new Button { Text = "Cancel", Bounds = new Rectangle(200, y + 10, 160, 40) };
            btnCreate.Click += BtnCreate_Click;
            btnCancel.Click += delegate { DialogResult = DialogResult.Cancel; Close(); };
            Controls.Add(btnCreate);
            Controls.Add(btnCancel);
            AcceptButton = btnCreate;
            CancelButton = btnCancel;
        }

        private void AddRow(string label, TextBox box, ref int y)
        {
            Controls.Add(new Label { Text = label, AutoSize = true, Location = new Point(20, y) });
            box.Bounds = new Rectangle(20, y + 20, 340, 24);
            Controls.Add(box);
            y += 60;
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                auth.SignUp(txtUser.Text, txtFull.Text, txtEmail.Text.Trim(), txtPass.Text, txtConfirm.Text);
                CreatedUserName = txtUser.Text.Trim();
                MessageBox.Show("Account created. You can now log in.", "Sign Up",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (DataAccessException ex)
            {
                MessageBox.Show(ex.Message, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
