using Microsoft.VisualBasic;
using System.Collections.ObjectModel;

namespace PhoneBook
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Person> Contacts { get; set; }
        public MainPage()
        {
            InitializeComponent();
            Contacts = new ObservableCollection<Person>
            {
                new Person("Jan", "Kowalski", "123 456 789"),
                new Person("Anna", "Nowak", "987 654 321")
            };
            BindingContext = this;
        }

        private void AddPerson_Clicked(object sender, EventArgs e)
        {
            try
            {
                Contacts.Add(new Person { FirstName = FirstNameEntry.Text, LastName = LastNameEntry.Text, PhoneNumber = PhoneNumberEntry.Text });
                FirstNameEntry.Text = string.Empty;
                LastNameEntry.Text = string.Empty;
                PhoneNumberEntry.Text = string.Empty;
            }
            catch (ArgumentException ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }
            BindingContext = this;
        }

        private void ClearContact_Clicked(object sender, EventArgs e)
        {
            var personToRemove = (Person)((Button)sender).BindingContext;
            if (Contacts.Contains(personToRemove))
            {
                Contacts.Remove(personToRemove);
            }
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            var selectedPerson = (Person)((Button)sender).BindingContext;
            if (selectedPerson != null)
            {
                await Navigation.PushModalAsync(new EditDataPage(selectedPerson));
            }
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            var searchString = search_bar.Text.ToLower();
            var collectionView = contacts_collection;

            var filteredContacts = Contacts.Where(c => c.FirstName.ToLower().Contains(searchString) || c.LastName.ToLower().Contains(searchString)).ToList();

            collectionView.BindingContext = new
            {
                Contacts = filteredContacts
            };
        }

        private void ResetFilter_Clicked(object sender, EventArgs e)
        {
            var collectionView = contacts_collection;

            collectionView.BindingContext = new
            {
                Contacts = Contacts
            };
        }
    }

    public class EditDataPage : ContentPage
    {
        public Person PersonToEdit { get; set; }

        public EditDataPage(Person person)
        {
            PersonToEdit = person;
            BindingContext = this;

            var titleLabel = new Label
            {
                Text = "Edytuj dane",
                FontSize = 18,
                HorizontalOptions = LayoutOptions.Center
            };

            var firstNameLabel = new Label
            {
                Text = "Imię",
                HorizontalOptions = LayoutOptions.Center
            };

            var firstNameEntry = new Entry
            {
                Placeholder = "Imię",
                WidthRequest = 300
            };
            firstNameEntry.SetBinding(Entry.TextProperty, "PersonToEdit.FirstName");

            var lastNameLabel = new Label
            {
                Text = "Nazwisko",
                HorizontalOptions = LayoutOptions.Center
            };

            var lastNameEntry = new Entry
            {
                Placeholder = "Nazwisko",
                WidthRequest = 300
            };
            lastNameEntry.SetBinding(Entry.TextProperty, "PersonToEdit.LastName");

            var phoneNumberLabel = new Label
            {
                Text = "Numer telefonu",
                HorizontalOptions = LayoutOptions.Center
            };

            var phoneNumberEntry = new Entry
            {
                Placeholder = "Numer telefonu",
                WidthRequest = 300
            };
            phoneNumberEntry.SetBinding(Entry.TextProperty, "PersonToEdit.PhoneNumber");

            var saveButton = new Button
            {
                Text = "Zapisz",
                Margin = new Thickness(0, 20, 0, 0),
            };
            saveButton.Clicked += async (s, e) =>
            { 
                await Navigation.PopModalAsync();
            };

            Content = new StackLayout
            {
                Padding = 20,
                Children = { titleLabel, firstNameLabel, firstNameEntry, lastNameLabel, lastNameEntry, phoneNumberLabel, phoneNumberEntry, saveButton }
            };
        }
    }
}