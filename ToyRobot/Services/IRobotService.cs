using ToyRobot.Enums;
using ToyRobot.Models;

namespace ToyRobot.Services
{
    public interface IRobotService
    {
        Result<RobotState> SetState(int x, int y, DirectionEnum orientation);
        Result<RobotState> SetState(int x, int y);
        Result<RobotState> ReportState();
        Result<RobotState> Move();
        Result<RobotState> TurnLeft();
        Result<RobotState> TurnRight();
    }
}