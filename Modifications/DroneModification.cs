using DroneFactory.Model;

namespace DroneFactory.Modifications;

// PATTERN DECORATOR
// Une modification enveloppe un IDroneModel (template de base ou modification
// deja appliquee) et en presente une version transformee, avec la meme interface.
// Les modifications sont donc composables : WITH puis WITHOUT puis REPLACE, etc.
public abstract class DroneModification : IDroneModel
{
    private readonly IDroneModel _inner;
    private readonly DroneParts _parts;

    // La transformation est passee par les sous-classes via le constructeur (elle
    // capture leurs arguments) : on evite ainsi tout appel virtuel dans le ctor.
    protected DroneModification(IDroneModel inner, Action<DroneParts> apply)
    {
        _inner = inner;
        _parts = DroneParts.From(inner);
        apply(_parts);            // peut lever une ModificationException
        _parts.EnsureStructure(); // garantit des emplacements uniques remplis
    }

    // Le drone modifie garde le nom du drone de base ("un DXF-1 modifie").
    public string Name => _inner.Name;
    public Piece Hull => _parts.Hull!;
    public Piece Core => _parts.Core!;
    public Piece System => _parts.System!;
    public Piece Processor => _parts.Processor!;
    public IReadOnlyList<Piece> Generators => _parts.Generators;
    public IReadOnlyList<Piece> Moves => _parts.Moves;

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

// Un couple quantite / piece, brique des clauses de modification.
public readonly record struct PieceAmount(int Quantity, Piece Piece);
