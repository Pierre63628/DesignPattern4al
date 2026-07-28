namespace DroneFactory.Model;

// Un modele (template) de drone nomme et immuable.
// Coque, module principal, systeme et module de controle sont uniques ;
// generateurs et modules de deplacement sont des listes (5.1.2).
public sealed class DroneTemplate : IDroneModel
{
    public string Name { get; }
    public Piece Hull { get; }
    public Piece Core { get; }
    public Piece System { get; }
    public Piece Processor { get; }
    public IReadOnlyList<Piece> Generators { get; }
    public IReadOnlyList<Piece> Moves { get; }

    public DroneTemplate(string name, Piece hull, Piece core, Piece system,
                         IEnumerable<Piece> generators, IEnumerable<Piece> moves, Piece processor)
    {
        Name = name;
        Hull = hull;
        Core = core;
        System = system;
        Processor = processor;
        Generators = generators.ToList();
        Moves = moves.ToList();
    }

    public IEnumerable<Piece> AllPieces()
    {
        yield return Hull;
        yield return Core;
        foreach (var generator in Generators) yield return generator;
        foreach (var move in Moves) yield return move;
        yield return Processor;
        yield return System;
    }
}
