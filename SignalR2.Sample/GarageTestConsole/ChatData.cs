namespace GarageTestConsole
{
    public class ChatData
    {
        public string Name { get; set; }
        public string Message { get; set; }

        public ChatData() { }

        public ChatData(string name, string message)
        {
            Name = name;
            Message = message;
        }
    }
}
