using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using MongoDB.Bson.Serialization;

namespace WorkflowAggregation
{
    class Program
    {
        private static string sourceEndpoint = "";
        private static string sourceKey = "";

        private static string destinationEndpoint = "";
        private static string destinationKey = "";
        static void Main(string[] args)
        {
            try
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                  .AddJsonFile("appSettings.json")
                  .Build();

                Program.sourceEndpoint = configuration["SourceCosmosDB"];
                if (string.IsNullOrEmpty(sourceEndpoint))
                {
                    throw new ArgumentNullException
                    ("Please specify a valid endpoint in the appSettings.json");
                }

                Program.sourceKey = configuration["SourceAuthorizationKeyPP"];
                if (string.IsNullOrEmpty(sourceKey) || string.Equals(sourceKey, "Super secret key"))
                {
                    throw new ArgumentException("Please specify a valid AuthorizationKey in the appSettings.json");
                }

                Program.destinationEndpoint = configuration["DestinationCosmosDB"];
                if (string.IsNullOrEmpty(destinationEndpoint))
                {
                    throw new ArgumentNullException
                    ("Please specify a valid endpoint in the appSettings.json");
                }

                Program.destinationKey = configuration["DestinationAuthorizationKey"];
                if (string.IsNullOrEmpty(destinationKey) || string.Equals(destinationKey, "Super secret key"))
                {
                    throw new ArgumentException("Please specify a valid AuthorizationKey in the appSettings.json");
                }

                var sourceDbClient = new MongoClient(Program.sourceKey);
                IMongoDatabase db = sourceDbClient.GetDatabase("export-doc-workflow-si-db");

                var workflowEventsColl = db.GetCollection<BsonDocument>("ddw-tpdoc-workflow-events-coll");
                var tpdocStateColl = db.GetCollection<BsonDocument>("ddw-si-tpdoc-state-coll");

                var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                    .Match(change => change.OperationType == ChangeStreamOperationType.Insert || change.OperationType == ChangeStreamOperationType.Update || change.OperationType == ChangeStreamOperationType.Replace)
                    .AppendStage<ChangeStreamDocument<BsonDocument>, ChangeStreamDocument<BsonDocument>, BsonDocument>(
                    "{ $project: { '_id': 1, 'fullDocument': 1, 'ns': 1, 'documentKey': 1 }}");

                var options = new ChangeStreamOptions
                {
                    FullDocument = ChangeStreamFullDocumentOption.UpdateLookup,
                    StartAtOperationTime = new BsonTimestamp(1654043810)
                };

                // var workflowEventsEnumerator = workflowEventsColl.Watch(pipeline, options).ToEnumerable().GetEnumerator();

                // while (workflowEventsEnumerator.MoveNext())
                // {                    
                //     Console.WriteLine(workflowEventsEnumerator.Current);
                //     dynamic completeExceptionInfo = workflowEventsEnumerator.Current;
                //     var feedDocument = completeExceptionInfo["fullDocument"] as BsonDocument;
                //     var exceptionInfo =  BsonSerializer.Deserialize<Root>(feedDocument);                    
                //     Console.ReadKey();
                // }
                // workflowEventsEnumerator.Dispose();


                var tpDocEventEnumerator = tpdocStateColl.Watch(pipeline, options).ToEnumerable().GetEnumerator();

                while (tpDocEventEnumerator.MoveNext())
                {                    
                    Console.WriteLine(tpDocEventEnumerator.Current);
                    dynamic tpDocSiStateInfo = tpDocEventEnumerator.Current;
                    var feedDocument = tpDocSiStateInfo["fullDocument"] as BsonDocument;
                    var exceptionInfo =  BsonSerializer.Deserialize<Tpdoc>(feedDocument);
                    StreamTransportInformation(exceptionInfo);                  
                    Console.ReadKey();
                }
                tpDocEventEnumerator.Dispose();

            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private static void StreamTransportInformation(Tpdoc TpdocInfo)
        {
            foreach (var tpdocState in TpdocInfo.status)
            {
                switch (tpdocState.status)
                {
                    case "TPDOC_IVC_COMPLETED":
                    var todocStream = new TpdocStream()
                    {
                        tpDocNumber = TpdocInfo.tpDocNumber,
                        Activity = "FreshSI",
                        ReceivedTime = GetSIReceivedTime(TpdocInfo),
                        CompletedTime = tpdocState.timeStamp,
                        Status = "IVCCompleted"
                    };
                    break;
                    default:
                    break;
                }
            }
           
        }

        private static DateTime GetSIReceivedTime(Tpdoc exceptionInfo)
        {
           DateTime tpDocReceivedTime = new DateTime(1,1,1);
           TransportDocumentStatus siReceived = exceptionInfo.status
                                    .Where(x =>x.status == "TPDOC_INSTRUCTIONS_RECEIVED")
                                    .FirstOrDefault();
                                   
            IEnumerable<ShippingInstructionInfo> shippingInstructionInfos = siReceived.siNumbers;
            if(shippingInstructionInfos is not null)
            {
                tpDocReceivedTime = shippingInstructionInfos.OrderByDescending(x=> x.timeStamp.Value)
                                    .FirstOrDefault()
                                    .timeStamp.Value; 
            }
            else 
            {
                tpDocReceivedTime = siReceived.timeStamp;
            }
           return tpDocReceivedTime;
        }
    }
}