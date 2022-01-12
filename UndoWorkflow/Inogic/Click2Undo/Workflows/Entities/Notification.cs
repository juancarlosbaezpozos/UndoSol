namespace Inogic.Click2Undo.Workflows.Entities
{
    public class Notification
    {
        public string From { get; set; }

        public string To { get; set; }

        public bool NotifyToInogic { get; set; }

        public string NotifyInterval { get; set; }
    }
}