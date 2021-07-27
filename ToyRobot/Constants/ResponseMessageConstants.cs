namespace ToyRobot.Constants
{
    public static class ResponseMessageConstants
    {
        public const string PositionNotInitialised = "Initial state has not been set.";
        public const string PositionSet = "Position set.";
        public const string PositionInvalid = "Invalid coordinates.";
        public const string RobotMoved = "Robot moved.";
        public const string RobotUnableToMove = "Unable to move; Robot at boundary.";
        public const string RobotTurnedLeft = "Robot turned left.";
        public const string RobotTurnedRight = "Robot turned right.";
        public const string CommandUnknown = "Unknown Command:";
        public const string CommandNotEnoughArguments = "Not enough arguments provided.";
        public const string CommandTooManyArguments = "Too many arguments provided.";
        public const string CommandInvalidArguments = "Invalid arguments were provided.";
    }
}