namespace GarageApp
{
    // Read-only facts about one garage, so the UI can show a list
    // without getting hold of the garage itself.
    public record GarageSummary(string Name, int Capacity, int ParkedCount, double FreeSpaces, bool IsCurrent);
}