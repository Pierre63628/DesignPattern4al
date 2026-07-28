using DroneFactory.Model;

namespace DroneFactory.Modifications;

// Erreur de modification de drone (piece introuvable, retrait impossible, ...).
public sealed class ModificationException : Exception
{
    public ModificationException(string message) : base(message) { }
}

// Structure de travail mutable representant les pieces d'un drone pendant
// l'application des modifications. Les emplacements uniques (coque, module
// principal, systeme, module de controle) restent toujours remplis apres une
// operation valide ; generateurs et modules de deplacement sont des listes.
public sealed class DroneParts
{
    public Piece? Hull { get; private set; }
    public Piece? Core { get; private set; }
    public Piece? System { get; private set; }
    public Piece? Processor { get; private set; }
    public List<Piece> Generators { get; } = new();
    public List<Piece> Moves { get; } = new();

    public static DroneParts From(IDroneModel model)
    {
        var parts = new DroneParts
        {
            Hull = model.Hull,
            Core = model.Core,
            System = model.System,
            Processor = model.Processor,
        };
        parts.Generators.AddRange(model.Generators);
        parts.Moves.AddRange(model.Moves);
        return parts;
    }

    // WITH : ajoute des pieces.
    public void AddPiece(Piece piece, int quantity)
    {
        switch (piece.Kind)
        {
            case PieceKind.Generator:
                for (int i = 0; i < quantity; i++) Generators.Add(piece);
                break;
            case PieceKind.Move:
                for (int i = 0; i < quantity; i++) Moves.Add(piece);
                break;
            case PieceKind.Hull:
                Hull = SetSingle(Hull, piece, quantity, "hull");
                break;
            case PieceKind.Core:
                Core = SetSingle(Core, piece, quantity, "main module");
                break;
            case PieceKind.Processor:
                Processor = SetSingle(Processor, piece, quantity, "control module");
                break;
            case PieceKind.System:
                System = SetSingle(System, piece, quantity, "system");
                break;
        }
    }

    // WITHOUT : retire des pieces (uniquement generateurs / modules de deplacement).
    public void RemovePiece(Piece piece, int quantity)
    {
        switch (piece.Kind)
        {
            case PieceKind.Generator:
                RemoveFromList(Generators, piece.Name, quantity, "generator");
                break;
            case PieceKind.Move:
                RemoveFromList(Moves, piece.Name, quantity, "move module");
                break;
            default:
                throw new ModificationException(
                    $"cannot remove `{piece.Name}`: use REPLACE to change a unique part");
        }
    }

    // REPLACE : retire l'ancienne piece puis ajoute la nouvelle (echange autorise
    // sur les emplacements uniques).
    public void ReplacePiece(Piece oldPiece, int oldQty, Piece newPiece, int newQty)
    {
        RemoveForReplace(oldPiece, oldQty);
        AddPiece(newPiece, newQty);
    }

    // Verifie que les emplacements uniques sont bien remplis (les operations les
    // gardent normalement remplis, ce filet de securite protege les accesseurs).
    public void EnsureStructure()
    {
        if (Hull == null) throw new ModificationException("a drone must have a hull");
        if (Core == null) throw new ModificationException("a drone must have a main module");
        if (System == null) throw new ModificationException("a drone must have a system");
        if (Processor == null) throw new ModificationException("a drone must have a control module");
    }

    private static Piece SetSingle(Piece? current, Piece piece, int quantity, string label)
    {
        if (current != null)
            throw new ModificationException($"a drone can only have one {label}");
        if (quantity != 1)
            throw new ModificationException($"a drone can only have one {label}");
        return piece;
    }

    private void RemoveForReplace(Piece piece, int quantity)
    {
        switch (piece.Kind)
        {
            case PieceKind.Generator:
                RemoveFromList(Generators, piece.Name, quantity, "generator");
                break;
            case PieceKind.Move:
                RemoveFromList(Moves, piece.Name, quantity, "move module");
                break;
            case PieceKind.Hull:
                Hull = ClearSingle(Hull, piece, "hull");
                break;
            case PieceKind.Core:
                Core = ClearSingle(Core, piece, "main module");
                break;
            case PieceKind.Processor:
                Processor = ClearSingle(Processor, piece, "control module");
                break;
            case PieceKind.System:
                System = ClearSingle(System, piece, "system");
                break;
        }
    }

    private static Piece? ClearSingle(Piece? current, Piece piece, string label)
    {
        if (current == null || current.Name != piece.Name)
            throw new ModificationException($"there is no `{piece.Name}` {label} to replace");
        return null;
    }

    private static void RemoveFromList(List<Piece> list, string pieceName, int quantity, string label)
    {
        int available = list.Count(p => p.Name == pieceName);
        if (available < quantity)
            throw new ModificationException($"not enough `{pieceName}` {label}(s) to remove");

        int removed = 0;
        for (int i = list.Count - 1; i >= 0 && removed < quantity; i--)
        {
            if (list[i].Name == pieceName) { list.RemoveAt(i); removed++; }
        }
    }
}
