using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GodotMidasDepth.Math;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace GodotMidasDepth.Inference; 

public class InferImageDepth {
    readonly Image _input = new();
    float[]? _output;
    int _width = 256, _height = 256;
    InferenceSession? _session;
    public const string DefaultModelPath = "assets/weights/MiDaS_model-small.onnx";
    string _modelPath = DefaultModelPath;

    public void LoadModel(string? path = null) {
        if (path != null) _modelPath = path;
        _session = new InferenceSession(_modelPath);
        GD.Print($"Model loaded. Version: {_session.ModelMetadata.Version}");
        if (_modelPath == DefaultModelPath) {
            SetDimensions(_session.InputMetadata["0"].Dimensions[2], _session.InputMetadata["0"].Dimensions[3]);
        }
    }
    
    void SetDimensions(int x, int y) {
        _width = x;
        _height = y;
    }

    void ResetInputOutput() {
        _input.Create(_width, _height, false, Image.Format.Rgbf);
    }

    public Image? Run(Image inputImage) {
        ResetInputOutput();

        var inputSize = inputImage.GetSize();

        inputImage.Convert(Image.Format.Rgbf);
        inputImage.Resize(_width, _height, Image.Interpolation.Cubic);
        _input.CopyFrom(inputImage);

        //try {
        RunModel(_input);
        /*}
        catch (Exception e) {
            GD.Print($"Exception when running model: {e}");
            return null;
        }*/

        if (_output == null) return null;
        
        var bytes = _output.ToByteArray();
        var outputImage = new Image();
        outputImage.CreateFromData(_width, _height, false, Image.Format.Rf, bytes);
        outputImage.Resize((int)inputSize.x, (int)inputSize.y, Image.Interpolation.Cubic);

        return outputImage;
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
        //Tensor<float> t1 = new DenseTensor<float>(dimensions);
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
            NamedOnnxValue.CreateFromTensor<float>("0", t1),
        };

        
        using var results = _session?.Run(inputs);
        var output = results.First().AsEnumerable<float>().ToArray();

        var depthMax = output.Max();
        var depthMin = output.Min();
        var depthRange = depthMax - depthMin;

        _output = output.Select(d => (d - depthMin) / depthRange)
            .Select(n => ((1f - n) * 0f + n * 1f)).ToArray();

        results?.Dispose();

        /*var min = output.Min();
        var max = output.Max();
            
        for (var i = 0; i < output.Length; i++) {
            _output![(i%_width)*_width + (i/_width)] = (output[i] - min) / (max - min); //col*_width + row;
        }*/
    }
}