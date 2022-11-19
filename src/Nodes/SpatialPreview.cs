using Godot;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes; 

public partial class SpatialPreview : Spatial {
    [OnReadyGet("%Camera")] Camera _camera = default!;
    [OnReadyGet("%PreviewSprite")] Sprite3D? _sprite3d;
    float _depth = 0.05f;

    public float Depth {
        get {
            if (_sprite3d is {MaterialOverlay: { }})
            {
                return  ((SpatialMaterial) _sprite3d.MaterialOverride).DepthScale;
            }
            return _depth;
        }
        set {
            _depth = value;
            if (_sprite3d is {MaterialOverlay: { }}) 
            {
                ((SpatialMaterial) _sprite3d.MaterialOverride).DepthScale = _depth;
            }
        }
    }

    public override void _Input(InputEvent @event) {
        if (_sprite3d == null) return;
        
        if (Input.IsActionPressed("camera_zoom_in")) 
        {
            _sprite3d.PixelSize += 0.001f;
        }
        
        if (Input.IsActionPressed("camera_zoom_out")) 
        {
            _sprite3d.PixelSize -= 0.001f;
        }
    }

    public void SetSpriteTexture(Texture? albedoTex, Texture? depthTex) {
        if (_sprite3d == null) return;
        
        var sprite3dMaterial = ((SpatialMaterial)_sprite3d.MaterialOverride);
        sprite3dMaterial.AlbedoTexture = albedoTex;
        sprite3dMaterial.DepthTexture = depthTex;
    }
}