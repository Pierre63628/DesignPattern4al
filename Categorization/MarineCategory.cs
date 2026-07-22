using DroneFactory.Model;

namespace DroneFactory.Categorization;

// Marin (M) : une coque etanche (S), un systeme (2D) et un module de deplacement (M).
public sealed class MarineCategory : IDroneCategory
{
    public string Name => "Marin";
    public char Code => 'M';

    public bool Matches(DroneTemplate drone)
        => drone.Hull.Is(PieceType.S)
        && drone.System.Is(PieceType.D2)
        && drone.Move.Is(PieceType.M);
}
