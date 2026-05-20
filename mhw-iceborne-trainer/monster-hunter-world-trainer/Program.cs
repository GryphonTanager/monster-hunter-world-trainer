using System;
using System.Threading;
using MonsterHunterWorldTrainer.Core;

namespace MonsterHunterWorldTrainer
{
    /// <summary>
    /// Entry point for Monster Hunter World: Iceborne Trainer.
    /// Provides a console-based interface for activating various cheats.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Monster Hunter World: Iceborne Trainer";
            Console.WriteLine("Trainer initialized. Attaching to MonsterHunterWorld.exe...");

            // Attempt to attach to the game process
            var memoryManager = new GameMemoryManager("MonsterHunterWorld");
            if (!memoryManager.IsAttached)
            {
                Console.WriteLine("Failed to attach to MonsterHunterWorld.exe. Ensure the game is running.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Attached successfully. Use the following keys to toggle features:");
            Console.WriteLine("  F1 - Infinite Health");
            Console.WriteLine("  F2 - Infinite Stamina");
            Console.WriteLine("  F3 - Max Zenny");
            Console.WriteLine("  F4 - One-Hit Kill");
            Console.WriteLine("  Escape - Exit");

            var trainer = new TrainerEngine(memoryManager);
            bool running = true;

            while (running)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.F1:
                            trainer.ToggleInfiniteHealth();
                            Console.WriteLine($"Infinite Health: {(trainer.InfiniteHealthEnabled ? "ON" : "OFF")}");
                            break;
                        case ConsoleKey.F2:
                            trainer.ToggleInfiniteStamina();
                            Console.WriteLine($"Infinite Stamina: {(trainer.InfiniteStaminaEnabled ? "ON" : "OFF")}");
                            break;
                        case ConsoleKey.F3:
                            trainer.SetMaxZenny();
                            Console.WriteLine("Zenny set to maximum.");
                            break;
                        case ConsoleKey.F4:
                            trainer.ToggleOneHitKill();
                            Console.WriteLine($"One-Hit Kill: {(trainer.OneHitKillEnabled ? "ON" : "OFF")}");
                            break;
                        case ConsoleKey.Escape:
                            running = false;
                            break;
                    }
                }
                Thread.Sleep(50);
            }

            memoryManager.Dispose();
            Console.WriteLine("Trainer detached. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
