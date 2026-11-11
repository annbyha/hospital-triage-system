using System.Text;
using HospitalTriage;

namespace HospitalTriageGUI
{
    public partial class Form1
    {
        /// <summary>Current-session and saved (history.json) records with search, filter, save and CSV export.</summary>
        private void ShowPatientHistory()
        {
            Panel content = CreatePage(PageHistory, "Search the triage records of this session and previously saved sessions", scrollable: false);
            bool showSaved = false;

            // ---------- toolbar ----------
            Panel toolbar = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Theme.Background };

            Button sessionTab = Theme.SecondaryButton("Current Session", 165, 40);
            sessionTab.Location = new Point(0, 8);
            Button savedTab = Theme.SecondaryButton("Saved History", 165, 40);
            savedTab.Location = new Point(165, 8);

            TextBox search = new TextBox
            {
                Location = new Point(0, 8),
                Size = new Size(220, 30),
                Font = Theme.F(10),
                PlaceholderText = "🔍 Search name or ID…"
            };
            ComboBox priorityFilter = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(235, 8),
                Size = new Size(140, 30),
                Font = Theme.F(10)
            };
            priorityFilter.Items.AddRange(new object[] { "All priorities", "Red", "Yellow", "Green" });
            priorityFilter.SelectedIndex = 0;

            Button saveButton = Theme.PrimaryButton("💾 Save History", 150, 40);
            Button exportButton = Theme.SecondaryButton("Export CSV", 115, 40);
            Button clearButton = Theme.MakeButton("Clear Saved", Color.White, Theme.Red, 115, 40);

            toolbar.Controls.AddRange(new Control[] { sessionTab, savedTab, saveButton, exportButton, clearButton });

