namespace DroneFactory;

// =====================================================================
//  PROJET DESIGN PATTERNS - Usine de drones
//
//  Point d'entree : simple boucle REPL. Toute la logique vit dans les
//  services et les instructions (pattern Command). Le programme se contente
//  de lire les lignes et de les confier au dispatcher.
//
//  Patterns en place :
//   - STRATEGY  : categorisation des drones      (Categorization/)
//   - COMMAND   : instructions utilisateur        (Instructions/)
//   - (a venir) BUILDER, DECORATOR, OBSERVER      (phases suivantes)
// =====================================================================
public static class Program
{
    public static void Main()
    {
        var factory = new Factory();
        Console.WriteLine("Usine de drones - tapez une instruction (EXIT pour quitter).");

        string? line;
        while ((line = Console.ReadLine()) != null)
        {
            line = line.Trim();
            if (line.Length == 0) continue;
            if (line.Equals("EXIT", StringComparison.OrdinalIgnoreCase)) break;

            factory.Dispatch(line);
        }
    }
}
