var total = new Wallet(250m);
Console.WriteLine($"начальный баланс: {total}");
Console.WriteLine($"после добавления 80: {total.Add(80m)}");
Console.WriteLine($"после добавления 15 и 25: {total.Add(15m, 25m)}");

var start = new RouteVector(2, 3);
var finish = new RouteVector(5, 7);
Console.WriteLine($"координаты: {start} + {finish} = {start + finish}");

Thermometer temp = (Thermometer)86.0;
Console.WriteLine($"86 F = {temp.Celsius} C");
Console.WriteLine($"обратное преобразование: {(double)temp}");

class Wallet(decimal amount)
{
    private readonly decimal _amount = amount;

    public Wallet Add(decimal delta) => new(_amount + delta);

    public Wallet Add(decimal first, decimal second) => new(_amount + first + second);

    public static Wallet operator +(Wallet left, Wallet right) => new(left._amount + right._amount);

    public static implicit operator double(Wallet value) => (double)value._amount;

    public override string ToString() => $"{_amount:C}";
}

class RouteVector(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;

    public static RouteVector operator +(RouteVector left, RouteVector right)
        => new(left.X + right.X, left.Y + right.Y);

    public override string ToString() => $"({X}; {Y})";
}

class Thermometer(double celsius)
{
    public double Celsius { get; } = celsius;

    public static explicit operator Thermometer(double fahrenheit)
        => new((fahrenheit - 32) * 5 / 9);

    public static explicit operator double(Thermometer value) => value.Celsius * 9 / 5 + 32;
}
