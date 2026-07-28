using DroneFactory.Model;

namespace DroneFactory.Instructions;

// STOCKS : affiche l'integralite du stock (drones produits puis pieces).
public sealed class StocksInstruction : IInstruction
{
    private readonly Factory _ctx;
    public StocksInstruction(Factory ctx) => _ctx = ctx;

    public void Execute(string args)
    {
        foreach (var template in _ctx.Templates.All)
            Console.WriteLine($"{_ctx.Stock.Get(template.Name)} {template.Name}");
        foreach (var piece in PieceCatalog.All)
            Console.WriteLine($"{_ctx.Stock.Get(piece.Name)} {piece.Name}");
    }
}
