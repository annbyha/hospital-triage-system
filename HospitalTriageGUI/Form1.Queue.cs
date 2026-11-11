using HospitalTriage;

namespace HospitalTriageGUI
{
    public partial class Form1
    {
        /// <summary>Waiting queue ordered Red → Yellow → Green, then by arrival time.</summary>
        private void ShowWaitingQueue()
        {
            Panel content = CreatePage(PageQueue, "Patients are ordered by priority (Red, Yellow, Green) and then by arrival time", scrollable: false);

            // ---------- toolbar ----------
            Panel toolbar = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Theme.Background };

            toolbar.Controls.Add(Theme.MakeLabel("Show:", 10, FontStyle.Bold, Theme.Navy, 0, 15));
            ComboBox filter = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(55, 11),
                Size = new Size(160, 30),
                Font = Theme.F(10)
            };
            filter.Items.AddRange(new object[] { "All priorities", "Red only", "Yellow only", "Green only" });
            filter.SelectedIndex = 0;
            toolbar.Controls.Add(filter);

            Label counts = Theme.MakeLabel("", 10, FontStyle.Regular, Theme.Muted, 235, 15);
            toolbar.Controls.Add(counts);

            Button callButton = Theme.MakeButton("Call Next Patient  ➜", Theme.Red, Color.White, 210, 42);
            Button detailsButton = Theme.SecondaryButton("Patient Details", 140, 42);
            toolbar.Controls.Add(callButton);
            toolbar.Controls.Add(detailsButton);

            // ---------- "now serving" banner ----------
            Panel spacer1 = new Panel { Dock = DockStyle.Top, Height = 12, BackColor = Theme.Background };
            Panel banner = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = Theme.Navy };
            Label bannerTitle = Theme.MakeLabel("NOW SERVING", 9, FontStyle.Bold, Color.Silver, 18, 10);
            Label bannerText = Theme.MakeLabel("", 13, FontStyle.Bold, Color.White, 16, 29);
            banner.Controls.Add(bannerTitle);
            banner.Controls.Add(bannerText);
            Panel spacer2 = new Panel { Dock = DockStyle.Top, Height = 12, BackColor = Theme.Background };

            // ---------- grid ----------
            DataGridView grid = new DataGridView { Dock = DockStyle.Fill };
            Theme.StyleGrid(grid);
            Theme.AddColumns(grid,
                ("#", 30), ("ID", 50), ("Name", 140), ("Age", 40), ("Gender", 60), ("Priority", 65),
                ("Heart Rate", 70), ("Systolic BP", 75), ("Symptoms", 200), ("Arrival", 60), ("Waiting", 70));

            Label emptyLabel = new Label
            {
                Text = "The waiting queue is empty.\nRegister a patient to add them to the queue.",
                Font = Theme.F(12),
                ForeColor = Theme.Muted,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Visible = false
            };

            // Dock order: Fill controls first, then Top controls in reverse visual order.
            content.Controls.Add(emptyLabel);
            content.Controls.Add(grid);
            content.Controls.Add(spacer2);
            content.Controls.Add(banner);
            content.Controls.Add(spacer1);
            content.Controls.Add(toolbar);

            void positionButtons()
            {
                callButton.Location = new Point(toolbar.Width - callButton.Width, 7);
                detailsButton.Location = new Point(callButton.Left - detailsButton.Width - 12, 7);
            }
            toolbar.Resize += (s, e) => positionButtons();

            // ---------- logic ----------
            void refresh()
            {
                List<Patient> patients = filter.SelectedIndex switch
                {
                    1 => engine.GetWaitingPatients(PriorityLevel.Red),
                    2 => engine.GetWaitingPatients(PriorityLevel.Yellow),
                    3 => engine.GetWaitingPatients(PriorityLevel.Green),
                    _ => engine.GetWaitingPatients()
                };
                List<Patient> all = engine.GetWaitingPatients();

                counts.Text = $"Waiting: {all.Count}    Red {engine.GetWaitingCount(PriorityLevel.Red)} · " +
                              $"Yellow {engine.GetWaitingCount(PriorityLevel.Yellow)} · " +
                              $"Green {engine.GetWaitingCount(PriorityLevel.Green)}";

                int? selectedId = grid.CurrentRow?.Tag is Patient sel ? sel.PersonID : null;

                grid.Rows.Clear();
                foreach (Patient p in patients)
                {
                    int position = all.IndexOf(p) + 1;
                    int index = grid.Rows.Add(position, p.PersonID, p.Name, p.Age, p.Gender, p.Priority.ToString().ToUpper(),
                        $"{p.Vitals.HeartRate} bpm", $"{p.Vitals.SystolicBP} mmHg", string.Join(", ", p.Symptoms),
                        p.ArrivalTime.ToString("HH:mm"), Theme.FormatWait(p.GetWaitingDuration()));
                    DataGridViewRow row = grid.Rows[index];
                    row.Tag = p;
                    Theme.ColourRow(row, p.Priority, "Priority");
                    if (p.PersonID == selectedId) row.Selected = true;
                }

                grid.Visible = patients.Count > 0;
                emptyLabel.Visible = patients.Count == 0;
                emptyLabel.Text = all.Count == 0
                    ? "The waiting queue is empty.\nRegister a patient to add them to the queue."
                    : "No patients with this priority are waiting.";

                callButton.Enabled = all.Count > 0;
                detailsButton.Enabled = patients.Count > 0;

                Patient? last = engine.LastCalledPatient;
                bannerText.Text = last == null
                    ? "No patient has been called yet"
                    : $"{last.Name}  (ID {last.PersonID})  –  {last.Priority.ToString().ToUpper()} priority";
                banner.BackColor = last == null ? Theme.Navy : Theme.PriorityColor(last.Priority);
                bannerTitle.ForeColor = last == null ? Color.Silver : Color.White;
            }

            filter.SelectedIndexChanged += (s, e) => refresh();

            callButton.Click += (s, e) =>
            {
                CallNextPatientWithMessage();
                refresh();
            };

            void showSelected()
            {
                if (grid.CurrentRow?.Tag is Patient p) ShowPatientDetails(p);
            }
            detailsButton.Click += (s, e) => showSelected();
            grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) showSelected(); };

            // Timer: only update the "Waiting" column so selection and scrolling are kept.
            pageTick = () =>
            {
                foreach (DataGridViewRow row in grid.Rows)
                    if (row.Tag is Patient p)
                        row.Cells["Waiting"].Value = Theme.FormatWait(p.GetWaitingDuration());
            };

            positionButtons();
            refresh();
        }
    }
}
