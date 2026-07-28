namespace DroneFactory.Assembly;

// PATTERN BUILDER (partie "Builder")
// Abstraction de la construction pas-a-pas d'une sequence d'assemblage.
// Le directeur (DroneAssembler) pilote les etapes ; le builder decide de la
// representation produite. Ici une representation console, mais on pourrait en
// ecrire d'autres (graphe, JSON...) sans changer le directeur.
public interface IAssemblyBuilder
{
    void StartDrone(string droneName);
    void GetOutStock(string pieceName);
    void Install(string systemName, string pieceName);

    // Point de depart de la chaine d'assemblage (typiquement la coque).
    void BeginFrom(string pieceNotation);

    // Assemble la piece courante avec la suivante ; nomme le resultat (TMPk).
    void AssembleWith(string pieceNotation);

    void FinishDrone(string droneName);
}
