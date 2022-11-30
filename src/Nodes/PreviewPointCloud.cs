using System.Globalization;
using System.Linq;
using Godot;


namespace GodotMidasDepth.Nodes;

public partial class PreviewPointCloud : Node3D
{
    /* "%PointCloud" */ [Export] public MultiMeshInstance3D PointCloud = default!;
    /* "%PointCloudPivot" */ [Export] public Node3D PcPivot = default!;
    /* "%DepthSlider" */ [Export] public Slider DepthSlider = default!;
    /* "%DepthEdit" */ [Export] public LineEdit DepthEdit = default!;

    
    

    Image? _albedo;
    float[]? _depthArray;

    public override void _Ready()
    {
	    ConnectSignals();
    }

    void ConnectSignals() {
        DepthSlider.DragEnded += OnDepthChanged;
    }

    void OnDepthChanged(bool valueChanged) {
        if (valueChanged) {
            DepthEdit.Text = DepthSlider.Value.ToString(CultureInfo.CurrentCulture);
            SetMultiMeshTransforms();
        }
    }

    
    

    public void SetData(Image albedo, float[] depthArray) {
        _albedo = albedo;
        _depthArray = depthArray;
    }

    public void SetMultiMeshTransforms() {
        if (_albedo == null || _depthArray == null) return;
        
        var mm = PointCloud.Multimesh;
        var albedoSize = _albedo.GetSize();
        var count = (int)albedoSize.x * (int)albedoSize.y;
        var translationScale = ((BoxMesh)mm.Mesh).Size.x;
        var sampleScale = new Vector2(256f / albedoSize.x, 256f / albedoSize.y);
        
        mm.InstanceCount = count;
        
        for (var i = 0; i < count; i++)
        {
            var x = i % albedoSize.x;
            var y = Mathf.Floor(i / albedoSize.x);
            var depth = -_depthArray[Mathf.FloorToInt(i / sampleScale.x)] * (float)DepthSlider.Value;
            var translation = new Vector3(x, y, depth);
            mm.SetInstanceTransform(i, new Transform3D(Basis.Identity, translation * translationScale));
            mm.SetInstanceColor(i, _albedo.GetPixel((int)translation.x, (int)translation.y));
        }
        
        PointCloud.Position = new Vector3(-albedoSize.x / 2f, -albedoSize.y / 2f, 0) * translationScale;
    }
}