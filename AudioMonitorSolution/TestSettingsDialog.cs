using System;
using AudioMonitor.Core.Models;
using AudioMonitor.Core.Services;
using AudioMonitor.UI.Views;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Testing SettingsWindow instantiation...");
                
                // Create default settings
                var settings = new ApplicationSettings();
                
                // Create audio service
                var audioService = new AudioService();
                
                // Try to create SettingsWindow
                var settingsWindow = new SettingsWindow(settings, audioService);
                
                Console.WriteLine("SUCCESS: SettingsWindow created without errors!");
                Console.WriteLine("The ComboBox ItemsSource issue has been resolved.");
                
                settingsWindow.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
