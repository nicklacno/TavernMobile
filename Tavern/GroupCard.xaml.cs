namespace Tavern;

public partial class GroupCard : ContentView
{
	private Group groupData = null;

	public GroupCard(Group group)
	{
		InitializeComponent();

		groupData = group;
		if (groupData != null)
		{
			lbName.Text = group.Name;
			lbBio.Text = group.Bio;
			lbMembers.Text = "Members: " + group.Members.Count;

			if (group.Tags.Count > 0)
			{
				foreach (Tag tag in group.Tags)
				{
					Label lb = new Label();
					lb.Text = tag.Name;
					hStackTag.Children.Add(lb);
				}
			}
			else
			{
				Label lb = new Label();
				lb.Text = "None";
				hStackTag.Children.Add(lb);
			}
			
		}
	}

	public void OnViewGroup(object sender, EventArgs e)
	{
		Navigation.PushAsync(new GroupPage(groupData));
	}
}