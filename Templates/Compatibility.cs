using DroneFactory.Model;

namespace DroneFactory.Templates;

// Regles de compatibilite entre pieces (4.2).
// Le sujet reste flou sur la definition exacte : on retient une lecture simple
// et coherente avec les 4 drones connus, a documenter dans le rapport.
public static class Compatibility
{
    // Le module principal doit pouvoir accueillir le systeme : toutes les
    // dimensions (2D/3D) du systeme doivent etre supportees par le core.
    public static bool CoreHostsSystem(Piece core, Piece system)
        => Dimensions(system).IsSubsetOf(Dimensions(core));

    // Le module de controle doit partager au moins une dimension avec le systeme.
    public static bool ProcessorMatchesSystem(Piece processor, Piece system)
        => Dimensions(processor).Overlaps(Dimensions(system));

    private static HashSet<PieceType> Dimensions(Piece piece)
        => piece.Types.Where(t => t is PieceType.D2 or PieceType.D3).ToHashSet();
}
