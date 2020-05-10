namespace JobScheduler.Models
{
    public class JobNode
    {
        public int JobId { get; set; }
        public int NodeId { get; set; }
        public Job Job { get; set; }
        public Node Node { get; set; }
    }
}