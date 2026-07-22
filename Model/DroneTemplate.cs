namespace DroneFactory.Model;

// Un modele (template) de drone : les 5 pieces + le systeme installe sur le core.
// Immuable : un template valide reste valide.
public sealed class DroneTemplate
{
    public string Name { get; }
    public Piece Hull { get; }
    public Piece Core { get; }
    public Piece System { get; }
    public Piece Generator { get; }
    public Piece Move { get; }
    public Piece Processor { get; }

    public DroneTemplate(string name, Piece hull, Piece core, Piece system,
                         Piece generator, Piece move, Piece processor)
    {
        Name = name;
        Hull = hull;
        Core = core;
        System = system;
        Generator = generator;
        Move = move;
        Processor = processor;
    }

    // Toutes les pieces consommees pour produire un exemplaire (systeme inclus).
    public IEnumerable<Piece> AllPieces()
    {
        yield return Hull;
        yield return Core;
        yield return Generator;
        yield return Move;
        yield return Processor;
        yield return System;
    }
}
