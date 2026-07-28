using DroneFactory.Model;

namespace DroneFactory.Categorization;

// Marin (M) : une coque etanche (S), un systeme (2D) et un module de deplacement (M).
public sealed class MarineCategory : IDroneCategory
{
    public string Name => "Marin";
    public char Code => 'M';

    public bool Matches(IDroneModel drone)
        => drone.Hull.Is(PieceType.S)
        && drone.System.Is(PieceType.D2)
        && drone.Moves.Any(m => m.Is(PieceType.M));
}
