namespace HospitalTriage;

public class Person
{
    public int PersonID { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }

    public Person(int personID, string name, int age)
    {
        PersonID = personID;
        Name = name;
        Age = age;
    }

    public virtual string GetDetails()
    {
        return $"{Name}, Age: {Age}";
    }
}