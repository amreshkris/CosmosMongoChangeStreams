using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WorkflowAggregation
{
    
    public class Tpdoc
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public object _id { get; set; }

        public string tpDocNumber { get; set; }

        #nullable enable
        public List<TransportDocumentStatus>? status { get; set; }

        #nullable disable
    }


    public class TransportDocumentStatus
    {
        public string statusId { get; set; }
        public string status { get; set; }
        public DateTime timeStamp { get; set; }

        #nullable enable
        public DateTime? gcssEventTime { get; set; }
        public string? userId { get; set; }

        #nullable enable
        public List<ShippingInstructionInfo>? siNumbers { get; set; }

        #nullable disable
    }

    public class ShippingInstructionInfo
    {
        public string siNumber { get; set; }

        #nullable enable
        public DateTime? timeStamp { get; set; }

        #nullable disable

    }
}