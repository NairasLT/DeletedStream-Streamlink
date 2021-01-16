using Newtonsoft.Json;
using System;
using System.IO;

class Config<T>
{
    public string Path;
    public Formatting JsonFormatting;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">Path to Configuration File</param>
    /// <param name="formatting">Json Formmating</param>
    public Config(string path, Formatting formatting = Formatting.Indented)
    {
        Path = path;
        JsonFormatting = formatting;
    }
    /// <summary>
    /// Writes to the file specified object, If File Does not Exist creates it.
    /// </summary>
    /// <param name="data">The object to be written</param>
    /// <returns>If Succeeds returns true</returns>
    public bool Write(T data)
    {
        try { File.WriteAllText(Path, JsonConvert.SerializeObject(data, JsonFormatting)); return true; } catch (Exception) { return false; }
    }
    /// <summary>
    /// Reads the Configuration file and returns the saved object
    /// </summary>
    /// <returns>If Succeeds returns true</returns>
    public T Read()
    {
        try { return JsonConvert.DeserializeObject<T>(File.ReadAllText(Path)); } catch (Exception) { return default(T); }
    }
    /// <summary>
    /// Creates the file if the file is not already created;
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool Create(T type) 
    {
        if (File.Exists(Path)) return true;

        if (Write(type)) return true; else return false;
    }
    /// <summary>
    /// Deletes the configuration file.
    /// </summary>
    /// <returns>If Succeeds returns true</returns>
    public bool Delete()
    {
        try { File.Delete(Path); return true; } catch (Exception) { return false; }
    }
}
