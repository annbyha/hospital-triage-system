namespace HospitalTriage;

public abstract class StaffMember : Person
{
    public string Role { get; set; }
    public bool IsOnDuty { get; set; }

    // Every alert this staff member has received (shown in the GUI's Staff page).
    public List<string> ReceivedAlerts { get; } = new();

    protected StaffMember(int personID, string name, int age, string role)
        : base(personID, name, age)
    {
        Role = role;
        IsOnDuty = true;
    }

    public override string GetDetails() => $"{Role}: {Name} (On duty: {IsOnDuty})";

    // Extra detail shown next to the role (ward for nurses, specialisation for doctors).
    public abstract string GetAssignment();

    // Polymorphic alert: each subclass builds its own message.
    // The message is stored, printed to the console and returned so the GUI can display it.
    public abstract string ReceiveAlert(Patient patient);

    protected string StoreAlert(string message)
    {
        string stamped = $"[{DateTime.Now:HH:mm:ss}] {message}";
        ReceivedAlerts.Add(stamped);
        Console.WriteLine(message);
        return message;
    }
}
