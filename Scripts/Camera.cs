using Godot;
using System;

public partial class Camera : Camera3D
{
    [Export]
    public RigidBody3D Target { get; set; }

    [Export]
    public float RotationSpeed { get; set; } = 2.0f; // Lower = lazier rotation

    private Vector3 _currentLookAtPoint;
    private TextureRect _crosshair;

    public override void _Ready()
    {
        // Get the crosshair node
        _crosshair = GetNode<TextureRect>("Crosshair");

        // Hide the OS cursor and lock the mouse in place (optional for FPS-style controls)
        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Target == null)
            return;

        // Smoothly interpolate the look-at point
        _currentLookAtPoint = _currentLookAtPoint.Lerp(
            Target.GlobalPosition, 
            (float)delta * RotationSpeed
        );

        // Look at the interpolated point
        LookAt(_currentLookAtPoint);
    }

    public override void _Process(double delta)
    {
        if (_crosshair != null)
        {
            // Move the crosshair to the mouse position
            _crosshair.Position = GetViewport().GetMousePosition() - (_crosshair.Size / 2);
        }
    }
}
