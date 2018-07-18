using System;
using System.Diagnostics;

namespace OmegaRace
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the instance
            NetworkGame game = new NetworkGame();
            Debug.Assert(game != null);

            Console.WriteLine("Enter Server IP address:");

            MyClient.Instance.SetIPAddress(Console.ReadLine());

            // Start the game
            game.Run();
        }
    }
}
