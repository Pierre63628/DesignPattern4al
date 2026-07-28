namespace DroneFactory.Instructions;

// PATTERN COMMAND
// Chaque instruction utilisateur (STOCKS, PRODUCE, ...) est encapsulee dans un
// objet a l'interface uniforme. Le REPL ne connait plus le detail des commandes :
// il delegue au dispatcher. Ajouter une instruction = ajouter une classe + un
// enregistrement, sans toucher a la boucle principale (Open/Closed).
public interface IInstruction
{
    // Recoit la portion d'arguments qui suit le mot-cle et produit sa sortie console.
    void Execute(string args);
}
