using System.Diagnostics;

namespace Tavern;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    public async Task RegisterButton(object sender, EventArgs e)
    {
        if (txtConfirmPassword.Text.Equals(txtPassword.Text))
        {
            //error handling here
            Debug.WriteLine("Confirm Password and Password dont match");
            return;
        }

        if (!ValidEmail())
        {
            //error handling here
            Debug.WriteLine("Invalid Email");
        }    

        if (!PasswordVerification())
        {
            //error handnling here
            Debug.WriteLine("Password missing requirement");
            return;
        }

        int id = await ProfileSingleton.GetInstance().Register(txtUsername.Text, txtPassword.Text);

        switch (id)
        {
            case -1:
                //error handling here
                Debug.WriteLine("Failed to connect to server");
                break;
            case -2:
                //error handling here
                Debug.WriteLine("Duplicate Username");
                break;
            case -3:
                //error handling here
                Debug.WriteLine("Duplicate Email");
                break;
            default:
                ProfileSingleton.GetInstance().loginSuccessful.Invoke();
                break;
        }



    }

    private bool ValidEmail()
    {
        return txtEmail.Text.Contains('.') && txtEmail.Text.Contains('.');
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
}