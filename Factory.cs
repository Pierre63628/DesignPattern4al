using DroneFactory.Categorization;
using DroneFactory.Instructions;
using DroneFactory.Model;
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
    public ArgsParser Parser { get; }
    public InstructionDispatcher Dispatcher { get; }

    public Factory()
    {
        Categories = new CategoryService();
        Templates = new TemplateRegistry(Categories);
        Stock = new StockService();
        Parser = new ArgsParser(Templates);
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
    }

    public void Dispatch(string line) => Dispatcher.Dispatch(line);
}
