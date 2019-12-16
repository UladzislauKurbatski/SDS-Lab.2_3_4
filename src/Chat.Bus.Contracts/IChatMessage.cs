namespace Chat.BusClient.Contracts
{
    public interface IChatMessage
    {
        string Message { set; get; }
        string User { set; get; }
    }

    public class ChatMessage : IChatMessage
    {
        public string Message { get; set; }
        public string User { get; set; }
    }
}
