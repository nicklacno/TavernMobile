using CommunityToolkit.Maui.Views;
using System.Diagnostics;

namespace Tavern;

public partial class ErrorPopup : Popup
{
    public ErrorPopup(string message = "")
    {
        InitializeComponent();
        Debug.WriteLine(message);
    }
}