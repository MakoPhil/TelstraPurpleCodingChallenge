using Shouldly;
using ToyRobot.Models;
using ToyRobot.Services;
using Xunit;

namespace ToyRobotTests.IntegrationTests
{
    public class ToyRobotIntegrationTests
    {
        private ICommandParserService _commandParserService;
        private IRobotService _robotService;

        public ToyRobotIntegrationTests()
        {
            ToyRobotConfig config = new ToyRobotConfig(6, 6);

            _robotService = new RobotService(config);
            _commandParserService = new CommandParserService(_robotService);
        }

        [Fact]
        public void ExampleA()
        {
            _commandParserService.Command("PLACE 0,0,NORTH");
            _commandParserService.Command("MOVE");
            var state = _commandParserService.Command("REPORT");

            state.Success.ShouldBeTrue();
            state.Message.ShouldBe("0,1,NORTH");
        }

        [Fact]
        public void ExampleB()
        {
            _commandParserService.Command("PLACE 0,0,NORTH");
            _commandParserService.Command("LEFT");
            var state = _commandParserService.Command("REPORT");

            state.Success.ShouldBeTrue();
            state.Message.ShouldBe("0,0,WEST");
        }

        [Fact]
        public void ExampleC()
        {
            _commandParserService.Command("PLACE 1,2,EAST");
            _commandParserService.Command("MOVE");
            _commandParserService.Command("MOVE");
            _commandParserService.Command("LEFT");
            _commandParserService.Command("MOVE");
            var state = _commandParserService.Command("REPORT");

            state.Success.ShouldBeTrue();
            state.Message.ShouldBe("3,3,NORTH");
        }

        [Fact]
        public void ExampleD()
        {
            _commandParserService.Command("PLACE 1,2,EAST");
            _commandParserService.Command("MOVE");
            _commandParserService.Command("LEFT");
            _commandParserService.Command("MOVE");
            _commandParserService.Command("PLACE 3,1");
            _commandParserService.Command("MOVE");
            var state = _commandParserService.Command("REPORT");

            state.Success.ShouldBeTrue();
            state.Message.ShouldBe("3,2,NORTH");
        }
    }
}