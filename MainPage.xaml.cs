using System.Collections.ObjectModel;
using System.Diagnostics;

namespace CollectionManager
{
    public partial class MainPage : ContentPage
    {
        private string collectionFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Collections");
        public ObservableCollection<CollectionPage> Collections = new ObservableCollection<CollectionPage>();
        public MainPage()
        {
            InitializeComponent();
            LoadCollections();
            collections.ItemsSource = Collections;

            if (!Directory.Exists(collectionFolderPath))
                Directory.CreateDirectory(collectionFolderPath);

            Debug.WriteLine(collectionFolderPath);
        }

        private async void AddCollection(object sender, EventArgs e)
        {
            string name = await DisplayPromptAsync("Tworzenie", "Podaj nazwę nowej kolekcji" , "Stwórz", "Anuluj");

            if (string.IsNullOrWhiteSpace(name) || name == "Anuluj")
                return;

            CollectionPage newCollection = new CollectionPage()
            {
                Name = name
            };

            SaveCollectionFile(newCollection);
            Collections.Add(newCollection);
            await Navigation.PushAsync(newCollection);
        }

        private void DeleteCollection(object sender, EventArgs e)
        {
            CollectionPage selectedCollection = collections.SelectedItem as CollectionPage;

            if (selectedCollection == null)
                return;

            DeleteCollectionFile(selectedCollection);
            Collections.Remove(selectedCollection);
        }

        private void SaveCollectionFile(CollectionPage collection)
        {
            string filePath = Path.Combine(collectionFolderPath, $"{collection.Name}.txt");
            try
            {
                File.Create(filePath).Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd w zapisywaniu kolekcji do pliku: {ex.Message}");
            }
        }

        private void DeleteCollectionFile(CollectionPage collection)
        {
            string filePath = Path.Combine(collectionFolderPath, $"{collection.Name}.txt");
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Błąd w usuwaniu kolekcji: {ex.Message}");
                }
            }
        }

        private async void GoToCollection(object sender, EventArgs e)
        {
            if (collections.SelectedItem == null)
                return;

            await Navigation.PushAsync(new CollectionPage() { Name = (collections.SelectedItem as CollectionPage).Name });
        }

        private void LoadCollections()
        {
            var files = Directory.GetFiles(collectionFolderPath);

            foreach(string file in files)
            {
                var fileName = Path.GetFileName(file).Split(".")[0];

                CollectionPage collection = new CollectionPage() { Name = fileName };

                Collections.Add(collection);
            };
        }
    }
}