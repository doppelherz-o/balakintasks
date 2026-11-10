var inventory = new Warehouse();
inventory.Add(new Laptop("macbook m5 pro", 98000m, "синий"));
inventory.Add(new Laptop("asus v16", 126500m, "чёрный"));
inventory.Add(new Tablet("asus tuf fx505dt", 43100m, "чёрный"));
inventory.Add(new Tablet("macbook m6 pro", 38900m, "белый"));

Console.WriteLine("каталог товаров:");
inventory.PrintCatalog();

Console.WriteLine();
Console.WriteLine("чек полиморфизма:");
foreach (var item in inventory.Items)
    Console.WriteLine($"- {item.Name}: {item.GetCategory()} | {item.GetStatus()}");

abstract class Product
{
    private readonly string _name;
    private readonly decimal _price;

    protected Product(string name, decimal price)
    {
        _name = name;
        _price = price;
    }

    public string Name => _name;
    public decimal Price => _price;
    public virtual string GetStatus() => "товар доступен";
    public abstract string GetCategory();
}

class Laptop : Product
{
    private readonly string _caseMaterial;

    public Laptop(string name, decimal price, string caseMaterial) : base(name, price)
        => _caseMaterial = caseMaterial;

    public override string GetCategory() => "ноутбук";
    public override string GetStatus() => $"в наличии, корпус: {_caseMaterial}";
}

class Tablet : Product
{
    private readonly string _color;

    public Tablet(string name, decimal price, string color) : base(name, price)
        => _color = color;

    public override string GetCategory() => "планшет";
    public override string GetStatus() => $"есть на складе, цвет: {_color}";
}

class Warehouse
{
    private readonly List<Product> _items = [];

    public IReadOnlyList<Product> Items => _items;

    public void Add(Product product) => _items.Add(product);

    public void PrintCatalog()
    {
        foreach (var item in _items)
            Console.WriteLine($"{item.GetCategory()} | {item.Name} | {item.Price:C}");
    }
}
