using Microsoft.Maui.Storage;
using System.Security.AccessControl;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Image = SixLabors.ImageSharp.Image;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Maui.Core.Primitives;

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
        GenerateGoBtn.IsVisible = true;
        ChangeStatus("Проверьте настройки размеров!");
    }

    private async void GenerateGo_Clicked(object sender, EventArgs e)
    {
        int progress = 0;
        var listSizes = new ListPage();
        // Получаем путь к выбранному изображению
        string selectedImagePath = MainImage.Source.ToString().Replace("File:", "").Trim();
        Folder folderPath = null;
        ChangeStatus("Генерирую...");
        try
        {
            // Создаем папку для сохранения дубликатов изображений
            folderPath = await FolderPicker.PickAsync(default);
        }
        catch {
            ChangeStatus("Передумали?");
            return;
        }

        // Проходим по размерам Android
        foreach (var size in listSizes.sizesAndroid)
        {
            // Создаем новый путь для изображения
            string newImagePath = Path.Combine(folderPath.Path, $"Android_{size}.png");

            // Создаем и сохраняем изображение для данного размера
            CreateAndSaveImage(selectedImagePath, newImagePath, size);
        }

        // Проходим по размерам iOS
        foreach (string size in listSizes.sizesIOS)
        {
            progress++;
            // Создаем новый путь для изображения
            string newImagePath = Path.Combine(folderPath.Path, $"iOS_{size}.png");

            // Создаем и сохраняем изображение для данного размера
            CreateAndSaveImage(selectedImagePath, newImagePath, size);
            if (progress == listSizes.sizesIOS.Count())
            {
                ChangeStatus("Иконки сгенерированы! По пути:"+ folderPath.Path);
            }
        }
    }

    private void CreateAndSaveImage(string sourceImagePath, string newImagePath, string size)
    {
        // Загружаем исходное изображение
        using (var image = Image.Load(sourceImagePath))
        {
            // Разбиваем размер из строки на ширину и высоту
            string[] dimensions = size.Split('x');
            int width = int.Parse(dimensions[0]);
            int height = int.Parse(dimensions[1]);

            // Меняем размер изображения
            image.Mutate(x => x.Resize(width, height));

            // Сохраняем измененное изображение в новом пути
            image.Save(newImagePath);
        }
    }

    private async void ToListPage(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ListPage());
        ChangeStatus("Хорошо! Можно генерить.");
    }

    public void ChangeStatus(string status)
    {
        StatusBar.Text = status;
    }
}

