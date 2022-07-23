using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class Character
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string AttachedPlayerId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PhoneId { get; set; }

        public string FirstName { get; set; }

        public string DMVPhoto { get; set; }
        public string LastName { get; set; }
        public DateOfBirth DOB { get; set; }
        public string WelfareId { get; set; }
        public ClothingData Clothing { get; set; }
        public CharacterPhysicalData Physical { get; set; }
        public string DriversLicenseId { get; set; }
        public bool IsDead { get; set; }
        public string NetworkId { get; set; }
        public bool IsOnline { get; set; }
        public float CashOnHand { get; set; }

        // Todo: add system for locking in / locking out when modifying data such as to do it a lot more safely.

        public List<InventoryItem> Inventory { get; set; }

        public bool IsWanted { get; set; } = false;

        public string WantedReason { get; set; }

        public bool IsIncarcerated { get; set; }
        public string IncarcerationRecord { get; set; }

        public bool IsMarkedDeceased { get; set; }
        public string DeathCertificate { get; set; }
    }
    public class CharacterPhysicalData
    {
        public int Torso { get; set; }
        public int Sex { get; set; }
        public int BeardId { get; set; }
        public float BeardOpacity { get; set; }
        public int BeardColorId { get; set; }

        public int AgeingStyleId { get; set; }
        public float AgeingStyleOpacity { get; set;}

        public int SunDamageStyleId { get; set; }
        public float SunDamageOpacity { get; set; }

        public int MoleStyleId { get; set; }
        public float MoleOpacity { get; set; }
        public int EyebrowStyleId { get; set; }
        public float EyebrowOpacity { get; set;}
        public int EyebrowColor { get; set; }
        public int HairId { get; set; }
        public int HairColorId { get; set; }
        public int HeadId { get; set; }


        public List<float> FaceFeaturesByIndex = new List<float>(20);

        public List<TattooItem> Tattoos = new List<TattooItem>();
    }
    // Backup in case other plan does not work
    public class FaceFeatures
    {
        public float NoseWidth { get; set; }
        public float NosePeak { get; set; }
        public float NoseLength { get; set; }
        public float NoseBoneCurve { get; set; }
        public float NoseTip { get; set; }
        public float NoseTwist { get; set; }

        public float EyebrowVertical { get; set; }
        public float EyebrowHorizontal { get; set; }
        public float Cheekbones { get; set; }
        public float CheekSideways { get; set; }
        public float CheekBoneWitdh { get; set; }
        public float EyeOpening { get; set; }
        public float LipThickness { get; set; }
        public float JawboneWidth { get; set; }
        public float JawboneShape { get; set; }

        public float ChinBone { get; set; }
        public float ChinBoneLength { get; set; }
        public float ChinBoneShape { get; set; }
        public float ChinHole { get; set; }

        public float Neckthickness { get; set; }
    }
    public class DrawableItem
    {
        public int Drawable { get; set; }
        public int Texture { get; set; }
    }
    public class ClothingData
    {
        public DrawableItem Shirt { get; set; }
        public DrawableItem Undershirt { get; set; }
        public DrawableItem Pants { get; set; }
        public DrawableItem Shoes { get; set; }
    }
    public class TattooItem
    {
        public string Name { get; set; }
        public string CollectionName { get; set; }
        public string ZoneName { get; set; }
    }
    public class DateOfBirth
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

    }
    public class CharacterInit
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOfBirth DOB { get; set; }
        public ClothingData Clothing { get; set; }
        public CharacterPhysicalData Physical { get; set; }
        public string AttachedPlayerId { get; set; }

        public string DMVPhoto { get; set; }
    }
}
