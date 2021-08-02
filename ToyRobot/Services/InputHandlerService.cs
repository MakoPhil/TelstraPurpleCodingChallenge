using System;
using System.Linq;
using ToyRobot.Constants;
using ToyRobot.Enums;
using ToyRobot.Models;

namespace ToyRobot.Services
{
    public class InputHandlerService : IInputHandlerService
    {
        private IRobotService _robotService;
        private bool _isReady = false;

        public InputHandlerService(IRobotService robotService)
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
                        return Report();
                    case CommandEnum.Left:
                        return Left();
                    case CommandEnum.Right:
                        return Right();
                    case CommandEnum.Move:
                        return Move();
                }
            }

            return Result.Failed($"{ResponseMessageConstants.CommandUnknown} {commandParts[0]}.");
        }

        public Result Left()
        {
            if (!_isReady)
            {
                return Result.Failed(ResponseMessageConstants.PositionNotInitialised);
            }

            return _robotService.TurnLeft();
        }

        public Result Right()
        {
            if (!_isReady)
            {
                return Result.Failed(ResponseMessageConstants.PositionNotInitialised);
            }

            return _robotService.TurnRight();
        }

        public Result Move()
        {
            if (!_isReady)
            {
                return Result.Failed(ResponseMessageConstants.PositionNotInitialised);
            }

            return _robotService.Move();
        }

        public Result Place(int xPosition, int yPosition, DirectionEnum orientation)
        {
            var result = _robotService.SetState(xPosition, yPosition, orientation);

            if (result.Success) _isReady = true;

            return result;
        }

        public Result Place(int xPosition, int yPosition)
        {
            if (!_isReady)
            {
                return Result.Failed(ResponseMessageConstants.PositionNotInitialised);
            }

            var result = _robotService.SetState(xPosition, yPosition);

            return result;
        }

        public Result<RobotState> Report()
        {
            if (!_isReady)
            {
                return Result<RobotState>.Failed(null, ResponseMessageConstants.PositionNotInitialised);
            }

            var result = _robotService.ReportState();

            return Result<RobotState>.Succeeded(result, $"{result.PositionX},{result.PositionY},{result.Orientation.ToString().ToUpper()}");
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
                return Place(x, y, orientation);
            }
            else if (splitArguments.Length == 2 &&
              int.TryParse(splitArguments[0], out x) &&
              int.TryParse(splitArguments[1], out y))
            {
                return Place(x, y);
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