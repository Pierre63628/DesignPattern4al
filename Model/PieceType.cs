namespace DroneFactory.Model;

// Les "tags" de type portes par les pieces et les systemes (section 5.2 du sujet V2).
//   F  = deplacement aerien        S  = etanche / submersible
//   L  = deplacement terrestre     D2 = systeme 2D
//   M  = deplacement marin         D3 = systeme 3D
// (D2 / D3 car un identifiant C# ne peut pas commencer par un chiffre.)
public enum PieceType
{
    F,
    L,
    M,
    S,
    D2,
    D3,
}

public static class PieceTypeExtensions
{
    // Affichage conforme au sujet : D2 -> "2D", D3 -> "3D".
    public static string Display(this PieceType type) => type switch
    {
        PieceType.D2 => "2D",
        PieceType.D3 => "3D",
        _ => type.ToString(),
    };
}
