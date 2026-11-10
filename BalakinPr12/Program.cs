var program = new EducationProgram("веб-разработка");
var mentor = new Teacher(407, "балакин д. и.", "оаип");
var student = new Student(407, "самсонов а. ю.", "3 курс");
var lecture = new Lesson("ОаИП", "2026-11-09", "Ауд. 407");
var mark = new Grade("отл", 98);

program.AddStudent(student);
program.AddTeacher(mentor);
program.AddLesson(lecture);
program.AddGrade(student, mark);

Console.WriteLine(program.GetSummary());
Console.WriteLine();
Console.WriteLine(mentor.Describe());
Console.WriteLine(student.Describe());
Console.WriteLine(lecture.Describe());
Console.WriteLine(mark.Describe());

abstract class Person(int id, string fullName)
{
    public int Id { get; } = id;
    public string FullName { get; } = fullName;

    public virtual string Describe() => $"ID: {Id}, ФИО: {FullName}";
}

class Teacher(int id, string fullName, string subject) : Person(id, fullName)
{
    public string Subject { get; } = subject;

    public override string Describe() => $"преподаватель: {FullName}, предмет: {Subject}";
}

class Student(int id, string fullName, string year) : Person(id, fullName)
{
    public string Year { get; } = year;

    public override string Describe() => $"студент: {FullName}, курс: {Year}";
}

class Lesson(string title, string date, string room)
{
    public string Title { get; } = title;
    public string Date { get; } = date;
    public string Room { get; } = room;

    public string Describe() => $"занятие: {Title}, дата: {Date}, аудитория: {Room}";
}

class Grade(string mark, int points)
{
    public string Mark { get; } = mark;
    public int Points { get; } = points;

    public string Describe() => $"оценка: {Mark}, баллы: {Points}";
}

class EducationProgram(string name)
{
    private readonly List<Student> _students = [];
    private readonly List<Teacher> _teachers = [];
    private readonly List<Lesson> _lessons = [];
    private readonly Dictionary<Student, Grade> _grades = [];

    public string Name { get; } = name;

    public void AddStudent(Student student) => _students.Add(student);
    public void AddTeacher(Teacher teacher) => _teachers.Add(teacher);
    public void AddLesson(Lesson lesson) => _lessons.Add(lesson);
    public void AddGrade(Student student, Grade grade) => _grades[student] = grade;

    public string GetSummary() =>
        $"программа: {Name}. преподавателей: {_teachers.Count}. " +
        $"студентов: {_students.Count}. занятий: {_lessons.Count}.";
}
