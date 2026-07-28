using DroneFactory.Model;

namespace DroneFactory.Modifications;

// WITH : ajoute des pieces au drone.
public sealed class AddPiecesModification : DroneModification
{
    public AddPiecesModification(IDroneModel inner, IReadOnlyList<PieceAmount> additions)
        : base(inner, parts =>
        {
            foreach (var add in additions)
                parts.AddPiece(add.Piece, add.Quantity);
        })
    { }
}
