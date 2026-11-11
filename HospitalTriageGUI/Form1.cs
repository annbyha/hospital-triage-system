using HospitalTriage;

namespace HospitalTriageGUI
{
    /// <summary>
    /// Main window. This file holds the shell (sidebar, header, navigation, timer and closing logic).
    /// Each screen is built in its own partial-class file:
    ///   Form1.Dashboard.cs, Form1.Registration.cs, Form1.Queue.cs,
    ///   Form1.History.cs, Form1.Staff.cs, Form1.Rules.cs
    /// </summary>
    public partial class Form1 : Form
    {
        private readonly TriageEngine engine;

        private Panel mainContent = null!;
        private Label sidebarStatus = null!;
        private Label? headerClock;
        private readonly Dictionary<string, Button> menuButtons = new();
        private string activePage = string.Empty;

        // Live refresh: called once per second for the page currently on screen.
        private readonly System.Windows.Forms.Timer refreshTimer = new() { Interval = 1000 };
        private Action? pageTick;
        private int tickCount;

        private const string PageDashboard = "Dashboard";
        private const string PageRegister = "Patient Registration";
        private const string PageQueue = "Waiting Queue";
        private const string PageHistory = "Patient History";
        private const string PageStaff = "Staff & Alerts";
        private const string PageRules = "Triage Rules";

        public Form1(TriageEngine engine)
        {
            this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
            InitializeComponent();

            Text = "Hospital Patient Triage System";
            Size = new Size(1320, 840);
            MinimumSize = new Size(1250, 650);
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            BackColor = Theme.Background;
            Font = Theme.F(10);

            BuildInterface();

            refreshTimer.Tick += (s, e) =>
            {
                tickCount++;
                if (headerClock != null) headerClock.Text = DateTime.Now.ToString("dddd, dd MMM yyyy   HH:mm:ss");
                try { pageTick?.Invoke(); } catch (ObjectDisposedException) { }
            };
            refreshTimer.Start();

            FormClosing += Form1_FormClosing;
        }

        // ------------------------------------------------------------------ Shell

        private void BuildInterface()
        {
            mainContent = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };
            Panel sidebar = new Panel { Dock = DockStyle.Left, Width = 240, BackColor = Theme.Navy };

            // Fill control is added first so the docked sidebar is laid out before it.
            Controls.Add(mainContent);
            Controls.Add(sidebar);

            BuildSidebar(sidebar);
            Navigate(PageDashboard);
        }

