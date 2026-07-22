using DroneFactory.Model;

namespace DroneFactory.Categorization;

// PATTERN STRATEGY
// Chaque categorie de drone (4.2) encapsule sa propre regle de classification.
// Le service de categorisation manipule uniquement cette abstraction : ajouter
// une nouvelle categorie = ajouter une classe, sans toucher au code existant
// (Open/Closed Principle).
public interface IDroneCategory
{
    string Name { get; }   // ex: "Aérien"
    char Code { get; }     // ex: 'F'
    bool Matches(DroneTemplate drone);
}
