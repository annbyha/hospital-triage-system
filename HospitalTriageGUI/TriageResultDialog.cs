using HospitalTriage;

namespace HospitalTriageGUI
{
    /// <summary>
    /// Pop-up shown after a patient is registered: priority, reasons, queue position and staff alerts.
    /// Returns DialogResult.Yes for "View Queue" and DialogResult.No for "Register Another".
    /// </summary>
    internal class TriageResultDialog : Form
    {
        public TriageResultDialog(Patient patient, List<string> reasons, List<string> alerts, int position, int totalWaiting)
        {
            bool isRed = patient.Priority == PriorityLevel.Red;

            Text = "Triage Result";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.White;
            Font = Theme.F(10);
            ClientSize = new Size(520, isRed ? 520 : 400);

            Panel band = new Panel { Dock = DockStyle.Top, Height = 110, BackColor = Theme.PriorityColor(patient.Priority) };
            band.Controls.Add(Theme.MakeLabel("TRIAGE RESULT", 9, FontStyle.Bold, Color.White, 25, 15));
            band.Controls.Add(Theme.MakeLabel($"{patient.Priority.ToString().ToUpper()} PRIORITY", 24, FontStyle.Bold, Color.White, 22, 33));
            band.Controls.Add(Theme.MakeLabel(Theme.PriorityDescription(patient.Priority), 10, FontStyle.Regular, Color.White, 25, 78));
            Controls.Add(band);

            int y = 125;
            Label who = Theme.MakeLabel($"{patient.Name}   (ID {patient.PersonID}, Age {patient.Age}, {patient.Gender})",
                11, FontStyle.Bold, Theme.Navy, 25, y);
            Controls.Add(who);
            y += 30;

            Controls.Add(Theme.MakeLabel($"Queue position: {position} of {totalWaiting}", 10, FontStyle.Regular, Theme.Muted, 25, y));
            y += 35;

            Controls.Add(Theme.MakeLabel("Reason for priority", 10, FontStyle.Bold, Theme.Navy, 25, y));
            y += 25;
            Label reasonText = new Label
            {
                Text = "• " + string.Join("\n• ", reasons),
                Location = new Point(25, y),
                Size = new Size(470, 75),
                ForeColor = Theme.Navy
            };
            Controls.Add(reasonText);
            y += 80;

            if (isRed)
            {
                Panel alertBox = new Panel
                {
                    Location = new Point(25, y),
                    Size = new Size(470, 120),
                    BackColor = Theme.RedTint
                };
                alertBox.Controls.Add(Theme.MakeLabel("⚠  STAFF ALERT SENT", 10, FontStyle.Bold, Theme.Red, 12, 10));
                Label alertText = new Label
                {
                    Text = alerts.Count == 0
                        ? "WARNING: no staff are currently on duty! Please assign staff immediately."
                        : string.Join("\n", alerts.Select(a => "✓ " + a)),
                    Location = new Point(12, 35),
                    Size = new Size(450, 80),
                    Font = Theme.F(9),
                    ForeColor = Theme.Navy
                };
                alertBox.Controls.Add(alertText);
                Controls.Add(alertBox);
                y += 135;
            }

            Button queueButton = Theme.PrimaryButton("View Waiting Queue", 200, 42);
            queueButton.Location = new Point(25, y + 5);
            queueButton.DialogResult = DialogResult.Yes;

            Button anotherButton = Theme.SecondaryButton("Register Another Patient", 220, 42);
            anotherButton.Location = new Point(240, y + 5);
            anotherButton.DialogResult = DialogResult.No;

            Controls.Add(queueButton);
            Controls.Add(anotherButton);
            AcceptButton = anotherButton;
            CancelButton = anotherButton;

            if (isRed) System.Media.SystemSounds.Exclamation.Play();
        }
    }
}
