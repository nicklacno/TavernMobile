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
        if (txtUsername.Text == "")
        {
            ShowErrorMessage("Enter Valid Username");
            return;
        }
        if (!txtConfirmPassword.Text.Equals(txtPassword.Text))
        {
            //error handling here
            ShowErrorMessage("Confirm Password and Password don't match");
            return;
        }

        if (!ValidEmail())
        {
            //error handling here
            ShowErrorMessage("Invalid Email");
            return;
        }
        int verification = PasswordVerification();
        if (verification == -1)
        {
            //error handnling here
            ShowErrorMessage("Password missing lowercase character");
            return;
        }
        else if (verification == -2) 
        {
            ShowErrorMessage("Password missing uppercase character");
            return;
        }
        else if (verification == -3)
        {
            ShowErrorMessage("Password missing special character");
            return;
        }
        else if (verification == -4)
        {
            ShowErrorMessage("Password missing number");
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

    private int PasswordVerification()
    {
        string special = "!@#$%^&*<>?:;'\"-+=|[]{}()";

        string password = txtPassword.Text;
        bool containsSpecial = false, containsUppercase = false, containsLowercase = false, containsNumber = false ;
        foreach (char c in password)
        {
            if (!containsLowercase && Char.IsLower(c))
                containsLowercase = true;
            else if (!containsUppercase && Char.IsUpper(c))
                containsUppercase = true;
            else if (!containsSpecial && special.Contains(c))
                containsSpecial = true;
            else if (!containsNumber && Char.IsDigit(c))
                containsNumber = true;

            if (containsLowercase && containsUppercase && containsSpecial && containsNumber)
            {
                return 0;
            }
        }
        if (!containsLowercase) return -1;
        else if (!containsUppercase) return -2;
        else if (!containsSpecial) return -3;
        else if (!containsNumber) return -4;
        return 0;//should not reach here
    }

    private void ShowErrorMessage(string message)
    {
        var popup = new ErrorPopup(message);
        this.ShowPopup(popup);
    }
}