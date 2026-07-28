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

    // Construit un DroneTemplate a partir des jetons de pieces fournis :
    // verifie le catalogue et exige une piece de chaque nature + un systeme sur le core.
    private static DroneTemplate? BuildCandidate(string name, IEnumerable<string> pieceTokens, out string error)
    {
        error = string.Empty;
        var byKind = new Dictionary<PieceKind, Piece>();
        Piece? system = null;
        Piece? systemHost = null;

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

            if (byKind.ContainsKey(basePiece!.Kind))
            {
                error = $"a drone needs exactly one {basePiece.Kind.ToString().ToLower()}";
                return null;
            }
            byKind[basePiece.Kind] = basePiece;

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

        foreach (var kind in new[] { PieceKind.Hull, PieceKind.Core, PieceKind.Generator, PieceKind.Move, PieceKind.Processor })
        {
            if (!byKind.ContainsKey(kind))
            {
                error = $"a drone needs a {kind.ToString().ToLower()}";
                return null;
            }
        }

        if (system == null || systemHost!.Kind != PieceKind.Core)
        {
            error = "the main module must have a system installed (Core{System})";
            return null;
        }

        return new DroneTemplate(name,
            byKind[PieceKind.Hull], byKind[PieceKind.Core], system,
            byKind[PieceKind.Generator], byKind[PieceKind.Move], byKind[PieceKind.Processor]);
    }
}
