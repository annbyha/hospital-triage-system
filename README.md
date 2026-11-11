# Hospital Patient Triage System (C# Windows Forms)

A complete Windows Forms application that registers patients, classifies them as **RED / YELLOW / GREEN**
using the group's P, Q, R triage rules, manages a priority waiting queue, alerts on-duty staff for RED patients,
and saves the patient history to `history.json`.

> The triage rules are the project's educational rules, not clinically validated medical guidance.

---

## How to run

**Requirements:** Windows 10/11 and the **.NET 10 SDK** (the projects target `net10.0` / `net10.0-windows`).

### Option A: Visual Studio (recommended)
1. Unzip the folder.
2. Double-click **`HospitalTriageSystem.sln`**.
3. Make sure **HospitalTriageGUI** is the startup project (right-click it, then **Set as Startup Project**).
4. Press **F5** (or the green ▶ Start button).

### Option B: double-click
Double-click **`RUN.bat`**. It builds and launches the GUI.

### Option C: command line
```
dotnet run --project HospitalTriageGUI
```

**Tip for presentations:** go to **Triage Rules → Load Sample Patients** to fill the queue instantly with 6 demo patients.

---

## Folder structure

```
HospitalTriageSystem/
├── HospitalTriageSystem.sln             ← open this in Visual Studio
├── RUN.bat                              ← one-click build + run
├── screenshots/                         ← pictures of every screen
├── HospitalTriageGUI/                   ← Windows Forms interface
│   ├── Program.cs                       ← startup: creates engine + default staff
│   ├── Form1.cs                         ← main window shell, sidebar, navigation, save-on-exit
│   ├── Form1.Dashboard.cs               ← Dashboard screen
│   ├── Form1.Registration.cs            ← Patient Registration screen (+ live triage preview)
│   ├── Form1.Queue.cs                   ← Waiting Queue screen
│   ├── Form1.History.cs                 ← Patient History screen (search, filter, save, CSV export)
│   ├── Form1.Staff.cs                   ← Staff & Alerts screen
│   ├── Form1.Rules.cs                   ← Triage Rules screen (thresholds, severe symptoms, demo data)
│   ├── TriageResultDialog.cs            ← pop-up shown after registering a patient
│   └── Theme.cs                         ← shared colours, fonts and control helpers
└── hospital-triage-system-full-code/    ← backend logic (OOP classes) + console test version
    ├── Program.cs                       ← console menu (for testing the logic)
    └── src/
        ├── Person.cs                    ← base class
        ├── Patient.cs                   ← inherits Person
        ├── StaffMember.cs               ← abstract, inherits Person
        ├── Nurse.cs, Doctor.cs          ← inherit StaffMember (polymorphic ReceiveAlert)
        ├── VitalSigns.cs
        ├── PriorityLevel.cs             ← enum Red / Yellow / Green
        ├── SevereSymptomRules.cs
        ├── PatientQueue.cs              ← priority queue
        ├── TriageHistoryEntry.cs        ← record saved to JSON
        └── TriageEngine.cs              ← main controller used by the GUI
```

---

## Features

| Screen | What it does |
|---|---|
| **Dashboard** | Live counts (waiting, red, yellow, green), patients registered/called, average waiting time, staff on duty, the next patient card with a **Call Next Patient** button, recent activity, quick actions. Refreshes automatically. |
| **Patient Registration** | Patient ID (auto-suggested), name, age, gender, heart rate, systolic BP, tick-box symptoms (severe ones in red) and free-text extra symptoms. **Live Triage Preview** shows P/Q/R and the resulting priority while typing. Full validation with error icons. After registering, a colour-coded result pop-up shows the reason, queue position and staff alerts. |
| **Waiting Queue** | Colour-coded table ordered Red → Yellow → Green, then by arrival. Live waiting timers, priority filter, **Call Next Patient**, "Now serving" banner, patient details (double-click a row). |
| **Patient History** | Current session and saved history (`history.json`) tabs, search by name/ID, filter by priority, **Save History**, **Export CSV**, clear saved history. |
| **Staff & Alerts** | Add nurses/doctors, toggle on/off duty (double-click), remove staff, view each person's alerts, and the full RED alert log. |
| **Triage Rules** | Explains P/Q/R and the classification. Edit the heart-rate/BP thresholds, add/remove severe symptoms, load sample patients. |

The app also asks to save unsaved history when you close it, and shows a friendly error message instead of crashing.

---

## Triage rules

- **P**: patient has at least one severe symptom (default: Difficulty breathing, Chest pain, Unconsciousness).
- **Q**: heart rate is below 50 or above 120 bpm.
- **R**: systolic blood pressure is below 90 or above 180 mmHg.

| Priority | Condition |
|---|---|
| 🔴 RED | P **or** (Q **and** R): on-duty staff are alerted |
| 🟡 YELLOW | Q **or** R (when Red does not apply) |
| 🟢 GREEN | none of P, Q or R |

Patients with the same priority are served in arrival order.

---

## OOP concepts used (for the report)

- **Inheritance:** `Patient` and `StaffMember` inherit `Person`; `Nurse` and `Doctor` inherit `StaffMember`.
- **Abstraction:** `StaffMember` is abstract with abstract `ReceiveAlert()` and `GetAssignment()`.
- **Polymorphism:** `ReceiveAlert()` and `GetDetails()` are overridden; the engine alerts every `StaffMember` without knowing whether it is a nurse or a doctor.
- **Encapsulation:** private fields with validation in constructors/methods (e.g. thresholds can only be changed via `UpdateThresholds()`, which checks min < max).
- **Separation of concerns:** all logic is in `TriageEngine` and the model classes; the GUI only calls the engine.

---

## Testing checklist (examples)

| Input | Expected |
|---|---|
| HR 80, BP 120, Fever | GREEN |
| HR 130, BP 120, Cough | YELLOW (Q) |
| HR 80, BP 200, Headache | YELLOW (R) |
| HR 40, BP 80, Cough | RED (Q and R) + staff alert |
| HR 80, BP 120, Chest pain | RED (P) + staff alert |
| HR 50, BP 90 (boundaries) | GREEN (limits are "normal") |
| Same Patient ID twice | Rejected with a message |
| Empty name / age 200 / letters in heart rate | Rejected with a message and red error icon |

`history.json` is stored next to the program (`HospitalTriageGUI\bin\Debug\net10.0-windows\`).
