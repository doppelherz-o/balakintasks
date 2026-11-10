Console.WriteLine("1. счётчик с порогом");
var counter = new Counter(5);
counter.ThresholdReached += (_, e) =>
    Console.WriteLine($"сигнал: значение {e.CurrentValue} достигло порога {e.Limit}");
for (var i = 1; i <= 7; i++)
    counter.Increase();

Console.WriteLine();
Console.WriteLine("2. датчик температуры");
var sensor = new TemperatureSensor();
sensor.TemperatureChanged += (_, e) =>
    Console.WriteLine($"температура изменена: {e.CurrentTemperature} C");
for (var temp = 18.0; temp <= 95; temp += 17)
    sensor.SetTemperature(temp);

Console.WriteLine();
Console.WriteLine("3. оформление заказа");
var orderService = new OrderService();
orderService.OrderPlaced += (_, e) =>
    Console.WriteLine($"заказ оформлен: {e.Item} на сумму {e.Total:C}");
orderService.PlaceOrder("смарт-колонка", 8590m);

class ThresholdReachedEventArgs(int currentValue, int limit) : EventArgs
{
    public int CurrentValue { get; } = currentValue;
    public int Limit { get; } = limit;
}

class Counter(int limit)
{
    public int Limit { get; } = limit;
    public int CurrentValue { get; private set; }

    public event EventHandler<ThresholdReachedEventArgs>? ThresholdReached;

    public void Increase()
    {
        CurrentValue++;
        if (CurrentValue >= Limit)
            OnThresholdReached(new ThresholdReachedEventArgs(CurrentValue, Limit));
    }

    protected virtual void OnThresholdReached(ThresholdReachedEventArgs e) =>
        ThresholdReached?.Invoke(this, e);
}

class TemperatureChangedEventArgs(double currentTemperature) : EventArgs
{
    public double CurrentTemperature { get; } = currentTemperature;
}

class TemperatureSensor
{
    public double CurrentTemperature { get; private set; }

    public event EventHandler<TemperatureChangedEventArgs>? TemperatureChanged;

    public void SetTemperature(double value)
    {
        CurrentTemperature = value;
        OnTemperatureChanged(new TemperatureChangedEventArgs(value));
    }

    protected virtual void OnTemperatureChanged(TemperatureChangedEventArgs e) =>
        TemperatureChanged?.Invoke(this, e);
}

class OrderPlacedEventArgs(string item, decimal total) : EventArgs
{
    public string Item { get; } = item;
    public decimal Total { get; } = total;
}

class OrderService
{
    public event EventHandler<OrderPlacedEventArgs>? OrderPlaced;

    public void PlaceOrder(string item, decimal total) => OnOrderPlaced(new OrderPlacedEventArgs(item, total));

    protected virtual void OnOrderPlaced(OrderPlacedEventArgs e) => OrderPlaced?.Invoke(this, e);
}
