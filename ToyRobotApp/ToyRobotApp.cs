using System;
using ToyRobot.Services;

namespace ToyRobot
{
    public class ToyRobotApp
    {
        private IInputHandlerService _inputHandlerService;

        public ToyRobotApp(IInputHandlerService inputHandlerService)
        {
            _inputHandlerService = inputHandlerService;
        }

        public void Run()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nCoding Challenge: Toy Robot");
            Console.WriteLine("===========================\n");
            Console.ResetColor();

            while (true)
            {
                var input = Console.ReadLine();

                if (input.Trim().ToLower() == "quit") break;

                var response = _inputHandlerService.Command(input);

                if (!response.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                Console.WriteLine($"\t> {response.Message}\n");
                Console.ResetColor();

            }
        }
    }
}