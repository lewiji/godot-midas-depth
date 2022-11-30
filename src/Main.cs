using Godot;
using GodotMidasDepth.Inference;
using GodotMidasDepth.Math;
using GodotMidasDepth.Nodes;


namespace GodotMidasDepth;

public partial class Main : Node {
	/* "%PreviewPanel" */ [Export] public PreviewPanel PreviewPanel = default!;
	/* "%ImagePicker" */ [Export] public ImagePicker ImagePicker = default!;

	public static readonly Config Config = new Config();
	InferImageDepth? _inferImageDepth;
	InferImageDepthOptionalModels? _inferHybridLarge;

	public override void _Ready()
	{
		LoadInferenceModel();
		ConnectSignals();
	}

	void LoadInferenceModel() {
		_inferImageDepth = new InferImageDepth();
		_inferImageDepth.LoadModel();
		//_inferHybridLarge = new InferImageDepthOptionalModels();
		//_inferHybridLarge.LoadModel();
	}

	void ConnectSignals()
	{
		PreviewPanel.LoadImageRequested += ImagePicker.Open;
		ImagePicker.ImageLoaded += PreviewPanel.SetPreviewImage;
		ImagePicker.PathSelected += PreviewPanel.SetPathLabel;
		ImagePicker.ImageLoaded += ProcessDepth;
	}

	void ProcessDepth(Image image) {
		CallDeferred(nameof(RunModel), image);
	}

	void RunModel(Image image) {
		var output = _inferImageDepth?.Run(image);
		if (output == null) return;
		PreviewPanel.SetResultImage(output);
		if (_inferImageDepth?.GetDataNormalised() is { } depthData)
		{
			var bytes = depthData.ToByteArray();
			var outputImage = Image.CreateFromData(256, 256, false, Image.Format.Rf, bytes);
			PreviewPanel.CreateVertexShadedMesh(outputImage);
			PreviewPanel.SwitchTab(1);
		}
	}
}
