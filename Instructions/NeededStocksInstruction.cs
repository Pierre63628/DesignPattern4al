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
            Console.WriteLine($"{item.Quantity} {item.Template.Name} :");
            foreach (var piece in item.Template.AllPieces())
                Console.WriteLine($"{item.Quantity} {piece.Name}");
        }

        Console.WriteLine("Total :");
        foreach (var (piece, qty) in command.NeededPieces())
            Console.WriteLine($"{qty} {piece}");
    }
}
