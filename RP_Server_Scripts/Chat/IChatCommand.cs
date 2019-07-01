namespace RP_Server_Scripts.Chat
{
    public interface IChatCommand
    {
        string[] Identifiers { get; }

        void HandleCommand(Client.Client sender, string[] parameter);

        string DescriptionText { get; }

        string UsageText { get; }
    }
}
