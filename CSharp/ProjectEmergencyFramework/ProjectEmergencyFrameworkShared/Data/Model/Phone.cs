using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class Phone
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string PhoneNumber { get; set; }
        public List<TextConversation> Conversations { get; set; }
        public List<Contact> Contacts { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string Owner { get; set; }
    }
    public class Contact
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class TextConversation
    {
        public string Recipient { get; set; }
        public List<TextMessage> Messages { get; set; }
    }
    public class TextMessage
    {
        public string Author { get; set; }
        public string Message { get; set; }
    }
    public class TextMessageResponse
    {
        public string Author { get; set; }
        public string Message { get; set; }
        public string PhoneNumber { get; set; }
    }
}
