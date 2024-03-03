using Microsoft.Maui.Controls;
using System.Diagnostics;
namespace Tavern.SwipingFunctionality;

public partial class SwipingView : SwipeView
{
	private Group groupData;

	public SwipingView(int groupId = -1)
	{
		InitializeComponent();
		PopulateData(groupId);
	}

	/**
	 * OnJoinGroup - sends request to join group in database
	 */
	async void OnJoinGroup(object sender, EventArgs e)
	{
		Debug.WriteLine("Join");
	}
	
	/**
	 * OnSkipGroup - sends request to app to move to the next group in the list
	 */
	async void OnSkipGroup(object sender, EventArgs e)
	{
		Debug.WriteLine("Skip");
	}

	async Task PopulateData(int groupId)
	{
		groupData = await ProfileSingleton.GetInstance().GetGroup(groupId);
		if (groupData != null)
		{
			lbGroupName.Text = groupData.Name;
			lbGroupBio.Text = groupData.Bio;
			lbMemberCount.Text = "Members: " + groupData.Members.Count;
		}

		
	}
}