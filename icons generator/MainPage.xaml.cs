using Microsoft.Maui.Storage;
using System.Security.AccessControl;

namespace icons_generator;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

    private async void PickPhoto_Clicked(object sender, EventArgs e)
    {
        if (CounterBtn.Text != "Настройка размеров")
        {
            FileResult result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Pick a Photo",
                FileTypes = FilePickerFileType.Images
            });

            if (result == null) return;

            FileName.Text = result.FileName;
            MainImage.Source = result.FullPath;
            CounterBtn.Text = "Настройка размеров";
        }
        else
        {
            ToListPage(sender, e);
        }
    }

    private async void ToListPage(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new ListPage());
    }
}

