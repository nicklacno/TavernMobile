using Microsoft.Maui.Controls;
using System.Diagnostics;
namespace Tavern.SwipingFunctionality;

public partial class SwipingView : SwipeView
{
	private Group groupData;

	public SwipingView(Group group = null)
	{
		InitializeComponent();
		PopulateData(group);
	}

	/**
	 * OnJoinGroup - sends request to join group in database
	 */
	async void OnJoinGroup(object sender, EventArgs e)
	{
		Debug.WriteLine("Join");
		SwipingSingleton.RequestGroup.Invoke();
	}
	
	/**
	 * OnSkipGroup - sends request to app to move to the next group in the list
	 */
	async void OnSkipGroup(object sender, EventArgs e)
	{
		Debug.WriteLine("Skip");
		SwipingSingleton.SkipGroup.Invoke();
	}

	void PopulateData(Group group)
	{
		groupData = group;
		if (groupData != null)
		{
			lbName.Text = groupData.Name;
			lbBio.Text = groupData.Bio;
			lbMembers.Text = "Members: " + groupData.Members.Count;

			foreach (string tag in groupData.Tags)
			{
				Label lb = new Label();
				lb.Text = tag;
				hStackTag.Children.Add(lb);
			}

			if (hStackTag.Children.Count == 0)
			{
				Label lb = new Label();
				lb.Text = "None";
				hStackTag.Children.Add(lb);
			}
		}
		
	}
}