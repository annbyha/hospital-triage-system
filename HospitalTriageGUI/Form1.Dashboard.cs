using HospitalTriage;

namespace HospitalTriageGUI
{
    public partial class Form1
    {
        /// <summary>Dashboard: live statistics, next patient, recent activity and quick actions.</summary>
        private void ShowDashboard()
        {
            Panel content = CreatePage(PageDashboard, "Welcome to the Hospital Patient Triage System – live overview of the emergency department");

            content.Controls.Add(Theme.MakeLabel("Patient Overview", 14, FontStyle.Bold, Theme.Navy, 30, 15));

            // Row 1: priority cards
            Label totalValue = AddSummaryCard(content, "WAITING PATIENTS", "Currently in the queue", Theme.Blue, 30, 55);
            Label redValue = AddSummaryCard(content, "RED PRIORITY", "Immediate attention", Theme.Red, 280, 55);
            Label yellowValue = AddSummaryCard(content, "YELLOW PRIORITY", "Urgent attention", Theme.Yellow, 530, 55);
            Label greenValue = AddSummaryCard(content, "GREEN PRIORITY", "Non-critical patients", Theme.Green, 780, 55);

            // Row 2: session statistics
            Label registeredValue = AddMiniStat(content, "Registered this session", 30, 190);
            Label calledValue = AddMiniStat(content, "Patients called", 280, 190);
            Label avgWaitValue = AddMiniStat(content, "Average waiting time", 530, 190);
            Label staffValue = AddMiniStat(content, "Staff on duty", 780, 190);

            // Next patient card
            Panel nextCard = Theme.Card(30, 285, 485, 230);
            content.Controls.Add(nextCard);
            nextCard.Controls.Add(Theme.MakeLabel("NEXT PATIENT TO BE CALLED", 9, FontStyle.Bold, Theme.Muted, 20, 15));

            Panel nextStripe = new Panel { Location = new Point(0, 0), Size = new Size(6, 230), BackColor = Theme.Border };
            nextCard.Controls.Add(nextStripe);

            Label nextName = Theme.MakeLabel("", 16, FontStyle.Bold, Theme.Navy, 20, 42);
            Label nextBadge = new Label
            {
                Font = Theme.F(9, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(120, 26),
                Location = new Point(345, 45)
            };
            Label nextInfo = new Label
            {
                Font = Theme.F(10),
                ForeColor = Theme.Muted,
                Location = new Point(20, 85),
                Size = new Size(445, 80)
            };
            Button callButton = Theme.PrimaryButton("Call Next Patient  ➜", 200, 40);
            callButton.Location = new Point(20, 172);
            Button detailsButton = Theme.SecondaryButton("View Details", 130, 40);
            detailsButton.Location = new Point(230, 172);

            nextCard.Controls.AddRange(new Control[] { nextName, nextBadge, nextInfo, callButton, detailsButton });

            // Recent activity card
            Panel activityCard = Theme.Card(535, 285, 495, 230);
            content.Controls.Add(activityCard);
            activityCard.Controls.Add(Theme.MakeLabel("RECENT ACTIVITY (THIS SESSION)", 9, FontStyle.Bold, Theme.Muted, 20, 15));

            ListView activity = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                BorderStyle = BorderStyle.None,
                Location = new Point(15, 42),
                Size = new Size(465, 175),
                Font = Theme.F(9.5f)
            };
            activity.Columns.Add("Time", 70);
            activity.Columns.Add("Patient", 185);
            activity.Columns.Add("Priority", 80);
            activity.Columns.Add("Status", 110);
            activityCard.Controls.Add(activity);

            // Quick actions
            content.Controls.Add(Theme.MakeLabel("Quick Actions", 14, FontStyle.Bold, Theme.Navy, 30, 535));

            Button registerButton = Theme.PrimaryButton("✚  Register New Patient", 230, 48);
            registerButton.Location = new Point(30, 575);
            registerButton.Click += (s, e) => Navigate(PageRegister);

            Button queueButton = Theme.SecondaryButton("View Waiting Queue", 200, 48);
            queueButton.Location = new Point(275, 575);
            queueButton.Click += (s, e) => Navigate(PageQueue);

            Button historyButton = Theme.SecondaryButton("Patient History", 180, 48);
            historyButton.Location = new Point(490, 575);
            historyButton.Click += (s, e) => Navigate(PageHistory);

            Button saveButton = Theme.SecondaryButton("💾  Save History", 180, 48);
            saveButton.Location = new Point(685, 575);
            saveButton.Click += (s, e) => { SaveHistoryWithMessage(); refresh(); };

            content.Controls.AddRange(new Control[] { registerButton, queueButton, historyButton, saveButton });

            // ---- refresh logic (also called by the timer) ----
            void refresh()
            {
                totalValue.Text = engine.GetWaitingCount().ToString();
                redValue.Text = engine.GetWaitingCount(PriorityLevel.Red).ToString();
                yellowValue.Text = engine.GetWaitingCount(PriorityLevel.Yellow).ToString();
                greenValue.Text = engine.GetWaitingCount(PriorityLevel.Green).ToString();

                registeredValue.Text = engine.GetSessionHistory().Count.ToString();
                calledValue.Text = engine.GetCalledCount().ToString();
                avgWaitValue.Text = engine.GetWaitingCount() == 0 ? "–" : Theme.FormatWait(engine.GetAverageWaitingTime());
                staffValue.Text = $"{engine.GetOnDutyStaff().Count} / {engine.GetStaff().Count}";

                Patient? next = engine.PeekNextPatient();
                if (next == null)
                {
                    nextStripe.BackColor = Theme.Border;
                    nextName.Text = "No patients waiting";
                    nextBadge.Visible = false;
                    nextInfo.Text = "The waiting queue is empty. Register a new patient to begin triage.";
                    callButton.Enabled = false;
                    detailsButton.Enabled = false;
                }
                else
                {
                    nextStripe.BackColor = Theme.PriorityColor(next.Priority);
                    nextName.Text = next.Name.Length > 26 ? next.Name.Substring(0, 26) + "…" : next.Name;
                    nextBadge.Visible = true;
                    nextBadge.Text = next.Priority.ToString().ToUpper();
                    nextBadge.BackColor = Theme.PriorityColor(next.Priority);
                    nextInfo.Text =
                        $"ID {next.PersonID}  •  Age {next.Age}  •  {next.Gender}\n" +
                        $"HR {next.Vitals.HeartRate} bpm  •  BP {next.Vitals.SystolicBP} mmHg\n" +
                        $"Symptoms: {string.Join(", ", next.Symptoms)}\n" +
                        $"Waiting: {Theme.FormatWait(next.GetWaitingDuration())}";
                    callButton.Enabled = true;
                    detailsButton.Enabled = true;
                }

                activity.BeginUpdate();
                activity.Items.Clear();
                foreach (TriageHistoryEntry h in engine.GetSessionHistory().OrderByDescending(h => h.RegisteredAt).Take(20))
                {
                    var item = new ListViewItem(h.RegisteredAt.ToString("HH:mm"));
                    item.SubItems.Add($"{h.PatientName} ({h.PatientId})");
                    item.SubItems.Add(h.Priority.ToString());
                    item.SubItems.Add(h.Status == "Called" && h.CalledAt.HasValue ? $"Called {h.CalledAt:HH:mm}" : "Waiting");
                    item.UseItemStyleForSubItems = false;
                    item.SubItems[2].ForeColor = Theme.PriorityColor(h.Priority);
                    item.SubItems[2].Font = Theme.F(9.5f, FontStyle.Bold);
                    activity.Items.Add(item);
                }
                if (activity.Items.Count == 0)
                    activity.Items.Add(new ListViewItem(new[] { "", "No activity yet", "", "" }));
                activity.EndUpdate();
            }

            callButton.Click += (s, e) => { CallNextPatientWithMessage(); refresh(); };
            detailsButton.Click += (s, e) =>
            {
                Patient? next = engine.PeekNextPatient();
                if (next != null) ShowPatientDetails(next);
            };

            refresh();
            pageTick = () => { if (tickCount % 5 == 0) refresh(); };
        }

        private static Label AddSummaryCard(Panel parent, string title, string detail, Color accent, int x, int y)
        {
            Panel card = Theme.Card(x, y, 235, 115);
            parent.Controls.Add(card);

            card.Controls.Add(new Panel { Location = new Point(0, 0), Size = new Size(235, 5), BackColor = accent });
            card.Controls.Add(Theme.MakeLabel(title, 9, FontStyle.Bold, Theme.Muted, 18, 18));

            Label value = Theme.MakeLabel("0", 26, FontStyle.Bold, accent, 15, 38);
            card.Controls.Add(value);
            card.Controls.Add(Theme.MakeLabel(detail, 9, FontStyle.Regular, Color.Gray, 18, 85));
            return value;
        }

        private static Label AddMiniStat(Panel parent, string title, int x, int y)
        {
            Panel card = Theme.Card(x, y, 235, 75);
            parent.Controls.Add(card);
            card.Controls.Add(Theme.MakeLabel(title, 9, FontStyle.Regular, Theme.Muted, 18, 12));
            Label value = Theme.MakeLabel("0", 16, FontStyle.Bold, Theme.Navy, 16, 33);
            card.Controls.Add(value);
            return value;
        }
    }
}
