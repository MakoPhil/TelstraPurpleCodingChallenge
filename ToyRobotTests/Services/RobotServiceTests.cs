using ToyRobot.Enums;
using ToyRobot.Models;
using ToyRobot.Services;
using Shouldly;
using Xunit;
using ToyRobot.Constants;

namespace ToyRobotTests.Services
{
    public class RobotServiceTests
    {
        [Theory]
        [InlineData(0, 0, DirectionEnum.North, 6, 6)]
        [InlineData(5, 5, DirectionEnum.South, 6, 6)]
        [InlineData(2, 4, DirectionEnum.East, 6, 6)]
        [InlineData(276, 180, DirectionEnum.West, 300, 300)]
        public void SetValidState(int x, int y, DirectionEnum orientation, int width, int height)
        {
            ToyRobotConfig config = new ToyRobotConfig(width, height);
            RobotService service = new RobotService(config);

            var result = service.SetState(x, y, orientation);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.PositionSet);

            result.Value.PositionX.ShouldBe(x);
            result.Value.PositionY.ShouldBe(y);
            result.Value.Orientation.ShouldBe(orientation);
        }

        [Theory]
        [InlineData(6, 0, DirectionEnum.North, 6, 6)]
        [InlineData(0, 6, DirectionEnum.South, 6, 6)]
        [InlineData(-1, 3, DirectionEnum.East, 6, 6)]
        [InlineData(4, -200, DirectionEnum.West, 300, 300)]
        public void SetInvalidState(int x, int y, DirectionEnum orientation, int width, int height)
        {
            ToyRobotConfig config = new ToyRobotConfig(width, height);
            RobotService service = new RobotService(config);

            var result = service.SetState(x, y, orientation);

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.PositionInvalid);
        }

        [Fact]
        public void SetInvalidStateAfterValidState()
        {
            ToyRobotConfig config = new ToyRobotConfig(3, 3);
            RobotService service = new RobotService(config);

            var result = service.SetState(2, 1, DirectionEnum.South);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.PositionSet);

            result.Value.PositionX.ShouldBe(2);
            result.Value.PositionY.ShouldBe(1);
            result.Value.Orientation.ShouldBe(DirectionEnum.South);

            result = service.SetState(9, -4, DirectionEnum.West);

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.PositionInvalid);

            result.Value.PositionX.ShouldBe(2);
            result.Value.PositionY.ShouldBe(1);
            result.Value.Orientation.ShouldBe(DirectionEnum.South);
        }

        [Fact]
        public void CannotSetPartialStateWithoutInitialState()
        {
            ToyRobotConfig config = new ToyRobotConfig(6, 6);
            RobotService service = new RobotService(config);

            var result = service.SetState(2, 1);

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.PositionNotInitialised);
        }

        [Fact]
        public void SetPartialStateAfterInitialState()
        {
            ToyRobotConfig config = new ToyRobotConfig(6, 6);
            RobotService service = new RobotService(config);

            var result = service.SetState(2, 1, DirectionEnum.South);

            result.Success.ShouldBeTrue();

            result = service.SetState(3, 4);

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.PositionSet);
            result.Value.PositionX.ShouldBe(3);
            result.Value.PositionY.ShouldBe(4);
            result.Value.Orientation.ShouldBe(DirectionEnum.South);
        }

        [Fact]
        public void SetInvalidPartialStateAfterInitialState()
        {
            ToyRobotConfig config = new ToyRobotConfig(6, 6);
            RobotService service = new RobotService(config);

            var result = service.SetState(2, 1, DirectionEnum.South);

            result.Success.ShouldBeTrue();

            result = service.SetState(7, 4);

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.PositionInvalid);
            result.Value.PositionX.ShouldBe(2);
            result.Value.PositionY.ShouldBe(1);
            result.Value.Orientation.ShouldBe(DirectionEnum.South);
        }

        [Fact]
        public void ReportState()
        {
            ToyRobotConfig config = new ToyRobotConfig(3, 3);
            RobotService service = new RobotService(config);

            service.SetState(2, 1, DirectionEnum.South);

            var state = service.ReportState();

            state.Success.ShouldBeTrue();
            state.Message.ShouldBe("2,1,SOUTH");
        }

        [Fact]
        public void ReportEmptyState()
        {
            ToyRobotConfig config = new ToyRobotConfig(3, 3);
            RobotService service = new RobotService(config);

            var state = service.ReportState();

            state.Success.ShouldBeFalse();
            state.Message.ShouldBe(ResponseMessageConstants.PositionNotInitialised);
        }

        [Theory]
        [InlineData(4, 4, DirectionEnum.North, 6, 6, 4, 5)]
        [InlineData(4, 4, DirectionEnum.East, 6, 6, 5, 4)]
        [InlineData(1, 1, DirectionEnum.West, 6, 6, 0, 1)]
        [InlineData(1, 1, DirectionEnum.South, 6, 6, 1, 0)]
        [InlineData(23, 56, DirectionEnum.North, 100, 100, 23, 57)]
        public void ValidMoves(int x, int y, DirectionEnum orientation, int width, int height, int newX, int newY)
        {
            ToyRobotConfig config = new ToyRobotConfig(width, height);
            RobotService service = new RobotService(config);

            service.SetState(x, y, orientation);

            var result = service.Move();

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.RobotMoved);

            result.Value.PositionX.ShouldBe(newX);
            result.Value.PositionY.ShouldBe(newY);
            result.Value.Orientation.ShouldBe(orientation);
        }

        [Theory]
        [InlineData(0, 0, DirectionEnum.West, 6, 6)]
        [InlineData(0, 0, DirectionEnum.South, 6, 6)]
        [InlineData(5, 5, DirectionEnum.North, 6, 6)]
        [InlineData(5, 5, DirectionEnum.East, 6, 6)]
        [InlineData(176, 89, DirectionEnum.North, 200, 90)]
        [InlineData(229, 76, DirectionEnum.East, 230, 100)]
        public void InvalidMoves(int x, int y, DirectionEnum orientation, int width, int height)
        {
            ToyRobotConfig config = new ToyRobotConfig(width, height);
            RobotService service = new RobotService(config);

            service.SetState(x, y, orientation);

            var result = service.Move();

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.RobotUnableToMove);

            result.Value.PositionX.ShouldBe(x);
            result.Value.PositionY.ShouldBe(y);
            result.Value.Orientation.ShouldBe(orientation);
        }

        [Fact]
        public void CanNotMoveWithNoState()
        {
            ToyRobotConfig config = new ToyRobotConfig(6, 6);
            RobotService service = new RobotService(config);

            var result = service.Move();

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.PositionNotInitialised);

            result.Value.ShouldBeNull();
        }

        [Theory]
        [InlineData(DirectionEnum.North, DirectionEnum.West)]
        [InlineData(DirectionEnum.West, DirectionEnum.South)]
        [InlineData(DirectionEnum.South, DirectionEnum.East)]
        [InlineData(DirectionEnum.East, DirectionEnum.North)]
        public void TurnLeft(DirectionEnum current, DirectionEnum next)
        {
            ToyRobotConfig config = new ToyRobotConfig(6, 6);
            RobotService service = new RobotService(config);

            service.SetState(2, 3, current);

            var result = service.TurnLeft();

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.RobotTurnedLeft);

            result.Value.PositionX.ShouldBe(2);
            result.Value.PositionY.ShouldBe(3);
            result.Value.Orientation.ShouldBe(next);
        }

        [Theory]
        [InlineData(DirectionEnum.North, DirectionEnum.East)]
        [InlineData(DirectionEnum.East, DirectionEnum.South)]
        [InlineData(DirectionEnum.South, DirectionEnum.West)]
        [InlineData(DirectionEnum.West, DirectionEnum.North)]
        public void TurnRight(DirectionEnum current, DirectionEnum next)
        {
            ToyRobotConfig config = new ToyRobotConfig(6, 6);
            RobotService service = new RobotService(config);

            service.SetState(2, 3, current);

            var result = service.TurnRight();

            result.Success.ShouldBeTrue();
            result.Message.ShouldBe(ResponseMessageConstants.RobotTurnedRight);

            result.Value.PositionX.ShouldBe(2);
            result.Value.PositionY.ShouldBe(3);
            result.Value.Orientation.ShouldBe(next);
        }

        [Fact]
        public void NoTurnLeftWithNoState()
        {
            ToyRobotConfig config = new ToyRobotConfig(6, 6);
            RobotService service = new RobotService(config);

            var result = service.TurnLeft();

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.PositionNotInitialised);

            result.Value.ShouldBeNull();
        }

        [Fact]
        public void NoTurnRightWithNoState()
        {
            ToyRobotConfig config = new ToyRobotConfig(6, 6);
            RobotService service = new RobotService(config);

            var result = service.TurnRight();

            result.Success.ShouldBeFalse();
            result.Message.ShouldBe(ResponseMessageConstants.PositionNotInitialised);

            result.Value.ShouldBeNull();
        }
    }
}