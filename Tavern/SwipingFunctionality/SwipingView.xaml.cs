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
    public async void OnJoinGroup(object sender, EventArgs e)
    {
        await SwipingSingleton.GetInstance().SwipeRight(groupData);
        SwipingSingleton.RequestGroup.Invoke();
    }

    /**
	 * OnSkipGroup - sends request to app to move to the next group in the list
	 */
    public async void OnSkipGroup(object sender, EventArgs e)
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
            lbName.FontFamily = "Algerian";
            lbName.FontAttributes = FontAttributes.Bold;
            lbBio.Text = groupData.Bio;
            lbBio.FontFamily = "Sedan";
            lbMembers.Text = "Members: " + groupData.Members.Count;
            lbMembers.FontFamily = "Sedan";
            lbMembers.FontAttributes = FontAttributes.Bold;

            foreach (Tag tag in groupData.Tags)
            {
                Label lb = new Label();
                lb.Text = tag.Name;
                lb.FontFamily = "Sedan";
                lb.FontSize = 25;
                hStackTag.Children.Add(lb);
            }

            if (hStackTag.Children.Count == 0)
            {
                Label lb = new Label();
                lb.Text = "None";
                lb.FontSize = 20;
                lb.FontFamily = "Sedan";
                hStackTag.Children.Add(lb);
            }
        }
    }
}