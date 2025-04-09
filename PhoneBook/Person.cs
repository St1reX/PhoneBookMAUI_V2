using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PhoneBook
{
    public class Person : INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private string _phoneNumber;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                FirstNameValidation(value);
                OnPropertyChanged();
                _firstName = value;
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                LastNameValidation(value);
                OnPropertyChanged();
                _lastName = value;
            }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                PhoneNumberValidation(value);
                OnPropertyChanged();
                _phoneNumber = value;
            }
        }

        public Person(string firstName, string lastName, string phoneNumber)
        {
            _firstName = firstName;
            _lastName = lastName;
            _phoneNumber = phoneNumber;
        }

        private void LastNameValidation(string lastName)
        {
            Regex regexPattern = new Regex(@"^[A-ZĄĆĘŁŃÓŚŻŹ][a-ząćęłńóśżź]+(?:[-'][A-ZĄĆĘŁŃÓŚŻŹ][a-ząćęłńóśżź]+)*$");

            if (!regexPattern.IsMatch(lastName))
            {
                throw new ArgumentException("Last name must start with a capital letter and consist only of letters. Compound last names are also allowed.");
            }
        }

        private void PhoneNumberValidation(string phoneNumber)
        {
            Regex regexPattern = new Regex(@"^(\+48)?\s?(\d{3})[\s\-]?\d{3}[\s\-]?\d{3}$");

            if (phoneNumber.Length >= 12)
            {
                throw new ArgumentException("Phone number must be 10 digits long.");
            }

            if (!regexPattern.IsMatch(phoneNumber))
            {
                throw new ArgumentException("Phone number must be in format: 123 456 789 or 123-456-789 or 123-456-789 or +48 123 456 789.");
            }
        }

        private void FirstNameValidation(string firstName)
        {
            Regex regexPattern = new Regex(@"^[A-ZĄĆĘŁŃÓŚŻŹ][a-ząćęłńóśżź]+(?:\s[A-ZĄĆĘŁŃÓŚŻŹ][a-ząćęłńóśżź]+)*$");

            if (!regexPattern.IsMatch(firstName))
            {
                throw new ArgumentException("First name must start with a capital letter and consist only of letters. Compound first names are also allowed.");
            }
        }
    }
}
