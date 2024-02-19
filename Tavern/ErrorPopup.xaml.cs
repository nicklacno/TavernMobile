using CommunityToolkit.Maui.Views;
using System.Diagnostics;

namespace Tavern;

public partial class ErrorPopup : Popup
{
    public ErrorPopup(string message = "", bool isReloadError = false)
    {
        InitializeComponent();
        Debug.WriteLine(message);
        txtErrorMessage.Text = message;
    }
}