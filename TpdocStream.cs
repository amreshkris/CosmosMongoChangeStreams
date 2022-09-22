namespace WorkflowAggregation
{    
    public class TpdocStream
    {
        public string tpDocNumber {get ; set;}

        public string Activity{ get; set; }

        public DateTime ReceivedTime{ get; set; }

        public string  Channel{ get; set;}

        public string Status{ get; set; }

        public DateTime CompletedTime{ get; set; }

    }
}