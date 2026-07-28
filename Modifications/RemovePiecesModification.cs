using DroneFactory.Model;

namespace DroneFactory.Modifications;

// WITHOUT : retire des pieces du drone.
public sealed class RemovePiecesModification : DroneModification
{
    public RemovePiecesModification(IDroneModel inner, IReadOnlyList<PieceAmount> removals)
        : base(inner, parts =>
        {
            foreach (var remove in removals)
                parts.RemovePiece(remove.Piece, remove.Quantity);
        })
    { }
}