            // second row: search, filter and record count
            Panel filterBar = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Theme.Background };
            Label info = new Label
            {
                Location = new Point(390, 8),
                Size = new Size(560, 30),
                AutoEllipsis = true,
                Font = Theme.F(9.5f),
                ForeColor = Theme.Muted,
                TextAlign = ContentAlignment.MiddleLeft
            };
            filterBar.Controls.AddRange(new Control[] { search, priorityFilter, info });

            DataGridView grid = new DataGridView { Dock = DockStyle.Fill };
            Theme.StyleGrid(grid);
            Theme.AddColumns(grid,
                ("ID", 50), ("Name", 140), ("Age", 40), ("Gender", 60), ("Priority", 65), ("HR", 55), ("BP", 55),
                ("Symptoms", 190), ("Registered", 115), ("Status", 60), ("Called At", 70), ("Waited", 65));

            content.Controls.Add(grid);
            content.Controls.Add(filterBar);
            content.Controls.Add(toolbar);

            void positionButtons()
            {
                saveButton.Location = new Point(toolbar.Width - saveButton.Width, 8);
                exportButton.Location = new Point(saveButton.Left - exportButton.Width - 10, 8);
                clearButton.Location = new Point(exportButton.Left - clearButton.Width - 10, 8);
            }
            toolbar.Resize += (s, e) => positionButtons();

            IEnumerable<TriageHistoryEntry> filteredRecords()
            {
                IEnumerable<TriageHistoryEntry> records = showSaved
                    ? engine.GetSavedHistory()
                    : engine.GetSessionHistory().OrderByDescending(h => h.RegisteredAt);

                string term = search.Text.Trim();
                if (term.Length > 0)
                    records = records.Where(h =>
                        h.PatientName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        h.PatientId.ToString().Contains(term));

                if (priorityFilter.SelectedIndex > 0)
                {
                    var level = (PriorityLevel)priorityFilter.SelectedIndex; // 1 = Red, 2 = Yellow, 3 = Green
                    records = records.Where(h => h.Priority == level);
                }
                return records.ToList();
            }

            void refresh()
            {
                void styleTab(Button b, bool active)
                {
                    b.BackColor = active ? Theme.Navy : Color.White;
                    b.ForeColor = active ? Color.White : Theme.Navy;
                    b.Font = Theme.F(10, active ? FontStyle.Bold : FontStyle.Regular);
                }
                styleTab(sessionTab, !showSaved);
                styleTab(savedTab, showSaved);
                sessionTab.Text = $"Current Session ({engine.GetSessionHistory().Count})";
                savedTab.Text = $"Saved History ({engine.GetSavedHistory().Count})";
                clearButton.Visible = showSaved;

                var records = filteredRecords();
                grid.Rows.Clear();
                foreach (TriageHistoryEntry h in records)
                {
                    string waited = h.CalledAt.HasValue
                        ? Theme.FormatWait(h.CalledAt.Value - h.RegisteredAt)
                        : (showSaved ? "–" : Theme.FormatWait(DateTime.Now - h.RegisteredAt));
                    int i = grid.Rows.Add(h.PatientId, h.PatientName, h.Age, h.Gender, h.Priority.ToString().ToUpper(),
                        h.HeartRate, h.SystolicBP, h.Symptoms, h.RegisteredAt.ToString("dd/MM/yyyy HH:mm"),
                        h.Status, h.CalledAt?.ToString("HH:mm") ?? "–", waited);
                    Theme.ColourRow(grid.Rows[i], h.Priority, "Priority");
                    grid.Rows[i].Cells["Status"].Style.Font = Theme.F(9.5f, FontStyle.Bold);
                    grid.Rows[i].Cells["Status"].Style.ForeColor = h.Status == "Called" ? Theme.Green : Theme.Yellow;
                    grid.Rows[i].Cells["Status"].Style.SelectionForeColor = h.Status == "Called" ? Theme.Green : Theme.Yellow;
                }

                string source = showSaved ? $"Saved file: {engine.HistoryFilePath}" : "Since the application started";
                string unsaved = !showSaved && engine.HasUnsavedChanges ? "   •   ● Not saved yet" : "";
                info.Text = $"Showing {records.Count()} record(s)   •   {source}{unsaved}";
                UpdateSidebarStatus();
            }

            sessionTab.Click += (s, e) => { showSaved = false; refresh(); };
            savedTab.Click += (s, e) => { showSaved = true; refresh(); };
            search.TextChanged += (s, e) => refresh();
            priorityFilter.SelectedIndexChanged += (s, e) => refresh();

            saveButton.Click += (s, e) => { SaveHistoryWithMessage(); refresh(); };

            exportButton.Click += (s, e) =>
            {
                var records = filteredRecords().ToList();
                if (records.Count == 0)
                {
                    ShowInfo("There are no records to export.", "Export CSV");
                    return;
                }
                using SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "CSV file (*.csv)|*.csv",
                    FileName = $"triage-history-{DateTime.Now:yyyyMMdd-HHmm}.csv"
                };
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    File.WriteAllText(dialog.FileName, BuildCsv(records), Encoding.UTF8);
                    ShowInfo($"Exported {records.Count} record(s) to:\n{dialog.FileName}", "Export Complete");
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    ShowError("Could not export: " + ex.Message);
                }
            };

            clearButton.Click += (s, e) =>
            {
                if (MessageBox.Show("Permanently delete ALL saved history (history.json)?\nThe current session is not affected.",
                        "Clear Saved History", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                try { engine.ClearSavedHistory(); }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { ShowError(ex.Message); }
                refresh();
            };

            positionButtons();
            refresh();
        }

        private static string BuildCsv(IEnumerable<TriageHistoryEntry> records)
        {
            static string esc(string? v)
            {
                v ??= string.Empty;
                return v.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0 ? "\"" + v.Replace("\"", "\"\"") + "\"" : v;
            }

            var sb = new StringBuilder();
            sb.AppendLine("PatientId,Name,Age,Gender,Priority,HeartRate,SystolicBP,Symptoms,RegisteredAt,Status,CalledAt");
            foreach (var h in records)
            {
                sb.AppendLine(string.Join(",",
                    h.PatientId, esc(h.PatientName), h.Age, esc(h.Gender), h.Priority, h.HeartRate, h.SystolicBP,
                    esc(h.Symptoms), h.RegisteredAt.ToString("yyyy-MM-dd HH:mm:ss"), h.Status,
                    h.CalledAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""));
            }
            return sb.ToString();
        }
    }
}
