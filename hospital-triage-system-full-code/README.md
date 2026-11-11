# Hospital Patient Triage System — Full C# Logic

This version extends the group's original dummy implementation while retaining its main OOP classes and triage rules. This folder contains the backend classes plus a console test version. The finished Windows Forms interface is in the `HospitalTriageGUI` folder (see the main README.md one level up).

## Included functionality
- Register patients with ID, name, age, symptoms, heart rate and systolic blood pressure.
- Classify patients as Red, Yellow or Green using the group's documented P, Q and R rules.
- Maintain a waiting queue ordered Red, Yellow, Green; patients in the same category are ordered by arrival time.
- View the waiting queue and call the next patient.
- Notify registered staff who are on duty when a Red-priority patient is identified.
- Keep current-session history and save/load history in `history.json`.
- Basic input validation and duplicate patient/staff ID checks.

## Triage rules retained from the project design
- P: patient has at least one severe symptom.
- Q: heart rate is below 50 or above 120.
- R: systolic blood pressure is below 90 or above 180.
- Red: P is true, or both Q and R are true.
- Yellow: Q or R is true (when Red does not apply).
- Green: none of P, Q or R is true.

These are the project's educational rules, not clinically validated medical guidance.

## Run
Requires the .NET 10 SDK (the project file currently targets `net10.0`). Open the project folder in Visual Studio or run:

```bash
dotnet run
```

The console menu is intended for testing the backend before the WinForms GUI is created. The `TriageEngine` methods are designed to be called from GUI event handlers in the next stage.

## Main classes
The original classes are retained: `Person`, `Patient`, `StaffMember`, `Nurse`, `Doctor`, `VitalSigns`, `PriorityLevel`, `PatientQueue`, `SevereSymptomRules` and `TriageEngine`. `TriageHistoryEntry` is added to represent saved history records.
