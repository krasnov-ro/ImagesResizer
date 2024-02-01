using icons_generator.Resources.Options;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Xml;

namespace icons_generator;

public partial class ListPage : ContentPage
{
    /// <summary>
    /// Путь к файлу json с размерами изображений для платформ
    /// </summary>
    String path = String.Empty;

    /// <summary>
    /// Объект в который помещены данные из json файла по пути @path
    /// </summary>
    PlatformType platformTypeInFile;

    /// <summary>
    /// Объект для вывода размеров изображений в ListView на странице приложения
    /// </summary>
    public ObservableCollection<ImageSizes> ImageSizes { get; set; } = new ObservableCollection<ImageSizes>();

    /// <summary>
    /// Список размеров изображений для Android
    /// </summary>
    List<string> sizesAndroid = new List<string>();
    /// <summary>
    /// Список размеров изображений для iOS
    /// </summary>
    List<string> sizesIOS = new List<string>();
    public ListPage()
    {
        InitializeComponent();
        ShowDefaultSettings();
    }

    private async void ConfirmClicked(object sender, EventArgs e)
    { 

    }
    private async void SizesListItemTapped(object sender, ItemTappedEventArgs e)
    {

    }
    /// <summary>
    /// Метод обрабатывает нажатие на кнопку AddButton(добавить)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ListItemAddClicked(object sender, EventArgs e)
    {
        // Переменная newSize хранит в себе данные введеные в popUp
        var newSize = await DisplayPromptAsync("Добавление размера", "Введите размер в формате(0х0):", "OK", "Отмена");
        if (newSize != null)
        {
            // Обновляем объект размеров изображения, добавляем туда новое значение
            List<string> sizes = platformTypeInFile.Android.Sizes.ToList();     ///
            sizes.Add(newSize);                                                 ///
            platformTypeInFile.Android.Sizes = sizes.ToArray();                 ///
            ///////////////////////////////////////////////////////////////////////

            // Добавляем в список размеров изображения новое значение
            sizesAndroid.Add(newSize);
           
            // Обновляем ListView которая находится на странице
            UpdateList();
        }
    }

    /// <summary>
    /// Метод обрабатывает нажатие на кнопку DeleteButton(удалить)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ListItemDelClicked(object sender, EventArgs e)
    {
        if (SizesList.SelectedItem != null)
        {
            try
            {
                // Находим выбранный элемент списка(UI) и удаляем ее из списка(переменная)  //
                var delItem = sizesAndroid.First(p => p == SizesList.SelectedItem);        //
                sizesAndroid.Remove(delItem);                                             //
                ///////////////////////////////////////////////////////////////////////////

                // Обновляем объект размеров изображения, удаляем выбранный элемент списка //
                List<string> sizes = platformTypeInFile.Android.Sizes.ToList();           //
                sizes.Remove(delItem);                                                   //
                platformTypeInFile.Android.Sizes = sizes.ToArray();                     //
                /////////////////////////////////////////////////////////////////////////
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
        UpdateList(false);
    }

    /// <summary>
    /// Функция обновления ListView
    /// </summary>
    private void UpdateList(bool needReSave = true)
    {
        sizesAndroid.Sort();

        var sizes = new ImageSizesViewModel(sizesAndroid.OrderBy(ParseSize).ToList());
        
        SizesList.BindingContext = sizes;

        if (needReSave)
        {
            ReSaveJson();
        }
    }

    private static int ParseSize(string size)
    {
        // Используйте разделитель "x" для разделения чисел
        string[] parts = size.Split('x');

        // Преобразуйте первую часть (до "x") в число
        if (parts.Length > 0 && int.TryParse(parts[0], out int result))
        {
            return result;
        }

        // Если не удается преобразовать, возвращаем максимальное значение int
        return int.MaxValue;
    }

    /// <summary>
    /// Пересохранение файла json, который содержит в себе размеры изображения для платформ
    /// </summary>
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
