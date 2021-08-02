using Moq;
using Shouldly;
using ToyRobot.Constants;
using ToyRobot.Enums;
using ToyRobot.Models;
using ToyRobot.Services;
using Xunit;

namespace ToyRobotTests.Services
{
    public class CommandParserServiceTests
    {
        private Mock<IRobotService> _robotServiceMock;

        public CommandParserServiceTests()
        {
            _robotServiceMock = new Mock<IRobotService>();
        }

        [Theory]
        [InlineData("Left")]
        [InlineData("LEFT")]
        [InlineData("left")]
        [InlineData("   left")]
        [InlineData("left     ")]
        public void TurnLeft(string command)
        {
            CommandParserService service = new CommandParserService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.TurnLeft())
                .Returns(Result<RobotState>.Succeeded(null, ResponseMessageConstants.RobotTurnedLeft));

            var result = service.Command(command);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.RobotTurnedLeft);

            _robotServiceMock.Verify(r => r.TurnLeft(), Times.Once());
        }

        [Theory]
        [InlineData("Right")]
        [InlineData("RIGHT")]
        [InlineData("right")]
        [InlineData("   right")]
        [InlineData("right     ")]
        public void TurnRight(string command)
        {
            CommandParserService service = new CommandParserService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.TurnRight())
                .Returns(Result<RobotState>.Succeeded(null, ResponseMessageConstants.RobotTurnedRight));

            var result = service.Command(command);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.RobotTurnedRight);

            _robotServiceMock.Verify(r => r.TurnRight(), Times.Once());
        }

        [Theory]
        [InlineData("Move")]
        [InlineData("MOVE")]
        [InlineData("move")]
        [InlineData("   move")]
        [InlineData("move     ")]
        public void MoveForwards(string command)
        {
            CommandParserService service = new CommandParserService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.Move())
                .Returns(Result<RobotState>.Succeeded(null, ResponseMessageConstants.RobotMoved));

            var result = service.Command(command);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.RobotMoved);

            _robotServiceMock.Verify(r => r.Move(), Times.Once());
        }

        [Theory]
        [InlineData("Report")]
        [InlineData("REPORT")]
        [InlineData("report")]
        [InlineData("   report")]
        [InlineData("report     ")]
        public void ReportState(string command)
        {
            CommandParserService service = new CommandParserService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.ReportState())
                .Returns(Result<RobotState>.Succeeded(new RobotState(14, 91, DirectionEnum.South), "14,91,SOUTH"));

            var result = service.Command(command);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe("14,91,SOUTH");

            _robotServiceMock.Verify(r => r.ReportState(), Times.Once());
        }

        [Fact]
        public void DismissUnknownCommand()
        {
            CommandParserService service = new CommandParserService(_robotServiceMock.Object);

            var response = service.Command("  Nonsense  Command  ");

            response.Success.ShouldBeFalse();
            response.Message.ShouldBe($"{ResponseMessageConstants.CommandUnknown} Nonsense.");
        }

        [Fact]
        public void DoNotPlaceWithNoArguments()
        {
            CommandParserService service = new CommandParserService(_robotServiceMock.Object);

            var response = service.Command("PLACE");

            response.Success.ShouldBeFalse();
            response.Message.ShouldBe($"{ResponseMessageConstants.CommandNotEnoughArguments}");
        }

        [Fact]
        public void DoNotPlaceWithTooManyArguments()
        {
            CommandParserService service = new CommandParserService(_robotServiceMock.Object);

            var response = service.Command("PLACE 4 5 south");

            response.Success.ShouldBeFalse();
            response.Message.ShouldBe($"{ResponseMessageConstants.CommandTooManyArguments}");
        }

        [Theory]
        [InlineData("text,text,text")]
        [InlineData("0.1,0.1,text")]
        [InlineData("0,text,text")]
        [InlineData("text,0,text")]
        [InlineData("1,0,1")]
        public void DoNotPlaceWithInvalidArguments(string arguments)
        {
            CommandParserService service = new CommandParserService(_robotServiceMock.Object);

            var response = service.Command($"PLACE {arguments}");

            response.Success.ShouldBeFalse();
            response.Message.ShouldBe($"{ResponseMessageConstants.CommandInvalidArguments}");
        }

        [Theory]
        [InlineData("  place    7,3,west  ", 7, 3, DirectionEnum.West)]
        [InlineData("PLACE 9,10,SOUTH", 9, 10, DirectionEnum.South)]
        [InlineData("PlAcE 14,0,EaSt", 14, 0, DirectionEnum.East)]
        public void DoPlace(string command, int xInput, int yInput, DirectionEnum orientationInput)
        {
            CommandParserService service = new CommandParserService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.SetState(
                It.Is<int>(x => x == xInput),
                It.Is<int>(y => y == yInput),
                It.Is<DirectionEnum>(orientation => orientation == orientationInput)
                ))
                .Returns(Result<RobotState>.Succeeded(new RobotState(xInput, yInput, orientationInput), ResponseMessageConstants.PositionSet));

            var result = service.Command(command);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.PositionSet);

            _robotServiceMock.Verify(r => r.SetState(
                It.Is<int>(x => x == xInput),
                It.Is<int>(y => y == yInput),
                It.Is<DirectionEnum>(orientation => orientation == orientationInput)
            ), Times.Once());
        }

        [Theory]
        [InlineData("  place    7,3  ", 7, 3)]
        [InlineData("PLACE 9,10", 9, 10)]
        [InlineData("PlAcE 14,0", 14, 0)]
        public void DoPartialPlace(string command, int xInput, int yInput)
        {
            CommandParserService service = new CommandParserService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.SetState(
                It.Is<int>(x => x == xInput),
                It.Is<int>(y => y == yInput)
                ))
                .Returns(Result<RobotState>.Succeeded(new RobotState(xInput, yInput, DirectionEnum.South), ResponseMessageConstants.PositionSet));

            var result = service.Command(command);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.PositionSet);

            _robotServiceMock.Verify(r => r.SetState(
                It.Is<int>(x => x == xInput),
                It.Is<int>(y => y == yInput)
            ), Times.Once());
        }

        public void IgnoreNumericCommands()
        {
            CommandParserService service = new CommandParserService(_robotServiceMock.Object);

            var result = service.Command("2");

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe($"{ResponseMessageConstants.CommandUnknown} {2}");

            _robotServiceMock.Verify(r => r.ReportState(), Times.Never());
        }
    }
}