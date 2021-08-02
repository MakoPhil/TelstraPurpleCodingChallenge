using ToyRobot.Enums;

namespace ToyRobot.Models
{
    public class RobotState
    {
        public int PositionX { get; private set; }
        public int PositionY { get; private set; }
        public DirectionEnum Orientation { get; private set; }

        public RobotState(int positionX, int positionY, DirectionEnum orientation)
        {
            PositionX = positionX;
            PositionY = positionY;
            Orientation = orientation;
        }

        public bool Ready => Orientation != DirectionEnum.Unknown;
    }
}