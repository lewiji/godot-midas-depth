using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;

namespace GodotMidasDepth;

public partial class Config {
    ConfigFile? _configFile;
    const string ConfigFilePath = "user://config.cfg";
    const string ConfigFileSection = "godot-midas";

    public enum ConfigKey {
        LastPath,
        AutoProcessDepth
    }

    readonly Dictionary<ConfigKey, string> _configKeys = new() {
        {ConfigKey.LastPath, "lastPath"},
        {ConfigKey.AutoProcessDepth, "autoProcessDepth"},
    };

    public Config() {
        _configFile = new ConfigFile();
        if (_configFile.Load(ConfigFilePath) == Error.Ok) return;
        
        _configFile.SetValue(ConfigFileSection, GetKey(ConfigKey.LastPath), "");
        _configFile.SetValue(ConfigFileSection, GetKey(ConfigKey.AutoProcessDepth), true);
        
        _configFile.Save(ConfigFilePath);
    }

    string? GetKey(ConfigKey key) => _configKeys.TryGetValue(key, out var value) ? value : null;

    public void SetValue(ConfigKey key, object? value) {
        if (GetKey(key) is not { } keyString) return;
        
        _configFile?.SetValue(ConfigFileSection, keyString, value);
        _configFile?.Save(ConfigFilePath);
    }
    
    public object? GetValue(ConfigKey key) => GetKey(key) is not { } keyString ? null : _configFile?.GetValue(ConfigFileSection, keyString);

}