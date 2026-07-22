using DroneFactory.Categorization;
using DroneFactory.Model;
using DroneFactory.Templates;

namespace DroneFactory;

// =====================================================================
//  PROJET DESIGN PATTERNS - Usine de drones
//  PHASE 2 (en cours) : mise en place progressive des design patterns.
//
//  Ce qui a change par rapport a la version naive :
//   - Modele de domaine propre : Piece (avec tags de type), DroneTemplate,
//     PieceCatalog  (namespace Model).
//   - PATTERN STRATEGY pour la categorisation des drones (namespace
//     Categorization) : une classe par categorie (4.2).
//   - Registre de templates + ADD_TEMPLATE avec validation (4.3).
//
//  Ce qui reste naif pour l'instant (prochaines tranches) :
//   - Generation des instructions d'assemblage (futur COMPOSITE + BUILDER).
//   - Le gros switch sur la commande (futur COMMAND).
// =====================================================================
public static class Program
{
    private static readonly CategoryService Categories = new();
    private static readonly TemplateRegistry Templates = new(Categories);

    // Stock : quantite par nom (pieces ET drones produits).
    private static readonly Dictionary<string, int> Stock = new();

    public static void Main()
    {
        InitStock();
        Console.WriteLine("Usine de drones - tapez une instruction (EXIT pour quitter).");

        string? line;
        while ((line = Console.ReadLine()) != null)
        {
            line = line.Trim();
            if (line.Length == 0) continue;
            if (line.Equals("EXIT", StringComparison.OrdinalIgnoreCase)) break;

            int sp = line.IndexOf(' ');
            string command = sp < 0 ? line : line[..sp];
            string args = sp < 0 ? "" : line[(sp + 1)..].Trim();

            switch (command)
            {
                case "STOCKS":         DoStocks(); break;
                case "NEEDED_STOCKS":  DoNeededStocks(args); break;
                case "INSTRUCTIONS":   DoInstructions(args); break;
                case "VERIFY":         DoVerify(args); break;
                case "PRODUCE":        DoProduce(args); break;
                case "ADD_TEMPLATE":   DoAddTemplate(args); break;
                default:
                    Console.WriteLine($"ERROR `{command}` is not a recognized instruction");
                    break;
            }
        }
    }

    // Le sujet ne donne pas de quantites initiales : pieces a 5, drones a 0.
    private static void InitStock()
    {
        foreach (var piece in PieceCatalog.All) Stock[piece.Name] = 5;
        foreach (var template in Templates.All) Stock[template.Name] = 0;
    }

    private static int StockOf(string name) => Stock.TryGetValue(name, out int q) ? q : 0;

    // -----------------------------------------------------------------
    //  PARSING DES ARGS : "2 DXF-1, 1 RDL-1" -> { DXF-1:2, RDL-1:1 }
    //  Doublons additionnes (3.1). Retourne null + 'error' si invalide.
    // -----------------------------------------------------------------
    private static Dictionary<string, int>? ParseArgs(string args, out string error)
    {
        error = string.Empty;
        var result = new Dictionary<string, int>();

        if (string.IsNullOrWhiteSpace(args))
        {
            error = "no command provided";
            return null;
        }

        foreach (var raw in args.Split(','))
        {
            var part = raw.Trim();
            if (part.Length == 0) continue;

            var tokens = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 2)
            {
                error = $"`{part}` is not a valid quantity/drone pair";
                return null;
            }

            if (!int.TryParse(tokens[0], out int qty) || qty <= 0)
            {
                error = $"`{tokens[0]}` is not a valid quantity";
                return null;
            }

            string drone = tokens[1];
            if (!Templates.Exists(drone))
            {
                error = $"`{drone}` is not a recognized drone";
                return null;
            }

            result[drone] = result.GetValueOrDefault(drone) + qty;
        }

