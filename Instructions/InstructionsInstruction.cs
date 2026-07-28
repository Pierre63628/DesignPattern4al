using DroneFactory.Model;

namespace DroneFactory.Instructions;

// INSTRUCTIONS ARGS : liste les instructions d'assemblage.
// Generation encore naive a l'etape 1 ; elle passera par un Builder a l'etape 2
// (necessaire pour gerer plusieurs generateurs / modules de deplacement).
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
                PrintDroneInstructions(item.Template);
    }

    private static void PrintDroneInstructions(DroneTemplate d)
    {
        Console.WriteLine($"PRODUCING {d.Name}");
        Console.WriteLine($"GET_OUT_STOCK 1 {d.Hull.Name}");
        Console.WriteLine($"GET_OUT_STOCK 1 {d.Core.Name}");
        Console.WriteLine($"GET_OUT_STOCK 1 {d.Generator.Name}");
        Console.WriteLine($"GET_OUT_STOCK 1 {d.Move.Name}");
        Console.WriteLine($"GET_OUT_STOCK 1 {d.Processor.Name}");
        Console.WriteLine($"INSTALL {d.System.Name} {d.Core.Name}");
        Console.WriteLine($"ASSEMBLE TMP1 {d.Hull.Name} {d.Generator.Name}");
        Console.WriteLine($"ASSEMBLE TMP2 TMP1 {d.Move.Name}");
        Console.WriteLine($"ASSEMBLE TMP2 {d.Core.Name}{{{d.System.Name}}}");
        Console.WriteLine($"ASSEMBLE [TMP2, {d.Core.Name}{{{d.System.Name}}}] {d.Processor.Name}");
        Console.WriteLine($"FINISHED {d.Name}");
    }
}
