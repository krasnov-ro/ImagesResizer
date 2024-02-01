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
    /// ���� � ����� json � ��������� ����������� ��� ��������
    /// </summary>
    String path = String.Empty;

    /// <summary>
    /// ������ � ������� �������� ������ �� json ����� �� ���� @path
    /// </summary>
    PlatformType platformTypeInFile;

    /// <summary>
    /// ������ ��� ������ �������� ����������� � ListView �� �������� ����������
    /// </summary>
    public ObservableCollection<ImageSizes> ImageSizesAndroid { get; set; } = new ObservableCollection<ImageSizes>();
    public ObservableCollection<ImageSizes> ImageSizesiOS { get; set; } = new ObservableCollection<ImageSizes>();

    /// <summary>
    /// ������ �������� ����������� ��� Android
    /// </summary>
    List<string> sizesAndroid = new List<string>();
    /// <summary>
    /// ������ �������� ����������� ��� iOS
    /// </summary>
    List<string> sizesIOS = new List<string>();
    public ListPage()
    {
        InitializeComponent();
        ShowDefaultSettings();
    }

    private async void ConfirmClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
    private async void SizesListItemTapped(object sender, ItemTappedEventArgs e)
    {

    }

    /// <summary>
    /// ����� ������������ ������� �� ������ AddButton(��������)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ListItemAddClicked(object sender, EventArgs e)
    {
        // ���������� newSize ������ � ���� ������ �������� � popUp
        var newSize = await DisplayPromptAsync("���������� �������", "������� ������ � �������(0�0):", "OK", "������");
        if (newSize != null && newSize.Length > 0)
        {
            ImageButton clickedButton = sender as ImageButton;
            if (clickedButton.StyleId.Contains("Android"))
            {
                // ��������� ������ �������� �����������, ��������� ���� ����� ��������
                List<string> sizes = platformTypeInFile.Android.Sizes.ToList();     ///
                sizes.Add(newSize);                                                 ///
                platformTypeInFile.Android.Sizes = sizes.ToArray();                 ///
                ///////////////////////////////////////////////////////////////////////

                // ��������� � ������ �������� ����������� ����� ��������
                sizesAndroid.Add(newSize);
            }
            else if(clickedButton.StyleId.Contains("iOS"))
            {
                // ��������� ������ �������� �����������, ��������� ���� ����� ��������
                List<string> sizes = platformTypeInFile.iOS.Sizes.ToList();     ///
                sizes.Add(newSize);                                                 ///
                platformTypeInFile.iOS.Sizes = sizes.ToArray();                 ///
                ///////////////////////////////////////////////////////////////////////

                // ��������� � ������ �������� ����������� ����� ��������
                sizesIOS.Add(newSize);
            }

            // ��������� ListView ������� ��������� �� ��������
            UpdateList();
        }
    }

    /// <summary>
    /// ����� ������������ ������� �� ������ DeleteButton(�������)
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ListItemDelClicked(object sender, EventArgs e)
    {
        if (SizesListAndroid.SelectedItem != null)
        {
            try
            {
                // ������� ��������� ������� ������(UI) � ������� �� �� ������(����������)  //
                var delItem = sizesAndroid.First(p => p == SizesListAndroid.SelectedItem);        //
                sizesAndroid.Remove(delItem);                                             //
                ///////////////////////////////////////////////////////////////////////////

                // ��������� ������ �������� �����������, ������� ��������� ������� ������ //
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
    /// ������� ���������� ListView
    /// </summary>
    private void UpdateList(bool needReSave = true)
    {
        var sizesAndroidSort = new ImageSizesViewModel(sizesAndroid.OrderBy(ParseSize).ToList());
        var sizesiOSSort = new ImageSizesViewModel(sizesIOS.OrderBy(ParseSize).ToList());

        SizesListAndroid.BindingContext = sizesAndroidSort;
        SizesListiOS.BindingContext = sizesiOSSort;

        //Header.Margin = new Thickness(Header.Margin.Left, sizesAndroid.Count * (-22.86), Header.Margin.Right, Header.Margin.Bottom);

        if (needReSave)
        {
            ReSaveJson();
        }
    }

    private static int ParseSize(string size)
    {
        // ����������� ����������� "x" ��� ���������� �����
        string[] parts = size.Split('x');

        // ������������ ������ ����� (�� "x") � �����
        if (parts.Length > 0 && int.TryParse(parts[0], out int result))
        {
            return result;
        }

        // ���� �� ������� �������������, ���������� ������������ �������� int
        return int.MaxValue;
    }

    /// <summary>
    /// �������������� ����� json, ������� �������� � ���� ������� ����������� ��� ��������
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
        private ObservableCollection<string> imageSizesAndroid;
        private ObservableCollection<string> imageSizesiOS;

        public ObservableCollection<string> ImageSizesAndroid
        {
            get { return imageSizesAndroid; }
            set
            {
                imageSizesAndroid = value;
                OnPropertyChanged(nameof(ImageSizesAndroid));
            }
        }

        public ObservableCollection<string> ImageSizesiOS
        {
            get { return imageSizesiOS; }
            set
            {
                imageSizesiOS = value;
                OnPropertyChanged(nameof(ImageSizesiOS));
            }
        }

        // � ������������ �������� ��������� ������
        public ImageSizesViewModel(List<string> imageSizes)
        {
            // ������������� ��������� ����� �����������
            ImageSizesAndroid = new ObservableCollection<string>(imageSizes);
            ImageSizesiOS = new ObservableCollection<string>(imageSizes);
        }
    }
}
