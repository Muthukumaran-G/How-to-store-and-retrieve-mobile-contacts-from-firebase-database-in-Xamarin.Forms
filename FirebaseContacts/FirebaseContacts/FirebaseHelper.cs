using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseContacts
{
    public class FirebaseHelper
    {
        FirebaseClient firebase;
        public FirebaseHelper()
        {
            firebase = new FirebaseClient("https://fir-contacts-97cf2-default-rtdb.firebaseio.com/");
        }

        public async Task<List<ContactInfo>> FetchContacts()
        {

            return (await firebase
              .Child("ContactInfo")
              .OnceAsync<ContactInfo>()).Select(item => new ContactInfo
              {
                  Name = item.Object.Name,
                  Number = item.Object.Number,
                  ID = item.Object.ID
              }).ToList();
        }

        public async Task AddContact(Guid id, string name, string number)
        {
            var recepientValue = new ContactInfo() { Name = name, Number = number, ID = id };
            string jsonString = JsonConvert.SerializeObject(recepientValue);
            await firebase
              .Child("ContactInfo")
              .PostAsync(jsonString);
        }

        public async Task<ContactInfo> GetContact(Guid id)
        {
            var allPersons = await FetchContacts();
            await firebase
              .Child("ContactInfo")
              .OnceAsync<ContactInfo>();
            return allPersons.Where(a => a.ID == id).FirstOrDefault();
        }

        public async Task UpdateContact(Guid id, string name, string number)
        {
            var toUpdatePerson = (await firebase
              .Child("ContactInfo")
              .OnceAsync<ContactInfo>()).Where(a => a.Object.ID == id).FirstOrDefault();

            await firebase
              .Child("ContactInfo")
              .Child(toUpdatePerson.Key)
              .PutAsync(new ContactInfo() { ID = id, Name = name, Number = number });
        }

        public async Task DeleteContact(Guid id)
        {
            var toDeletePerson = (await firebase
              .Child("ContactInfo")
              .OnceAsync<ContactInfo>()).Where(a => a.Object.ID == id).FirstOrDefault();
            await firebase.Child("ContactInfo").Child(toDeletePerson.Key).DeleteAsync();
        }
    }
}
