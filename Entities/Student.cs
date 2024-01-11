
public class Student

{
     public int StudentID { get; set; }
    public string? NameSurname { get; set;}

    public ICollection<Lesson>? Lessons {get; set;}
}