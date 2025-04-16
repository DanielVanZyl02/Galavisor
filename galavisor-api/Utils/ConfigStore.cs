using System.Text.Json;

namespace GalavisorApi.Utils;

public static class ConfigStore
{
    private static readonly string ConfigFile = "server_config.json";
    private static readonly JsonSerializerOptions JsonSerializerOpt = new() { WriteIndented = true };
    private static Dictionary<string, string> _config = Load();

    private static Dictionary<string, string> Load()
    {
        try
        {
            if (!File.Exists(ConfigFile)){
                return [];
            } else {
                var Json = File.ReadAllText(ConfigFile);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(Json)
                    ?? [];
            }
        }
        catch (Exception Error)
        {
            Console.WriteLine($"Failed to load config: {Error.Message}");
            return [];
        }
    }

    private static void Save()
    {
        var Json = JsonSerializer.Serialize(_config, JsonSerializerOpt);
        File.WriteAllText(ConfigFile, Json);
    }

    public static void Set(string Key, string Value)
    {
        _config[Key] = Value;
        Save();
    }

    public static string Get(string Key)
    {
        if(_config.TryGetValue(Key, out var Value)){
            return Value;
        } else{
            throw new Exception($"Env not found: {Key}");
        }
    }

    public static void Remove(string Key)
    {
        if(Exists(Key) && _config.Remove(Key)){
            Save();
        } else{
            // we do nothing as key most likely doesn't exist
        }
    }

    public static void Clear()
    {
        _config.Clear();
        Save();
    }

    public static bool Exists(string Key)
    {
        try{
            return Get(Key) != "";
        } catch
        {
            return false;
        }
    }
}
