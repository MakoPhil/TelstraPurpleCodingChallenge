using ToyRobot.Constants;
using ToyRobot.Enums;
using ToyRobot.Models;

namespace ToyRobot.Services
{
    public class RobotService : IRobotService
    {
        private ToyRobotConfig _config;
        private RobotState _state;

        public RobotService(ToyRobotConfig config)
        {
            _config = config;
        }

        public Result<RobotState> SetState(int x, int y, DirectionEnum orientation)
        {
            if (x < 0 || y < 0 || x >= _config.GridWidth || y >= _config.GridHeight)
            {
                return Result<RobotState>.Failed(_state, ResponseMessageConstants.PositionInvalid);
            }

            _state = new RobotState(x, y, orientation);

            return Result<RobotState>.Succeeded(_state, ResponseMessageConstants.PositionSet);
        }

        public Result<RobotState> SetState(int x, int y)
        {
            if (_state == null || !_state.Ready)
            {
                return Result<RobotState>.Failed(_state, ResponseMessageConstants.PositionNotInitialised);
            }

            return SetState(x, y, _state.Orientation);
        }

        public Result<RobotState> ReportState()
        {
            if (_state == null || !_state.Ready) return Result<RobotState>.Failed(null, ResponseMessageConstants.PositionNotInitialised);

            return Result<RobotState>.Succeeded(_state, $"{_state.PositionX},{_state.PositionY},{_state.Orientation.ToString().ToUpper()}");
        }

        public Result<RobotState> Move()
        {
            if (_state == null || !_state.Ready)
            {
                return Result<RobotState>.Failed(_state, ResponseMessageConstants.PositionNotInitialised);
            }

            if (!canMove())
            {
                return Result<RobotState>.Failed(_state, ResponseMessageConstants.RobotUnableToMove);
            }

            int newX = _state.PositionX;
            int newY = _state.PositionY;

            switch (_state.Orientation)
            {
                case DirectionEnum.North:
                    newY++;
                    break;
                case DirectionEnum.East:
                    newX++;
                    break;
                case DirectionEnum.South:
                    newY--;
                    break;
                case DirectionEnum.West:
                    newX--;
                    break;
            }

            _state = new RobotState(newX, newY, _state.Orientation);

            return Result<RobotState>.Succeeded(_state, ResponseMessageConstants.RobotMoved);
        }

        public Result<RobotState> TurnLeft()
        {
            if (_state == null || !_state.Ready)
            {
                return Result<RobotState>.Failed(_state, ResponseMessageConstants.PositionNotInitialised);
            }

            DirectionEnum newOrientation = _state.Orientation;

            switch (_state.Orientation)
            {
                case DirectionEnum.North:
                    newOrientation = DirectionEnum.West;
                    break;
                case DirectionEnum.West:
                    newOrientation = DirectionEnum.South;
                    break;
                case DirectionEnum.South:
                    newOrientation = DirectionEnum.East;
                    break;
                case DirectionEnum.East:
                    newOrientation = DirectionEnum.North;
                    break;
            }

            _state = new RobotState(_state.PositionX, _state.PositionY, newOrientation);

            return Result<RobotState>.Succeeded(_state, ResponseMessageConstants.RobotTurnedLeft);
        }

        public Result<RobotState> TurnRight()
        {
            if (_state == null || !_state.Ready)
            {
                return Result<RobotState>.Failed(_state, ResponseMessageConstants.PositionNotInitialised);
            }

            DirectionEnum newOrientation = _state.Orientation;

            switch (_state.Orientation)
            {
                case DirectionEnum.North:
                    newOrientation = DirectionEnum.East;
                    break;
                case DirectionEnum.East:
                    newOrientation = DirectionEnum.South;
                    break;
                case DirectionEnum.South:
                    newOrientation = DirectionEnum.West;
                    break;
                case DirectionEnum.West:
                    newOrientation = DirectionEnum.North;
                    break;
            }

            _state = new RobotState(_state.PositionX, _state.PositionY, newOrientation);

            return Result<RobotState>.Succeeded(_state, ResponseMessageConstants.RobotTurnedRight);
        }

        private bool canMove()
        {
            bool canMove = false;

            if (_state != null)
            {
                switch (_state.Orientation)
                {
                    case DirectionEnum.North:
                        canMove = _state.PositionY < _config.GridHeight - 1;
                        break;
                    case DirectionEnum.East:
                        canMove = _state.PositionX < _config.GridWidth - 1;
                        break;
                    case DirectionEnum.South:
                        canMove = _state.PositionY > 0;
                        break;
                    case DirectionEnum.West:
                        canMove = _state.PositionX > 0;
                        break;
                }
            }

            return canMove;
        }
    }
}