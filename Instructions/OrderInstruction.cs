namespace DroneFactory.Instructions;

// ORDER ARGS (5.2.2) : enregistre une commande et renvoie son identifiant unique.
// La commande est seulement validee (drones/modifications corrects) ; le stock
// sera consomme plus tard via SEND.
public sealed class OrderInstruction : IInstruction
{
    private readonly Factory _ctx;
    public OrderInstruction(Factory ctx) => _ctx = ctx;

    public void Execute(string args)
    {
        if (!_ctx.Parser.TryParse(args, out var command, out string error))
        {
            Console.WriteLine($"ERROR {error}");
            return;
        }

        var order = _ctx.Orders.Create(command!.ByDroneName());
        Console.WriteLine(order.Id);
    }
}
