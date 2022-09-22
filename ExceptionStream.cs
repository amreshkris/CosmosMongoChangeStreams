namespace WorkflowAggregation
{
    public class StreamException
    {
        public string TransportDocumentNumber { get; set; }

        public string Activity { get; set; }

        public DateTime ReceivedTime { get; set; }

        public string Channel { get; set; }

        public string Status { get; set; }

        public string CompletedTime { get; set; }
    }
}