using icons_generator.Resources.Options;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
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
    public ObservableCollection<ImageSizes> ImageSizes { get; set; } = new ObservableCollection<ImageSizes>();

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
        if (newSize != null)
        {
            // ��������� ������ �������� �����������, ��������� ���� ����� ��������
            List<string> sizes = platformTypeInFile.Android.Sizes.ToList();     ///
            sizes.Add(newSize);                                                 ///
            platformTypeInFile.Android.Sizes = sizes.ToArray();                 ///
            ///////////////////////////////////////////////////////////////////////

            // ��������� � ������ �������� ����������� ����� ��������
            sizesAndroid.Add(newSize);
           
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
        if (SizesList.SelectedItem != null)
        {
            try
            {
                // ������� ��������� ������� ������(UI) � ������� �� �� ������(����������)  //
                var delItem = sizesAndroid.First(p => p == SizesList.SelectedItem);        //
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
        var sizes = new ImageSizesViewModel(sizesAndroid);
        SizesList.BindingContext = sizes;

        if (needReSave)
        {
            ReSaveJson();
        }
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

        // � ������������ �������� ��������� ������
        public ImageSizesViewModel(List<string> imageSizes)
        {
            // ������������� ��������� ����� �����������
            ImageSizes = new ObservableCollection<string>(imageSizes);
        }
    }
}