using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WorkflowAggregation
{
    public class Exception
    {
        public string exceptionId { get; set; }
        public ProcessInfo processInfo { get; set; }
        public ExceptionInfo exceptionInfo { get; set; }
    }

     public class ExceptionInfo
    {
        public string exceptionType { get; set; }
        public string exceptionSubType { get; set; }
        public string exceptionLevel { get; set; }
        public string source { get; set; }
        public string reason { get; set; }
        public string responseCode { get; set; }
        public string txnId { get; set; }
        public DateTime exceptionTime { get; set; }
    }

    public class LatestSiInfo
    {
        public string siNumber { get; set; }
        public DateTime siSubmissionTime { get; set; }
        public string siSubmittedChannel { get; set; }
    }

    public class ProcessInfo
    {
        public string taskId { get; set; }
        public string assignmentGroup { get; set; }
        public string assignmentStatus { get; set; }
        public DateTime assignmentStatusTime { get; set; }
        public string taskStatus { get; set; }
        public string taskStatusTime { get; set; }
        public string assignedTo { get; set; }
        public string assignedBy { get; set; }
        public string modifiedBy { get; set; }
        public string workflowDefinitionId { get; set; }
    }

    public class Root
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public object _id { get; set; }
        public string processInstanceId { get; set; }
        public string shipmentType { get; set; }
        public string shipmentSubType { get; set; }
        public TpDocInfo tpDocInfo { get; set; }
        public List<Exception> exceptions { get; set; }
        public string _class { get; set; }
    }

    public class TpDocInfo
    {
        public string tpDocNumber { get; set; }
        public string motherShipmentNumber { get; set; }

        [BsonElement("operator")]
        public string @operator { get; set; }
        public string businessUnit { get; set; }
        //public string productDelivery { get; set; }
        public string placeOfReceiptCode { get; set; }
        public string placeOfReceipt { get; set; }
        public string placeOfDeliveryCode { get; set; }
        public string placeOfDelivery { get; set; }
        public string bookedByCode { get; set; }
        public string priceOwnerCode { get; set; }
        public LatestSiInfo latestSiInfo { get; set; }
    }
}
