namespace DroneFactory.Instructions;

// "Invoker" du pattern Command : associe un mot-cle a son instruction et route
// la ligne saisie vers la bonne commande.
public sealed class InstructionDispatcher
{
    private readonly Dictionary<string, IInstruction> _instructions = new();

    public void Register(string keyword, IInstruction instruction)
        => _instructions[keyword] = instruction;

    public void Dispatch(string line)
    {
        int sp = line.IndexOf(' ');
        string keyword = sp < 0 ? line : line[..sp];
        string args = sp < 0 ? "" : line[(sp + 1)..].Trim();

        if (_instructions.TryGetValue(keyword, out var instruction))
            instruction.Execute(args);
        else
            Console.WriteLine($"ERROR `{keyword}` is not a recognized instruction");
    }
}
