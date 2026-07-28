using DroneFactory.Model;

namespace DroneFactory.Instructions;

// ADD_TEMPLATE NAME, Piece1, ..., PieceN  (4.3)
// Le systeme est fourni via la notation Core{System} (3.3). L'ajout est valide
// par le registre (compatibilite + categorie).
public sealed class AddTemplateInstruction : IInstruction
{
    private readonly Factory _ctx;
    public AddTemplateInstruction(Factory ctx) => _ctx = ctx;

    public void Execute(string args)
    {
        var parts = args.Split(',');
        if (parts.Length < 2)
        {
            Console.WriteLine("ERROR ADD_TEMPLATE expects a name and a list of pieces");
            return;
        }

        string name = parts[0].Trim();
        if (name.Length == 0)
        {
            Console.WriteLine("ERROR template name is empty");
            return;
        }

        var candidate = BuildCandidate(name, parts.Skip(1), out string error);
        if (candidate == null) { Console.WriteLine($"ERROR {error}"); return; }

        if (!_ctx.Templates.TryAdd(candidate, out error)) { Console.WriteLine($"ERROR {error}"); return; }

        _ctx.Stock.Set(candidate.Name, 0);
        Console.WriteLine("TEMPLATE_ADDED");
    }

    // Construit un DroneTemplate a partir des jetons de pieces fournis.
    // Coque / module principal / module de controle sont uniques ; generateurs
    // et modules de deplacement peuvent etre multiples (les comptes sont valides
    // ensuite par ConstructionConstraints via le registre).
    private static DroneTemplate? BuildCandidate(string name, IEnumerable<string> pieceTokens, out string error)
    {
        error = string.Empty;
        Piece? hull = null, core = null, processor = null, system = null, systemHost = null;
        var generators = new List<Piece>();
        var moves = new List<Piece>();

        foreach (var rawToken in pieceTokens)
        {
            var token = rawToken.Trim();
            if (token.Length == 0) continue;

            // Notation Core{System} : on separe la piece de base de son systeme.
            string baseName = token;
            string? systemName = null;
            int brace = token.IndexOf('{');
            if (brace >= 0)
            {
                int close = token.IndexOf('}');
                if (close < brace)
                {
                    error = $"`{token}` has a malformed system notation";
                    return null;
                }
                baseName = token[..brace];
                systemName = token[(brace + 1)..close];
            }

            if (!PieceCatalog.TryGet(baseName, out var basePiece))
            {
                error = $"`{baseName}` is not a recognized piece";
                return null;
            }

            switch (basePiece!.Kind)
            {
                case PieceKind.Hull:
                    if (hull != null) { error = "a drone needs exactly one hull"; return null; }
                    hull = basePiece; break;
                case PieceKind.Core:
                    if (core != null) { error = "a drone needs exactly one main module"; return null; }
                    core = basePiece; break;
                case PieceKind.Processor:
                    if (processor != null) { error = "a drone needs exactly one control module"; return null; }
                    processor = basePiece; break;
                case PieceKind.Generator:
                    generators.Add(basePiece); break;
                case PieceKind.Move:
                    moves.Add(basePiece); break;
                case PieceKind.System:
                    error = "a system must be installed on the main module (Core{System})"; return null;
            }

            if (systemName != null)
            {
                if (!PieceCatalog.TryGet(systemName, out var systemPiece) || systemPiece!.Kind != PieceKind.System)
                {
                    error = $"`{systemName}` is not a recognized system";
                    return null;
                }
                if (system != null)
                {
                    error = "a drone can only have one system installed";
                    return null;
                }
                system = systemPiece;
                systemHost = basePiece;
            }
        }

        if (hull == null) { error = "a drone needs a hull"; return null; }
        if (core == null) { error = "a drone needs a main module"; return null; }
        if (processor == null) { error = "a drone needs a control module"; return null; }
        if (system == null || systemHost!.Kind != PieceKind.Core)
        {
            error = "the main module must have a system installed (Core{System})";
            return null;
        }

        return new DroneTemplate(name, hull, core, system, generators, moves, processor);
    }
}
