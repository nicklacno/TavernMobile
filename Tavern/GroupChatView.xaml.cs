
using System.Collections.ObjectModel;

namespace Tavern;

public partial class GroupChatView : ContentView
{
    public int groupId;
    public int totalMessages;

    public List<MessageByDay> Messages { get; set; } = new List<MessageByDay>();

    public GroupChatView()
    {
        InitializeComponent();
    }

    public GroupChatView(int groupId)
    {
        InitializeComponent();
        this.groupId = groupId;
        AddMessages(groupId);
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
            var messages = await ProfileSingleton.GetInstance().SendMessage(groupId, txtMessage.Text);
            if (messages != null)
            {
                if (messages.Count > 0)
                {
                    if (messages.First().DateSent.Equals(Messages.Last().DateSent))
                    {
                        foreach(var message in messages.First())
                        {
                            Messages.Last().Add(message);
                            totalMessages++;
                        }
                    }
                    messages.RemoveAt(0);
                    foreach(var message in messages)
                    {
                        Messages.Add(message);
                        totalMessages += message.Count;
                    }
                }
                messageBox.ScrollTo(totalMessages);
            }
            txtMessage.Text = "";
        }
    }
}