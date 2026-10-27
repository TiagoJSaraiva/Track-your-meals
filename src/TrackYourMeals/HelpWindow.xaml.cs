using System.Windows;
using System.Windows.Input;

namespace TrackYourMeals;

public partial class HelpWindow : Window
{
    public HelpWindow()
    {
        InitializeComponent();
    }

    private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        Close();
    }
}
