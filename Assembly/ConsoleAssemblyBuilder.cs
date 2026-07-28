namespace DroneFactory.Assembly;

// Builder concret : accumule les lignes d'instructions au format console du sujet.
// Choix assume : on nomme systematiquement chaque assemblage intermediaire
// (TMP1, TMP2, ...). C'est toujours valide (le sujet autorise a nommer ou non)
// et cela se generalise proprement a plusieurs generateurs / modules de deplacement.
public sealed class ConsoleAssemblyBuilder : IAssemblyBuilder
{
    private readonly List<string> _lines = new();
    private string _current = string.Empty;
    private int _tmpIndex;

    public void StartDrone(string droneName) => _lines.Add($"PRODUCING {droneName}");

    public void GetOutStock(string pieceName) => _lines.Add($"GET_OUT_STOCK 1 {pieceName}");

    public void Install(string systemName, string pieceName)
        => _lines.Add($"INSTALL {systemName} {pieceName}");

    public void BeginFrom(string pieceNotation) => _current = pieceNotation;

    public void AssembleWith(string pieceNotation)
    {
        _tmpIndex++;
        string result = $"TMP{_tmpIndex}";
        _lines.Add($"ASSEMBLE {result} {_current} {pieceNotation}");
        _current = result;
    }

    public void FinishDrone(string droneName) => _lines.Add($"FINISHED {droneName}");

    public IReadOnlyList<string> Lines => _lines;
}
