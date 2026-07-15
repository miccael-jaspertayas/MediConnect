using System;
using Microsoft.Maui.Controls;
using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.Views
{
    public partial class AddVitalsPage : ContentPage
    {
        public AddVitalsPage(VitalsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        // Go back to the previous page manually if they click a custom back button
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}