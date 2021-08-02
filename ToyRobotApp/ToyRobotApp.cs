using System;
using ToyRobot.Services;

namespace ToyRobot
{
    public class ToyRobotApp
    {
        private ICommandParserService _commandParserService;

        public ToyRobotApp(ICommandParserService commandParserService)
        {
            _commandParserService = commandParserService;
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

                if (input.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase)) break;

                var response = _commandParserService.Command(input);

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