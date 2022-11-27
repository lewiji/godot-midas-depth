using Godot;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes; 

public partial class ImagePicker : MarginContainer {
    [OnReadyGet("%FileDialog")] FileDialog _fileDialog = default!;
    public string InitialPath = "";

    [Signal]
    public delegate void ImageLoaded(Image image);
    
    [Signal]
    public delegate void PathSelected(string path);

    [OnReady]
    void ConnectFileDialogSignals() {
        if (Main.Config.GetValue(Config.ConfigKey.LastPath) is string lastPath) {
            InitialPath = lastPath;
        }
        _fileDialog.Connect("file_selected", this, nameof(OnFileSelected));
    }

    public void Open() {
        if (InitialPath != "") {
            _fileDialog.CurrentDir = InitialPath;
        }
        _fileDialog.PopupCentered();
    }

    void OnFileSelected(string path) {
        var image = new Image();
        if (image.Load(path) == Error.Ok) {
            EmitSignal(nameof(ImageLoaded), image);
        }

        EmitSignal(nameof(PathSelected), path);
        Main.Config.SetValue(Config.ConfigKey.LastPath, path.GetBaseDir());
    }
}