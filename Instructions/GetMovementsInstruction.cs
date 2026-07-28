using DroneFactory.Services;

namespace DroneFactory.Instructions;

// GET_MOVEMENTS [ARGS] (5.2.3) : historique des mouvements de stock.
// Sans argument : tous les mouvements. Avec ARGS : uniquement les elements listes.
public sealed class GetMovementsInstruction : IInstruction
{
    private readonly MovementLog _log;
    public GetMovementsInstruction(MovementLog log) => _log = log;

    public void Execute(string args)
    {
        IEnumerable<StockMovement> movements;

        if (string.IsNullOrWhiteSpace(args))
        {
            movements = _log.All;
        }
        else
        {
            var items = args.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .ToHashSet();
            movements = _log.For(items);
        }

        foreach (var movement in movements)
            Console.WriteLine(movement.ToString());
    }
}
