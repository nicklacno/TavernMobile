
namespace Tavern;

public partial class GroupPage : ContentPage
{
	public Group GroupData { get; set; }

	public GroupPage(int id = -1)
	{
		InitializeComponent();
		UpdatePage(id);
	}

    private async Task UpdatePage(int id)
    {
		GroupData = await ProfileSingleton.GetInstance().GetGroup(id);
        if (GroupData != null)
		{
			lbGroupName.Text = GroupData.Name;
			lbGroupBio.Text = GroupData.Bio;

			Label lb;

			foreach (string tag in GroupData.Tags)
			{
				lb = new Label();
				lb.Text = tag;
				layoutTags.Add(lb);
			}

			lb = new Label();
			lb.FontSize = 20;
			lb.Text = GroupData.Members[0];
			layoutMembers.Add(lb);

			for (int i = 1; i < GroupData.Members.Count; i++)
			{
				lb = new Label();
				lb.Text = GroupData.Members[i];
				layoutMembers.Add(lb);
			}
		}
    }
}