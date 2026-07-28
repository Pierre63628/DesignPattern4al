using DroneFactory.Model;

namespace DroneFactory.Modifications;

// Un remplacement : "oldQty oldPiece" -> "newQty newPiece".
public readonly record struct PieceReplacement(
    int OldQuantity, Piece OldPiece, int NewQuantity, Piece NewPiece);

// REPLACE : remplace des pieces par d'autres.
public sealed class ReplacePiecesModification : DroneModification
{
    public ReplacePiecesModification(IDroneModel inner, IReadOnlyList<PieceReplacement> replacements)
        : base(inner, parts =>
        {
            foreach (var r in replacements)
                parts.ReplacePiece(r.OldPiece, r.OldQuantity, r.NewPiece, r.NewQuantity);
        })
    { }
}
