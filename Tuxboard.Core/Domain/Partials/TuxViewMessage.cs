namespace Tuxboard.Core.Domain.Entities
{
    public class TuxViewMessage
    {
        public string Id { get; set; }
        public bool Success { get; set; }
        public string Text { get; set; }
        public TuxMessageType Type { get; set; }

        public TuxViewMessage(string text, TuxMessageType type, bool success = true, string id = "")
        {
            Text = text;
            Type = type;
            Success = success;
            Id = id;
        }
    }
}
