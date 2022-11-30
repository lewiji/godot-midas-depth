using System.Collections.Generic;
using Godot;

namespace GodotMidasDepth.Inference; 

public partial class InferImageDepthOptionalModels : IInferImageDepth {
	public enum ModelSize {Hybrid, Large}
	static readonly Dictionary<ModelSize, string> ModelPaths = new Dictionary<ModelSize, string> {
		{ModelSize.Hybrid, "assets/weights/hybrid_module.dat"},
		{ModelSize.Large, "assets/weights/large_module.dat"},
	};
	static readonly Dictionary<ModelSize, string> WeightsPaths = new Dictionary<ModelSize, string> {
		{ModelSize.Hybrid, "assets/weights/hybrid_model_weights.dat"},
		{ModelSize.Large, "assets/weights/large_model_weights.dat"},
	};
	const ModelSize DefaultModel = ModelSize.Hybrid;
	//jit.ScriptModule? _scriptModule { get; set; }
	string _modelPath { get; set; } = ModelPaths[DefaultModel];
	string _weightsPath { get; set; } = WeightsPaths[DefaultModel];
	
	public void LoadModel(ModelSize modelSize) {
		_modelPath = ModelPaths[modelSize];
		_weightsPath = WeightsPaths[modelSize];
		
		if (!FileAccess.FileExists(_modelPath) || !FileAccess.FileExists(_weightsPath)) {
			GD.PushWarning($"Model {modelSize} couldn't be loaded using torchscript: {_modelPath} and "
			               + $"weights: {_weightsPath}. Use `extract_model_weights.py`.");
			return;
		}
		
		//torch.InitializeDeviceType(DeviceType.CPU);
		//_scriptModule = jit.load(_modelPath);
		//_scriptModule.load(_weightsPath);
		GD.Print($"Loaded torch model and weights: {modelSize}");
	}
	

	public void LoadModel() {
		LoadModel(DefaultModel);
	}
	
	public float[]? GetData() { throw new System.NotImplementedException(); }
	public float[]? GetDataNormalised() { throw new System.NotImplementedException(); }
	public Image? Run(Image inputImage) { throw new System.NotImplementedException(); }
}