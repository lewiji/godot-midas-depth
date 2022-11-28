using Godot;
namespace GodotMidasDepth.Inference;

public interface IInferImageDepth {
	void LoadModel();
	float[]? GetData();
	float[]? GetDataNormalised();
	Image? Run(Image inputImage);
}