        if (result.Count == 0)
        {
            error = "no command provided";
            return null;
        }
        return result;
    }

    // Somme totale des pieces necessaires a une commande.
    private static Dictionary<string, int> NeededPieces(Dictionary<string, int> command)
    {
        var total = new Dictionary<string, int>();
        foreach (var (droneName, qty) in command)
        {
            Templates.TryGet(droneName, out var drone);
            foreach (var piece in drone!.AllPieces())
                total[piece.Name] = total.GetValueOrDefault(piece.Name) + qty;
        }
        return total;
    }

    // -----------------------------------------------------------------
    //  STOCKS
    // -----------------------------------------------------------------
    private static void DoStocks()
    {
        foreach (var template in Templates.All)
            Console.WriteLine($"{StockOf(template.Name)} {template.Name}");
        foreach (var piece in PieceCatalog.All)
            Console.WriteLine($"{StockOf(piece.Name)} {piece.Name}");
    }

    // -----------------------------------------------------------------
    //  NEEDED_STOCKS ARGS
    // -----------------------------------------------------------------
    private static void DoNeededStocks(string args)
    {
        var command = ParseArgs(args, out string error);
        if (command == null) { Console.WriteLine($"ERROR {error}"); return; }

        foreach (var (droneName, qty) in command)
        {
            Templates.TryGet(droneName, out var drone);
            Console.WriteLine($"{qty} {droneName} :");
            foreach (var piece in drone!.AllPieces())
                Console.WriteLine($"{qty} {piece.Name}");
        }

        Console.WriteLine("Total :");
        foreach (var (piece, qty) in NeededPieces(command))
            Console.WriteLine($"{qty} {piece}");
    }

    // -----------------------------------------------------------------
    //  INSTRUCTIONS ARGS  (encore naif : sera refactore en Composite/Builder)
    // -----------------------------------------------------------------
    private static void DoInstructions(string args)
    {
        var command = ParseArgs(args, out string error);
        if (command == null) { Console.WriteLine($"ERROR {error}"); return; }

        foreach (var (droneName, qty) in command)
        {
            Templates.TryGet(droneName, out var drone);
            for (int i = 0; i < qty; i++)
                PrintDroneInstructions(drone!);
        }
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

    // -----------------------------------------------------------------
    //  VERIFY ARGS
    // -----------------------------------------------------------------
    private static void DoVerify(string args)
    {
        var command = ParseArgs(args, out string error);
        if (command == null) { Console.WriteLine($"ERROR {error}"); return; }

        Console.WriteLine(HasEnoughStock(command) ? "AVAILABLE" : "UNAVAILABLE");
    }

    private static bool HasEnoughStock(Dictionary<string, int> command)
        => NeededPieces(command).All(kv => StockOf(kv.Key) >= kv.Value);

    // -----------------------------------------------------------------
    //  PRODUCE ARGS
    // -----------------------------------------------------------------
    private static void DoProduce(string args)
    {
        var command = ParseArgs(args, out string error);
        if (command == null) { Console.WriteLine($"ERROR {error}"); return; }

        if (!HasEnoughStock(command))
        {
            Console.WriteLine("ERROR not enough pieces in stock to produce this command");
            return;
        }

        foreach (var (piece, qty) in NeededPieces(command))
            Stock[piece] = StockOf(piece) - qty;
        foreach (var (drone, qty) in command)
            Stock[drone] = StockOf(drone) + qty;

        Console.WriteLine("STOCK_UPDATED");
    }

    // -----------------------------------------------------------------
    //  ADD_TEMPLATE NAME, Piece1, ..., PieceN   (4.3)
    //  Le systeme est fourni via la notation Core{System} (3.3).
    // -----------------------------------------------------------------
    private static void DoAddTemplate(string args)
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

        if (!Templates.TryAdd(candidate, out error)) { Console.WriteLine($"ERROR {error}"); return; }

        Stock[candidate.Name] = 0;
        Console.WriteLine("TEMPLATE_ADDED");
    }

    // Construit un DroneTemplate a partir des jetons de pieces fournis.
    // Verifie le catalogue et la composition (une piece de chaque nature).
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

        // Verifie qu'on a bien une piece de chaque nature + un systeme sur le core.
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
