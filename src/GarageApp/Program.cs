namespace GarageApp
{
    internal class Program
    {
        static void Main()
        {
            GarageHandler handler = new GarageHandler();
            ConsoleUI ui = new ConsoleUI(handler);
            ui.Run();
        }
    }
}