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
			lbGroupName.Text = groupData.Name;
			lbGroupBio.Text = groupData.Bio;
			lbMemberCount.Text = "Members: " + groupData.Members.Count;
		}

		
	}
}