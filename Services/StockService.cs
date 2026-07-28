namespace DroneFactory.Services;

// Etat du stock de l'usine : quantite par nom (pieces ET drones produits).
// Volontairement simple ; la tracabilite (Observer) sera greffee en phase 3.
public sealed class StockService
{
    private readonly Dictionary<string, int> _quantities = new();

    public int Get(string name) => _quantities.GetValueOrDefault(name);

    public void Set(string name, int quantity) => _quantities[name] = quantity;

    public void Add(string name, int quantity) => _quantities[name] = Get(name) + quantity;

    public void Remove(string name, int quantity) => _quantities[name] = Get(name) - quantity;

    public bool HasEnough(IReadOnlyDictionary<string, int> needed)
        => needed.All(kv => Get(kv.Key) >= kv.Value);
}
