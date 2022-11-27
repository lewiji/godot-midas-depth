using System.Globalization;
using System.Linq;
using Godot;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes;

public partial class PreviewPointCloud : Spatial
{
    [OnReadyGet("%PointCloud")] MultiMeshInstance _pointCloud = default!;
    [OnReadyGet("%PointCloudPivot")] Spatial _pcPivot = default!;
    [OnReadyGet("%DepthSlider")] Slider _depthSlider = default!;
    [OnReadyGet("%DepthEdit")] LineEdit _depthEdit = default!;

    
    

    Image? _albedo;
    float[]? _depthArray;

    [OnReady]
    void ConnectSignals() {
        _depthSlider.Connect("drag_ended", this, nameof(OnDepthChanged));
    }

    void OnDepthChanged(bool valueChanged) {
        if (valueChanged) {
            _depthEdit.Text = _depthSlider.Value.ToString(CultureInfo.CurrentCulture);
            SetMultiMeshTransforms();
        }
    }

    
    

    public void SetData(Image albedo, float[] depthArray) {
        _albedo = albedo;
        _depthArray = depthArray;
    }

    public void SetMultiMeshTransforms() {
        if (_albedo == null || _depthArray == null) return;
        
        var mm = _pointCloud.Multimesh;
        var albedoSize = _albedo.GetSize();
        var count = (int)albedoSize.x * (int)albedoSize.y;
        var translationScale = ((CubeMesh)mm.Mesh).Size.x;
        var sampleScale = new Vector2(256f / albedoSize.x, 256f / albedoSize.y);
        
        mm.InstanceCount = count;
        _albedo.Lock();
        
        for (var i = 0; i < count; i++)
        {
            var x = i % albedoSize.x;
            var y = Mathf.Floor(i / albedoSize.x);
            var depth = -_depthArray[Mathf.FloorToInt(i / sampleScale.x)] * (float)_depthSlider.Value;
            var translation = new Vector3(x, y, depth);
            mm.SetInstanceTransform(i, new Transform(Basis.Identity, translation * translationScale));
            mm.SetInstanceColor(i, _albedo.GetPixel((int)translation.x, (int)translation.y));
        }
        
        _albedo.Unlock();

        _pointCloud.Translation = new Vector3(-albedoSize.x / 2f, -albedoSize.y / 2f, 0) * translationScale;
    }
}