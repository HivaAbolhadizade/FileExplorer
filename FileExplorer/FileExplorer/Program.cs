namespace FileExplorer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            if (File.ReadAllText(@"Texts/FirstRun.txt") == "1")
            {
                File.WriteAllText(@"Texts/FirstRun.txt", "0");
                Application.Run(new Getsize_page());
            }
            Application.Run(new mainForm());
        }
    }
}