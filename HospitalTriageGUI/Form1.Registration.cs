using System.Text.RegularExpressions;
using HospitalTriage;

namespace HospitalTriageGUI
{
    public partial class Form1
    {
        // Common (non-severe) symptoms offered as tick boxes. Severe symptoms come from SevereSymptomRules.
        private static readonly string[] CommonSymptoms =
        {
            "Fever", "Headache", "Cough", "Sore throat", "Nausea / vomiting",
            "Abdominal pain", "Dizziness", "Fatigue", "Rash", "Minor injury", "Back pain"
        };

        private static readonly Regex NamePattern = new(@"^[A-Za-z][A-Za-z\s'.\-/@]*$");

        /// <summary>Patient registration form with live triage preview.</summary>
        private void ShowPatientRegistration()
        {
            Panel content = CreatePage(PageRegister, "Enter patient information, vital signs and symptoms – the priority is calculated automatically");
            ErrorProvider errors = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

            // ================= Left card: the form =================
            Panel form = Theme.Card(30, 15, 610, 640);
            content.Controls.Add(form);
            form.Controls.Add(Theme.MakeLabel("Patient Information", 13, FontStyle.Bold, Theme.Navy, 25, 15));

            TextBox idBox = AddField(form, "Patient ID", 25, 55);
            idBox.Text = engine.GetNextPatientId().ToString();
            TextBox nameBox = AddField(form, "Full Name", 315, 55);
            nameBox.PlaceholderText = "e.g. Ahmad bin Ismail";
            TextBox ageBox = AddField(form, "Age (years)", 25, 125);
            ageBox.PlaceholderText = "0 - 130";

            form.Controls.Add(Theme.MakeLabel("Gender", 10, FontStyle.Bold, Theme.Navy, 315, 125));
            ComboBox genderBox = new ComboBox
            {
                Location = new Point(315, 150),
                Size = new Size(260, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = Theme.F(10)
            };
            genderBox.Items.AddRange(new object[] { "Male", "Female" });
            form.Controls.Add(genderBox);

            form.Controls.Add(Theme.MakeLabel("Vital Signs", 13, FontStyle.Bold, Theme.Navy, 25, 200));
            TextBox heartBox = AddField(form, "Heart Rate (bpm)", 25, 235);
            TextBox bpBox = AddField(form, "Systolic Blood Pressure (mmHg)", 315, 235);
            Label heartHint = Theme.MakeLabel("", 8.5f, FontStyle.Regular, Theme.Muted, 25, 290);
            Label bpHint = Theme.MakeLabel("", 8.5f, FontStyle.Regular, Theme.Muted, 315, 290);
            form.Controls.Add(heartHint);
            form.Controls.Add(bpHint);
            heartHint.Text = $"Normal range: {engine.MinHeartRate} – {engine.MaxHeartRate} bpm";
            bpHint.Text = $"Normal range: {engine.MinSystolicBP} – {engine.MaxSystolicBP} mmHg";

            form.Controls.Add(Theme.MakeLabel("Symptoms", 13, FontStyle.Bold, Theme.Navy, 25, 320));
            form.Controls.Add(Theme.MakeLabel("Tick all that apply.  Red = severe symptom (automatic RED priority)", 8.5f, FontStyle.Regular, Theme.Muted, 125, 326));

            FlowLayoutPanel symptomPanel = new FlowLayoutPanel
            {
                Location = new Point(25, 352),
                Size = new Size(560, 145),
                AutoScroll = true,
                BackColor = Color.FromArgb(250, 251, 253),
                Padding = new Padding(4)
            };
            form.Controls.Add(symptomPanel);

            var symptomChecks = new List<CheckBox>();
            var allSymptoms = engine.Rules.GetSevereSymptoms()
                .Concat(CommonSymptoms.Where(c => !engine.Rules.IsSevereSymptom(c)));
            foreach (string symptom in allSymptoms)
            {
                bool severe = engine.Rules.IsSevereSymptom(symptom);
                CheckBox cb = new CheckBox
                {
                    Text = symptom,
                    Width = 168,
                    Height = 28,
                    Font = Theme.F(9.5f, severe ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = severe ? Theme.Red : Theme.Navy
                };
                symptomChecks.Add(cb);
                symptomPanel.Controls.Add(cb);
            }

            form.Controls.Add(Theme.MakeLabel("Other symptoms (separate with commas)", 10, FontStyle.Bold, Theme.Navy, 25, 507));
            TextBox otherBox = new TextBox
            {
                Location = new Point(25, 532),
                Size = new Size(560, 30),
                Font = Theme.F(10),
                PlaceholderText = "e.g. Blurred vision, Ear pain"
            };
            form.Controls.Add(otherBox);

            Button registerButton = Theme.PrimaryButton("✚  REGISTER PATIENT", 220, 46);
            registerButton.Location = new Point(25, 578);
            Button clearButton = Theme.SecondaryButton("Clear Form", 130, 46);
            clearButton.Location = new Point(260, 578);
            form.Controls.Add(registerButton);
            form.Controls.Add(clearButton);
            AcceptButton = registerButton;

            // ================= Right card: live triage preview =================
            Panel preview = Theme.Card(660, 15, 370, 640);
            content.Controls.Add(preview);
            preview.Controls.Add(Theme.MakeLabel("Live Triage Preview", 13, FontStyle.Bold, Theme.Navy, 22, 15));
            preview.Controls.Add(Theme.MakeLabel("Updates as you type", 9, FontStyle.Regular, Theme.Muted, 24, 42));

            Label pState = AddRuleRow(preview, "P", "Severe symptom present", 80);
            Label qState = AddRuleRow(preview, "Q", "Heart rate abnormal", 130);
            Label rState = AddRuleRow(preview, "R", "Systolic BP abnormal", 180);

            Panel resultBox = new Panel { Location = new Point(22, 240), Size = new Size(326, 110), BackColor = Theme.Border };
            preview.Controls.Add(resultBox);
            Label resultTitle = new Label
            {
                Font = Theme.F(22, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 18),
                Size = new Size(326, 45)
            };
            Label resultSub = new Label
            {
                Font = Theme.F(10),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 65),
                Size = new Size(326, 25)
            };
            resultBox.Controls.Add(resultTitle);
            resultBox.Controls.Add(resultSub);

            Label reasonLabel = new Label
            {
                Location = new Point(22, 365),
                Size = new Size(326, 120),
                Font = Theme.F(9.5f),
                ForeColor = Theme.Navy
            };
            preview.Controls.Add(reasonLabel);

            Label legend = new Label
            {
                Location = new Point(22, 495),
                Size = new Size(326, 130),
                Font = Theme.F(8.5f),
                ForeColor = Theme.Muted,
                Text = "Classification rules\n" +
                       "RED     = P  OR  (Q AND R)\n" +
                       "YELLOW  = Q  OR  R   (when not Red)\n" +
                       "GREEN   = none of P, Q or R\n\n" +
                       "RED patients trigger an alert to all on-duty staff.\n" +
                       "Educational rules only – not clinical guidance."
            };
            preview.Controls.Add(legend);

            // ================= Logic =================
            List<string> collectSymptoms()
            {
                var list = symptomChecks.Where(c => c.Checked).Select(c => c.Text).ToList();
                list.AddRange(otherBox.Text.Split(',').Select(x => x.Trim()).Where(x => x.Length > 0));
                return list.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            }

            void setState(Label label, bool? value)
            {
                label.Text = value == null ? "—" : value.Value ? "YES" : "NO";
                label.BackColor = value == null ? Theme.Border : value.Value ? Theme.Red : Theme.Green;
            }

            void updatePreview()
            {
                var symptoms = collectSymptoms();
                bool hasHeart = int.TryParse(heartBox.Text, out int hr) && hr > 0;
                bool hasBp = int.TryParse(bpBox.Text, out int bp) && bp > 0;
                var vitals = new VitalSigns(hasHeart ? hr : engine.MinHeartRate, hasBp ? bp : engine.MinSystolicBP);

                bool p = engine.EvaluateP(symptoms);
                bool q = hasHeart && engine.EvaluateQ(vitals);
                bool r = hasBp && engine.EvaluateR(vitals);
                setState(pState, symptoms.Count == 0 ? null : p);
                setState(qState, hasHeart ? q : null);
                setState(rState, hasBp ? r : null);

                if (!hasHeart && !hasBp && symptoms.Count == 0)
                {
                    resultBox.BackColor = Theme.Border;
                    resultTitle.Text = "–";
                    resultTitle.ForeColor = Theme.Muted;
                    resultSub.Text = "Enter vitals and symptoms";
                    resultSub.ForeColor = Theme.Muted;
                    reasonLabel.Text = "";
                    return;
                }

                PriorityLevel level = engine.Classify(p, q, r);
                resultBox.BackColor = Theme.PriorityColor(level);
                resultTitle.ForeColor = Color.White;
                resultSub.ForeColor = Color.White;
                resultTitle.Text = level.ToString().ToUpper();
                resultSub.Text = Theme.PriorityDescription(level) + (hasHeart && hasBp ? "" : " (vitals incomplete)");

                var reasons = new List<string>();
                if (p) reasons.Add("Severe symptom: " + string.Join(", ", symptoms.Where(engine.Rules.IsSevereSymptom)));
                if (q) reasons.Add($"Heart rate {hr} bpm is outside {engine.MinHeartRate}–{engine.MaxHeartRate}");
                if (r) reasons.Add($"Systolic BP {bp} mmHg is outside {engine.MinSystolicBP}–{engine.MaxSystolicBP}");
                if (reasons.Count == 0) reasons.Add("No severe symptoms; vital signs normal so far.");
                reasonLabel.Text = "Why?\n• " + string.Join("\n• ", reasons);
            }

            foreach (CheckBox cb in symptomChecks) cb.CheckedChanged += (s, e) => updatePreview();
            heartBox.TextChanged += (s, e) => updatePreview();
            bpBox.TextChanged += (s, e) => updatePreview();
            otherBox.TextChanged += (s, e) => updatePreview();

            void clearForm()
            {
                errors.Clear();
                idBox.Text = engine.GetNextPatientId().ToString();
                nameBox.Clear();
                ageBox.Clear();
                genderBox.SelectedIndex = -1;
                heartBox.Clear();
                bpBox.Clear();
                otherBox.Clear();
                foreach (CheckBox cb in symptomChecks) cb.Checked = false;
                updatePreview();
                nameBox.Focus();
            }

            clearButton.Click += (s, e) => clearForm();

            registerButton.Click += (s, e) =>
            {
                errors.Clear();
                var problems = new List<string>();

                void fail(Control c, string message)
                {
                    errors.SetError(c, message);
                    problems.Add(message);
                }

                if (!int.TryParse(idBox.Text.Trim(), out int id) || id <= 0)
                    fail(idBox, "Patient ID must be a positive whole number.");
                else if (engine.IsPatientIdInUse(id))
                    fail(idBox, $"Patient ID {id} is already registered in this session.");

                string name = Regex.Replace(nameBox.Text.Trim(), @"\s+", " ");
                if (name.Length == 0)
                    fail(nameBox, "Name cannot be empty.");
                else if (name.Length < 2 || name.Length > 60 || !NamePattern.IsMatch(name))
                    fail(nameBox, "Name must be 2–60 characters and contain letters only (spaces, ' . - / @ allowed).");

                if (!int.TryParse(ageBox.Text.Trim(), out int age) || age < 0 || age > 130)
                    fail(ageBox, "Age must be a whole number between 0 and 130.");

                if (genderBox.SelectedIndex < 0)
                    fail(genderBox, "Please select a gender.");

                if (!int.TryParse(heartBox.Text.Trim(), out int heartRate) || heartRate < 1 || heartRate > 300)
                    fail(heartBox, "Heart rate must be a whole number between 1 and 300 bpm.");

                if (!int.TryParse(bpBox.Text.Trim(), out int systolic) || systolic < 1 || systolic > 300)
                    fail(bpBox, "Systolic BP must be a whole number between 1 and 300 mmHg.");

                var symptoms = collectSymptoms();
                if (symptoms.Count == 0)
                    fail(otherBox, "Please tick or enter at least one symptom.");

                if (problems.Count > 0)
                {
                    MessageBox.Show("Please correct the following:\n\n• " + string.Join("\n• ", problems),
                        "Invalid Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    var patient = new Patient(id, name, age, new VitalSigns(heartRate, systolic))
                    {
                        Gender = genderBox.SelectedItem!.ToString()!
                    };
                    foreach (string symptom in symptoms) patient.AddSymptom(symptom);

                    PriorityLevel priority = engine.TriagePatient(patient);
                    List<string> alerts = priority == PriorityLevel.Red ? engine.AlertStaff(patient) : new List<string>();
                    int position = engine.GetWaitingPatients().FindIndex(p => p.PersonID == patient.PersonID) + 1;

                    UpdateSidebarStatus();

                    using var dialog = new TriageResultDialog(patient,
                        engine.ExplainTriage(patient.Symptoms, patient.Vitals), alerts, position, engine.GetWaitingCount());
                    DialogResult choice = dialog.ShowDialog(this);

                    if (choice == DialogResult.Yes) Navigate(PageQueue);
                    else clearForm();
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    ShowError(ex.Message, "Registration Failed");
                }
            };

            updatePreview();
            nameBox.Focus();
        }

        private static TextBox AddField(Panel parent, string label, int x, int y)
        {
            parent.Controls.Add(Theme.MakeLabel(label, 10, FontStyle.Bold, Theme.Navy, x, y));
            TextBox box = new TextBox
            {
                Location = new Point(x, y + 25),
                Size = new Size(260, 30),
                Font = Theme.F(10.5f)
            };
            parent.Controls.Add(box);
            return box;
        }

        private static Label AddRuleRow(Panel parent, string letter, string text, int y)
        {
            Label badge = new Label
            {
                Text = letter,
                Font = Theme.F(12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Theme.Navy,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(22, y),
                Size = new Size(36, 36)
            };
            parent.Controls.Add(badge);
            parent.Controls.Add(Theme.MakeLabel(text, 10, FontStyle.Regular, Theme.Navy, 70, y + 8));

            Label state = new Label
            {
                Text = "—",
                Font = Theme.F(9, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Theme.Border,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(278, y + 5),
                Size = new Size(70, 26)
            };
            parent.Controls.Add(state);
            return state;
        }
    }
}
