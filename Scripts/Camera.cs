using Godot;

public partial class Camera : Camera3D
{
    [Export]
    public RigidBody3D Target { get; set; }

    [Export]
    public float RotationSpeed { get; set; } = 2.0f; // Lower = lazier rotation
    
    private Vector3 _currentLookAtPoint;
    private TextureRect _crosshair;
    private RayCast3D _rayCast;

    public override void _Ready()
    {
        _crosshair = GetNode<TextureRect>("Crosshair");
        _rayCast = GetNode<RayCast3D>("RayCast3D");

        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Target == null) return;

        _currentLookAtPoint = _currentLookAtPoint.Lerp(Target.GlobalPosition, (float)delta * RotationSpeed);
        LookAt(_currentLookAtPoint);
    }

    public override void _Process(double delta)
    {
        UpdateCrosshairPosition();
        UpdateRaycastPosition();
    }

    private void UpdateCrosshairPosition()
    {
        if (_crosshair != null)
        {
            _crosshair.Position = GetViewport().GetMousePosition() - (_crosshair.Size / 2);
        }
    }

    private void UpdateRaycastPosition()
    {
        if (_rayCast == null) return;

        Vector2 mousePos = GetViewport().GetMousePosition();

        _rayCast.GlobalPosition = ProjectRayOrigin(mousePos);
        _rayCast.TargetPosition = ProjectRayNormal(mousePos) *100f;
        //_rayCast.TargetPosition = _rayCast.ToLocal(_rayCast.GlobalPosition + ProjectRayNormal(mousePos));
        
        GD.Print($"Raycast Location: {_rayCast.GlobalPosition}, Raycast Target: {_rayCast.TargetPosition}");
    }

}
