using DroneFactory.Categorization;
using DroneFactory.Model;

namespace DroneFactory.Templates;

// Registre des templates de drones. Contient les drones connus (5.2) et
// accepte l'ajout dynamique via ADD_TEMPLATE (4.3), sous reserve de validation.
// Toutes les instructions (VERIFY, PRODUCE, ...) passent par ce registre, donc
// un template ajoute devient automatiquement utilisable partout.
public sealed class TemplateRegistry
{
    private readonly Dictionary<string, DroneTemplate> _templates = new();
    private readonly CategoryService _categories;

    public TemplateRegistry(CategoryService categories)
    {
        _categories = categories;
        SeedKnownDrones();
    }

    private void SeedKnownDrones()
    {
        Register(Build("DXF-1", "Hull_HF1", "Core_C3D1", "System_S3D1", "Generator_GF1", "Move_MF1", "Processor_P3D1"));
        Register(Build("RDL-1", "Hull_HG1", "Core_CG1", "System_SG1", "Generator_GG1", "Move_ML1", "Processor_PG1"));
        Register(Build("WDS-1", "Hull_HS1", "Core_C3D1", "System_S3D1", "Generator_GS1", "Move_MS1", "Processor_P3D1"));
        Register(Build("DYM-1", "Hull_HG1", "Core_CG1", "System_SG1", "Generator_GG1", "Move_MM1", "Processor_PG1"));
    }

    private static DroneTemplate Build(string name, string hull, string core, string system,
                                       string generator, string move, string processor)
        => new(name,
               PieceCatalog.Get(hull), PieceCatalog.Get(core), PieceCatalog.Get(system),
               new[] { PieceCatalog.Get(generator) }, new[] { PieceCatalog.Get(move) }, PieceCatalog.Get(processor));

    private void Register(DroneTemplate template) => _templates[template.Name] = template;

    public bool Exists(string name) => _templates.ContainsKey(name);

    public bool TryGet(string name, out DroneTemplate? template) => _templates.TryGetValue(name, out template);

    public IEnumerable<DroneTemplate> All => _templates.Values;

    // Tente d'ajouter un template deja construit. Valide la compatibilite des
    // pieces puis l'appartenance a au moins une categorie (4.2 / 4.3).
    public bool TryAdd(DroneTemplate candidate, out string error)
    {
        if (Exists(candidate.Name))
        {
            error = $"template `{candidate.Name}` already exists";
            return false;
        }

        if (!ConstructionConstraints.Validate(candidate, out error))
            return false;

        if (!Compatibility.CoreHostsSystem(candidate.Core, candidate.System))
        {
            error = $"`{candidate.Core.Name}` cannot host system `{candidate.System.Name}`";
            return false;
        }

        if (!Compatibility.ProcessorMatchesSystem(candidate.Processor, candidate.System))
        {
            error = $"`{candidate.Processor.Name}` is not compatible with system `{candidate.System.Name}`";
            return false;
        }

        if (!_categories.HasAnyCategory(candidate))
        {
            error = "the resulting drone does not match any category";
            return false;
        }

        Register(candidate);
        error = string.Empty;
        return true;
    }
}
