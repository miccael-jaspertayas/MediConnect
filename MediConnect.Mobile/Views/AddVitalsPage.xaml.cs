using System;
using Microsoft.Maui.Controls;
using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.Views
{
    public partial class AddVitalsPage : ContentPage
    {
        
        public AddVitalsPage(AddVitalsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}