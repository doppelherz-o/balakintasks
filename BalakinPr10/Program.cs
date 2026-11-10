Console.WriteLine("сумма цифр числа:");
Console.WriteLine($"sumDigits(4782) = {SumDigits(4782)}");

Console.WriteLine();
Console.WriteLine("нод");
Console.WriteLine($"gcd(252, 105) = {Gcd(252, 105)}");

Console.WriteLine();
Console.WriteLine("hanoi:");
List<string> steps = [];
Hanoi(3, 'A', 'B', 'C', steps);
foreach (var step in steps)
    Console.WriteLine(step);

static int SumDigits(int value) => value < 10 ? value : value % 10 + SumDigits(value / 10);

static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

static void Hanoi(int disks, char source, char buffer, char target, List<string> steps)
{
    if (disks == 0)
        return;

    Hanoi(disks - 1, source, target, buffer, steps);
    steps.Add($"{source} -> {target}");
    Hanoi(disks - 1, buffer, source, target, steps);
}
