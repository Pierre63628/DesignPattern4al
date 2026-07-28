using DroneFactory.Categorization;
using DroneFactory.Model;
using DroneFactory.Modifications;
using DroneFactory.Templates;

namespace DroneFactory.Parsing;

// Analyse la liste d'arguments ARGS et en fait une DroneCommand.
//  - Format initial : "2 DXF-1, 1 RDL-1" (drones separes par des virgules).
//  - Format etendu (5.2.1) : des qu'une clause WITH / WITHOUT / REPLACE est
//    presente, les drones sont separes par ';' et chaque drone peut porter des
//    modifications (appliquees via le pattern Decorator).
public sealed class ArgsParser
{
    private static readonly string[] Keywords = { "WITH", "WITHOUT", "REPLACE" };

    private readonly TemplateRegistry _templates;
    private readonly CategoryService _categories;

    public ArgsParser(TemplateRegistry templates, CategoryService categories)
    {
        _templates = templates;
        _categories = categories;
    }

    public bool TryParse(string args, out DroneCommand? command, out string error)
    {
        command = null;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(args))
        {
            error = "no command provided";
            return false;
        }

        return HasModifications(args)
            ? TryParseModified(args, out command, out error)
            : TryParseSimple(args, out command, out error);
    }

    private static bool HasModifications(string args)
        => args.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
               .Any(token => Keywords.Contains(token));

    // Format initial : virgules, doublons additionnes (3.1).
    private bool TryParseSimple(string args, out DroneCommand? command, out string error)
    {
        command = null;
        error = string.Empty;

        var quantities = new Dictionary<string, int>();
        foreach (var raw in args.Split(','))
        {
            var part = raw.Trim();
            if (part.Length == 0) continue;

            var tokens = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 2)
            {
                error = $"`{part}` is not a valid quantity/drone pair";
                return false;
            }
            if (!int.TryParse(tokens[0], out int qty) || qty <= 0)
            {
                error = $"`{tokens[0]}` is not a valid quantity";
                return false;
            }
            if (!_templates.Exists(tokens[1]))
            {
                error = $"`{tokens[1]}` is not a recognized drone";
                return false;
            }
            quantities[tokens[1]] = quantities.GetValueOrDefault(tokens[1]) + qty;
        }

        if (quantities.Count == 0)
        {
            error = "no command provided";
            return false;
        }

        var result = new DroneCommand();
        foreach (var (name, qty) in quantities)
        {
            _templates.TryGet(name, out var template);
            result.Add(template!, qty);
        }
        command = result;
        return true;
    }

    // Format etendu : drones separes par ';', chacun avec ses modifications.
    private bool TryParseModified(string args, out DroneCommand? command, out string error)
    {
        command = null;
        error = string.Empty;
        var result = new DroneCommand();

        foreach (var raw in args.Split(';'))
        {
            var segment = raw.Trim();
            if (segment.Length == 0) continue;

            if (!TryParseSegment(segment, out var requested, out error))
                return false;

            result.Items.Add(requested!);
        }

        if (result.Items.Count == 0)
        {
            error = "no command provided";
            return false;
        }

        command = result;
        return true;
    }

    private bool TryParseSegment(string segment, out RequestedDrone? requested, out string error)
    {
        requested = null;
        error = string.Empty;

        // On isole les virgules comme jetons a part pour separer les items des clauses.
        var tokens = segment.Replace(",", " , ").Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length < 2)
        {
            error = $"`{segment}` is not a valid drone entry";
            return false;
        }
        if (!int.TryParse(tokens[0], out int qty) || qty <= 0)
        {
            error = $"`{tokens[0]}` is not a valid quantity";
            return false;
        }
        if (!_templates.TryGet(tokens[1], out var template))
        {
            error = $"`{tokens[1]}` is not a recognized drone";
            return false;
        }

        try
        {
            IDroneModel model = template!;
            int i = 2;
            while (i < tokens.Length)
            {
                string keyword = tokens[i++];
                if (!Keywords.Contains(keyword))
                {
                    error = $"unexpected token `{keyword}`";
                    return false;
                }

                if (!ReadClauseItems(tokens, ref i, out var items, out error))
                    return false;

                model = keyword switch
                {
                    "WITH" => new AddPiecesModification(model, items),
                    "WITHOUT" => new RemovePiecesModification(model, items),
                    "REPLACE" => BuildReplace(model, items, out error),
                    _ => model,
                };
                if (error.Length > 0) return false;
            }

            if (!DroneValidator.Validate(model, _categories, out error))
                return false;

            requested = new RequestedDrone(model, qty);
            return true;
        }
        catch (ModificationException ex)
        {
            error = ex.Message;
            return false;
        }
    }

    // Lit une liste "q1 Piece1, q2 Piece2, ..." jusqu'au prochain mot-cle ou la fin.
    private bool ReadClauseItems(string[] tokens, ref int i, out List<PieceAmount> items, out string error)
    {
        items = new List<PieceAmount>();
        error = string.Empty;

        while (i < tokens.Length && !Keywords.Contains(tokens[i]))
        {
            if (tokens[i] == ",") { i++; continue; }

            if (i + 1 >= tokens.Length || tokens[i + 1] == ",")
            {
                error = "incomplete modification clause";
                return false;
            }
            if (!int.TryParse(tokens[i], out int q) || q <= 0)
            {
                error = $"`{tokens[i]}` is not a valid quantity";
                return false;
            }
            if (!PieceCatalog.TryGet(tokens[i + 1], out var piece))
            {
                error = $"`{tokens[i + 1]}` is not a recognized piece";
                return false;
            }
            items.Add(new PieceAmount(q, piece!));
            i += 2;
        }

        if (items.Count == 0)
        {
            error = "empty modification clause";
            return false;
        }
        return true;
    }

    private static IDroneModel BuildReplace(IDroneModel model, List<PieceAmount> items, out string error)
    {
        error = string.Empty;
        if (items.Count % 2 != 0)
        {
            error = "REPLACE expects pairs of pieces (old then new)";
            return model;
        }

        var replacements = new List<PieceReplacement>();
        for (int j = 0; j < items.Count; j += 2)
            replacements.Add(new PieceReplacement(
                items[j].Quantity, items[j].Piece,
                items[j + 1].Quantity, items[j + 1].Piece));

        return new ReplacePiecesModification(model, replacements);
    }
}
