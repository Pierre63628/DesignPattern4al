namespace DroneFactory.Orders;

// Une commande passee par l'utilisateur, suivie par son reste a livrer.
// Le suivi se fait par nom de drone (un drone modifie est suivi sous le nom
// de son template de base) : simplification assumee.
public sealed class Order
{
    public string Id { get; }
    private readonly Dictionary<string, int> _remaining;

    public Order(string id, Dictionary<string, int> remaining)
    {
        Id = id;
        _remaining = remaining;
    }

    public int RemainingOf(string drone) => _remaining.GetValueOrDefault(drone);

    public void Ship(string drone, int quantity) => _remaining[drone] = RemainingOf(drone) - quantity;

    public bool IsComplete => _remaining.Values.All(q => q <= 0);

    // Detail des drones restants, format "q1 Drone1, q2 Drone2".
    public string RemainingText()
        => string.Join(", ", _remaining.Where(kv => kv.Value > 0).Select(kv => $"{kv.Value} {kv.Key}"));
}
