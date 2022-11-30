using Godot;
using GodotMidasDepth.Inference;
using GodotMidasDepth.Math;
using GodotMidasDepth.Nodes;
using GodotOnReady.Attributes;

namespace GodotMidasDepth;

public partial class Main : Node {
    [OnReadyGet("%PreviewPanel")] PreviewPanel _previewPanel = default!;
    [OnReadyGet("%ImagePicker")] ImagePicker _imagePicker = default!;

    public static readonly Config Config = new Config();
    InferImageDepth? _inferImageDepth;
    InferImageDepthOptionalModels? _inferHybridLarge;
    
    [OnReady]
    void LoadInferenceModel() {
        _inferImageDepth = new InferImageDepth();
        _inferImageDepth.LoadModel();
        //_inferHybridLarge = new InferImageDepthOptionalModels();
        //_inferHybridLarge.LoadModel();
    }

    [OnReady]
    void ConnectSignals() {
        _previewPanel.Connect(nameof(PreviewPanel.LoadImageRequested), _imagePicker, nameof(ImagePicker.Open));
        _imagePicker.Connect(nameof(ImagePicker.ImageLoaded), _previewPanel, nameof(PreviewPanel.SetPreviewImage));
        _imagePicker.Connect(nameof(ImagePicker.PathSelected), _previewPanel, nameof(PreviewPanel.SetPathLabel));
        _imagePicker.Connect(nameof(ImagePicker.ImageLoaded), this, nameof(ProcessDepth));
    }

    void ProcessDepth(Image image) {
        CallDeferred(nameof(RunModel), image);
    }

    void RunModel(Image image) {
        var output = _inferImageDepth?.Run(image);
        if (output == null) return;
        _previewPanel.SetResultImage(output);
        if (_inferImageDepth?.GetDataNormalised() is { } depthData)
        {
	        var bytes = depthData.ToByteArray();
            var outputImage = new Image();
            outputImage.CreateFromData(256, 256, false, Image.Format.Rf, bytes);
            _previewPanel.CreateVertexShadedMesh(outputImage);
            _previewPanel.SwitchTab(1);
        }
    }
}