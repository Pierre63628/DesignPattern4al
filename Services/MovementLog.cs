namespace DroneFactory.Services;

// Observateur concret : conserve l'historique des mouvements de stock (5.2.3).
// Decouple de StockService : le stock ignore qui l'observe.
public sealed class MovementLog : IStockObserver
{
    private readonly List<StockMovement> _movements = new();

    public void OnMovement(StockMovement movement) => _movements.Add(movement);

    public IReadOnlyList<StockMovement> All => _movements;

    public IEnumerable<StockMovement> For(ISet<string> items)
        => _movements.Where(m => items.Contains(m.Item));
}
