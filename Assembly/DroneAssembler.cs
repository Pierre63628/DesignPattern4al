using DroneFactory.Model;

namespace DroneFactory.Assembly;

// PATTERN BUILDER (partie "Directeur")
// Encode l'ORDRE d'assemblage impose par le sujet, independamment de la sortie :
//   1. sortir toutes les pieces du stock (3.2.3) ;
//   2. installer le systeme sur le module principal ;
//   3. assembler : coque -> generateur(s) -> module(s) de deplacement
//      -> module principal{systeme} -> module de controle.
// Contraintes respectees : seul le generateur precede le module principal,
// le module de deplacement vient apres la coque, le systeme est installe avant.
public static class DroneAssembler
{
    public static IReadOnlyList<string> BuildInstructions(IDroneModel drone)
    {
        var builder = new ConsoleAssemblyBuilder();

        builder.StartDrone(drone.Name);

        // 1. Sortie de stock, dans l'ordre coque, core, generateurs, deplacements, controle.
        builder.GetOutStock(drone.Hull.Name);
        builder.GetOutStock(drone.Core.Name);
        foreach (var generator in drone.Generators) builder.GetOutStock(generator.Name);
        foreach (var move in drone.Moves) builder.GetOutStock(move.Name);
        builder.GetOutStock(drone.Processor.Name);

        // 2. Installation du systeme sur le module principal.
        builder.Install(drone.System.Name, drone.Core.Name);

        // 3. Chaine d'assemblage.
        builder.BeginFrom(drone.Hull.Name);
        foreach (var generator in drone.Generators) builder.AssembleWith(generator.Name);
        foreach (var move in drone.Moves) builder.AssembleWith(move.Name);
        builder.AssembleWith($"{drone.Core.Name}{{{drone.System.Name}}}");
        builder.AssembleWith(drone.Processor.Name);

        builder.FinishDrone(drone.Name);

        return builder.Lines;
    }
}
