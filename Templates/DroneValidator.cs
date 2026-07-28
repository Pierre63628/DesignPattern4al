using DroneFactory.Categorization;
using DroneFactory.Model;

namespace DroneFactory.Templates;

// Validation complete d'un drone, partagee par ADD_TEMPLATE et les modifications
// de drone : contraintes de construction (5.1.2), compatibilite des pieces (4.2)
// et appartenance a au moins une categorie (4.2).
public static class DroneValidator
{
    public static bool Validate(IDroneModel drone, CategoryService categories, out string error)
    {
        if (!ConstructionConstraints.Validate(drone, out error))
            return false;

        if (!Compatibility.CoreHostsSystem(drone.Core, drone.System))
        {
            error = $"`{drone.Core.Name}` cannot host system `{drone.System.Name}`";
            return false;
        }

        if (!Compatibility.ProcessorMatchesSystem(drone.Processor, drone.System))
        {
            error = $"`{drone.Processor.Name}` is not compatible with system `{drone.System.Name}`";
            return false;
        }

        if (!categories.HasAnyCategory(drone))
        {
            error = "the resulting drone does not match any category";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
