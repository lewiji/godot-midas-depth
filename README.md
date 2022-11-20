# godot-midas-depth

Inspired by https://github.com/parkchamchi/DepthViewer

Uses the small midas depth prediction ONNX model to infer depth from an input texture in godot, to use as a depth map in a SpatialMaterial, plus a voxelised depth model using a MultiMeshInstance.

Using a very large input texture will grind godot to a halt, I haven't yet optimised the voxel renderer.

Note: on Linux, you will have to obtain the libonnxruntime .so library files from the microsoft onnxruntime releases and put them in /usr/lib, then run ldconfig to enable the project to find the native onnxruntime methods. On windows you'll need a recent vcruntime installed.
