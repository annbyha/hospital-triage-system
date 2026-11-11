using HospitalTriage;

namespace HospitalTriageGUI
{
    public partial class Form1
    {
        /// <summary>Explains the P/Q/R rules and lets the user edit thresholds and the severe-symptom list.</summary>
        private void ShowTriageRules()
        {
            Panel content = CreatePage(PageRules, "How patients are classified – view and adjust the triage rules");

            // ================= Rule explanation =================
            Panel rulesCard = Theme.Card(30, 15, 600, 300);
            content.Controls.Add(rulesCard);
            rulesCard.Controls.Add(Theme.MakeLabel("Triage Conditions", 13, FontStyle.Bold, Theme.Navy, 22, 15));

            Label conditionText = new Label
            {
                Location = new Point(22, 50),
                Size = new Size(560, 80),
                Font = Theme.F(10),
                ForeColor = Theme.Navy
            };
            rulesCard.Controls.Add(conditionText);

            AddClassRow(rulesCard, "RED", Theme.Red, "P  OR  (Q AND R)", "Immediate attention – on-duty staff are alerted", 140);
            AddClassRow(rulesCard, "YELLOW", Theme.Yellow, "Q  OR  R  (when Red does not apply)", "Urgent attention", 190);
            AddClassRow(rulesCard, "GREEN", Theme.Green, "None of P, Q or R", "Non-critical", 240);

            // ================= Thresholds =================
            Panel thresholdCard = Theme.Card(30, 330, 600, 230);
            content.Controls.Add(thresholdCard);
            thresholdCard.Controls.Add(Theme.MakeLabel("Vital-Sign Thresholds (normal range)", 13, FontStyle.Bold, Theme.Navy, 22, 15));

            NumericUpDown minHr = AddNumber(thresholdCard, "Min heart rate (bpm)", 22, 55, engine.MinHeartRate);
            NumericUpDown maxHr = AddNumber(thresholdCard, "Max heart rate (bpm)", 162, 55, engine.MaxHeartRate);
            NumericUpDown minBp = AddNumber(thresholdCard, "Min systolic BP", 302, 55, engine.MinSystolicBP);
            NumericUpDown maxBp = AddNumber(thresholdCard, "Max systolic BP", 442, 55, engine.MaxSystolicBP);

            Button applyButton = Theme.PrimaryButton("Apply Thresholds", 170, 40);
            applyButton.Location = new Point(22, 125);
            Button resetButton = Theme.SecondaryButton("Reset to Default", 160, 40);
            resetButton.Location = new Point(202, 125);
            thresholdCard.Controls.Add(applyButton);
            thresholdCard.Controls.Add(resetButton);

            thresholdCard.Controls.Add(new Label
            {
                Text = "Defaults: heart rate 50–120 bpm, systolic BP 90–180 mmHg.\n" +
                       "New thresholds apply to patients registered from now on.",
                Location = new Point(22, 178),
                Size = new Size(560, 40),
                Font = Theme.F(9),
                ForeColor = Theme.Muted
            });

            // ================= Severe symptoms =================
            Panel severeCard = Theme.Card(650, 15, 380, 400);
            content.Controls.Add(severeCard);
            severeCard.Controls.Add(Theme.MakeLabel("Severe Symptoms (Rule P)", 13, FontStyle.Bold, Theme.Navy, 20, 15));
            severeCard.Controls.Add(Theme.MakeLabel("Any of these makes a patient RED priority.", 9, FontStyle.Regular, Theme.Muted, 22, 45));

            ListBox severeList = new ListBox
            {
                Location = new Point(20, 75),
                Size = new Size(340, 200),
                Font = Theme.F(10.5f),
                ForeColor = Theme.Red,
                BorderStyle = BorderStyle.FixedSingle
            };
            severeCard.Controls.Add(severeList);

            TextBox newSymptom = new TextBox
            {
                Location = new Point(20, 290),
                Size = new Size(230, 30),
                Font = Theme.F(10),
                PlaceholderText = "e.g. Severe bleeding"
            };
            Button addSymptom = Theme.PrimaryButton("Add", 100, 32);
            addSymptom.Location = new Point(260, 288);
            Button removeSymptom = Theme.MakeButton("Remove Selected", Color.White, Theme.Red, 340, 38);
            removeSymptom.Location = new Point(20, 340);
            severeCard.Controls.AddRange(new Control[] { newSymptom, addSymptom, removeSymptom });

            // ================= Demo data =================
            Panel demoCard = Theme.Card(650, 430, 380, 130);
            content.Controls.Add(demoCard);
            demoCard.Controls.Add(Theme.MakeLabel("Demonstration Data", 13, FontStyle.Bold, Theme.Navy, 20, 15));
            demoCard.Controls.Add(Theme.MakeLabel("Adds 6 sample patients (Red, Yellow, Green).", 9, FontStyle.Regular, Theme.Muted, 22, 45));
            Button demoButton = Theme.SecondaryButton("Load Sample Patients", 340, 40);
            demoButton.Location = new Point(20, 75);
            demoCard.Controls.Add(demoButton);

            // ================= Logic =================
            void refresh()
            {
                conditionText.Text =
                    $"P  –  Patient has at least one severe symptom ({engine.Rules.GetSevereSymptoms().Count} defined)\n" +
                    $"Q  –  Heart rate below {engine.MinHeartRate} or above {engine.MaxHeartRate} bpm\n" +
                    $"R  –  Systolic blood pressure below {engine.MinSystolicBP} or above {engine.MaxSystolicBP} mmHg";

                severeList.Items.Clear();
                foreach (string s in engine.Rules.GetSevereSymptoms()) severeList.Items.Add(s);
            }

            applyButton.Click += (s, e) =>
            {
                try
                {
                    engine.UpdateThresholds((int)minHr.Value, (int)maxHr.Value, (int)minBp.Value, (int)maxBp.Value);
                    refresh();
                    ShowInfo("Thresholds updated successfully.", "Triage Rules");
                }
                catch (ArgumentException ex)
                {
                    ShowError(ex.Message, "Invalid Thresholds");
                }
            };

            resetButton.Click += (s, e) =>
            {
                minHr.Value = 50; maxHr.Value = 120; minBp.Value = 90; maxBp.Value = 180;
                engine.UpdateThresholds(50, 120, 90, 180);
                refresh();
            };

            addSymptom.Click += (s, e) =>
            {
                string symptom = newSymptom.Text.Trim();
                if (symptom.Length < 3)
                {
                    ShowError("Please enter a symptom of at least 3 characters.", "Invalid Symptom");
                    return;
                }
                if (engine.Rules.IsSevereSymptom(symptom))
                {
                    ShowError($"\"{symptom}\" is already in the severe symptom list.", "Duplicate Symptom");
                    return;
                }
                engine.Rules.AddSevereSymptom(symptom);
                newSymptom.Clear();
                refresh();
            };

            removeSymptom.Click += (s, e) =>
            {
                if (severeList.SelectedItem is not string symptom)
                {
                    ShowInfo("Select a symptom in the list first.", "Remove Symptom");
                    return;
                }
                if (engine.Rules.GetSevereSymptoms().Count <= 1)
                {
                    ShowError("At least one severe symptom must remain.", "Remove Symptom");
                    return;
                }
                if (MessageBox.Show($"Remove \"{symptom}\" from the severe symptom list?", "Remove Symptom",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    engine.Rules.RemoveSevereSymptom(symptom);
                    refresh();
                }
            };

            demoButton.Click += (s, e) =>
            {
                int added = LoadSamplePatients();
                ShowInfo($"{added} sample patient(s) added to the waiting queue.", "Sample Data");
                Navigate(PageQueue);
            };

            refresh();
        }

        /// <summary>Adds sample patients with staggered arrival times for demonstrations.</summary>
        private int LoadSamplePatients()
        {
            var samples = new (string Name, int Age, string Gender, int Hr, int Bp, string[] Symptoms, int MinutesAgo)[]
            {
                ("Ahmad bin Ismail", 58, "Male", 132, 85, new[] { "Chest pain", "Dizziness" }, 4),
                ("Nur Aisyah binti Rahman", 24, "Female", 88, 118, new[] { "Fever", "Sore throat" }, 25),
                ("Tan Wei Ming", 45, "Male", 128, 150, new[] { "Headache" }, 18),
                ("Priya a/p Raman", 67, "Female", 45, 195, new[] { "Fatigue" }, 9),
                ("Lim Mei Ling", 33, "Female", 76, 185, new[] { "Headache", "Nausea / vomiting" }, 14),
                ("Muhammad Hafiz", 19, "Male", 72, 120, new[] { "Minor injury" }, 31)
            };

            int added = 0;
            foreach (var s in samples)
            {
                int id = engine.GetNextPatientId();
                var patient = new Patient(id, s.Name, s.Age, new VitalSigns(s.Hr, s.Bp))
                {
                    Gender = s.Gender,
                    ArrivalTime = DateTime.Now.AddMinutes(-s.MinutesAgo)
                };
                foreach (string symptom in s.Symptoms) patient.AddSymptom(symptom);

                if (engine.TriagePatient(patient) == PriorityLevel.Red) engine.AlertStaff(patient);
                added++;
            }
            UpdateSidebarStatus();
            return added;
        }

        private static void AddClassRow(Panel parent, string name, Color color, string rule, string meaning, int y)
        {
            parent.Controls.Add(new Label
            {
                Text = name,
                Font = Theme.F(10, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = color,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(22, y),
                Size = new Size(95, 36)
            });
            parent.Controls.Add(Theme.MakeLabel(rule, 10.5f, FontStyle.Bold, Theme.Navy, 132, y + 1));
            parent.Controls.Add(Theme.MakeLabel(meaning, 9, FontStyle.Regular, Theme.Muted, 133, y + 20));
        }

        private static NumericUpDown AddNumber(Panel parent, string label, int x, int y, int value)
        {
            parent.Controls.Add(Theme.MakeLabel(label, 8.5f, FontStyle.Bold, Theme.Navy, x, y));
            var number = new NumericUpDown
            {
                Location = new Point(x, y + 22),
                Size = new Size(125, 30),
                Minimum = 1,
                Maximum = 300,
                Value = value,
                Font = Theme.F(11)
            };
            parent.Controls.Add(number);
            return number;
        }
    }
}
