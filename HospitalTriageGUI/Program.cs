using System;
using System.Windows.Forms;
using HospitalTriage;

namespace HospitalTriageGUI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Show a friendly message instead of crashing if something unexpected happens.
            Application.ThreadException += (s, e) =>
                MessageBox.Show("An unexpected error occurred:\n\n" + e.Exception.Message,
                    "Hospital Triage System", MessageBoxButtons.OK, MessageBoxIcon.Error);

            var rules = new SevereSymptomRules();
            var engine = new TriageEngine(rules);

            // Default staff on duty when the application starts.
            engine.RegisterStaff(new Nurse(101, "Siti", 30, "Emergency Ward"));
            engine.RegisterStaff(new Doctor(102, "Aiman", 40, "Emergency Medicine"));

            Application.Run(new Form1(engine));
        }
    }
}
