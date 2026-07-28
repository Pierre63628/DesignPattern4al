using DroneFactory.Model;

namespace DroneFactory.Categorization;

// Aerien (F) : un module de deplacement (F) et un systeme (3D).
public sealed class AerialCategory : IDroneCategory
{
    public string Name => "Aérien";
    public char Code => 'F';

    public bool Matches(IDroneModel drone)
        => drone.Moves.Any(m => m.Is(PieceType.F)) && drone.System.Is(PieceType.D3);
}
