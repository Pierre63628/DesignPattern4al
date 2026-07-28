namespace DroneFactory.Instructions;

// VERIFY ARGS : AVAILABLE si le stock suffit, UNAVAILABLE sinon, ERROR si invalide.
public sealed class VerifyInstruction : IInstruction
{
    private readonly Factory _ctx;
    public VerifyInstruction(Factory ctx) => _ctx = ctx;

    public void Execute(string args)
    {
        if (!_ctx.Parser.TryParse(args, out var command, out string error))
        {
            Console.WriteLine($"ERROR {error}");
            return;
        }

        Console.WriteLine(_ctx.Stock.HasEnough(command!.NeededPieces()) ? "AVAILABLE" : "UNAVAILABLE");
    }
}
