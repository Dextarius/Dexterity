namespace Core
{
    public static class Settings
    {
        public static IFactorLogging Logging { get; set; } = new FactorLogging();
    }
}