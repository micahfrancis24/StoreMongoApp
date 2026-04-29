using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StoreMongoApp.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("category")]
        public string Category { get; set; } = null!;

        [BsonElement("inStock")]
        public bool InStock { get; set; }

        [BsonElement("tags")]
        public List<string>? Tags { get; set; }

        [BsonElement("specifications")]
        public ProductSpecifications? Specifications { get; set; }

        [BsonElement("reviews")]
        public List<ProductReview>? Reviews { get; set; }

        [BsonElement("discount")]
        public ProductDiscount? Discount { get; set; }
    }

    public class ProductSpecifications
    {
        [BsonElement("color")]
        public string? Color { get; set; }

        [BsonElement("connection")]
        public string? Connection { get; set; }

        [BsonElement("batteryLife")]
        public string? BatteryLife { get; set; }

        [BsonElement("brightnessLevels")]
        public int? BrightnessLevels { get; set; }

        [BsonElement("usbCharging")]
        public bool? UsbCharging { get; set; }

        [BsonElement("size")]
        public string? Size { get; set; }

        [BsonElement("waterResistant")]
        public bool? WaterResistant { get; set; }
    }

    public class ProductReview
    {
        [BsonElement("user")]
        public string? User { get; set; }

        [BsonElement("rating")]
        public int Rating { get; set; }

        [BsonElement("comment")]
        public string? Comment { get; set; }
    }

    public class ProductDiscount
    {
        [BsonElement("active")]
        public bool Active { get; set; }

        [BsonElement("percentage")]
        public int Percentage { get; set; }
    }
}