namespace DroneFactory.Model;

// Un drone demande dans une commande : un modele + une quantite.
// (En phase 3, le modele pourra etre un template decore par des modifications.)
public sealed class RequestedDrone
{
    public DroneTemplate Template { get; }
    public int Quantity { get; }

    public RequestedDrone(DroneTemplate template, int quantity)
    {
        Template = template;
        Quantity = quantity;
    }
}

// Une commande = un listing quantifie de drones (les "ARGS" du sujet).
public sealed class DroneCommand
{
    public List<RequestedDrone> Items { get; } = new();

    public void Add(DroneTemplate template, int quantity)
        => Items.Add(new RequestedDrone(template, quantity));

    // Somme totale des pieces necessaires a la commande complete.
    public Dictionary<string, int> NeededPieces()
    {
        var total = new Dictionary<string, int>();
        foreach (var item in Items)
            foreach (var piece in item.Template.AllPieces())
                total[piece.Name] = total.GetValueOrDefault(piece.Name) + item.Quantity;
        return total;
    }
}
