
using System.Collections.ObjectModel;

namespace Tavern;

public partial class GroupChatView : ContentView
{
    public int groupId;
    public int totalMessages;
    public DateTime latestRetrieval;
    public bool isRetrievingMessages;

    public ObservableCollection<MessageByDay> Messages { get; set; } = new ObservableCollection<MessageByDay>();

    public GroupChatView()
    {
        InitializeComponent();
    }

    public GroupChatView(int groupId)
    {
        InitializeComponent();
        this.groupId = groupId;
        latestRetrieval= DateTime.UtcNow;
        isRetrievingMessages = true;
        AddMessages(groupId);

        Task.Run(RetrieveNewMessages);
    }

    private async Task AddMessages(int groupId)
    {
        Messages = await ProfileSingleton.GetInstance().GetMessages(groupId);
        totalMessages = Messages.Count;
        foreach(var message in Messages)
        {
            totalMessages += message.Count;
        }
        messageBox.ItemsSource = Messages;
        messageBox.ScrollTo(totalMessages);
    }

    private async void SendMessage(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtMessage.Text))
        {
            await ProfileSingleton.GetInstance().SendMessage(groupId, txtMessage.Text);
            txtMessage.Text = "";
        }
    }

    public async Task RetrieveNewMessages()
    {
        while (isRetrievingMessages)
        {
            var newMessages = await ProfileSingleton.GetInstance().GetMessages(groupId, latestRetrieval);
            latestRetrieval = DateTime.UtcNow;
            if (newMessages != null && newMessages.Count > 0)
            {
                if (newMessages.First().DateSent == Messages.Last().DateSent)
                {
                    foreach (var message in newMessages.First())
                    {
                        Messages.Last().Add(message);
                        totalMessages++;
                    }
                    newMessages.RemoveAt(0);
                }
                foreach (var message in newMessages)
                {
                    totalMessages += message.Count;
                    Messages.Add(message);
                }
                messageBox.ScrollTo(totalMessages);
            }
            Thread.Sleep(3000);
        }
    }
}