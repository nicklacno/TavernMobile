using CommunityToolkit.Maui.Views;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Tavern;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
        dropState.SelectedIndex = 0;
    }

    public async void RegisterButton(object sender, EventArgs e)
    {
        if (!txtConfirmPassword.Text.Equals(txtPassword.Text))
        {
            //error handling here
            ShowErrorMessage("Confirm Password and Password dont match");
            return;
        }

        if (!ValidEmail())
        {
            //error handling here
            ShowErrorMessage("Invalid Email");
            return;
        }    

        if (!PasswordVerification())
        {
            //error handnling here
            ShowErrorMessage("Password missing requirement");
            return;
        }

        int id = await ProfileSingleton.GetInstance().Register(txtUsername.Text, txtPassword.Text, 
            txtEmail.Text, txtCity.Text, dropState.SelectedItem.ToString());

        switch (id)
        {
            case -1:
                //error handling here
                ShowErrorMessage("Failed to connect to server");
                break;
            case -2:
                //error handling here
                ShowErrorMessage("Duplicate Username");
                break;
            case -3:
                //error handling here
                ShowErrorMessage("Duplicate Email");
                break;
            default:
                ProfileSingleton.GetInstance().switchMainPage(new NavigationPage(new TabbedMainPage()));
                break;
        }
    }

    private bool ValidEmail()
    {
        bool hasAtSign = false, hasDomain = false;
        foreach(char c in txtEmail.Text)
        {
            if (c == '@')
                hasAtSign = true;
            else if (hasAtSign && c == '.')
                hasDomain = true;

            if (hasDomain)
                return true;
        }
        return false;
    }

    private bool PasswordVerification()
    {
        string special = "!@#$%^&*<>?:;'\"-+=|[]{}()";

        string password = txtPassword.Text;
        bool containsSpecial = false, containsUppercase = false, containsLowercase = false;
        foreach (char c in password)
        {
            if (!containsLowercase && Char.IsLower(c))
                containsLowercase = true;
            else if (!containsUppercase && Char.IsUpper(c))
                containsUppercase = true;
            else if (!containsSpecial && special.Contains(c))
                containsSpecial = true;

            if (containsLowercase && containsUppercase && containsSpecial)
            {
                return true;
            }
        }
        return false;
    }

    private void ShowErrorMessage(string message)
    {
        var popup = new ErrorPopup(message);
        this.ShowPopup(popup);
    }
}