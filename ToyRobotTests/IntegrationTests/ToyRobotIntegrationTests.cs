using Shouldly;
using ToyRobot.Models;
using ToyRobot.Services;
using Xunit;

namespace ToyRobotTests.IntegrationTests
{
    public class ToyRobotIntegrationTests
    {
        private IInputHandlerService _inputService;
        private IRobotService _robotService;

        public ToyRobotIntegrationTests()
        {
            ToyRobotConfig config = new ToyRobotConfig(6, 6);

            _robotService = new RobotService(config);
            _inputService = new InputHandlerService(_robotService);
        }

        [Fact]
        public void ExampleA()
        {
            _inputService.Command("PLACE 0,0,NORTH");
            _inputService.Command("MOVE");
            var state = _inputService.Command("REPORT");

            state.Success.ShouldBeTrue();
            state.Message.ShouldBe("0,1,NORTH");
        }

        [Fact]
        public void ExampleB()
        {
            _inputService.Command("PLACE 0,0,NORTH");
            _inputService.Command("LEFT");
            var state = _inputService.Command("REPORT");

            state.Success.ShouldBeTrue();
            state.Message.ShouldBe("0,0,WEST");
        }

        [Fact]
        public void ExampleC()
        {
            _inputService.Command("PLACE 1,2,EAST");
            _inputService.Command("MOVE");
            _inputService.Command("MOVE");
            _inputService.Command("LEFT");
            _inputService.Command("MOVE");
            var state = _inputService.Command("REPORT");

            state.Success.ShouldBeTrue();
            state.Message.ShouldBe("3,3,NORTH");
        }

        [Fact]
        public void ExampleD()
        {
            _inputService.Command("PLACE 1,2,EAST");
            _inputService.Command("MOVE");
            _inputService.Command("LEFT");
            _inputService.Command("MOVE");
            _inputService.Command("PLACE 3,1");
            _inputService.Command("MOVE");
            var state = _inputService.Command("REPORT");

            state.Success.ShouldBeTrue();
            state.Message.ShouldBe("3,2,NORTH");
        }
    }
}