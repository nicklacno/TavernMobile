
namespace Tavern;

public partial class GroupChatView : ContentView
{
    public int groupId;
    public DateTime? latestDate = null;

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
        List<Dictionary<string, string>> messages = await ProfileSingleton.GetInstance().GetMessages(groupId);
        if (messages != null)
        {
            foreach (var message in messages)
            {
                DateTime time = Convert.ToDateTime(message["timestamp"]).ToLocalTime();
                AddMessage(time, message["sender"], message["message"]);
            }
        }
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
                    DateTime time = Convert.ToDateTime(message["timestamp"]).ToLocalTime();
                    AddMessage(time, message["sender"], message["message"]);
                }
            }
            txtMessage.Text = "";
        }
    }

    private void AddMessage(DateTime time, string sender, string message)
    {
        if (latestDate == null || latestDate != time.Date)
        {
            latestDate = time.Date;
            AddDate(latestDate);
        }

        HorizontalStackLayout stack = new HorizontalStackLayout();
        var lb = new Label();
        lb.Text = time.ToShortTimeString();
        stack.Add(lb);
        lb = new Label();
        lb.Text = " " + sender + ":";
        stack.Add(lb);
        lb = new Label();
        lb.LineBreakMode = LineBreakMode.WordWrap;
        lb.Text = message;
        stack.Add(lb);

        messageBox.Add(stack);
    }

    private void AddDate(DateTime? date)
    {
        var label = new Label();
        label.HorizontalTextAlignment = TextAlignment.Center;
        label.HorizontalOptions = LayoutOptions.Center;
        label.Text = date?.ToLongDateString();
        messageBox.Add(label);
    }
}