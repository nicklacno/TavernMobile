namespace Tavern;

public partial class GroupCard : ContentView
{
	public GroupCard(Group group)
	{
		InitializeComponent();

		if (group != null )
		{
			lbName.Text = group.Name;
			lbBio.Text = group.Bio;
			lbMembers.Text = "Members: " + group.Members.Count;

			if (group.Tags.Count > 0)
			{
				foreach (string tag in group.Tags)
				{
					Label lb = new Label();
					lb.Text = tag;
					hStackTag.Children.Add(lb);
				}
			}
			
		}
	}
}