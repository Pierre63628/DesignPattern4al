namespace DroneFactory.Services;

// Etat du stock de l'usine : quantite par nom (pieces ET drones produits).
// "Sujet" du pattern Observer : chaque entree/sortie reelle notifie les
// observateurs (ex : le journal des mouvements). Le seeding initial (Set) ne
// declenche volontairement aucune notification.
public sealed class StockService
{
    private readonly Dictionary<string, int> _quantities = new();
    private readonly List<IStockObserver> _observers = new();

    public void Subscribe(IStockObserver observer) => _observers.Add(observer);

    public int Get(string name) => _quantities.GetValueOrDefault(name);

    public void Set(string name, int quantity) => _quantities[name] = quantity;

    public void Add(string name, int quantity, string reason)
    {
        _quantities[name] = Get(name) + quantity;
        Notify(name, quantity, reason);
    }

    public void Remove(string name, int quantity, string reason)
    {
        _quantities[name] = Get(name) - quantity;
        Notify(name, -quantity, reason);
    }

    public bool HasEnough(IReadOnlyDictionary<string, int> needed)
        => needed.All(kv => Get(kv.Key) >= kv.Value);

    private void Notify(string name, int delta, string reason)
    {
        var movement = new StockMovement(name, delta, reason);
        foreach (var observer in _observers)
            observer.OnMovement(movement);
    }
}
