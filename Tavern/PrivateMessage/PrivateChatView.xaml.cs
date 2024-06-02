using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Tavern.PrivateMessage;

public partial class PrivateChatView : ContentView
{
    public int otherId;
    public int totalMessages;
    public DateTime latestRetrieval;
    public bool isRetrievingMessages;
    private OtherUserPage parentPage;

    public ObservableCollection<MessageByDay> Messages { get; set; } = new ObservableCollection<MessageByDay>();

    public PrivateChatView()
    {
        InitializeComponent();
    }

    public PrivateChatView(int otherId, OtherUserPage parent)
    {
        InitializeComponent();
        this.otherId = otherId;
        isRetrievingMessages = true;
        AddMessages(otherId);
        parentPage = parent;

       // Task.Run(RetrieveNewMessages);
    }

    private async Task AddMessages(int groupId)
    {
        Messages = await ProfileSingleton.GetInstance().GetPrivateChat(otherId, null);
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
            int i = await ProfileSingleton.GetInstance().SendPrivateMessage(otherId, txtMessage.Text); ;
            if (i < 0)
            {
                await parentPage.ShowErrorMessage("Failed to send Message");
            }
            txtMessage.Text = "";
        }
    }

    public async Task RetrieveNewMessages()
    {
        var newMessages = await ProfileSingleton.GetInstance().GetPrivateChat(otherId, latestRetrieval);
        if (newMessages != null && newMessages.Count > 0)
        {
            if (Messages.Count > 0 && newMessages.First().DateSent == Messages.Last().DateSent)
            {
                foreach (var message in newMessages.First())
                {
                    Messages.Last().Add(message);
                    totalMessages++;
                }
                Messages.Last().LastMessageTime = newMessages.First().LastMessageTime;
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