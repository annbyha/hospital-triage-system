using System.Text.RegularExpressions;
using HospitalTriage;

namespace HospitalTriageGUI
{
    public partial class Form1
    {
        /// <summary>Staff management (nurses and doctors), duty status and the RED alert log.</summary>
        private void ShowStaff()
        {
            Panel content = CreatePage(PageStaff, "Manage nurses and doctors – on-duty staff receive an alert for every RED priority patient", scrollable: false);

            // ================= Bottom: alert log =================
            Panel alertPanel = new Panel { Dock = DockStyle.Bottom, Height = 210, BackColor = Theme.Background, Padding = new Padding(0, 15, 0, 0) };
            Panel alertCard = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(15, 40, 15, 12) };
            alertCard.Controls.Add(Theme.MakeLabel("⚠  RED PRIORITY ALERT LOG", 10, FontStyle.Bold, Theme.Red, 15, 12));
            ListBox alertList = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                Font = Theme.F(9.5f),
                ForeColor = Theme.Navy,
                HorizontalScrollbar = true
            };
            alertCard.Controls.Add(alertList);
            alertPanel.Controls.Add(alertCard);

            // ================= Right: add staff form =================
            Panel addPanel = new Panel { Dock = DockStyle.Right, Width = 330, BackColor = Theme.Background, Padding = new Padding(15, 0, 0, 0) };
            Panel addCard = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            addPanel.Controls.Add(addCard);
            addCard.Controls.Add(Theme.MakeLabel("Add Staff Member", 13, FontStyle.Bold, Theme.Navy, 20, 15));

