using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace PhoneBook
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<Person> contacts_ = new ObservableCollection<Person>();
        public ObservableCollection<Person> filteredContacts_ = new ObservableCollection<Person>();

        public ObservableCollection<Person> Contacts 
        { 
            get => contacts_;
            set
            {
                contacts_ = new ObservableCollection<Person>(value.OrderBy(c => c.LastName).ThenBy(c => c.FirstName));
                OnPropertyChanged(nameof(Contacts));
            }
        }
        public ObservableCollection<Person> FilteredContacts { get; set; }
        public Collection<Person> ContactsToDelete { get; set; }
        protected override void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public MainPage()
        {
            InitializeComponent();
            Contacts = new ObservableCollection<Person>
            {
                new Person("John", "Doe", "123424221"),
                new Person("Jakub", "Uryga", "21312213"),
                new Person("Szymon", "Filipek", "56766575665"),
                new Person("Tymoteusz", "Spokojny", "21093821321"),
            };
            ContactsToDelete = new Collection<Person>();
            FilteredContacts = new ObservableCollection<Person>();
            BindingContext = this;
        }


        //ADDING
        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            ContactsToDelete.Clear();   
            await Navigation.PushModalAsync(new AddDataPage(Contacts, this));
            BindingContext = this;
        }


        //RESPONSIVE DESIGN
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width < 600)
            {
                add_button.HorizontalOptions = LayoutOptions.Fill;
                add_button.WidthRequest = width;

                delete_button.HorizontalOptions = LayoutOptions.Fill;
                delete_button.WidthRequest = width;

                search_bar.HorizontalOptions = LayoutOptions.Fill;
                search_bar.WidthRequest = width;

                reset_filter_button.HorizontalOptions = LayoutOptions.Fill;
                reset_filter_button.WidthRequest = width;

                contacts_collection.WidthRequest = width;
            }
            else
            { 
                

                add_button.WidthRequest = 300;
                add_button.HorizontalOptions = LayoutOptions.Start;

                delete_button.WidthRequest = 300;
                delete_button.HorizontalOptions = LayoutOptions.Start;

                search_bar.WidthRequest = 300;
                search_bar.HorizontalOptions = LayoutOptions.Start;

                reset_filter_button.WidthRequest = 300;
                reset_filter_button.HorizontalOptions = LayoutOptions.Start;

                contacts_collection.WidthRequest = 300;
            }
        }


        //DELETING
        private async void DeleteContact_Clicked(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                var button = (Button)sender;

                await DeleteMultiple();  
            }
            else if (sender is MenuFlyoutItem menuItem)
            {
                DeleteSingle((Person)menuItem.CommandParameter);
            }
            else if (sender is ToolbarItem)
            {
                await DeleteMultiple();
            }
            else if (sender is SwipeItem swipeItem)
            {
                DeleteSingle((Person)swipeItem.BindingContext);
            }
        }
        private void CheckBox_ToDelete_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if(sender is CheckBox checkBox)
            {
                Person PersonToDelete = (Person)checkBox.BindingContext;

                if(ContactsToDelete.Contains(PersonToDelete))
                {
                    ContactsToDelete.Remove(PersonToDelete);
                }
                else
                {
                    ContactsToDelete.Add(PersonToDelete);
                }
            }
        }

        private async Task DeleteMultiple()
        {
            bool confirm = await DisplayAlert("Confirm", "Delete selected contacts?", "Yes", "No");
            if (!confirm) return;

            if (ContactsToDelete.Count == 0)
            {
                await DisplayAlert("Error", "No contacts selected for deletion.", "OK");
                return;
            }

            foreach (var Ptd in ContactsToDelete)
            {
                Contacts.Remove(Ptd);
                FilteredContacts.Remove(Ptd);
            }
            ContactsToDelete.Clear();
        }

        private void DeleteSingle(Person personToDelete)
        {
            Contacts.Remove(personToDelete);
            ContactsToDelete.Remove(personToDelete);
            FilteredContacts.Remove(personToDelete);
            
        }


        //FILTERING
        private void FilterContacts(object sender, EventArgs e)
        {
            var searchString = search_bar.Text.ToLower();
            var collectionView = contacts_collection;

            FilteredContacts = new ObservableCollection<Person>(Contacts
                .Where(c => c.FirstName.ToLower().Contains(searchString) || 
                c.LastName.ToLower().Contains(searchString) || 
                Regex.Replace(c.PhoneNumber.ToLower(), @"\D", "").Contains(searchString))
                .ToList());

            collectionView.BindingContext = new
            {
                Contacts = FilteredContacts
            };


            if (searchString == "")
            {
                collectionView.BindingContext = new
                {
                    Contacts = Contacts
                };
            }
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
            ContactsToDelete.Clear();

            if (sender is MenuFlyoutItem)
            {
                var menuItem = (MenuFlyoutItem)sender;
                var selectedPerson = (Person)menuItem.CommandParameter;
                if (selectedPerson != null)
                {
                    await Navigation.PushModalAsync(new EditDataPage(Contacts, FindIndexToModify(selectedPerson), this));
                }
            }
            else if (sender is Button)
            {
                var button = (Button)sender;
                var selectedPerson = (Person)button.BindingContext;
                if (selectedPerson != null)
                {
                    await Navigation.PushModalAsync(new EditDataPage(Contacts, FindIndexToModify(selectedPerson), this));
                }
            }
            else if (sender is SwipeItem)
            {
                var swipeItem = (SwipeItem)sender;
                var selectedPerson = (Person)swipeItem.BindingContext;
                if (selectedPerson != null)
                {
                    await Navigation.PushModalAsync(new EditDataPage(Contacts, FindIndexToModify(selectedPerson), this));
                }
            }

            ResetFilter_Clicked(null, EventArgs.Empty);
        }
    }


    //NEW CARD TO MODIFYING
    public class EditDataPage : ContentPage
    {
        private Person PersonToEdit { get; set; }

        public EditDataPage(ObservableCollection<Person> contacts, int indexToEdit, MainPage mainPage)
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

                    //Refresh the contacts list on the main page
                    mainPage.Contacts = contacts;

                    //Close modal
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

    //NEW CARD TO ADDING
    public class AddDataPage : ContentPage
    {
        public AddDataPage(ObservableCollection<Person> contacts, MainPage mainPage)
        {
            var titleLabel = new Label
            {
                Text = "Add new contact",
                FontSize = 18,
                HorizontalOptions = LayoutOptions.Center
            };

            var firstNameLabel = new Label
            {
                Text = "Name:",
                HorizontalOptions = LayoutOptions.Center
            };

            var firstNameEntry = new Entry
            {
                WidthRequest = 300
            };

            var lastNameLabel = new Label
            {
                Text = "Surname:",
                HorizontalOptions = LayoutOptions.Center
            };

            var lastNameEntry = new Entry
            {
                WidthRequest = 300
            };

            var phoneNumberLabel = new Label
            {
                Text = "Phone Number:",
                HorizontalOptions = LayoutOptions.Center
            };

            var phoneNumberEntry = new Entry
            {
                WidthRequest = 300
            };

            var addButton = new Button
            {
                Text = "Add",
                WidthRequest = 300,
                Margin = new Thickness(0, 25, 0, 0)
            };

            addButton.Clicked += async (sender, args) =>
            {
                try
                {
                    var newPerson = new Person(firstNameEntry.Text, lastNameEntry.Text, phoneNumberEntry.Text);
                    contacts.Add(newPerson);
                    mainPage.Contacts = contacts;

                    await Navigation.PopModalAsync();
                }
                catch (ArgumentException ex)
                {
                    await DisplayAlert("Error occurred while adding new contact.", ex.Message, "Continue");
                }
            };

            Content = new StackLayout
            {
                Padding = 20,
                VerticalOptions = LayoutOptions.Center,
                Children =
            {
                titleLabel,
                firstNameLabel, firstNameEntry,
                lastNameLabel, lastNameEntry,
                phoneNumberLabel, phoneNumberEntry,
                addButton
            }
            };
        }
    }


}