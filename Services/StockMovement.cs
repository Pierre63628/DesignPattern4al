namespace DroneFactory.Services;

// Un mouvement de stock : quel element, de combien, et pourquoi.
public sealed class StockMovement
{
    public string Item { get; }
    public int Delta { get; }     // positif = entree, negatif = sortie
    public string Reason { get; } // instruction a l'origine du mouvement

    public StockMovement(string item, int delta, string reason)
    {
        Item = item;
        Delta = delta;
        Reason = reason;
    }

    public override string ToString()
    {
        string sign = Delta >= 0 ? "+" : "";
        return $"{sign}{Delta} {Item} [{Reason}]";
    }
}

// PATTERN OBSERVER : contrat de l'observateur notifie a chaque mouvement de stock.
public interface IStockObserver
{
    void OnMovement(StockMovement movement);
}
