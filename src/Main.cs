using Godot;
using GodotMidasDepth.Inference;
using GodotMidasDepth.Math;
using GodotMidasDepth.Nodes;


namespace GodotMidasDepth;

public partial class Main : Node {
	[Export] public PreviewPanel PreviewPanel = default!;
	[Export] public ImagePicker ImagePicker = default!;

	public static readonly Config Config = new Config();
	InferImageDepth? _inferImageDepth;

	public override void _Ready()
	{
		LoadInferenceModel();
		ConnectSignals();
	}

	void LoadInferenceModel() {
		_inferImageDepth = new InferImageDepth();
		_inferImageDepth.LoadModel();
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
			var dims = _inferImageDepth.GetDimensions();
			var outputImage = Image.CreateFromData((int)dims.x, (int)dims.y, false, Image.Format.Rf, bytes);
			PreviewPanel.CreateVertexShadedMesh(outputImage);
			PreviewPanel.SwitchTab(1);
		}
	}
}
