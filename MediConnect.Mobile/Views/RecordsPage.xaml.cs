using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.Views;

public partial class RecordsPage : ContentPage
{
    private readonly RecordsViewModel _viewModel;

    public RecordsPage(RecordsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadRecordsAsync();
    }
}