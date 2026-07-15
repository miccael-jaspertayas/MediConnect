using MediConnect.Mobile.ViewModels;

namespace MediConnect.Mobile.Views;

public partial class TriagePage : ContentPage
{
	public TriagePage(TriageViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}