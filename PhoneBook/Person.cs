using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace PhoneBook
{
    public class Person : INotifyPropertyChanged
    {
        private string _firstName = default!;
        private string _lastName = default!;
        private string _phoneNumber = default!;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public string FirstName
        {
            get => _firstName;
            set
            {
                ValidateFirstName(value);
                _firstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                ValidateLastName(value);
                _lastName = value;
                OnPropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                ValidatePhoneNumber(value);
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        public Person(string firstName, string lastName, string phoneNumber)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
        }

        private void ValidateFirstName(string firstName)
        {
            Regex regex = new Regex(@"^[A-ZĄĆĘŁŃÓŚŻŹ][a-ząćęłńóśżź]+(?: [A-ZĄĆĘŁŃÓŚŻŹ][a-ząćęłńóśżź]+)*$");
            if (String.IsNullOrEmpty(firstName) || !regex.IsMatch(firstName))
            {
                throw new ArgumentException("First name must start with a capital letter and contain only letters. Compound names are allowed.");
            }
        }

        private void ValidateLastName(string lastName)
        {
            Regex regex = new Regex(@"^[A-ZĄĆĘŁŃÓŚŻŹ][a-ząćęłńóśżź]+(?:[-'][A-ZĄĆĘŁŃÓŚŻŹ][a-ząćęłńóśżź]+)*$");
            if (String.IsNullOrEmpty(lastName) || !regex.IsMatch(lastName))
            {
                throw new ArgumentException("Last name must start with a capital letter and contain only letters. Compound last names are allowed.");
            }
        }

        private void ValidatePhoneNumber(string phoneNumber)
        {
            var phoneAttr = new PhoneAttribute();

            if (String.IsNullOrEmpty(phoneNumber))
            {
                throw new ArgumentException("Phone number cannot be empty.");
            }

            if (phoneNumber.Length > 15)
            {
                throw new ArgumentException("Phone number is too long.");
            }

            if (!phoneAttr.IsValid(phoneNumber))
            {
                throw new ArgumentException("Enter a valid phone number, e.g., +48 123 456 789.");
            }
        }
    }
}
