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

    private void EditButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is RecordsViewModel vm &&
            sender is Button button &&
            button.CommandParameter is MedicalRecord record)
        {
            vm.EditRecordCommand.Execute(record);
        }
    }

    private void DeleteButton_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is RecordsViewModel vm &&
            sender is Button button &&
            button.CommandParameter is MedicalRecord record)
        {
            vm.DeleteRecordCommand.Execute(record);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is RecordsViewModel vm)
        {
            await vm.LoadRecordsAsync();
        }
    }
}