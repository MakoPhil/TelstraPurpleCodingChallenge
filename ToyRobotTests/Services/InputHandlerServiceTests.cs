using Moq;
using Shouldly;
using ToyRobot.Constants;
using ToyRobot.Enums;
using ToyRobot.Models;
using ToyRobot.Services;
using Xunit;

namespace ToyRobotTests.Services
{
    public class InputHandlerServiceTests
    {
        private Mock<IRobotService> _robotServiceMock;

        public InputHandlerServiceTests()
        {
            _robotServiceMock = new Mock<IRobotService>();
        }

        [Fact]
        public void ValidPlacement()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.SetState(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DirectionEnum>()))
                .Returns(Result<RobotState>.Succeeded(new RobotState(4, 5, DirectionEnum.East), ResponseMessageConstants.PositionSet));

            var result = service.Place(4, 5, DirectionEnum.East);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.PositionSet);

            _robotServiceMock.Verify(r =>
                r.SetState(
                    It.Is<int>(i => i == 4),
                    It.Is<int>(i => i == 5),
                    It.Is<DirectionEnum>(d => d == DirectionEnum.East)
                ), Times.Once());
        }

        [Fact]
        public void InvalidPlacement()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.SetState(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DirectionEnum>()))
                .Returns(Result<RobotState>.Failed(new RobotState(4, 5, DirectionEnum.East), ResponseMessageConstants.PositionInvalid));

            var result = service.Place(4, 5, DirectionEnum.East);

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.PositionInvalid);

            _robotServiceMock.Verify(r =>
                r.SetState(
                    It.Is<int>(i => i == 4),
                    It.Is<int>(i => i == 5),
                    It.Is<DirectionEnum>(d => d == DirectionEnum.East)
                ), Times.Once());
        }

        [Fact]
        public void PartialPlacementWithoutInitialPlacement()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            var result = service.Place(4, 5);

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.PositionNotInitialised);

            _robotServiceMock.Verify(r =>
                r.SetState(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<DirectionEnum>()
                ), Times.Never());

            _robotServiceMock.Verify(r =>
                r.SetState(
                    It.IsAny<int>(),
                    It.IsAny<int>()
                ), Times.Never());
        }

        [Fact]
        public void TurnLeft()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.SetState(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DirectionEnum>()))
                .Returns(Result<RobotState>.Succeeded(new RobotState(4, 5, DirectionEnum.East), ResponseMessageConstants.PositionSet));

            _robotServiceMock.Setup(r => r.TurnLeft())
                .Returns(Result<RobotState>.Succeeded(null, ResponseMessageConstants.RobotTurnedLeft));

            service.Place(0, 0, DirectionEnum.North);

            var result = service.Left();

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.RobotTurnedLeft);

            _robotServiceMock.Verify(r => r.TurnLeft(), Times.Once());
        }

        [Fact]
        public void CannotTurnLeftWithoutInitialState()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.TurnLeft())
                .Returns(Result<RobotState>.Succeeded(null, ResponseMessageConstants.RobotTurnedLeft));

            var result = service.Left();

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.PositionNotInitialised);

            _robotServiceMock.Verify(r => r.TurnLeft(), Times.Never());
        }

        [Fact]
        public void TurnRight()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.SetState(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DirectionEnum>()))
                .Returns(Result<RobotState>.Succeeded(new RobotState(4, 5, DirectionEnum.East), ResponseMessageConstants.PositionSet));

            _robotServiceMock.Setup(r => r.TurnRight())
                .Returns(Result<RobotState>.Succeeded(null, ResponseMessageConstants.RobotTurnedRight));

            service.Place(0, 0, DirectionEnum.North);

            var result = service.Right();

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.RobotTurnedRight);

            _robotServiceMock.Verify(r => r.TurnRight(), Times.Once());
        }

        [Fact]
        public void CannotTurnRightWithoutInitialState()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.TurnRight())
                .Returns(Result<RobotState>.Succeeded(null, ResponseMessageConstants.RobotTurnedRight));

            var result = service.Right();

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.PositionNotInitialised);

            _robotServiceMock.Verify(r => r.TurnRight(), Times.Never());
        }

        [Fact]
        public void MoveForwards()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.SetState(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DirectionEnum>()))
                .Returns(Result<RobotState>.Succeeded(new RobotState(4, 5, DirectionEnum.East), ResponseMessageConstants.PositionSet));

            _robotServiceMock.Setup(r => r.Move())
                .Returns(Result<RobotState>.Succeeded(null, ResponseMessageConstants.RobotMoved));

            service.Place(0, 0, DirectionEnum.North);

            var result = service.Move();

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.RobotMoved);

            _robotServiceMock.Verify(r => r.Move(), Times.Once());
        }

        [Fact]
        public void CannotMoveForwardsWithoutInitialState()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.Move())
                .Returns(Result<RobotState>.Succeeded(null, ResponseMessageConstants.RobotMoved));

            var result = service.Move();

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.PositionNotInitialised);

            _robotServiceMock.Verify(r => r.Move(), Times.Never());
        }

        [Fact]
        public void ReportState()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.SetState(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DirectionEnum>()))
                .Returns(Result<RobotState>.Succeeded(new RobotState(4, 5, DirectionEnum.East), ResponseMessageConstants.PositionSet));

            _robotServiceMock.Setup(r => r.ReportState())
                .Returns(new RobotState(14, 91, DirectionEnum.South));

            service.Place(14, 91, DirectionEnum.South);

            var result = service.Report();

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe("14,91,SOUTH");

            result.Value.PositionX.ShouldBe(14);
            result.Value.PositionY.ShouldBe(91);
            result.Value.Orientation.ShouldBe(DirectionEnum.South);

            _robotServiceMock.Verify(r => r.ReportState(), Times.Once());
        }

        [Fact]
        public void CannotReportStateWithoutInitialState()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.ReportState())
                .Returns(null as RobotState);

            var result = service.Report();

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.PositionNotInitialised);

            _robotServiceMock.Verify(r => r.Move(), Times.Never());
        }

        [Fact]
        public void DismissUnknownCommand()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            var response = service.Command("  Nonsense  Command  ");

            response.Success.ShouldBeFalse();
            response.Message.ShouldBe($"{ResponseMessageConstants.CommandUnknown} Nonsense.");
        }

        [Fact]
        public void DoNotPlaceWithNoArguments()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            var response = service.Command("PLACE");

            response.Success.ShouldBeFalse();
            response.Message.ShouldBe($"{ResponseMessageConstants.CommandNotEnoughArguments}");
        }

        [Fact]
        public void DoNotPlaceWithTooManyArguments()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

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
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

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
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

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
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.SetState(
                It.Is<int>(x => x == 1),
                It.Is<int>(y => y == 2),
                It.Is<DirectionEnum>(orientation => orientation == DirectionEnum.South)
                ))
                .Returns(Result<RobotState>.Succeeded(new RobotState(1, 2, DirectionEnum.South), ResponseMessageConstants.PositionSet));

            _robotServiceMock.Setup(r => r.SetState(
                It.Is<int>(x => x == xInput),
                It.Is<int>(y => y == yInput)
                ))
                .Returns(Result<RobotState>.Succeeded(new RobotState(xInput, yInput, DirectionEnum.South), ResponseMessageConstants.PositionSet));

            service.Place(1, 2, DirectionEnum.South);

            var result = service.Command(command);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.PositionSet);

            _robotServiceMock.Verify(r => r.SetState(
                It.Is<int>(x => x == xInput),
                It.Is<int>(y => y == yInput)
            ), Times.Once());
        }

        [Theory]
        [InlineData("  left  ")]
        [InlineData("LeFt")]
        [InlineData("LEFT")]
        public void Left(string command)
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.SetState(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DirectionEnum>()))
                .Returns(Result<RobotState>.Succeeded(new RobotState(1, 2, DirectionEnum.North), ResponseMessageConstants.PositionSet));

            _robotServiceMock.Setup(r => r.TurnLeft())
                .Returns(Result<RobotState>.Succeeded(new RobotState(1, 2, DirectionEnum.West), ResponseMessageConstants.RobotTurnedLeft));

            service.Place(1, 2, DirectionEnum.North);

            var result = service.Command(command);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.RobotTurnedLeft);

            _robotServiceMock.Verify(r => r.TurnLeft(), Times.Once());
        }

        [Theory]
        [InlineData("  right  ")]
        [InlineData("RiGhT")]
        [InlineData("RIGHT")]
        public void Right(string command)
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.SetState(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DirectionEnum>()))
                .Returns(Result<RobotState>.Succeeded(new RobotState(1, 2, DirectionEnum.North), ResponseMessageConstants.PositionSet));

            _robotServiceMock.Setup(r => r.TurnRight())
                .Returns(Result<RobotState>.Succeeded(new RobotState(1, 2, DirectionEnum.East), ResponseMessageConstants.RobotTurnedRight));

            service.Place(1, 2, DirectionEnum.North);

            var result = service.Command(command);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.RobotTurnedRight);

            _robotServiceMock.Verify(r => r.TurnRight(), Times.Once());
        }

        [Theory]
        [InlineData("  move  ")]
        [InlineData("MoVe")]
        [InlineData("MOVE")]
        public void Move(string command)
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.SetState(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DirectionEnum>()))
                .Returns(Result<RobotState>.Succeeded(new RobotState(1, 2, DirectionEnum.North), ResponseMessageConstants.PositionSet));

            _robotServiceMock.Setup(r => r.Move())
                .Returns(Result<RobotState>.Succeeded(new RobotState(1, 3, DirectionEnum.North), ResponseMessageConstants.RobotMoved));

            service.Place(1, 2, DirectionEnum.North);

            var result = service.Command(command);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.RobotMoved);

            _robotServiceMock.Verify(r => r.Move(), Times.Once());
        }

        [Theory]
        [InlineData("  report  ")]
        [InlineData("RePoRt")]
        [InlineData("REPORT")]
        public void Report(string command)
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            _robotServiceMock.Setup(r => r.SetState(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DirectionEnum>()))
                .Returns(Result<RobotState>.Succeeded(new RobotState(1, 2, DirectionEnum.North), ResponseMessageConstants.PositionSet));

            _robotServiceMock.Setup(r => r.ReportState())
                .Returns(new RobotState(1, 2, DirectionEnum.North));

            service.Place(1, 2, DirectionEnum.North);

            var result = service.Command(command);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe("1,2,NORTH");

            _robotServiceMock.Verify(r => r.ReportState(), Times.Once());
        }

        public void IgnoreNumericCommands()
        {
            InputHandlerService service = new InputHandlerService(_robotServiceMock.Object);

            var result = service.Command("2");

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe($"{ResponseMessageConstants.CommandUnknown} {2}");

            _robotServiceMock.Verify(r => r.ReportState(), Times.Never());
        }
    }
}