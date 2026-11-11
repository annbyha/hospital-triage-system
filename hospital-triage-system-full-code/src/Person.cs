namespace HospitalTriage;

public class Person
{
    public int PersonID { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }

    public Person(int personID, string name, int age)
    {
        if (personID <= 0) throw new ArgumentOutOfRangeException(nameof(personID));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (age < 0 || age > 130) throw new ArgumentOutOfRangeException(nameof(age));
        PersonID = personID;
        Name = name.Trim();
        Age = age;
    }

    public virtual string GetDetails() => $"{Name}, Age: {Age}";
}
