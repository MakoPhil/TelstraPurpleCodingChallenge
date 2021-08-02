using ToyRobot.Models;

namespace ToyRobot.Services
{
    public interface ICommandParserService
    {
        Result Command(string command);
    }
}