            addCard.Controls.Add(Theme.MakeLabel("Role", 9.5f, FontStyle.Bold, Theme.Navy, 20, 55));
            ComboBox roleBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(20, 77),
                Size = new Size(130, 30),
                Font = Theme.F(10)
            };
            roleBox.Items.AddRange(new object[] { "Nurse", "Doctor" });
            roleBox.SelectedIndex = 0;
            addCard.Controls.Add(roleBox);

            addCard.Controls.Add(Theme.MakeLabel("Staff ID", 9.5f, FontStyle.Bold, Theme.Navy, 165, 55));
            TextBox idBox = new TextBox { Location = new Point(165, 77), Size = new Size(125, 30), Font = Theme.F(10) };
            addCard.Controls.Add(idBox);

            addCard.Controls.Add(Theme.MakeLabel("Full Name", 9.5f, FontStyle.Bold, Theme.Navy, 20, 117));
            TextBox nameBox = new TextBox { Location = new Point(20, 139), Size = new Size(270, 30), Font = Theme.F(10) };
            addCard.Controls.Add(nameBox);

            addCard.Controls.Add(Theme.MakeLabel("Age", 9.5f, FontStyle.Bold, Theme.Navy, 20, 179));
            TextBox ageBox = new TextBox { Location = new Point(20, 201), Size = new Size(80, 30), Font = Theme.F(10) };
            addCard.Controls.Add(ageBox);

            Label assignmentLabel = Theme.MakeLabel("Ward", 9.5f, FontStyle.Bold, Theme.Navy, 115, 179);
            addCard.Controls.Add(assignmentLabel);
            TextBox assignmentBox = new TextBox { Location = new Point(115, 201), Size = new Size(175, 30), Font = Theme.F(10) };
            addCard.Controls.Add(assignmentBox);

            Button addButton = Theme.PrimaryButton("✚  Add Staff", 270, 42);
            addButton.Location = new Point(20, 250);
            addCard.Controls.Add(addButton);

            Label note = new Label
            {
                Text = "Tip: double-click a staff member in the table to switch them on or off duty.",
                Location = new Point(20, 305),
                Size = new Size(270, 45),
                Font = Theme.F(8.5f),
                ForeColor = Theme.Muted
            };
            addCard.Controls.Add(note);

            // ================= Fill: staff table =================
            Panel tablePanel = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };
            Panel tableToolbar = new Panel { Dock = DockStyle.Top, Height = 54, BackColor = Theme.Background };
            Button toggleButton = Theme.PrimaryButton("Toggle On / Off Duty", 190, 40);
            toggleButton.Location = new Point(0, 4);
            Button alertsButton = Theme.SecondaryButton("View Staff Alerts", 160, 40);
            alertsButton.Location = new Point(200, 4);
            Button removeButton = Theme.MakeButton("Remove", Color.White, Theme.Red, 100, 40);
            removeButton.Location = new Point(370, 4);
            tableToolbar.Controls.AddRange(new Control[] { toggleButton, alertsButton, removeButton });

            DataGridView grid = new DataGridView { Dock = DockStyle.Fill };
            Theme.StyleGrid(grid);
            Theme.AddColumns(grid, ("ID", 45), ("Name", 120), ("Age", 40), ("Role", 60), ("Ward / Specialisation", 140), ("Status", 70), ("Alerts", 50));
            tablePanel.Controls.Add(grid);
            tablePanel.Controls.Add(tableToolbar);

            content.Controls.Add(tablePanel);
            content.Controls.Add(addPanel);
            content.Controls.Add(alertPanel);

            // ================= Logic =================
            StaffMember? selected() => grid.CurrentRow?.Tag as StaffMember;

            void refresh()
            {
                int? selectedId = selected()?.PersonID;
                grid.Rows.Clear();
                foreach (StaffMember staff in engine.GetStaff())
                {
                    int i = grid.Rows.Add(staff.PersonID, staff.Name, staff.Age, staff.Role, staff.GetAssignment(),
                        staff.IsOnDuty ? "● On duty" : "○ Off duty", staff.ReceivedAlerts.Count);
                    DataGridViewRow row = grid.Rows[i];
                    row.Tag = staff;
                    row.Cells["Status"].Style.ForeColor = staff.IsOnDuty ? Theme.Green : Theme.Muted;
                    row.Cells["Status"].Style.SelectionForeColor = staff.IsOnDuty ? Theme.Green : Theme.Muted;
                    row.Cells["Status"].Style.Font = Theme.F(9.5f, FontStyle.Bold);
                    if (staff.PersonID == selectedId) grid.CurrentCell = row.Cells[0];
                }

                alertList.BeginUpdate();
                alertList.Items.Clear();
                foreach (string alert in engine.GetAlertLog().Reverse()) alertList.Items.Add(alert);
                if (alertList.Items.Count == 0) alertList.Items.Add("No RED priority alerts have been sent in this session.");
                alertList.EndUpdate();

                idBox.Text = engine.GetNextStaffId().ToString();
                UpdateSidebarStatus();
            }

            roleBox.SelectedIndexChanged += (s, e) =>
                assignmentLabel.Text = roleBox.SelectedIndex == 0 ? "Ward" : "Specialisation";

            void toggleSelected()
            {
                StaffMember? staff = selected();
                if (staff == null) return;
                staff.IsOnDuty = !staff.IsOnDuty;
                refresh();
            }

            toggleButton.Click += (s, e) => toggleSelected();
            grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) toggleSelected(); };

            alertsButton.Click += (s, e) =>
            {
                StaffMember? staff = selected();
                if (staff == null) return;
                string text = staff.ReceivedAlerts.Count == 0
                    ? "This staff member has not received any alerts yet."
                    : string.Join("\n", staff.ReceivedAlerts);
                ShowInfo(text, $"Alerts received by {staff.Role} {staff.Name}");
            };

            removeButton.Click += (s, e) =>
            {
                StaffMember? staff = selected();
                if (staff == null) return;
                if (MessageBox.Show($"Remove {staff.Role} {staff.Name} (ID {staff.PersonID})?", "Remove Staff",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                engine.RemoveStaff(staff.PersonID);
                refresh();
            };

            addButton.Click += (s, e) =>
            {
                var problems = new List<string>();
                if (!int.TryParse(idBox.Text.Trim(), out int id) || id <= 0) problems.Add("Staff ID must be a positive whole number.");
                string name = Regex.Replace(nameBox.Text.Trim(), @"\s+", " ");
                if (name.Length < 2 || !NamePattern.IsMatch(name)) problems.Add("Please enter a valid name (letters only).");
                if (!int.TryParse(ageBox.Text.Trim(), out int age) || age < 18 || age > 80) problems.Add("Staff age must be between 18 and 80.");
                string assignment = assignmentBox.Text.Trim();
                if (assignment.Length == 0) problems.Add($"Please enter the {assignmentLabel.Text.ToLower()}.");

                if (problems.Count > 0)
                {
                    MessageBox.Show("Please correct the following:\n\n• " + string.Join("\n• ", problems),
                        "Invalid Staff Details", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    StaffMember staff = roleBox.SelectedIndex == 0
                        ? new Nurse(id, name, age, assignment)
                        : new Doctor(id, name, age, assignment);
                    engine.RegisterStaff(staff);
                    nameBox.Clear();
                    ageBox.Clear();
                    assignmentBox.Clear();
                    refresh();
                    ShowInfo($"{staff.Role} {staff.Name} has been added and is on duty.", "Staff Added");
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    ShowError(ex.Message, "Could Not Add Staff");
                }
            };

            refresh();
        }
    }
}
