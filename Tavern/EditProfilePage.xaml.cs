namespace Tavern;

public partial class EditProfilePage : ContentPage
{
	public EditProfilePage()
	{
		InitializeComponent();
		PopulateEntryFields();
	}

	public void PopulateEntryFields()
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance();
		entryUsername.Text = singleton.ProfileName;
		entryBio.Text = singleton.ProfileBio;
	}

	public void Update(object sender, EventArgs e)
	{
		ProfileSingleton singleton = ProfileSingleton.GetInstance();
		singleton.ProfileName = entryUsername.Text;
		singleton.ProfileBio = entryBio.Text;

		singleton.updateProfile.Invoke();
	}
}