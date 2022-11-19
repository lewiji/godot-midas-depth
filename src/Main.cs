using Godot;
using Godot.Collections;
using GodotMidasDepth.Inference;
using GodotMidasDepth.Nodes;
using GodotOnReady.Attributes;

namespace GodotMidasDepth; 

public partial class Main : Node {
    [OnReadyGet("%PreviewPanel")] PreviewPanel _previewPanel = default!;
    [OnReadyGet("%ImagePicker")] ImagePicker _imagePicker = default!;
    InferImageDepth? _inferImageDepth;
    const string ConfigFilePath = "user://config.cfg";
    ConfigFile? _configFile;

    [OnReady]
    void ProcessConfig() {
        _configFile = new ConfigFile();
        if (_configFile.Load(ConfigFilePath) != Error.Ok) {
            _configFile.SetValue("file", "lastPath", "");
            _configFile.Save(ConfigFilePath);
        }

        _imagePicker.InitialPath = (string)_configFile.GetValue("file", "lastPath");
    }

    void SaveLastPathToConfig(string path) {
        _configFile?.SetValue("file", "lastPath", path.GetBaseDir());
        _configFile?.Save(ConfigFilePath);
    }

    [OnReady]
    void LoadInferenceModel() {
        _inferImageDepth = new InferImageDepth();
        _inferImageDepth.LoadModel(InferImageDepth.DefaultModelPath);
    }

    [OnReady]
    void ConnectSignals() {
        _previewPanel.Connect(nameof(PreviewPanel.LoadImageRequested), _imagePicker, nameof(ImagePicker.Open));
        _imagePicker.Connect(nameof(ImagePicker.ImageLoaded), _previewPanel, nameof(PreviewPanel.SetPreviewImage));
        _imagePicker.Connect(nameof(ImagePicker.PathSelected), _previewPanel, nameof(PreviewPanel.SetPathLabel));
        _imagePicker.Connect(nameof(ImagePicker.PathSelected), this, nameof(SaveLastPathToConfig));
        _previewPanel.Connect(nameof(PreviewPanel.ProcessDepthRequested), this, nameof(OnProcessDepthRequested));
    }

    void OnProcessDepthRequested(Image image) {
        CallDeferred(nameof(RunModel), image);
    }

    void RunModel(Image image) {
        var output = _inferImageDepth.Run(image);
        if (output != null) {
            _previewPanel.SetResultImage(output);
            _previewPanel.SetSprite3dImage();
        }
    }
}