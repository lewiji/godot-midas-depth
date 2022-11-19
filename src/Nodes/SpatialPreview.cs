using Godot;
using GodotOnReady.Attributes;

namespace GodotMidasDepth.Nodes; 

public partial class SpatialPreview : Spatial {
    [OnReadyGet("%Camera")] Camera _camera = default!;
    [OnReadyGet("%PreviewSprite")] Sprite3D _sprite = default!;

    public override void _Input(InputEvent @event) {
        if (Input.IsActionPressed("camera_zoom_in")) {
            _sprite.PixelSize += 0.001f;
        }
        
        if (Input.IsActionPressed("camera_zoom_out")) {
            _sprite.PixelSize -= 0.001f;
        }
    }
}