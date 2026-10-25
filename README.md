# 🏥 Hospital Patient Triage System

![C#](https://img.shields.io/badge/Language-C%23-512BD4?logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/Platform-.NET-512BD4?logo=dotnet)
![Course](https://img.shields.io/badge/Course-Object--Oriented%20Programming-blue)
![Academic Year](https://img.shields.io/badge/Academic%20Year-2026-gold)

A C#-based **Hospital Patient Triage System** developed to demonstrate Object-Oriented Programming (OOP), propositional logic, patient prioritisation, and medical staff coordination.

##  1. Project Overview

Patient triage is the process of assessing patients and prioritising their treatment based on the urgency of their conditions. During busy periods, an organised triage process can help medical staff identify patients who require immediate attention.

This project models a hospital triage system that evaluates patient symptoms and vital signs, assigns priority levels, manages the patient queue, and supports communication between nurses and doctors.

##  2. Problem Statement

Manual patient prioritisation can become challenging during peak hours. Without a consistent assessment process, urgent cases may experience delays while medical staff manage multiple patients.

The Hospital Patient Triage System aims to demonstrate how predefined decision rules and an organised queue can support a more consistent patient prioritisation process.

##  3. Project Objectives

- To apply Object-Oriented Programming principles using C#.
- To represent patient triage decisions using propositional logic.
- To evaluate patient symptoms and vital signs.
- To classify patients into Red, Yellow, and Green priority levels.
- To manage the patient queue according to triage priority.
- To model interactions between patients, nurses, and doctors.

##  4. System Features

- **Patient Information Management** — stores patient details, symptoms, and vital signs.
- **Triage Evaluation** — evaluates patient conditions using predefined rules.
- **Priority Classification** — assigns Red, Yellow, or Green priority levels.
- **Queue Management** — organises waiting patients according to priority and arrival time.
- **Staff Coordination** — models communication between the triage system and medical staff.
- **Severe Symptom Detection** — checks symptoms against predefined severe symptom rules.

##  5. Triage Priority Levels

| Priority | Category | Description |
|---|---|---|
| 🔴 Red | Immediate | Critical cases requiring immediate attention |
| 🟡 Yellow | Urgent | Patients requiring urgent assessment |
| 🟢 Green | Non-critical | Patients whose conditions are less urgent |

*The priority assigned depends on the rules implemented in the system. This project is an academic simulation and is not intended for real-world medical decision-making.*

## 🧠 6. Propositional Logic

The system uses propositional logic to represent the conditions involved in patient triage.

### Propositional Variables

- **P** — The patient has severe symptoms.
- **Q** — The patient's heart rate is abnormal.
- **R** — The patient's systolic blood pressure is abnormal.

### Triage Decision Rules

| Rule | Logical Expression | Priority |
|:---:|---|---|
| 1 | P | 🔴 Red — Immediate |
| 2 | ¬P ∧ Q ∧ R | 🔴 Red — Immediate |
| 3 | ¬P ∧ (Q ∨ R) | 🟡 Yellow — Urgent |
| 4 | ¬P ∧ ¬Q ∧ ¬R | 🟢 Green — Non-critical |

### Logical Operators

| Symbol | Meaning |
|:---:|---|
| ¬ | NOT — the condition is false |
| ∧ | AND — all conditions must be true |
| ∨ | OR — at least one condition must be true |

**Note:** These rules describe the intended triage logic. They should match the final decision-making conditions implemented in `TriageEngine`.

##  7. System Classes

The system consists of 10 classes and one enum based on the project UML design.

| Class / Enum | Responsibility |
|---|---|
| `Person` | Stores common personal information. |
| `StaffMember` | Represents hospital staff and inherits from `Person`. |
| `Nurse` | Represents a nurse and supports patient monitoring. |
| `Doctor` | Represents a doctor and supports patient treatment. |
| `Patient` | Stores patient details, symptoms, vital signs, and priority. |
| `VitalSigns` | Stores and validates heart rate and systolic blood pressure. |
| `PriorityLevel` | Defines Red, Yellow, and Green priority levels. |
| `PatientQueue` | Manages the queue of waiting patients. |
| `SevereSymptomRules` | Defines and checks severe symptoms. |
| `TriageEngine` | Evaluates patient conditions and determines triage priority. |

##  8. Object-Oriented Design

The project demonstrates key Object-Oriented Programming concepts:

- **Inheritance:** `StaffMember`, `Nurse`, `Doctor`, and `Patient` follow the inheritance relationships defined in the UML.
- **Encapsulation:** Classes organise data and the operations associated with it.
- **Abstraction:** Each class has a specific responsibility within the system.
- **Association:** The triage engine interacts with patients, vital signs, the patient queue, and staff.



##  9. How to Run

### Prerequisites

- .NET SDK
- Git
- A C# development environment, such as Visual Studio or Visual Studio Code

### Clone the Repository

```bash
git clone https://github.com/YOUR-USERNAME/YOUR-REPOSITORY.git
cd YOUR-REPOSITORY
```

### Run the Application

Navigate to the directory containing the executable C# project file (`.csproj`), then run:

```bash
dotnet restore
dotnet run
```

Replace `YOUR-USERNAME/YOUR-REPOSITORY` with the actual GitHub repository path.

If the project contains only a class library, a separate executable project is required to run the demonstration.

##  10. Group Members — Group 04

| No. | Name | Programme | Student ID |
|---:|---|:---:|---:|
| 1 | Wan Nur Huda Binti Wan Mokhtar | CS | 25008242 |
| 2 | Nur Farisha Afza Binti Azhari | IT | 24007988 |
| 3 | Ainin Nabihah Binti Osman | CS | 26002665 |
| 4 | Humaira Kaisah Binti Ahmad Sherhan | IT | 24006090 |
| 5 | Deenira Lipin | CS | 26000190 |

##  12. Academic Information

**Institution:** Universiti Teknologi PETRONAS  
**Academic Year:** 2026  
**Course:** Object-Oriented Programming  
**Course Codes:** TGB1103 / DBB1093 / TFB1033 / TEB1043  
**Lecturer:** M Nordin B Zakaria

---

*Developed by Group 04 for academic purposes to demonstrate propositional logic and Object-Oriented Programming in C#.*

