using MongoDB.Driver;
using ProjectEmergencyBot.CommandHandlers;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyBot.DataManagers
{
    public class EmailDataManager
    {
        internal static IMongoCollection<Email> _mail;
        public static void CreateMailbox(string character, string email, EmailIx.EmailProvider provider, string name)
        {
            string maild = email;
            switch (provider)
            {
                case EmailIx.EmailProvider.Eyefind:
                    maild += "@eyefind.info";
                    break;
                case EmailIx.EmailProvider.Snotmail:
                    maild += "@snotmail.com";
                    break;
                case EmailIx.EmailProvider.JOL:
                    maild += "@jol.com";
                    break;
                case EmailIx.EmailProvider.OMail:
                    maild += "@omail.com";
                    break;
            }
            _mail.InsertOne(new Email()
            {
                ConnectedCharacter=character,
                EmailAddress=maild,
                Name=name,
                Frozen=false,
                Incoming = new List<EmailItem>(),
                Outgoing = new List<EmailItem>()
            });
        }
        public static void CreateOrgMailbox(string org, string character, string email, string name)
        {
            string maild = email;
            _mail.InsertOne(new Email()
            {
                ConnectedCharacter = character,
                ConnectedOrganization=org,
                EmailAddress = maild,
                Name = name,
                Frozen = false,
                Incoming = new List<EmailItem>(),
                Outgoing = new List<EmailItem>()
            });
        }
        public static Email[] GetEmailsForCharacter(string character)
        {
            return _mail.Find(e => e.ConnectedCharacter == character && !e.Frozen).ToList().ToArray();
        }
        public static Email GetMailboxFromId(string id)
        {
            return _mail.Find(e => e.Id == id).FirstOrDefault();
        }
        public static Email GetMailboxFromAddress(string address)
        {
            return _mail.Find(e => e.EmailAddress == address).FirstOrDefault();
        }
        public static void UpdateEmail(Email e) {
            _mail.ReplaceOne(ex => ex.Id == e.Id, e);
        }
        public static Email SendMail(string toAddress, string fromId, string subject, string message)
        {
            var toEmail = GetMailboxFromAddress(toAddress);
            var fromEmail = GetMailboxFromId(fromId);
            toEmail.Incoming.Add(new EmailItem()
            {
                ToAddress = toEmail.EmailAddress,
                Body = message,
                Subject = subject,
                FromAddress = fromEmail.EmailAddress,
                SendTime = UserManager.Timestamp()
            });
            fromEmail.Outgoing.Add(new EmailItem()
            {
                ToAddress = toEmail.EmailAddress,
                Body = message,
                Subject = subject,
                FromAddress = fromEmail.EmailAddress,
                SendTime = UserManager.Timestamp()
            });
            UpdateEmail(toEmail);
            UpdateEmail(fromEmail);
            return toEmail;
        }
        public static Email SendMailAddressOnly(string toAddress, string fromAddress, string subject, string message)
        {
            var toEmail = GetMailboxFromAddress(toAddress);
            var fromEmail = GetMailboxFromAddress(fromAddress);
            toEmail.Incoming.Add(new EmailItem()
            {
                ToAddress = toEmail.EmailAddress,
                Body = message,
                Subject = subject,
                FromAddress = fromEmail.EmailAddress,
                SendTime = UserManager.Timestamp()
            });
            fromEmail.Outgoing.Add(new EmailItem()
            {
                ToAddress = toEmail.EmailAddress,
                Body = message,
                Subject = subject,
                FromAddress = fromEmail.EmailAddress,
                SendTime = UserManager.Timestamp()
            });
            UpdateEmail(toEmail);
            UpdateEmail(fromEmail);
            return toEmail;
        }
    }
}
