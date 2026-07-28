using DroneFactory.Model;

namespace DroneFactory.Templates;

// Contraintes de construction d'un drone (5.1.2, v3) :
//  - 1 a 3 modules de deplacement, 1 a 2 generateurs ;
//  - des qu'un drone a 2 modules de deplacement ou plus, il lui faut 2 generateurs.
// Ces regles s'appliquent partout ou un drone est valide (templates, modifications).
public static class ConstructionConstraints
{
    public const int MaxMoves = 3;
    public const int MaxGenerators = 2;

    public static bool Validate(IDroneModel drone, out string error)
    {
        if (drone.Moves.Count < 1)
        {
            error = "a drone needs at least one move module";
            return false;
        }
        if (drone.Moves.Count > MaxMoves)
        {
            error = $"a drone can have at most {MaxMoves} move modules";
            return false;
        }
        if (drone.Generators.Count < 1)
        {
            error = "a drone needs at least one generator";
            return false;
        }
        if (drone.Generators.Count > MaxGenerators)
        {
            error = $"a drone can have at most {MaxGenerators} generators";
            return false;
        }
        if (drone.Moves.Count >= 2 && drone.Generators.Count != 2)
        {
            error = "a drone with 2 or more move modules must have exactly 2 generators";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
