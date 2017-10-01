namespace Skynet.WebClient
{
    public class Command
    {
        public Intent Intent { get; set; }
        public Entity Entity { get; set; }
    }

    public enum Entity
    {
        Green,
        Yellow,
        All
    }

    public enum Intent
    {
        LightOn,
        LightOff,
        None
    }
}