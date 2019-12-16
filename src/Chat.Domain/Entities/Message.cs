using System;

namespace Chat.Domain.Entities
{
    public class Message
    {
        public string UserName { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public override string ToString()
        {
            return $"<[{Date.ToString("dddd, dd MMMM yyyy HH:mm:ss")} {UserName}]> {Text}";
        }
    }
}
