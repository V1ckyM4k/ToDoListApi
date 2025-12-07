namespace ToDoList.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime DueBy { get; set; }
        public DateTime CreatedAt {  get; set; }

    }
}
