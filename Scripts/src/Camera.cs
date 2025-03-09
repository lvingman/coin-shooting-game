using CoinShootingGame.Scripts.Communication;
using CommunityToolkit.Mvvm.Messaging;
using Godot;

public partial class Camera : Camera3D
{
	#region Attributes
	[Export]
	public RigidBody3D Target { get; set; }

	[Export]
	public float RotationSpeed { get; set; } = 2.0f; // Lower = lazier rotation
	
	private Vector3 _currentLookAtPoint;
	private TextureRect _crosshair;
	private RayCast3D _rayCast;
	private AudioStreamPlayer _gunSfx;
	
	#endregion
	
	#region Overrides

	
	public override void _Ready()
	{
		_crosshair = GetNode<TextureRect>("Crosshair");
		_rayCast = GetNode<RayCast3D>("RayCast3D");
		_gunSfx = GetNode<AudioStreamPlayer>("GunSFX");

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

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("fire"))
		{
			ShootGun();
		}
	}

	#endregion

	#region methods
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
		
		GD.Print($"Raycast Location: {_rayCast.GlobalPosition}, Raycast Target: {_rayCast.TargetPosition}");
	}

	private void ShootGun()
	{
		_rayCast.ForceRaycastUpdate();
		if (_rayCast.IsColliding())
		{
			Rid colliderRid = _rayCast.GetColliderRid();
			StrongReferenceMessenger.Default.Send<Hit>(new(colliderRid));
		}
		
		if (_gunSfx != null)
		{
			_gunSfx.Play();
		}
		else
		{
			GD.Print("Whoops, no sfx assigned");
		}
		
	}
	
	#endregion
	
}
