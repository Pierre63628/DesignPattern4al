namespace DroneFactory.Instructions;

// PRODUCE ARGS : consomme les pieces et ajoute les drones produits au stock.
public sealed class ProduceInstruction : IInstruction
{
    private readonly Factory _ctx;
    public ProduceInstruction(Factory ctx) => _ctx = ctx;

    public void Execute(string args)
    {
        if (!_ctx.Parser.TryParse(args, out var command, out string error))
        {
            Console.WriteLine($"ERROR {error}");
            return;
        }

        var needed = command!.NeededPieces();
        if (!_ctx.Stock.HasEnough(needed))
        {
            Console.WriteLine("ERROR not enough pieces in stock to produce this command");
            return;
        }

        foreach (var (piece, qty) in needed)
            _ctx.Stock.Remove(piece, qty);
        foreach (var item in command.Items)
            _ctx.Stock.Add(item.Template.Name, item.Quantity);

        Console.WriteLine("STOCK_UPDATED");
    }
}
