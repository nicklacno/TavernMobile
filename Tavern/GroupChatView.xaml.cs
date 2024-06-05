
using System.Collections.ObjectModel;

namespace Tavern;

public partial class GroupChatView : ContentView
{
    public int groupId;
    public int totalMessages;
    public DateTime latestRetrieval;
    public bool isRetrievingMessages;
    private GroupPage parentPage;

    public ObservableCollection<MessageByDay> Messages { get; set; } = new ObservableCollection<MessageByDay>();

    public GroupChatView()
    {
        InitializeComponent();
    }

    public GroupChatView(int groupId, GroupPage parent)
    {
        InitializeComponent();
        this.groupId = groupId;
        isRetrievingMessages = true;
        AddMessages(groupId);
        parentPage = parent;

        //Task.Run(RetrieveNewMessages);
    }

    private async Task AddMessages(int groupId)
    {
        Messages = await ProfileSingleton.GetInstance().GetMessages(groupId);
        if (Messages == null) Messages = new ObservableCollection<MessageByDay>();
        totalMessages = Messages.Count;
        foreach (var message in Messages)
        {
            totalMessages += message.Count;
        }
        if (Messages.Count > 0) latestRetrieval = Messages.Last().LastMessageTime.AddSeconds(1);
        messageBox.ItemsSource = Messages;
        messageBox.ScrollTo(totalMessages);
    }

    private async void SendMessage(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtMessage.Text))
        {
            int i = await ProfileSingleton.GetInstance().SendMessage(groupId, txtMessage.Text);
            if (i < 0)
            {
                await parentPage.ShowErrorMessage("Failed to send Message");
            }
            txtMessage.Text = "";
        }
    }

    public async Task RetrieveNewMessages()
    {
        var newMessages = await ProfileSingleton.GetInstance().GetMessages(groupId, latestRetrieval);
        latestRetrieval = DateTime.UtcNow;
        if (newMessages != null && newMessages.Count > 0)
        {
            if (Messages.Count > 0 && newMessages.First().DateSent == Messages.Last().DateSent)
            {
                foreach (var message in newMessages.First())
                {
                    Messages.Last().Add(message);
                    totalMessages++;
                }
                Messages.Last().LastMessageTime = newMessages.First().LastMessageTime.AddSeconds(1);
                newMessages.RemoveAt(0);
            }
            foreach (var message in newMessages)
            {
                totalMessages += message.Count;
                Messages.Add(message);
            }
            latestRetrieval = Messages.Last().LastMessageTime.AddSeconds(1);
            messageBox.ScrollTo(totalMessages);
        }
    }
}