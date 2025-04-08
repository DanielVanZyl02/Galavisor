namespace GalavisorApi.Models;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public bool Done { get; set; } = false;
}
