namespace ToyRobot.Models
{
    public class Result<T> : Result
    {
        public T Value { get; private set; }

        protected Result(T value, bool success, string message = null) : base(success, message)
        {
            Value = value;
        }

        public static Result<T> Succeeded(T value, string message = null)
        {
            return new Result<T>(value, true, message);
        }

        public static Result<T> Failed(T value, string message = null)
        {
            return new Result<T>(value, false, message);
        }
    }

    public class Result
    {
        public string Message { get; private set; }
        public bool Success { get; private set; }

        protected Result(bool success, string message = null)
        {
            Success = success;
            Message = message;
        }

        public static Result Succeeded(string message = null)
        {
            return new Result(true, message);
        }

        public static Result Failed(string message = null)
        {
            return new Result(false, message);
        }
    }
}