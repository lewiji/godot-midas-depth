using Godot;


namespace GodotMidasDepth.Nodes; 

public partial class ImagePicker : Control {
    /* "%FileDialog" */ [Export] public FileDialog FileDialog = default!;
    /* "%PreviewTexture" */ [Export] public TextureRect PreviewTextureRect = default!;
    /* "%CanvasLayer" */ [Export] public CanvasLayer CanvasLayer = default!;
    /* "%BackgroundContainer" */ [Export] public Control Bg = default!;
    
    Tree? _fileTree;
    LineEdit? _fileDirectoryLineEdit;
    
    public string InitialPath = "";

    [Signal]
    public delegate void ImageLoadedEventHandler(Image image);
    
    [Signal]
    public delegate void PathSelectedEventHandler(string path);

    public override void _Ready()
    {
	    ConnectFileDialogSignals();
    }
	

    void ConnectFileDialogSignals() {
        if (Main.Config.GetValue(Config.ConfigKey.LastPath) is string lastPath) {
            InitialPath = lastPath;
        }
        FileDialog.FileSelected += OnFileSelected;
        FileDialog.CloseRequested += ResetVisibility;
        FileDialog.FileSelected += (_) => ResetVisibility();
        
        GetInternalComponentsFromFileDialog();
        ResetVisibility();
    }

    void GetInternalComponentsFromFileDialog() {
	    var vbox = FileDialog.GetVbox();
	    _fileTree = vbox.GetChild(2).GetChild(0) as Tree;
	    _fileDirectoryLineEdit = (LineEdit) FileDialog.GetVbox().GetChild(0).GetChild(5);
	    if (_fileTree != null) _fileTree.ItemSelected += OnTreeItemSelected;
    }


    void OnTreeItemSelected() {
	    if (_fileTree == null) return;
	    var item = _fileTree.GetSelected();
	    var itemText = item.GetText(0);
	    var dirPath = _fileDirectoryLineEdit?.Text;
	    var filePath = $"{dirPath}/{itemText}";

	    if (filePath.GetFile() != "" && FileAccess.FileExists(filePath)) {
		    var image = new Image();

		    if (image.Load(filePath) == Error.Ok) {
			    var tex = ImageTexture.CreateFromImage(image); 
			    PreviewTextureRect.Texture = tex;
		    }
	    }
    }

    void ResetVisibility() {
	    CanvasLayer.Visible = false;
	    Bg.Visible = false;
    }

    public void Open() {
        if (InitialPath != "") {
            FileDialog.CurrentDir = InitialPath;
        }
        FileDialog.Popup();
        CanvasLayer.Visible = true;
        Bg.Visible = true;
    }

    void OnFileSelected(string path) {
        var image = new Image();
        if (image.Load(path) == Error.Ok) {
            EmitSignal(SignalName.ImageLoaded, image);
        }

        EmitSignal(SignalName.PathSelected, path);
        Main.Config.SetValue(Config.ConfigKey.LastPath, path.GetBaseDir());
    }
}