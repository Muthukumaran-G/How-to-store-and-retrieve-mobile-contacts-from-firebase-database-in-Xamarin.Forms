using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace FirebaseContacts
{
    public class ContactInfo : INotifyPropertyChanged
    {
        private string name;
        private string number;
        private Guid id;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                RaisePropertyChanged();
            }
        }

        public string Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
                RaisePropertyChanged();
            }
        }

        public Guid ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propName ="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
