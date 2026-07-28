using DroneFactory.Categorization;
using DroneFactory.Instructions;
using DroneFactory.Model;
using DroneFactory.Orders;
using DroneFactory.Parsing;
using DroneFactory.Services;
using DroneFactory.Templates;

namespace DroneFactory;

// Contexte applicatif : cree et relie les services, ensemence le stock et
// enregistre les instructions (Command). Sert de point d'acces unique partage
// par les instructions.
public sealed class Factory
{
    public TemplateRegistry Templates { get; }
    public CategoryService Categories { get; }
    public StockService Stock { get; }
    public MovementLog Movements { get; }
    public OrderService Orders { get; }
    public ArgsParser Parser { get; }
    public InstructionDispatcher Dispatcher { get; }

    public Factory()
    {
        Categories = new CategoryService();
        Templates = new TemplateRegistry(Categories);
        Stock = new StockService();
        Movements = new MovementLog();
        Stock.Subscribe(Movements); // Observer : le journal ecoute le stock.
        Orders = new OrderService();
        Parser = new ArgsParser(Templates, Categories);
        Dispatcher = new InstructionDispatcher();

        SeedStock();
        RegisterInstructions();
    }

    // Le sujet ne fixe pas de quantites initiales : pieces a 5, drones a 0.
    private void SeedStock()
    {
        foreach (var piece in PieceCatalog.All) Stock.Set(piece.Name, 5);
        foreach (var template in Templates.All) Stock.Set(template.Name, 0);
    }

    private void RegisterInstructions()
    {
        Dispatcher.Register("STOCKS", new StocksInstruction(this));
        Dispatcher.Register("NEEDED_STOCKS", new NeededStocksInstruction(this));
        Dispatcher.Register("INSTRUCTIONS", new InstructionsInstruction(this));
        Dispatcher.Register("VERIFY", new VerifyInstruction(this));
        Dispatcher.Register("PRODUCE", new ProduceInstruction(this));
        Dispatcher.Register("ADD_TEMPLATE", new AddTemplateInstruction(this));
        Dispatcher.Register("RECEIVE", new ReceiveInstruction(this));
        Dispatcher.Register("GET_MOVEMENTS", new GetMovementsInstruction(Movements));
        Dispatcher.Register("ORDER", new OrderInstruction(this));
        Dispatcher.Register("SEND", new SendInstruction(this));
        Dispatcher.Register("LIST_ORDER", new ListOrderInstruction(Orders));
    }

    public void Dispatch(string line) => Dispatcher.Dispatch(line);
}
