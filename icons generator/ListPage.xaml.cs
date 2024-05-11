using CommunityToolkit.Maui.Core.Platform;
using icons_generator.Resources.Options;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
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
    public List<string> sizesAndroid = new List<string>();
    /// <summary>
    /// ������ �������� ����������� ��� iOS
    /// </summary>
    public List<string> sizesIOS = new List<string>();
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
        ListView listView = sender as ListView;
        if (listView.StyleId.Contains("Android"))
        {
            if (e.Item != null)
            {
                SizesListAndroid.SelectedItem = e.Item;
            }
        }
        else if (listView.StyleId.Contains("iOS"))
        {

            if (e.Item != null)
            {
                SizesListiOS.SelectedItem = e.Item;
            }
        }
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
        newSize = newSize.Replace("�", "x");

        bool isValid = Regex.IsMatch(newSize, @"^[0-9x]+$");

        if (!isValid)
        {
            ChangeStatus("������ �����������! "+ newSize);
            return;
        }

        if (newSize != null && newSize.Length > 0)
        {
            Button clickedButton = sender as Button;
            if (clickedButton.StyleId.Contains("Android"))
            {
                // ��������� ������ �������� �����������, ��������� ���� ����� ��������
                List<string> sizes = platformTypeInFile.Android.Sizes.ToList();     ///
                sizes.Add(newSize);                                                 ///
                platformTypeInFile.Android.Sizes = sizes.ToArray();                 ///
                ///////////////////////////////////////////////////////////////////////

                // ��������� � ������ �������� ����������� ����� ��������
                sizesAndroid.Add(newSize);
                ChangeStatus("��������� " + newSize + " ��� Android");
            }
            else if (clickedButton.StyleId.Contains("iOS"))
            {
                // ��������� ������ �������� �����������, ��������� ���� ����� ��������
                List<string> sizes = platformTypeInFile.iOS.Sizes.ToList();     ///
                sizes.Add(newSize);                                                 ///
                platformTypeInFile.iOS.Sizes = sizes.ToArray();                 ///
                ///////////////////////////////////////////////////////////////////////

                // ��������� � ������ �������� ����������� ����� ��������
                sizesIOS.Add(newSize);
                ChangeStatus("��������� " + newSize + " ��� iOS");
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
        Button clickedButton = sender as Button;
        if (clickedButton.StyleId.Contains("Android"))
        {
            if (SizesListAndroid.SelectedItem != null)
            {
                try
                {
                    // ������� ��������� ������� ������(UI) � ������� �� �� ������(����������)  //
                    var delItem = sizesAndroid.First(p => p == SizesListAndroid.SelectedItem); //
                    sizesAndroid.Remove(delItem);                                             //
                    ///////////////////////////////////////////////////////////////////////////

                    // ��������� ������ �������� �����������, ������� ��������� ������� ������ //
                    List<string> sizes = platformTypeInFile.Android.Sizes.ToList();           //
                    sizes.Remove(delItem);                                                   //
                    platformTypeInFile.Android.Sizes = sizes.ToArray();                     //
                    /////////////////////////////////////////////////////////////////////////
                    ChangeStatus("������� " + SizesListAndroid.SelectedItem + " ��� Android");
                }
                catch
                {

                }
                UpdateList();
            }
        }
        if (clickedButton.StyleId.Contains("iOS"))
        {
            if (SizesListiOS.SelectedItem != null)
            {
                try
                {
                    // ������� ��������� ������� ������(UI) � ������� �� �� ������(����������)  //
                    var delItem = sizesIOS.First(p => p == SizesListiOS.SelectedItem);        //
                    sizesIOS.Remove(delItem);                                             //
                    ///////////////////////////////////////////////////////////////////////////

                    // ��������� ������ �������� �����������, ������� ��������� ������� ������ //
                    List<string> sizes = platformTypeInFile.iOS.Sizes.ToList();           //
                    sizes.Remove(delItem);                                                   //
                    platformTypeInFile.iOS.Sizes = sizes.ToArray();                     //
                    /////////////////////////////////////////////////////////////////////////
                    ChangeStatus("������� " + SizesListAndroid.SelectedItem + " ��� iOS");
                }
                catch
                {

                }
                UpdateList();
            }
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
    public void ChangeStatus(string status)
    {
        StatusBar.Text = status;
    }
}
