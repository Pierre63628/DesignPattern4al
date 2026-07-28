using DroneFactory.Model;

namespace DroneFactory.Categorization;

// Terrestre (L) : un module de deplacement (L) et un systeme (2D).
public sealed class GroundCategory : IDroneCategory
{
    public string Name => "Terrestre";
    public char Code => 'L';

    public bool Matches(IDroneModel drone)
        => drone.Moves.Any(m => m.Is(PieceType.L)) && drone.System.Is(PieceType.D2);
}
