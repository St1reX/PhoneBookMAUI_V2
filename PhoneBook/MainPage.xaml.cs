using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace PhoneBook
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<Person> contacts_ = new ObservableCollection<Person>();
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Person> Contacts 
        { 
            get => contacts_;
            set
            {
                contacts_ = new ObservableCollection<Person>(value.OrderBy(c => c.FirstName));
                OnPropertyChanged(nameof(Contacts));
                foreach (var contact in  contacts_)
                {
                    Debug.WriteLine(contact.FirstName);
                }
            }
        }

        protected override void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainPage()
        {
            InitializeComponent();
            Contacts = new ObservableCollection<Person>
            {
                new Person("John", "Doe", "111-222-333"),
                new Person("Jakub", "Uryga", "444-555-666")
            };
            BindingContext = this;
        }


        //ADDING
        private void AddPerson_Clicked(object sender, EventArgs e)
        {
            try
            {
                Contacts.Add((new Person(FirstNameEntry.Text, LastNameEntry.Text, PhoneNumberEntry.Text)));
                Contacts = Contacts;
                FirstNameEntry.Text = string.Empty;
                LastNameEntry.Text = string.Empty;
                PhoneNumberEntry.Text = string.Empty;
            }
            catch (ArgumentException ex)
            {
                DisplayAlert("Error occurred while adding new contact.", ex.Message, "Continue");
            }
            BindingContext = this;
        }


        //DELETING
        private void DeleteContact_Clicked(object sender, EventArgs e)
        {
            var personToRemove = (Person)((Button)sender).BindingContext;
            if (Contacts.Contains(personToRemove))
            {
                Contacts.Remove(personToRemove);
            }
        }


        //FILTERING
        private void FilterContacts(object sender, EventArgs e)
        {
            var searchString = search_bar.Text.ToLower();
            var collectionView = contacts_collection;

            var filteredContacts = Contacts
                .Where(c => c.FirstName.ToLower().Contains(searchString) || 
                c.LastName.ToLower().Contains(searchString) || 
                Regex.Replace(c.PhoneNumber.ToLower(), @"\D", "").Contains(searchString))
                .ToList();

            collectionView.BindingContext = new
            {
                Contacts = filteredContacts
            };
        }

        private void ResetFilter_Clicked(object sender, EventArgs e)
        {
            var collectionView = contacts_collection;
            search_bar.Text = string.Empty;

            collectionView.BindingContext = new
            {
                Contacts = Contacts
            };
        }

        //MODIFY
        protected int FindIndexToModify(Person personToFind)
        {
            int index = 0;

            foreach(var contact in Contacts)
            {
                if(contact.FirstName == personToFind.FirstName && contact.LastName == personToFind.LastName)
                {
                    return index;
                }
                index++;
            }

            return -1;
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            var selectedPerson = (Person)((Button)sender).BindingContext;
            if (selectedPerson != null)
            {
                await Navigation.PushModalAsync(new EditDataPage(Contacts, FindIndexToModify(selectedPerson)));
            }
        }

    }


    //NEW CARD TO MODIFYING
    public class EditDataPage : ContentPage
    {
        private Person PersonToEdit { get; set; }

        public EditDataPage(ObservableCollection<Person> contacts, int indexToEdit)
        {
            PersonToEdit = contacts[indexToEdit];

            var titleLabel = new Label
            {
                Text = "Edit data",
                FontSize = 18,
                HorizontalOptions = LayoutOptions.Center
            };

            var firstNameLabel = new Label
            {
                Text = "Name",
                HorizontalOptions = LayoutOptions.Center
            };

            var firstNameEntry = new Entry
            {
                Placeholder = "Name",
                WidthRequest = 300,
                Text = PersonToEdit.FirstName
            };

            var lastNameLabel = new Label
            {
                Text = "Surname",
                HorizontalOptions = LayoutOptions.Center
            };

            var lastNameEntry = new Entry
            {
                Placeholder = "Surname",
                WidthRequest = 300,
                Text = PersonToEdit.LastName
            };

            var phoneNumberLabel = new Label
            {
                Text = "Phone number",
                HorizontalOptions = LayoutOptions.Center
            };

            var phoneNumberEntry = new Entry
            {
                Placeholder = "Phone number",
                WidthRequest = 300,
                Text = PersonToEdit.PhoneNumber
            };


            var saveButton = new Button
            {
                Text = "Save Changes",
                Margin = new Thickness(0, 20, 0, 0),
            };
            saveButton.Clicked += async (sender, event_) =>
            {

                try
                {
                    contacts[indexToEdit].FirstName = firstNameEntry.Text;
                    contacts[indexToEdit].LastName = lastNameEntry.Text;
                    contacts[indexToEdit].PhoneNumber = phoneNumberEntry.Text;

                    await Navigation.PopModalAsync();
                }
                catch (ArgumentException ex)
                {
                    await DisplayAlert("Error occurred while editing contact.", ex.Message, "Continue");
                    return;
                }
            };

            Content = new StackLayout
            {
                Padding = 20,
                Children = { titleLabel, firstNameLabel, firstNameEntry, lastNameLabel, lastNameEntry, phoneNumberLabel, phoneNumberEntry, saveButton }
            };
        }
    }
}