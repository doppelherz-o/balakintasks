using System.Runtime.CompilerServices;

Console.WriteLine("1. отмена заказа через событие OrderCancelling");

var service = new OrderService();
var stock = new StockValidator();
var fraud = new FraudDetector();
var audit = new AuditLog();

service.OrderCancelling += stock.Validate;
service.OrderCancelling += fraud.Check;
service.OrderCancelling += audit.Log;
service.OrderPlaced += (_, e) =>
    Console.WriteLine($"[placed] заказ #{e.OrderId} на {e.Amount:C} принят");

service.PlaceOrder(1, 5_000m, "user@test");
service.PlaceOrder(2, 25_000m, "user@test");
service.PlaceOrder(3, 15_000m, "user@test");

Console.WriteLine();
Console.WriteLine("дополнительная проверка фильтра мошенничества:");
service.PlaceOrder(4, 8_000m, "user@test.test");

Console.WriteLine();
Console.WriteLine("2. EventAggregator");

var aggregator = new EventAggregator();
var emailNotifier = new EmailNotifier();
var warehouseService = new WarehouseService();
var alarm = new Alarm();

aggregator.Subscribe<OrderPlaced>(emailNotifier.Handle);
aggregator.Subscribe<OrderPlaced>(warehouseService.Handle);
aggregator.Subscribe<TemperatureChanged>(alarm.Handle);

aggregator.Publish(new OrderPlaced(1, 1_200m));
aggregator.Publish(new TemperatureChanged(23.5));

aggregator.Unsubscribe<OrderPlaced>(warehouseService.Handle);
Console.WriteLine("WarehouseService отписан, публикуем OrderPlaced повторно:");
aggregator.Publish(new OrderPlaced(2, 700m));

emailNotifier.Verbose = false;
var handledBefore = emailNotifier.Count;
Parallel.For(0, 1000, i => aggregator.Publish(new OrderPlaced(i, i)));
Console.WriteLine($"стресс-тест: 1000 сообщений, EmailNotifier вызван "
    + $"{emailNotifier.Count - handledBefore} раз, исключений нет");

Console.WriteLine();
Console.WriteLine("3. Утечка подписки и её исправление");

var publisher = new LongLivedPublisher();

var leaked = CreateLeakingSubscriber(publisher);
publisher.Raise();
ForceGc();
Console.WriteLine($"подписчик без отписки жив после сборки мусора: {leaked.IsAlive}");

var disposed = CreateDisposableSubscriber(publisher);
ForceGc();
Console.WriteLine($"подписка с Dispose жива после сборки мусора: {disposed.IsAlive}");

GC.KeepAlive(publisher);

static void ForceGc()
{
    GC.Collect();
    GC.WaitForPendingFinalizers();
    GC.Collect();
}

[MethodImpl(MethodImplOptions.NoInlining)]
static WeakReference CreateLeakingSubscriber(LongLivedPublisher pub)
{
    var subscriber = new Subscriber();
    pub.Tick += subscriber.OnTick;
    return new WeakReference(subscriber);
}

[MethodImpl(MethodImplOptions.NoInlining)]
static WeakReference CreateDisposableSubscriber(LongLivedPublisher pub)
{
    using var subscriber = new DisposableSubscriber(pub);
    pub.Raise();
    return new WeakReference(subscriber);
}

class OrderCancellingEventArgs(int orderId, decimal amount, string customerEmail) : EventArgs
{
    public int OrderId { get; } = orderId;
    public decimal Amount { get; } = amount;
    public string CustomerEmail { get; } = customerEmail;
    public bool Cancel { get; set; }
    public string Reason { get; set; } = string.Empty;
}

class OrderPlacedEventArgs(int orderId, decimal amount, string customerEmail) : EventArgs
{
    public int OrderId { get; } = orderId;
    public decimal Amount { get; } = amount;
    public string CustomerEmail { get; } = customerEmail;
}

class OrderService
{
    public event EventHandler<OrderCancellingEventArgs>? OrderCancelling;
    public event EventHandler<OrderPlacedEventArgs>? OrderPlaced;

