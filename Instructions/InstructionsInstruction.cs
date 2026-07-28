using DroneFactory.Assembly;

namespace DroneFactory.Instructions;

// INSTRUCTIONS ARGS : liste les instructions d'assemblage.
// La generation est deleguee au Builder (DroneAssembler), qui gere un nombre
// quelconque de generateurs / modules de deplacement (5.1.2).
public sealed class InstructionsInstruction : IInstruction
{
    private readonly Factory _ctx;
    public InstructionsInstruction(Factory ctx) => _ctx = ctx;

    public void Execute(string args)
    {
        if (!_ctx.Parser.TryParse(args, out var command, out string error))
        {
            Console.WriteLine($"ERROR {error}");
            return;
        }

        foreach (var item in command!.Items)
            for (int i = 0; i < item.Quantity; i++)
                foreach (var line in DroneAssembler.BuildInstructions(item.Model))
                    Console.WriteLine(line);
    }
}
