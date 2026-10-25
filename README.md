# 🏥 Hospital Patient Triage System

![C#](https://img.shields.io/badge/Language-C%23-512BD4?logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/Platform-.NET-512BD4?logo=dotnet)
![Course](https://img.shields.io/badge/Course-Object--Oriented%20Programming-blue)
![Academic Year](https://img.shields.io/badge/Academic%20Year-2026-gold)

A C#-based **Hospital Patient Triage System** developed to demonstrate Object-Oriented Programming (OOP), propositional logic, patient prioritisation, and medical staff coordination.

##  Project Overview

Patient triage is the process of assessing patients and prioritising their treatment based on the urgency of their conditions. During busy periods, an organised triage process can help medical staff identify patients who require immediate attention.

This project models a hospital triage system that evaluates patient symptoms and vital signs, assigns priority levels, manages the patient queue, and supports communication between nurses and doctors.


##  Triage Priority Levels

| Priority | Category | Description |
|---|---|---|
|  Red | Immediate | Critical cases requiring immediate attention |
|  Yellow | Urgent | Patients requiring urgent assessment |
|  Green | Non-critical | Patients whose conditions are less urgent |


##  Propositional Logic

The system uses propositional logic to represent the conditions involved in patient triage.

### Propositional Variables

- **P** — The patient has severe symptoms.
- **Q** — The patient's heart rate is abnormal.
- **R** — The patient's systolic blood pressure is abnormal.

### Triage Decision Rules

| Rule | Logical Expression | Priority |
|:---:|---|---|
| 1 | P |  Red — Immediate |
| 2 | ¬P ∧ Q ∧ R |  Red — Immediate |
| 3 | ¬P ∧ (Q ∨ R) |  Yellow — Urgent |
| 4 | ¬P ∧ ¬Q ∧ ¬R |  Green — Non-critical |



##  Group Members — Group 04

| No. | Name | Programme | Student ID |
|---:|---|:---:|---:|
| 1 | Wan Nur Huda Binti Wan Mokhtar | CS | 25008242 |
| 2 | Nur Farisha Afza Binti Azhari | IT | 24007988 |
| 3 | Ainin Nabihah Binti Osman | CS | 26002665 |
| 4 | Humaira Kaisah Binti Ahmad Sherhan | IT | 24006090 |
| 5 | Deenira Lipin | CS | 26000190 |

##  Academic Information

**Institution:** Universiti Teknologi PETRONAS  
**Academic Year:** 2026  
**Course:** Object-Oriented Programming  
**Course Codes:** TGB1103 / DBB1093 / TFB1033 / TEB1043  
**Lecturer:** M Nordin B Zakaria

---

*Developed by Group 04 for academic purposes to demonstrate propositional logic and Object-Oriented Programming in C#.*

