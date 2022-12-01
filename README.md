# godot-midas-depth

Monocular depth inference and renderer in Godot 4 beta via machine learning. Inspired by: https://github.com/parkchamchi/DepthViewer

https://user-images.githubusercontent.com/233380/202898966-9f1ea9a9-55f1-49a5-85dc-e5733ee0a836.mp4

Uses the midas depth prediction ONNX model to infer depth from an input texture in Godot 4. The resulting depth texture is used to render the texture with adjustable depth using a vertex displacement shader + normal recalculation by averaging sampled points from multiple samples.

The "small" midas 2.1 model is included as it's reasonably sized (~63mb). Optionally you can use the hybrid or large models by exporting the official `.pt` models to `.onnx` using my fork of the midas repo here:

https://github.com/lewiji/MiDaS

These aren't bundled as they are ~500mb and ~1.3gb respectively.

Note: on Linux, you will have to obtain the libonnxruntime .so library files from the microsoft onnxruntime releases and put them in /usr/lib, then run ldconfig to enable the project to find the native onnxruntime methods. On windows you'll need a recent vcruntime installed.
