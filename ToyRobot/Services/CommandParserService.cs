using System;
using System.Linq;
using ToyRobot.Constants;
using ToyRobot.Enums;
using ToyRobot.Models;

namespace ToyRobot.Services
{
    public class CommandParserService : ICommandParserService
    {
        private IRobotService _robotService;

        public CommandParserService(IRobotService robotService)
        {
            _robotService = robotService;
        }

        public Result Command(string command)
        {
            var trimmedCommand = command.Trim();
            var commandParts = trimmedCommand.Split(' ').Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();

            CommandEnum selectedCommand;

            if (parseEnum<CommandEnum>(commandParts[0], out selectedCommand))
            {
                switch (selectedCommand)
                {
                    case CommandEnum.Place:
                        return placeCommand(commandParts.Skip(1).ToArray());
                    case CommandEnum.Report:
                        return _robotService.ReportState();
                    case CommandEnum.Left:
                        return _robotService.TurnLeft();
                    case CommandEnum.Right:
                        return _robotService.TurnRight();
                    case CommandEnum.Move:
                        return _robotService.Move();
                }
            }

            return Result.Failed($"{ResponseMessageConstants.CommandUnknown} {commandParts[0]}.");
        }

        private Result placeCommand(string[] arguments)
        {
            if (arguments.Length == 0)
            {
                return Result.Failed(ResponseMessageConstants.CommandNotEnoughArguments);
            }
            else if (arguments.Length > 1)
            {
                return Result.Failed(ResponseMessageConstants.CommandTooManyArguments);
            }

            var splitArguments = arguments[0].Split(',');

            int x;
            int y;
            DirectionEnum orientation;

            if (splitArguments.Length == 3 &&
                int.TryParse(splitArguments[0], out x) &&
                int.TryParse(splitArguments[1], out y) &&
                parseEnum<DirectionEnum>(splitArguments[2], out orientation))
            {
                return _robotService.SetState(x, y, orientation);
            }
            else if (splitArguments.Length == 2 &&
              int.TryParse(splitArguments[0], out x) &&
              int.TryParse(splitArguments[1], out y))
            {
                return _robotService.SetState(x, y);
            }

            return Result.Failed(ResponseMessageConstants.CommandInvalidArguments);
        }

        private bool parseEnum<T>(string value, out T result) where T : struct, Enum
        {
            return Enum.TryParse<T>(value, true, out result) &&
                !result.Equals(default(T)) &&
                Enum.IsDefined<T>(result) &&
                string.Equals(value, result.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}