namespace DroneFactory.Model;

// La nature d'une piece. Utile pour la categorisation : les regles portent
// sur la coque, le generateur et le module de deplacement ; le module
// principal (Core) et de controle (Processor) ne sont pas limitants (4.2).
public enum PieceKind
{
    Hull,       // coque
    Core,       // module principal
    Generator,  // generateur
    Move,        // module de deplacement
    Processor,  // module de controle
    System,     // systeme installe sur le core
}

// Une piece elementaire du catalogue : un nom, une nature et un jeu de tags.
public sealed class Piece
{
    public string Name { get; }
    public PieceKind Kind { get; }
    public IReadOnlySet<PieceType> Types { get; }

    public Piece(string name, PieceKind kind, params PieceType[] types)
    {
        Name = name;
        Kind = kind;
        Types = new HashSet<PieceType>(types);
    }

    public bool Is(PieceType type) => Types.Contains(type);

    public override string ToString() => Name;
}
