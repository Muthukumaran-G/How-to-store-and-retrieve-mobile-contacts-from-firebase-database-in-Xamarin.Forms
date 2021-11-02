# How-to-store-and-retrieve-mobile-contacts-from-firebase-database-in-Xamarin.Forms

Firebase offers a realtime database to store data on the go. It is the blooming backend platform which offers various toolkits to make the data manipulation easier and safer for applications. Also firebase offers free plan(Spark plan) which covers almost all the basic needs for a small scale app development. You can check with the below link for detailed description of the firebase plans.

https://firebase.google.com/pricing

## Firebase DB creation

Now moving on to creating a DB in firebase. Login to the firebase console using the below link.

https://console.firebase.google.com/

Create a new project by clicking the 'Add Project' button.

Enter the details of the project and you will be proceeded to your project dashboard. You can see the Realtime Database on the left.
![image](https://user-images.githubusercontent.com/26808947/139808972-9178673a-3491-4a89-95fe-a959a346506d.png)

Click 'Create Database' and you will be getting a popup related to DB begin used for Test or Locked environment. Create using 'Locked' environment. We can alter the rule set based on our requirement. For using DB without any authentication set the below rules.

![image](https://user-images.githubusercontent.com/26808947/139809317-a07dbf88-d7af-43e1-98ff-5957a11f0d5a.png)

You will get a unique link for this DB in the 'Data' section of the project which is similar to the client connection link to connect with the front end.

## Connecting with Xamarin

DB nodes will be created based on the string value(resourceString) passed in the Child method. So when trying to perform CRUD actions, note that the resourceString is properly mentioned.

### CREATE

Create a connection with the DB using the FireBaseClient by passing the unique link generated in 'Data' section of the project dashboard. Adding a new value into the DB requires a string value instead of direct model object. So the model value should be serialized before posting into the DB.

### READ

Have an unique identifier for each objects stored in the DB. In my case, I have considered a GUID string which generates a unique value for each model objects created. You can check for this ID with the DB collection converted into list objects.

### UPDATE

Each DB object holds a unique key rather the existing model objects. It will be automatically generated when pushing the serialized value into the DB. You can check for this Key value and perform put action to update the existing value.

### REMOVE

Deleting an item from DB requires a unique key similar to the read and update actions. However you can either use the object key or the unique object value to check for the object to be deleted from DB.

```
public class FirebaseHelper
{
    FirebaseClient firebase;
    public FirebaseHelper()
    {
        firebase = new FirebaseClient("/* Your unique DB link */");
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
```

## Fetching mobile contacts

Accessing the device contacts directory had been made easier with Xamarin.Forms.Essentials. It has a static class 'Contacts' to open directory and get the selected contact information. 

```
private async void ExecuteAdd(object obj)
{
    var contact = await Contacts.PickContactAsync();
    if (contact == null || contact.DisplayName == null || contact.Phones[0].PhoneNumber == null)
        return;

    await FirebaseHelper.AddContact(new Guid(), contact.DisplayName, contact.Phones[0].PhoneNumber);
}
```

Make sure to configure the access permission in all the platform projects which can be referred from below link.

https://docs.microsoft.com/en-us/xamarin/essentials/contacts

That would be all ;)
