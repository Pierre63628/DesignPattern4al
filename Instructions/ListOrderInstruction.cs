using DroneFactory.Orders;

namespace DroneFactory.Instructions;

// LIST_ORDER (5.2.2) : liste les commandes non encore satisfaites et leur reste.
public sealed class ListOrderInstruction : IInstruction
{
    private readonly OrderService _orders;
    public ListOrderInstruction(OrderService orders) => _orders = orders;

    public void Execute(string args)
    {
        foreach (var order in _orders.Remaining)
            Console.WriteLine($"{order.Id}: {order.RemainingText()}");
    }
}
