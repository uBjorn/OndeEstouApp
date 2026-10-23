using OndeEstouApp.ViewModels;
namespace OndeEstouApp.Views;

public partial class LocalPage : ContentPage
{
    public LocalPage(LocalViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}