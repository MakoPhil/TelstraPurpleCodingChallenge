using ToyRobot.Enums;
using ToyRobot.Models;

namespace ToyRobot.Services
{
    public interface ICommandParserService
    {
        Result Command(string command);
        Result Place(int xPosition, int yPosition, DirectionEnum orientation);
        Result Place(int xPosition, int yPosition);
        Result Move();
        Result Left();
        Result Right();
        Result<RobotState> Report();
    }
}