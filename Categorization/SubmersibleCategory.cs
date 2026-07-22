using DroneFactory.Model;

namespace DroneFactory.Categorization;

// Submersible (S) : "toutes les pieces sont de type (S)" et un systeme (3D).
// Les modules principal et de controle ne sont pas limitants (4.2), on ne
// contraint donc que la coque, le generateur et le module de deplacement.
public sealed class SubmersibleCategory : IDroneCategory
{
    public string Name => "Submersible";
    public char Code => 'S';

    public bool Matches(DroneTemplate drone)
        => drone.Hull.Is(PieceType.S)
        && drone.Generator.Is(PieceType.S)
        && drone.Move.Is(PieceType.S)
        && drone.System.Is(PieceType.D3);
}
