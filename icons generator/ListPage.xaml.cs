using icons_generator.Resources.Options;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Xml;
using Windows.Security.Cryptography.Certificates;

namespace icons_generator;

public partial class ListPage : ContentPage
{
    String path = String.Empty;
    PlatformType platformTypeInFile;
    public ObservableCollection<ImageSizes> ImageSizes { get; set; } = new ObservableCollection<ImageSizes>();

    List<string> sizesAndroid = new List<string>();
    List<string> sizesIOS = new List<string>();
    public ListPage()
    {
        InitializeComponent();
        ShowDefaultSettings();
    }

    private async void ConfirmClicked(object sender, EventArgs e)
    { }
    private async void SizesListItemTapped(object sender, ItemTappedEventArgs e)
    {

    }
    private async void ListItemAddClicked(object sender, EventArgs e)
    {
        var newSize = await DisplayPromptAsync("Добавление размера", "Введите размер в формате(0х0):", "OK", "Отмена");
        if (newSize != null)
        {
            sizesAndroid.Add(newSize);
            List<string> sizes = platformTypeInFile.Android.Sizes.ToList();
            sizes.Add(newSize);
            platformTypeInFile.Android.Sizes = sizes.ToArray();
            UpdateList();
        }
    }

    private async void ListItemDelClicked(object sender, EventArgs e)
    {
        if (SizesList.SelectedItem != null)
        {
            try
            {
                var delItem = sizesAndroid.First(p => p == SizesList.SelectedItem);
                sizesAndroid.Remove(delItem);
                List<string> sizes = platformTypeInFile.Android.Sizes.ToList();
                sizes.Remove(delItem);
                platformTypeInFile.Android.Sizes = sizes.ToArray();
            }
            catch
            {

            }
            UpdateList();
        }
    }

    private void ShowDefaultSettings()
    {
        var exePath = AppDomain.CurrentDomain.BaseDirectory;
        path = Path.Combine(exePath, "Resources\\Options\\ImagesSize.json");

        using (StreamReader r = new StreamReader(path))
        {
            string json = r.ReadToEnd();
            if (json.Length > 0)
            {
                platformTypeInFile = JsonSerializer.Deserialize<PlatformType>(json);
                sizesAndroid = platformTypeInFile.Android.Sizes.ToList();
                sizesIOS = platformTypeInFile.iOS.Sizes.ToList();
            }
        }
    }

    private void UpdateList()
    {
        var sizes = new ImageSizesViewModel(sizesAndroid);
        SizesList.BindingContext = sizes;
        ReSaveJson();
    }

    private void ReSaveJson()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        using (StreamWriter w = new StreamWriter(path))
        {
            var jsonToString = JsonSerializer.Serialize<PlatformType>(platformTypeInFile);
            w.Write(jsonToString);
        }
    }

    public class ImageSizesViewModel : BindableObject
    {
        private ObservableCollection<string> imageSizes;

        public ObservableCollection<string> ImageSizes
        {
            get { return imageSizes; }
            set
            {
                imageSizes = value;
                OnPropertyChanged(nameof(ImageSizes));
            }
        }

        // В конструкторе добавьте начальные данные
        public ImageSizesViewModel(List<string> imageSizes)
        {
            // Инициализация коллекции через конструктор
            ImageSizes = new ObservableCollection<string>(imageSizes);
        }
    }
}