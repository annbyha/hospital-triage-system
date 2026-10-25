using System;
using System.Collections.Generic;

namespace HospitalTriage;

public class Patient : Person
{
    public List<string> Symptoms { get; set; }
    public VitalSigns Vitals { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime ArrivalTime { get; set; }

    public Patient(
        int patientId, string name, int age, VitalSigns vitals)
        : base(patientId, name, age)
    {
        Symptoms = new List<string>();
        Vitals = vitals;
        Priority = PriorityLevel.Green;
        ArrivalTime = DateTime.Now;
    }

    public void AddSymptom(string symptom)
    {
        Symptoms.Add(symptom);
    }

    public DateTime GetWaitingTime()
    {
        return ArrivalTime;
    }

    public override string GetDetails()
    {
        return $"Patient ID: {PersonID}, Name: {Name}, Priority: {Priority}";
    }
}