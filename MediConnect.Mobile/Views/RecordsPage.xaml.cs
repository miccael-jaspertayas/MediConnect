using MediConnect.Mobile.Models;
using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.Views;

public partial class RecordsPage : ContentPage
{
    public RecordsPage(RecordsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is RecordsViewModel vm)
        {
            await vm.LoadRecordsAsync();
        }
    }

    // Navigate to Add Record page
    private async void AddRecordFab_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddRecordPage));
    }

    // Open an existing record for editing
    private async void RecordCard_Tapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is MedicalRecord record)
        {
            await Shell.Current.GoToAsync(
                $"{nameof(AddRecordPage)}?recordId={record.RecordID}");
        }
    }
}