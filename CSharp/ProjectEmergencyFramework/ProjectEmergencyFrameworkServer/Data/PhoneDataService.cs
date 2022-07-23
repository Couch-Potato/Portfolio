using MongoDB.Driver;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public class PhoneDataService
    {
        internal static IMongoCollection<Phone> _phone;
        internal static IMongoCollection<Twat> _twatter;

        public static Phone GetPhoneById(string number)
        {
            return _phone.Find(p => p.Id == number).FirstOrDefault();
        }
        public static Phone GetPhoneByNumber(string number)
        {
            return _phone.Find(p => p.PhoneNumber == number).FirstOrDefault();
        }
        public static void UpdatePhone(Phone p)
        {
            _phone.ReplaceOne(phx => p.Id == phx.Id, p);
        }
        public static Character GetPhoneOwner(Phone p)
        {
            return PlayerDataService.GetCharacter(p.Owner);
        } 
        public static void CreatePhoneAndBind(Character c)
        {
            var phone = new Phone();
            phone.PhoneNumber = ServerCharacterHandlers.RandomString(3) + "-" + ServerCharacterHandlers.RandomString(4);
            phone.Owner = c.Id;
            phone.Conversations = new List<TextConversation>();
            phone.Contacts = new List<Contact>();

            _phone.InsertOne(phone);
            c.PhoneId = phone.Id;
            PlayerDataService.UpdateCharacter(c);
        }
        public static string ResolveContact(string number, Phone owner)
        {
            foreach (var contact in owner.Contacts)
            {
                if (contact.PhoneNumber == number) return contact.FirstName +" "+ contact.LastName;
            }
            return number;
        }
        public static List<TextMessageResponse> GetRecentMessagesForPhone(Phone p)
        {
            List<TextMessageResponse> imessageList = new List<TextMessageResponse>();
            foreach (var convo in p.Conversations)
            {
                imessageList.Add(new TextMessageResponse()
                {
                    Author = ResolveContact(convo.Recipient, p),
                    Message = convo.Messages[convo.Messages.Count - 1].Message,
                    PhoneNumber = convo.Recipient
                });
            }
            return imessageList;
        }
        public static List<TextMessage> GetMessagesInConversation(Phone owner, string number)
        {
            foreach (var convo in owner.Conversations)
            {
                if (convo.Recipient == number) return convo.Messages;
            }
            return new List<TextMessage>();
        }
        public static void CreateContact(Phone owner, string fn, string ln, string number)
        {
            owner.Contacts.Add(new Contact()
            {
                FirstName = fn,
                LastName = ln,
                PhoneNumber = number
            });
            UpdatePhone(owner);
        }
        public static void SendTextMessage(Phone owner, string recipient, string text)
        {
            foreach (var convo in owner.Conversations)
            {
                if (convo.Recipient == recipient) convo.Messages.Add(new TextMessage() { Author = owner.PhoneNumber, Message = text });
                UpdatePhone(owner);
            }
            var recPhone = GetPhoneByNumber(recipient);
            foreach (var convo in recPhone.Conversations)
            {
                if (convo.Recipient == recipient) convo.Messages.Add(new TextMessage() { Author = owner.PhoneNumber, Message = text });
                UpdatePhone(recPhone);
            }
        }
        public static List<Contact> GetContacts(Phone owner)
        {
            return owner.Contacts;
        }
        public static List<Twat> GetTwats()
        {
            return _twatter.Find(tw => tw.Id != null).ToList();
        }
        public static void SendTwat(Character owner, string message)
        {
            _twatter.InsertOne(new Twat()
            {
                Message=message,
                SenderName = owner.FirstName + " " + owner.LastName,
                SenderUsername = owner.FirstName + owner.LastName
            });
        }
    }
}
