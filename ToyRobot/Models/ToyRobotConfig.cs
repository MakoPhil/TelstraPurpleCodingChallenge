namespace ToyRobot.Models
{
    public class ToyRobotConfig
    {
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }

        public ToyRobotConfig() { }

        public ToyRobotConfig(int gridWidth, int gridHeight)
        {
            GridWidth = gridWidth;
            GridHeight = gridHeight;
        }
    }
}