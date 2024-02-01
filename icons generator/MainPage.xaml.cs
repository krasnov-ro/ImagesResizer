using Microsoft.Maui.Storage;
using System.Security.AccessControl;

namespace icons_generator;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
        CounterBtn.Clicked += PickPhoto_Clicked;
    }

    private async void PickPhoto_Clicked(object sender, EventArgs e)
    {
        FileResult result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Pick a Photo",
            FileTypes = FilePickerFileType.Images
        });

        if (result == null) return;
        FileName.Text = result.FileName;
        MainImage.Source = result.FullPath;

        if (ReOpenBtn.IsVisible == true) return;
        CounterBtn.Text = "Настройка размеров";
        CounterBtn.Clicked -= PickPhoto_Clicked;
        CounterBtn.Clicked += ToListPage;
        ReOpenBtn.IsVisible = true;
    }

    private async void ToListPage(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ListPage());
    }
}

