# 🏥 Hospital Patient Triage System

![C#](https://img.shields.io/badge/C%23-.NET-512BD4?logo=dotnet)
![Status](https://img.shields.io/badge/status-completed-brightgreen)

A C# class library that classifies emergency patients into **Red (Immediate)**, **Yellow (Urgent)** and **Green (Non-critical)** using propositional logic.

##  Problem
Manual triage is inconsistent during peak hours and may delay critical patients.

##  Features
- Classifies patients using logic rules (P, Q, R)
- Alerts medical staff for RED patients
- Patient queue sorted by priority

##  Logic
| Rule | Condition | Priority |
|------|-----------|----------|
| 1 | P | 🔴 Red |
| 2 | ¬P ∧ Q ∧ R | 🔴 Red |
| 3 | ¬P ∧ (Q ∨ R) | 🟡 Yellow |
| 4 | ¬P ∧ ¬Q ∧ ¬R | 🟢 Green |

## Classes
`Patient` · `VitalSigns` · `MedicalStaff` · `TriageEngine` · `TriageResult` · `AlertService` · `PatientQueue`

##  How to Run
```bash
git clone https://github.com/ORG/hospital-triage-system.git
cd hospital-triage-system/src
dotnet run
```

##  UML Diagram
![UML](docs/uml.png)

##  Group 04
| Name | No ID |
|Ainin Nabihah binti Osman |26002665|
| ... | ... |

Universiti Teknologi PETRONAS · Discrete Mathematics / OOP · 2026
