namespace DroneFactory.Orders;

// Gere le cycle de vie des commandes (5.2.2) : creation avec identifiant unique,
// acces et listing des commandes non satisfaites.
public sealed class OrderService
{
    private readonly Dictionary<string, Order> _orders = new();
    private int _counter;

    public Order Create(Dictionary<string, int> drones)
    {
        _counter++;
        string id = $"ORDER-{_counter}";
        var order = new Order(id, drones);
        _orders[id] = order;
        return order;
    }

    public bool TryGet(string id, out Order? order) => _orders.TryGetValue(id, out order);

    public IEnumerable<Order> Remaining => _orders.Values.Where(o => !o.IsComplete);
}
