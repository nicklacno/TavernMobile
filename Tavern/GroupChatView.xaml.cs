
using System.Collections.ObjectModel;

namespace Tavern;

public partial class GroupChatView : ContentView
{
    public int groupId;

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
        messageBox.ItemsSource = Messages;
        messageBox.ScrollTo(Messages.Count);
    }

    private async void SendMessage(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtMessage.Text))
        {
            var messages = await ProfileSingleton.GetInstance().SendMessage(groupId, txtMessage.Text);
            if (messages != null)
            {
                foreach (var message in messages)
                {
                    Messages.Add(message);
                }
                messageBox.ScrollTo(Messages.Count);
            }
            txtMessage.Text = "";
        }
    }
}