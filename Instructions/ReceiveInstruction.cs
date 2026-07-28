using DroneFactory.Model;

namespace DroneFactory.Instructions;

// RECEIVE ARGS (5.1.1) : entree en stock d'une liste quantifiee d'elements
// (pieces, drones ou assemblages). Format "5 Hull_HF1, 2 DXF-1".
public sealed class ReceiveInstruction : IInstruction
{
    private readonly Factory _ctx;
    public ReceiveInstruction(Factory ctx) => _ctx = ctx;

    public void Execute(string args)
    {
        if (string.IsNullOrWhiteSpace(args))
        {
            Console.WriteLine("ERROR no elements to receive");
            return;
        }

        // On valide tout avant d'appliquer, pour ne pas laisser un stock a moitie modifie.
        var toAdd = new List<(string name, int qty)>();
        foreach (var raw in args.Split(','))
        {
            var part = raw.Trim();
            if (part.Length == 0) continue;

            int sp = part.IndexOf(' ');
            if (sp < 0)
            {
                Console.WriteLine($"ERROR `{part}` is not a valid quantity/element pair");
                return;
            }

            if (!int.TryParse(part[..sp], out int qty) || qty <= 0)
            {
                Console.WriteLine($"ERROR `{part[..sp]}` is not a valid quantity");
                return;
            }

            string name = part[(sp + 1)..].Trim();
            if (!IsStockable(name))
            {
                Console.WriteLine($"ERROR `{name}` is not a recognized element");
                return;
            }

            toAdd.Add((name, qty));
        }

        foreach (var (name, qty) in toAdd)
            _ctx.Stock.Add(name, qty, "RECEIVE");

        Console.WriteLine("STOCK_UPDATED");
    }

    // Un element stockable est une piece connue, un drone connu, ou un assemblage
    // (notation entre crochets).
    private bool IsStockable(string name)
        => PieceCatalog.TryGet(name, out _)
        || _ctx.Templates.Exists(name)
        || (name.StartsWith('[') && name.EndsWith(']'));
}
