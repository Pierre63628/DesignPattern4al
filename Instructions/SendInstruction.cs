namespace DroneFactory.Instructions;

// SEND ORDERID, ARGS (5.2.2) : sort du stock les drones envoyes pour une commande.
// Repond "Remaining for ORDERID : ..." tant qu'il reste des drones, "COMPLETED
// ORDERID" une fois la commande satisfaite.
public sealed class SendInstruction : IInstruction
{
    private readonly Factory _ctx;
    public SendInstruction(Factory ctx) => _ctx = ctx;

    public void Execute(string args)
    {
        int comma = args.IndexOf(',');
        if (comma < 0)
        {
            Console.WriteLine("ERROR SEND expects an order id and a list of drones");
            return;
        }

        string id = args[..comma].Trim();
        string rest = args[(comma + 1)..].Trim();

        if (!_ctx.Orders.TryGet(id, out var order))
        {
            Console.WriteLine($"ERROR `{id}` is not a known order");
            return;
        }

        if (!_ctx.Parser.TryParse(rest, out var command, out string error))
        {
            Console.WriteLine($"ERROR {error}");
            return;
        }

        // On valide tout l'envoi avant d'appliquer quoi que ce soit.
        var toSend = command!.ByDroneName();
        foreach (var (drone, qty) in toSend)
        {
            int remaining = order!.RemainingOf(drone);
            if (remaining <= 0)
            {
                Console.WriteLine($"ERROR order {id} does not need `{drone}`");
                return;
            }
            if (qty > remaining)
            {
                Console.WriteLine($"ERROR order {id} only needs {remaining} more `{drone}`");
                return;
            }
            if (_ctx.Stock.Get(drone) < qty)
            {
                Console.WriteLine($"ERROR not enough `{drone}` in stock to send");
                return;
            }
        }

        foreach (var (drone, qty) in toSend)
        {
            _ctx.Stock.Remove(drone, qty, "SEND");
            order!.Ship(drone, qty);
        }

        if (order!.IsComplete)
            Console.WriteLine($"COMPLETED {id}");
        else
            Console.WriteLine($"Remaining for {id} : {order.RemainingText()}");
    }
}
