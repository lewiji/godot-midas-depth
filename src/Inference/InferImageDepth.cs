using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotMidasDepth.Math;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace GodotMidasDepth.Inference; 

public partial class InferImageDepth : IInferImageDepth {
    Image? _input;
    float[]? _output;
    int _width = 256, _height = 256;
    string _inputName = "0";
    InferenceSession? _session;
    const string DefaultModelPath = "assets/weights/MiDaS_model-small.onnx";
    string _modelPath = DefaultModelPath;

    public void LoadModel(string path) {
        _modelPath = path;
        _session = new InferenceSession(_modelPath);
        GD.Print($"Model loaded. Version: {_session.ModelMetadata.Version}");
        _inputName = _session.InputMetadata.Keys.First();
        SetDimensions(_session.InputMetadata[_inputName].Dimensions[2], _session.InputMetadata[_inputName].Dimensions[3]);
    }

    public void LoadModel() {
	    LoadModel(DefaultModelPath);
    }

    public float[]? GetData() {
        return _output;
    }

    public float[]? GetDataNormalised() {
        return _output != null ? NormaliseOutput(_output) : null;
    }

    public Vector2 GetDimensions()
    {
	    return new Vector2(_width, _height);
    }
    
    void SetDimensions(int x, int y) {
        _width = x;
        _height = y;
    }

    void ResetInputOutput() {
        _input = Image.Create(_width, _height, false, Image.Format.Rgbf);
    }

    public Image? Run(Image inputImage) {
        ResetInputOutput();

        var inputSize = inputImage.GetSize();

        inputImage.Convert(Image.Format.Rgbf);
        inputImage.Resize(_width, _height, Image.Interpolation.Lanczos);
        _input!.CopyFrom(inputImage);

        RunModel(_input);

        if (_output == null) return null;
        
        var normalisedOutput = NormaliseOutput(_output);
        var bytes = normalisedOutput.ToByteArray();
        var outputImage = Image.CreateFromData(_width, _height, false, Image.Format.Rf, bytes);
        outputImage.Resize((int)inputSize.x, (int)inputSize.y, Image.Interpolation.Cubic);
        
        return outputImage;
    }

    float[] NormaliseOutput(float[] floats)
    {
        var depthMax = floats.Max();
        var depthMin = floats.Min();
        var depthRange = depthMax - depthMin;

        var normalisedOutput = floats.Select(d => (d - depthMin) / depthRange)
            .Select(n => ((1f - n) * 0f + n * 1f)).ToArray();
        return normalisedOutput;
    }

    void RunModel(Image source) {
        var bytes = source.GetData();
        var dimensionsLength = _width * _height;
        var floats = bytes.ToFloat32Array();
        var floatCounter = 0;
        var rFloats = new float[dimensionsLength];
        var gFloats = new float[dimensionsLength];
        var bFloats = new float[dimensionsLength];
        for (var i = 0; i < dimensionsLength; i++) {
            rFloats[i] = floats[floatCounter++];
            gFloats[i] = floats[floatCounter++];
            bFloats[i] = floats[floatCounter++];
        }

        var dimensions = new ReadOnlySpan<int>(new []{1, 3, _height, _width});
        var t1 = new DenseTensor<float>(dimensions);
        for (var y = 0; y < _height; y++) {
            for (var x = 0; x < _width; x++) {
                var index = y * _height + x;
                t1[0, 0, y, x] = rFloats[index];
                t1[0, 1, y, x] = gFloats[index];
                t1[0, 2, y, x] = bFloats[index];
            }
        }

        var inputs = new List<NamedOnnxValue>() {
            NamedOnnxValue.CreateFromTensor<float>(_inputName, t1),
        };

        using var results = _session?.Run(inputs);
        _output = results?.First().AsEnumerable<float>().ToArray();

        results?.Dispose();
    }
}