    public void PlaceOrder(int orderId, decimal amount, string customerEmail)
    {
        var cancelling = new OrderCancellingEventArgs(orderId, amount, customerEmail);
        Raise(OrderCancelling, cancelling);

        if (cancelling.Cancel)
        {
            Console.WriteLine($"заказ #{orderId} отменён: {cancelling.Reason}");
            return;
        }

        Raise(OrderPlaced, new OrderPlacedEventArgs(orderId, amount, customerEmail));
        Console.WriteLine($"заказ #{orderId} оформлен");
    }

    private void Raise<T>(EventHandler<T>? handlers, T args)
    {
        if (handlers is null)
            return;

        foreach (EventHandler<T> handler in handlers.GetInvocationList())
        {
            try
            {
                handler(this, args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[error] обработчик {handler.Method.Name}: {ex.Message}");
            }
        }
    }
}

class StockValidator
{
    public void Validate(object? sender, OrderCancellingEventArgs e)
    {
        if (e.Amount > 20_000m)
        {
            e.Cancel = true;
            e.Reason = "недостаточно товара на складе";
        }
    }
}

class FraudDetector
{
    public void Check(object? sender, OrderCancellingEventArgs e)
    {
        if (e.CustomerEmail.EndsWith(".test", StringComparison.OrdinalIgnoreCase))
        {
            e.Cancel = true;
            e.Reason = "подозрительный адрес";
        }
    }
}

class AuditLog
{
    public void Log(object? sender, OrderCancellingEventArgs e)
        => Console.WriteLine($"[audit] попытка оформления #{e.OrderId}");
}

record OrderPlaced(int Id, decimal Amount);
record TemperatureChanged(double New);

sealed class EventAggregator
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = [];
    private readonly object _lock = new();

    public void Subscribe<T>(Action<T> handler)
    {
        lock (_lock)
        {
            if (!_handlers.TryGetValue(typeof(T), out var list))
                _handlers[typeof(T)] = list = [];
            list.Add(handler);
        }
    }

    public void Unsubscribe<T>(Action<T> handler)
    {
        lock (_lock)
            if (_handlers.TryGetValue(typeof(T), out var list))
                list.Remove(handler);
    }

    public void Publish<T>(T message)
    {
        Delegate[] snapshot;

        lock (_lock)
            snapshot = _handlers.TryGetValue(typeof(T), out var list) ? list.ToArray() : [];

        foreach (var handler in snapshot)
            ((Action<T>)handler)(message);
    }
}

class EmailNotifier
{
    private int _count;

    public bool Verbose { get; set; } = true;
    public int Count => Volatile.Read(ref _count);

    public void Handle(OrderPlaced message)
    {
        Interlocked.Increment(ref _count);
        if (Verbose)
            Console.WriteLine($"[email] заказ #{message.Id} на {message.Amount:C} " +
                "— письмо отправлено");
    }
}

class WarehouseService
{
    private int _count;

    public bool Verbose { get; set; } = true;
    public int Count => Volatile.Read(ref _count);

    public void Handle(OrderPlaced message)
    {
        Interlocked.Increment(ref _count);
        if (Verbose)
            Console.WriteLine($"[warehouse] заказ #{message.Id} — товар зарезервирован");
    }
}

class Alarm
{
    public void Handle(TemperatureChanged message)
        => Console.WriteLine($"[alarm] температура изменилась: {message.New} °C");
}

class LongLivedPublisher
{
    public event EventHandler? Tick;

    public void Raise() => Tick?.Invoke(this, EventArgs.Empty);
}

class Subscriber
{
    public int Ticks { get; private set; }

    public void OnTick(object? sender, EventArgs e) => Ticks++;
}

class DisposableSubscriber(LongLivedPublisher publisher) : IDisposable
{
    public void OnTick(object? sender, EventArgs e)
    {
    }

    public void Dispose() => publisher.Tick -= OnTick;
}
