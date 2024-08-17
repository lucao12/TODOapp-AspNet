namespace Teste.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public bool isCompleted { get; set; }
        public DateTime DateLimit { get; set; }
        public int UserId { get; set; }
    }
}
