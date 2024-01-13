namespace ChatRealTimeClient
{
    public class Message
    {
        public string name { get; set; }
        public string message { get; set; }

        public Message(string name, string message)
        {
            this.name = name;
            this.message = message;
        }

    }
}
