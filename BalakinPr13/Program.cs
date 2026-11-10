Console.WriteLine("1. Живые существа");
List<LivingBeing> living =
[
    new Acellularia("Вирус SARS-CoV-2"),
    new Prokaryota("Бактерия E. coli"),
    new Fungi("Пеницилл"),
    new Plantae("Роза"),
    new Animalia("Человек")
];
foreach (var being in living)
    Console.WriteLine($"{being.Name} -> {being.GetGroup()}");

Console.WriteLine();
Console.WriteLine("2. Геометрические фигуры");
List<Shape> shapes = [new Circle(5), new Rectangle(4, 6), new Triangle(3, 4, 5), new Square(7)];
foreach (var shape in shapes)
    Console.WriteLine($"{shape.Description()}: площадь = {shape.GetArea():F2}, " +
        $"периметр = {shape.GetPerimeter():F2}");

Console.WriteLine();
Console.WriteLine("3. Профили на сайте поиска вакансий");
new Vacancy("разработчик данных", 180000).Describe();
new Resume("сварщик", 5).Describe();

abstract class LivingBeing(string name)
{
    public string Name { get; } = name;

    public abstract string GetGroup();
}

class Acellularia(string name) : LivingBeing(name)
{
    public override string GetGroup() => "бесклеточные";
}

class Cellularia(string name) : LivingBeing(name)
{
    public override string GetGroup() => "клеточные";
}

class Prokaryota(string name) : Cellularia(name)
{
    public override string GetGroup() => "прокариоты";
}

class Eukaryota(string name) : Cellularia(name)
{
    public override string GetGroup() => "эукариоты";
}

class Unicellularia(string name) : Eukaryota(name)
{
    public override string GetGroup() => "одноклеточные";
}

class Fungi(string name) : Eukaryota(name)
{
    public override string GetGroup() => "грибы";
}

class Plantae(string name) : Eukaryota(name)
{
    public override string GetGroup() => "растения";
}

class Animalia(string name) : Eukaryota(name)
{
    public override string GetGroup() => "животные";
}

abstract class Shape
{
    public abstract string Description();
    public abstract double GetArea();
    public abstract double GetPerimeter();
}

class Circle(double radius) : Shape
{
    public override string Description() => "круг";
    public override double GetArea() => Math.PI * radius * radius;
    public override double GetPerimeter() => 2 * Math.PI * radius;
}

class Rectangle(double width, double height) : Shape
{
    public override string Description() => "прямоугольник";
    public override double GetArea() => width * height;
    public override double GetPerimeter() => 2 * (width + height);
}

class Triangle(double a, double b, double c) : Shape
{
    public override string Description() => "треугольник";
    public override double GetPerimeter() => a + b + c;

    public override double GetArea()
    {
        var p = GetPerimeter() / 2;
        return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
    }
}

class Square(double side) : Rectangle(side, side)
{
    public override string Description() => "квадрат";
}

class Profile(string profession)
{
    public string Profession { get; } = profession;

    public virtual string Info() => string.Empty;

    public void Describe() => Console.WriteLine($"профессия: {Profession} | {Info()}");
}

class Vacancy(string profession, decimal salary) : Profile(profession)
{
    public override string Info() => $"предлагаемая зарплата: {salary:C}";
}

class Resume(string profession, int experience) : Profile(profession)
{
    public override string Info() => $"стаж работы: {experience} лет";
}
