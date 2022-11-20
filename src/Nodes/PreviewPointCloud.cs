using System.Linq;
using Godot;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes;

public partial class PreviewPointCloud : Spatial
{
    [OnReadyGet("%PointCloud")] MultiMeshInstance _pointCloud = default!;
    [OnReadyGet("%PointCloudPivot")] Spatial _pcPivot = default!;
    

    public void SetData(Image albedo, float[] depthArray)
    {
        var mm = _pointCloud.Multimesh;
        var albedoSize = albedo.GetSize();
        var count = (int)albedoSize.x * (int)albedoSize.y;
        var translationScale = ((CubeMesh)mm.Mesh).Size.x;
        var sampleScale = new Vector2(256f / albedoSize.x, 256f / albedoSize.y);
        
        mm.InstanceCount = count;
        albedo.Lock();
        
        for (var i = 0; i < count; i++)
        {
            var x = i % albedoSize.x;
            var y = Mathf.Floor(i / albedoSize.x);
            var depth = -depthArray[Mathf.FloorToInt(i / sampleScale.x)] / 10f;
            var translation = new Vector3(x, y, depth);
            mm.SetInstanceTransform(i, new Transform(Basis.Identity, translation * translationScale));
            mm.SetInstanceColor(i, albedo.GetPixel((int)translation.x, (int)translation.y));
        }

        _pointCloud.Translation = new Vector3(-albedoSize.x / 2f, -albedoSize.y / 2f, 0) * translationScale;
    }
}