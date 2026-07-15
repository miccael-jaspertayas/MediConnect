using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.Views;

[QueryProperty(nameof(RecordId), "recordId")]
public partial class AddRecordPage : ContentPage
{
    private readonly AddRecordViewModel _vm;

    public AddRecordPage(AddRecordViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    public int RecordId
    {
        set
        {
            if (value > 0)
            {
                _ = _vm.LoadRecordAsync(value);
            }
        }
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}