using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FirebaseContacts
{
    public class ViewModel:INotifyPropertyChanged
    {
        public ObservableCollection<ContactInfo> contactsCollection;

        public object collectionViewSelectedItem;
        public ObservableCollection<ContactInfo> ContactsCollection
        { 
            get
            {
                return contactsCollection;
            }
            set
            {
                contactsCollection = value;
                RaisePropertyChanged("ContactsCollection");
            }
        }

        public Command AddCommand { get; set; }
        public Command RemoveCommand { get; set; }
        public Command UpdateCommand { get; set; }
        public Command FetchCommand { get; set; }

        public object CollectionViewSelectedItem
        {
            get
            {
                return collectionViewSelectedItem;
            }
            set
            {
                collectionViewSelectedItem = value;
                RaisePropertyChanged("CollectionViewSelectedItem");
            }
        }
        FirebaseHelper FirebaseHelper = new FirebaseHelper();

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            AddCommand = new Command(ExecuteAdd);
            RemoveCommand = new Command(ExecuteRemove);
            UpdateCommand = new Command(ExecuteUpdate);
            FetchCommand = new Command(ExecuteFetch);
        }

        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private async void ExecuteFetch(object obj)
        {
            var contacts = await FirebaseHelper.FetchContacts();
            ContactsCollection = new ObservableCollection<ContactInfo>(contacts);
        }

        private async void ExecuteUpdate(object obj)
        {
            if (CollectionViewSelectedItem != null)
            {
                var updateContact = CollectionViewSelectedItem as ContactInfo;
                if (updateContact.Name.Contains("/"))
                    return;

                await FirebaseHelper.UpdateContact(updateContact.ID, "Mr/Mrs." + updateContact.Name, "+11" + updateContact.Number);
            }
        }

        private async void ExecuteRemove(object obj)
        {
            if (CollectionViewSelectedItem != null)
            {
                var deleteContact = CollectionViewSelectedItem as ContactInfo;
                await FirebaseHelper.DeleteContact(deleteContact.ID);
            }
        }

        private async void ExecuteAdd(object obj)
        {
            var contact = await Contacts.PickContactAsync();
            if (contact == null || contact.DisplayName == null || contact.Phones[0].PhoneNumber == null)
                return;

            await FirebaseHelper.AddContact(new Guid(), contact.DisplayName, contact.Phones[0].PhoneNumber);
        }
    }
}
