namespace DroneFactory.Model;

// Un drone demande dans une commande : un modele + une quantite.
// Le modele est une abstraction : template simple ou template decore (phase 3).
public sealed class RequestedDrone
{
    public IDroneModel Model { get; }
    public int Quantity { get; }

    public RequestedDrone(IDroneModel model, int quantity)
    {
        Model = model;
        Quantity = quantity;
    }
}

// Une commande = un listing quantifie de drones (les "ARGS" du sujet).
public sealed class DroneCommand
{
    public List<RequestedDrone> Items { get; } = new();

    public void Add(IDroneModel model, int quantity)
        => Items.Add(new RequestedDrone(model, quantity));

    // Somme totale des pieces necessaires a la commande complete.
    public Dictionary<string, int> NeededPieces()
    {
        var total = new Dictionary<string, int>();
        foreach (var item in Items)
            foreach (var piece in item.Model.AllPieces())
                total[piece.Name] = total.GetValueOrDefault(piece.Name) + item.Quantity;
        return total;
    }
}
