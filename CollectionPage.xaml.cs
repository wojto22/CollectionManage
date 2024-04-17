using System.Collections.ObjectModel;

namespace CollectionManager;

public partial class CollectionPage : ContentPage
{
	public string Name { get; set; }

	public ObservableCollection<Element> Elements = new ObservableCollection<Element>();


	public CollectionPage()
	{
		InitializeComponent();

        elements.ItemsSource = Elements;
        LoadElementsFromFile();
	}

    private async void AddElement(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("Nowy element", "Podaj nazwê elementu", "Dodaj", "Anuluj");
        if (string.IsNullOrWhiteSpace(name) || name == "Anuluj")
            return;

        Element element = new Element { Name = name };
        Elements.Add(element);
        SaveElementsToFile();
    }

    private void DeleteElement(object sender, EventArgs e)
    {
        if (elements.SelectedItem is Element selectedElement)
        {
            Elements.Remove(selectedElement);
            SaveElementsToFile();
        }
    }

    private async void EditElement(object sender, EventArgs e)
    {
        if (elements.SelectedItem is Element selectedElement)
        {
            string newName = await DisplayPromptAsync("Edycja elementu", "Nowa nazwa elementu", initialValue: selectedElement.Name, accept: "Edytuj", cancel: "Anuluj");
            if (!string.IsNullOrWhiteSpace(newName) && newName != "Anuluj")
            {
                selectedElement.Name = newName;
                SaveElementsToFile();
            }
        }
    }

    private void LoadElementsFromFile()
    {
        string filePath = GetFilePath();
        if (File.Exists(filePath))
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                Elements.Add(new Element { Name = line });
            }
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadElementsFromFile();
    }

    private void SaveElementsToFile()
    {
        string filePath = GetFilePath();
        File.WriteAllLines(filePath, Elements.Select(e => e.Name));
    }

    private string GetFilePath()
    {
        return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Collections"), $"{Name}.txt");
    }
}