        private void BuildSidebar(Panel sidebar)
        {
            Label icon = new Label
            {
                Text = "+",
                Font = Theme.F(26, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Theme.Red,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(22, 28),
                Size = new Size(50, 50)
            };
            sidebar.Controls.Add(icon);

            Label logo = new Label
            {
                Text = "HOSPITAL\nTRIAGE SYSTEM",
                ForeColor = Color.White,
                Font = Theme.F(12.5f, FontStyle.Bold),
                Location = new Point(82, 28),
                Size = new Size(155, 52),
                TextAlign = ContentAlignment.MiddleLeft
            };
            sidebar.Controls.Add(logo);

            sidebar.Controls.Add(Theme.MakeLabel("MAIN MENU", 9, FontStyle.Bold, Color.Silver, 22, 115));

            string[] pages = { PageDashboard, PageRegister, PageQueue, PageHistory, PageStaff, PageRules };
            string[] icons = { "■", "✚", "☰", "⟲", "☺", "⚙" };
            int top = 145;
            for (int i = 0; i < pages.Length; i++)
            {
                AddMenuButton(sidebar, icons[i] + "   " + pages[i], pages[i], top);
                top += 52;
            }

            sidebarStatus = new Label
            {
                ForeColor = Color.LightGray,
                Font = Theme.F(8.5f),
                Dock = DockStyle.Bottom,
                Height = 105,
                Padding = new Padding(22, 0, 10, 15)
            };
            sidebar.Controls.Add(sidebarStatus);
            UpdateSidebarStatus();
        }

        private void AddMenuButton(Panel sidebar, string text, string page, int top)
        {
            text = text.Replace("&", "&&"); // "&" would otherwise be shown as a keyboard shortcut underline
            Button button = new Button
            {
                Text = text,
                Tag = text,
                Location = new Point(10, top),
                Size = new Size(220, 46),
                FlatStyle = FlatStyle.Flat,
                BackColor = Theme.Navy,
                ForeColor = Color.White,
                Font = Theme.F(10),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.MouseEnter += (s, e) => { if (activePage != page) button.BackColor = Theme.NavyHover; };
            button.MouseLeave += (s, e) => { if (activePage != page) button.BackColor = Theme.Navy; };
            button.Click += (s, e) => Navigate(page);

            menuButtons[page] = button;
            sidebar.Controls.Add(button);
        }

        /// <summary>Switches the main area to another screen.</summary>
        private void Navigate(string page)
        {
            activePage = page;
            pageTick = null;
            headerClock = null;
            AcceptButton = null;

            foreach (var pair in menuButtons)
            {
                bool active = pair.Key == page;
                pair.Value.BackColor = active ? Theme.NavyActive : Theme.Navy;
                pair.Value.Font = Theme.F(10, active ? FontStyle.Bold : FontStyle.Regular);
            }

            mainContent.SuspendLayout();
            while (mainContent.Controls.Count > 0)
            {
                Control c = mainContent.Controls[0];
                mainContent.Controls.RemoveAt(0);
                c.Dispose();
            }

            try
            {
                switch (page)
                {
                    case PageDashboard: ShowDashboard(); break;
                    case PageRegister: ShowPatientRegistration(); break;
                    case PageQueue: ShowWaitingQueue(); break;
                    case PageHistory: ShowPatientHistory(); break;
                    case PageStaff: ShowStaff(); break;
                    case PageRules: ShowTriageRules(); break;
                }
            }
            finally
            {
                mainContent.ResumeLayout();
            }
            UpdateSidebarStatus();
        }

        /// <summary>
        /// Creates the white page header and returns the content area below it.
        /// </summary>
        private Panel CreatePage(string title, string subtitle, bool scrollable = true)
        {
            Panel content = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Background,
                AutoScroll = scrollable,
                Padding = new Padding(30, 20, 30, 20)
            };

            Panel header = new Panel { Dock = DockStyle.Top, Height = 92, BackColor = Color.White };
            header.Paint += (s, e) =>
            {
                using var pen = new Pen(Theme.Border);
                e.Graphics.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };

            mainContent.Controls.Add(content);
            mainContent.Controls.Add(header);

            header.Controls.Add(Theme.MakeLabel(title, 20, FontStyle.Bold, Theme.Navy, 28, 14));
            header.Controls.Add(Theme.MakeLabel(subtitle, 10, FontStyle.Regular, Color.Gray, 31, 58));

            headerClock = new Label
            {
                Text = DateTime.Now.ToString("dddd, dd MMM yyyy   HH:mm:ss"),
                Font = Theme.F(10),
                ForeColor = Theme.Muted,
                AutoSize = false,
                Size = new Size(330, 25),
                TextAlign = ContentAlignment.MiddleRight,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            header.Controls.Add(headerClock);
            headerClock.Location = new Point(header.Width - headerClock.Width - 25, 34);

            return content;
        }

        /// <summary>Keeps the sidebar queue badge and save-status text up to date.</summary>
        private void UpdateSidebarStatus()
        {
            if (menuButtons.TryGetValue(PageQueue, out Button? queueButton))
            {
                int waiting = engine.GetWaitingCount();
                string baseText = (string)queueButton.Tag!;
                queueButton.Text = waiting > 0 ? $"{baseText}  ({waiting})" : baseText;
            }

            if (sidebarStatus != null)
            {
                int onDuty = engine.GetOnDutyStaff().Count;
                sidebarStatus.Text =
                    $"Staff on duty: {onDuty}\n" +
                    (engine.HasUnsavedChanges ? "History: ● unsaved changes" : "History: ✓ saved") +
                    "\n\nHospital Management System\nTriage rules are for education only";
            }
        }

        // ------------------------------------------------------------------ Shared helpers

        private static void ShowError(string message, string title = "Error") =>
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

        private static void ShowInfo(string message, string title) =>
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        /// <summary>Calls the next patient from the queue (used by Dashboard and Queue screens).</summary>
        private Patient? CallNextPatientWithMessage()
        {
            Patient? patient = engine.CallNextPatient();
            if (patient == null)
            {
                ShowInfo("There are no patients waiting in the queue.", "Queue Empty");
                return null;
            }

            Doctor? doctor = engine.GetOnDutyStaff().OfType<Doctor>().FirstOrDefault();
            string assigned = doctor != null ? doctor.TreatPatient(patient) : "No doctor is currently on duty.";

            MessageBox.Show(
                $"Now calling: {patient.Name} (ID {patient.PersonID})\n\n" +
                $"Priority: {patient.Priority.ToString().ToUpper()} – {Theme.PriorityDescription(patient.Priority)}\n" +
                $"Waited: {Theme.FormatWait(patient.GetWaitingDuration())}\n\n" +
                assigned,
                "Next Patient Called",
                MessageBoxButtons.OK,
                patient.Priority == PriorityLevel.Red ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

            UpdateSidebarStatus();
            return patient;
        }

        private void SaveHistoryWithMessage()
        {
            try
            {
                engine.SaveHistory();
                UpdateSidebarStatus();
                ShowInfo($"History saved successfully.\n\nFile: {engine.HistoryFilePath}\nTotal saved records: {engine.GetSavedHistory().Count}",
                    "History Saved");
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                ShowError("Could not save history: " + ex.Message);
            }
        }

        private void ShowPatientDetails(Patient p)
        {
            string symptoms = p.Symptoms.Count == 0 ? "None recorded" : string.Join(", ", p.Symptoms);
            string reasons = string.Join("\n  • ", engine.ExplainTriage(p.Symptoms, p.Vitals));
            MessageBox.Show(
                $"Patient ID:   {p.PersonID}\n" +
                $"Name:         {p.Name}\n" +
                $"Age:          {p.Age}\n" +
                $"Gender:       {p.Gender}\n\n" +
                $"Heart rate:   {p.Vitals.HeartRate} bpm\n" +
                $"Systolic BP:  {p.Vitals.SystolicBP} mmHg\n" +
                $"Symptoms:     {symptoms}\n\n" +
                $"Priority:     {p.Priority.ToString().ToUpper()} ({Theme.PriorityDescription(p.Priority)})\n" +
                $"Reason:\n  • {reasons}\n\n" +
                $"Arrived:      {p.ArrivalTime:dd/MM/yyyy HH:mm:ss}\n" +
                $"Waiting:      {Theme.FormatWait(p.GetWaitingDuration())}",
                "Patient Details",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // ------------------------------------------------------------------ Closing

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (!engine.HasUnsavedChanges) return;

            DialogResult answer = MessageBox.Show(
                "You have unsaved patient history from this session.\n\nSave it to history.json before exiting?",
                "Save Before Exit",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (answer == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }

            if (answer == DialogResult.Yes)
            {
                try { engine.SaveHistory(); }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    ShowError("Could not save history: " + ex.Message);
                    e.Cancel = true;
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            refreshTimer.Stop();
            refreshTimer.Dispose();
            base.OnFormClosed(e);
        }
    }
}
