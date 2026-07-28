namespace DroneFactory.Model;

// Abstraction d'un drone assemblable, consommee par la categorisation, la
// validation et l'assemblage. Un template en est l'implementation nommee ;
// en phase 3, les modifications de drone (Decorator) l'implementent aussi.
//
// Depuis la v3 (5.1.2) un drone peut avoir jusqu'a 3 modules de deplacement
// et 2 generateurs : ce sont donc des listes.
public interface IDroneModel
{
    string Name { get; }
    Piece Hull { get; }
    Piece Core { get; }
    Piece System { get; }
    Piece Processor { get; }
    IReadOnlyList<Piece> Generators { get; }
    IReadOnlyList<Piece> Moves { get; }

    // Toutes les pieces consommees pour un exemplaire (systeme inclus, doublons compris).
    IEnumerable<Piece> AllPieces();
}
