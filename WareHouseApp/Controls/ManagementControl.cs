using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Exceptions;

namespace WareHouseApp.Controls
{
    /// <summary>
    /// Template for every CRUD screen: search box, data grid, input fields and
    /// Add / Update / Delete / Clear buttons. Subclasses supply the entity specific parts.
    /// All exceptions are handled here once (exception handling in one place).
    /// </summary>
    public abstract class ManagementControl : UserControl
    {
        private readonly DataGridView grid = new DataGridView();
        private readonly TextBox txtSearch = new TextBox();
        private readonly FlowLayoutPanel fields = new FlowLayoutPanel();
        private readonly Label title = new Label();

        protected int SelectedId;

        protected abstract string ScreenTitle { get; }
        protected abstract DataTable LoadData(string keyword);
        protected abstract void AddRecord();
        protected abstract void UpdateRecord();
        protected abstract void DeleteRecord(int id);
        protected abstract void FillFields(DataGridViewRow row);
        protected abstract void ClearFields();

        protected ManagementControl()
        {
            Size = new Size(806, 510);
            BackColor = Color.WhiteSmoke;

            title.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold);
            title.ForeColor = Color.RoyalBlue;
            title.SetBounds(10, 6, 400, 30);

            var lblSearch = new Label { Text = "Search:", AutoSize = true, Location = new Point(440, 14) };
            txtSearch.SetBounds(500, 10, 200, 24);
            txtSearch.TextChanged += delegate { RefreshGrid(); };
            var btnRefresh = MakeButton("Refresh", 710, 9, 80);
            btnRefresh.Click += delegate { txtSearch.Clear(); RefreshGrid(); };

            grid.SetBounds(10, 42, 786, 250);
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = Color.White;
            grid.RowHeadersVisible = false;
            grid.SelectionChanged += Grid_SelectionChanged;

            fields.SetBounds(10, 298, 786, 150);
            fields.AutoScroll = true;

            var btnAdd = MakeButton("Add", 10, 458, 110);
            var btnUpdate = MakeButton("Update", 130, 458, 110);
            var btnDelete = MakeButton("Delete", 250, 458, 110);
            var btnClear = MakeButton("Clear", 370, 458, 110);
            btnAdd.Click += delegate { Run(delegate { AddRecord(); }, "Record added."); };
            btnUpdate.Click += delegate
            {
                if (SelectedId == 0) { Warn("Select a row from the table first."); return; }
                Run(delegate { UpdateRecord(); }, "Record updated.");
            };
            btnDelete.Click += delegate
            {
                if (SelectedId == 0) { Warn("Select a row from the table first."); return; }
                if (MessageBox.Show("Delete the selected record? This cannot be undone.", "Confirm delete",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                Run(delegate { DeleteRecord(SelectedId); }, "Record deleted.");
            };
            btnClear.Click += delegate { ResetForm(); };

            Controls.AddRange(new Control[] { title, lblSearch, txtSearch, btnRefresh, grid, fields,
                                              btnAdd, btnUpdate, btnDelete, btnClear });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            title.Text = ScreenTitle;
            if (!DesignMode) RefreshGrid();
        }

        /// <summary>Adds a labelled input (label above the control) to the form area.</summary>
        protected void AddField(string label, Control input)
        {
            var host = new Panel { Width = 190, Height = 50, Margin = new Padding(3) };
            var lbl = new Label { Text = label, AutoSize = true, Location = new Point(0, 0) };
            input.SetBounds(0, 20, 185, 24);
            host.Controls.Add(lbl);
            host.Controls.Add(input);
            fields.Controls.Add(host);
        }

        public void RefreshGrid()
        {
            try
            {
                grid.DataSource = LoadData(txtSearch.Text);
                grid.ClearSelection();
                SelectedId = 0;
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void ResetForm()
        {
            SelectedId = 0;
            ClearFields();
            grid.ClearSelection();
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            DataGridViewRow row = grid.SelectedRows[0];
            SelectedId = Convert.ToInt32(row.Cells[0].Value);
            FillFields(row);
        }

        /// <summary>Runs a data action, reports success, and converts every failure to a friendly message.</summary>
        private void Run(Action action, string successMessage)
        {
            try
            {
                action();
                ResetForm();
                RefreshGrid();
                MessageBox.Show(successMessage, ScreenTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { ShowError(ex); }
        }

        protected static void Warn(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private static void ShowError(Exception ex)
        {
            if (ex is ValidationException)
                MessageBox.Show(ex.Message, "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else if (ex is DataAccessException)
                MessageBox.Show(ex.Message, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static Button MakeButton(string text, int x, int y, int w)
        {
            var b = new Button { Text = text, FlatStyle = FlatStyle.Flat, BackColor = Color.RoyalBlue, ForeColor = Color.White };
            b.SetBounds(x, y, w, 32);
            return b;
        }

        // ---- parsing helpers shared by subclasses (throw ValidationException on bad input) ----
        protected static int ParseInt(string text, string field)
        {
            int v;
            if (!int.TryParse((text ?? "").Trim(), out v)) throw new ValidationException(field + " must be a whole number.");
            return v;
        }

        protected static decimal ParseDecimal(string text, string field)
        {
            decimal v;
            if (!decimal.TryParse((text ?? "").Trim(), out v)) throw new ValidationException(field + " must be a number.");
            return v;
        }

        protected static string Cell(DataGridViewRow row, string column)
        {
            object v = row.Cells[column].Value;
            return v == null || v == DBNull.Value ? "" : Convert.ToString(v);
        }
    }
}
