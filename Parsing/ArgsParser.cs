using DroneFactory.Model;
using DroneFactory.Templates;

namespace DroneFactory.Parsing;

// Analyse la liste d'arguments ARGS et en fait une DroneCommand.
// Etape 1 : format initial "2 DXF-1, 1 RDL-1" (drones separes par des virgules,
// doublons additionnes, cf. 3.1). Le format etendu (WITH/WITHOUT/REPLACE, ';')
// sera ajoute lors du module de modification de drones (5.2.1).
public sealed class ArgsParser
{
    private readonly TemplateRegistry _templates;

    public ArgsParser(TemplateRegistry templates) => _templates = templates;

    public bool TryParse(string args, out DroneCommand? command, out string error)
    {
        command = null;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(args))
        {
            error = "no command provided";
            return false;
        }

        // On additionne les quantites par nom de drone pour le format simple.
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

            string droneName = tokens[1];
            if (!_templates.Exists(droneName))
            {
                error = $"`{droneName}` is not a recognized drone";
                return false;
            }

            quantities[droneName] = quantities.GetValueOrDefault(droneName) + qty;
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
}
