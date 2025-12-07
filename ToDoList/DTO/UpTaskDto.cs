namespace ToDoList.DTO
{
    public class UpTaskDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "ToDo";
        public DateTime DueBy { get; set; }
    }
}
