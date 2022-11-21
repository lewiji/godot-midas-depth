using Godot;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes; 

public partial class ImagePicker : MarginContainer {
    [OnReadyGet("%FileDialog")] FileDialog _fileDialog = default!;
    [OnReadyGet("%PreviewTexture")] TextureRect _previewTextureRect = default!;
    [OnReadyGet("%CanvasLayer")] CanvasLayer _canvasLayer = default!;
    Tree? _fileTree;
    LineEdit? _fileDirectoryLineEdit;
    
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
        _fileDialog.Connect("popup_hide", this, nameof(OnFileDialogClosed));
        var vbox = _fileDialog.GetVbox();
        _fileTree = vbox.GetChild(2).GetChild(0) as Tree;
        _fileDirectoryLineEdit = (LineEdit) _fileDialog.GetVbox().GetChild(0).GetChild(3);
        _fileTree?.Connect("item_selected", this, nameof(OnTreeItemSelected));
    }

    void OnTreeItemSelected() {
	    if (_fileTree == null) return;
	    var item = _fileTree.GetSelected();
	    var itemText = item.GetText(0);
	    var dirPath = _fileDirectoryLineEdit?.Text;
	    var filePath = $"{dirPath}/{itemText}";
	    var file = new File();

	    if (filePath.GetFile() != "" && file.FileExists(filePath)) {
		    using var image = new Image();

		    if (image.Load(filePath) == Error.Ok) {
			    var tex = new ImageTexture();
			    tex.CreateFromImage(image);
			    _previewTextureRect.Texture = tex;
		    }
	    }
    }

    void OnFileDialogClosed() {
	    _canvasLayer.Visible = false;
    }

    public void Open() {
        if (InitialPath != "") {
            _fileDialog.CurrentDir = InitialPath;
        }
        _fileDialog.Popup_();
        _canvasLayer.Visible = true;
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