using System.Globalization;
using Godot;


namespace GodotMidasDepth.Nodes; 

public partial class SpatialPreview : Node3D {
    /* "%Camera3D" */ [Export] public Camera3D Camera = default!;
    /* "%PreviewSprite" */ [Export] public Sprite3D? Sprite3d;
    /* "%DepthSlider" */ [Export] public Slider DepthSlider = default!;
    /* "%DepthEdit" */ [Export] public LineEdit DepthEdit = default!;
    
    double _depth = 0.05f;

    public double Depth {
        get {
            if (Sprite3d is {MaterialOverride: { }})
            {
                return  ((StandardMaterial3D) Sprite3d.MaterialOverride).HeightmapScale;
            }
            return _depth;
        }
        set {
            _depth = value;
            if (Sprite3d is {MaterialOverride: { }}) 
            {
                ((StandardMaterial3D) Sprite3d.MaterialOverride).HeightmapScale = (float)_depth;
            }
        }
    }

    public override void _Ready()
    {
	    ConnectDepthSlider();
	    SetDepthUiValues();
    }

    void ConnectDepthSlider()
    {
        DepthSlider.ValueChanged += OnDepthChanged;
    }
    
    void SetDepthUiValues() {
        var depth = Depth;
        DepthEdit.Text = depth.ToString(CultureInfo.CurrentCulture);
        DepthSlider.Value = depth;
    }

    void OnDepthChanged(double value) {
        Depth = value;
        DepthEdit.Text = value.ToString(CultureInfo.CurrentCulture);
    }

    public override void _Input(InputEvent @event) {
        if (Sprite3d == null) return;
        
        if (Input.IsActionPressed("camera_zoom_in")) 
        {
            Sprite3d.PixelSize += 0.001f;
        }
        
        if (Input.IsActionPressed("camera_zoom_out")) 
        {
            Sprite3d.PixelSize -= 0.001f;
        }
    }

    public void SetSpriteTexture(Texture2D? albedoTex, Texture2D? depthTex) {
        if (Sprite3d == null) return;
        
        var sprite3dMaterial = ((StandardMaterial3D)Sprite3d.MaterialOverride);
        sprite3dMaterial.AlbedoTexture = albedoTex;
        sprite3dMaterial.HeightmapTexture = depthTex;
    }
}