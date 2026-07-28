namespace DroneFactory.Instructions;

// NEEDED_STOCKS ARGS : detaille les pieces necessaires drone par drone, puis le total.
public sealed class NeededStocksInstruction : IInstruction
{
    private readonly Factory _ctx;
    public NeededStocksInstruction(Factory ctx) => _ctx = ctx;

    public void Execute(string args)
    {
        if (!_ctx.Parser.TryParse(args, out var command, out string error))
        {
            Console.WriteLine($"ERROR {error}");
            return;
        }

        foreach (var item in command!.Items)
        {
            Console.WriteLine($"{item.Quantity} {item.Model.Name} :");
            // On regroupe les pieces identiques (un drone peut avoir 2 generateurs
            // ou plusieurs modules de deplacement identiques).
            foreach (var group in item.Model.AllPieces().GroupBy(p => p.Name))
                Console.WriteLine($"{item.Quantity * group.Count()} {group.Key}");
        }

        Console.WriteLine("Total :");
        foreach (var (piece, qty) in command.NeededPieces())
            Console.WriteLine($"{qty} {piece}");
    }
}
