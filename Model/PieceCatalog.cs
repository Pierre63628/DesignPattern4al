namespace DroneFactory.Model;

// Catalogue en dur de toutes les pieces connues de l'usine (section 5.2, V2).
// Source unique de verite : les templates et les commandes s'y referent.
public static class PieceCatalog
{
    private static readonly Dictionary<string, Piece> Pieces = new();

    static PieceCatalog()
    {
        // Coques
        Add(new Piece("Hull_HG1", PieceKind.Hull, PieceType.S));
        Add(new Piece("Hull_HF1", PieceKind.Hull));
        Add(new Piece("Hull_HS1", PieceKind.Hull, PieceType.S));

        // Modules principaux (necessitent un systeme installe)
        Add(new Piece("Core_CG1", PieceKind.Core, PieceType.D2));
        Add(new Piece("Core_C3D1", PieceKind.Core, PieceType.D2, PieceType.D3));

        // Generateurs
        Add(new Piece("Generator_GG1", PieceKind.Generator));
        Add(new Piece("Generator_GF1", PieceKind.Generator));
        Add(new Piece("Generator_GS1", PieceKind.Generator, PieceType.S));

        // Modules de deplacement
        Add(new Piece("Move_MF1", PieceKind.Move, PieceType.F));
        Add(new Piece("Move_ML1", PieceKind.Move, PieceType.L));
        Add(new Piece("Move_MS1", PieceKind.Move, PieceType.S));
        Add(new Piece("Move_MM1", PieceKind.Move, PieceType.M));
        Add(new Piece("Move_MU1", PieceKind.Move, PieceType.M, PieceType.L));
        Add(new Piece("Move_MS2", PieceKind.Move, PieceType.M, PieceType.S));

        // Modules de controle
        Add(new Piece("Processor_PG1", PieceKind.Processor, PieceType.D2));
        Add(new Piece("Processor_P3D1", PieceKind.Processor, PieceType.D3));
        Add(new Piece("Processor_PU1", PieceKind.Processor, PieceType.D2, PieceType.D3));

        // Systemes principaux
        Add(new Piece("System_SG1", PieceKind.System, PieceType.D2));
        Add(new Piece("System_S3D1", PieceKind.System, PieceType.D2, PieceType.D3));
    }

    private static void Add(Piece piece) => Pieces[piece.Name] = piece;

    public static bool TryGet(string name, out Piece? piece) => Pieces.TryGetValue(name, out piece);

    // Acces "de confiance" pour les donnees en dur (seed des drones connus).
    public static Piece Get(string name) => Pieces[name];

    public static IEnumerable<Piece> All => Pieces.Values;
}
