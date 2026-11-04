using GarageApp.Interfaces;

namespace GarageApp
{
    internal class Program
    {
        static void Main()
        {
            IHandler handler = new GarageHandler();
            IUI ui = new ConsoleUI(handler);
            ui.Run();
        }
    }
}