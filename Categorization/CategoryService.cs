using DroneFactory.Model;

namespace DroneFactory.Categorization;

// Contexte du pattern Strategy : detient la liste des categories et applique
// leurs regles. Un drone doit appartenir a au moins une categorie (4.2).
public sealed class CategoryService
{
    private readonly List<IDroneCategory> _categories = new()
    {
        new AerialCategory(),
        new MarineCategory(),
        new GroundCategory(),
        new SubmersibleCategory(),
    };

    public IReadOnlyList<IDroneCategory> CategoriesOf(DroneTemplate drone)
        => _categories.Where(c => c.Matches(drone)).ToList();

    public bool HasAnyCategory(DroneTemplate drone)
        => _categories.Any(c => c.Matches(drone));
}
