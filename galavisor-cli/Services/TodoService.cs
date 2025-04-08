using System.Text.Json;
using GalavisorCli.Models;

namespace GalavisorCli.Services;

public class TodoService
{
    private readonly string _storagePath = "todo_data.json";
    private readonly List<TodoItem> _items;
    private int _idCounter;

    public TodoService()
    {
        _items = LoadItems();
        _idCounter = _items.Count > 0 ? _items.Max(i => i.Id) + 1 : 1;
    }

    public List<TodoItem> GetAll() => _items;

    public TodoItem Add(string title)
    {
        var item = new TodoItem { Id = _idCounter++, Title = title };
        _items.Add(item);
        SaveItems();
        return item;
    }

    public bool Delete(int id)
    {
        bool removed = _items.RemoveAll(x => x.Id == id) > 0;
        if (removed) SaveItems();
        return removed;
    }

    public bool Update(int id, string? title = null, bool? done = null)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        if (item == null) return false;

        if (title != null) item.Title = title;
        if (done.HasValue) item.Done = done.Value;

        SaveItems();
        return true;
    }

    private List<TodoItem> LoadItems()
    {
        if (!File.Exists(_storagePath))
            return new List<TodoItem>();

        string json = File.ReadAllText(_storagePath);
        return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
    }

    private void SaveItems()
    {
        string json = JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_storagePath, json);
    }
}
