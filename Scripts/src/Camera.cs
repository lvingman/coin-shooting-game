using CoinShootingGame.Scripts.Communication;
using CoinShootingGame.Scripts.Communication.Listener;
using CommunityToolkit.Mvvm.Messaging;
using Godot;

public partial class Camera : Camera3D, ScoreListener
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
	private Label _scoreLbl;
	
	#endregion
	
	#region Overrides

	
	public override void _Ready()
	{
		_crosshair = GetNode<TextureRect>("Crosshair");
		_rayCast = GetNode<RayCast3D>("RayCast3D");
		_gunSfx = GetNode<AudioStreamPlayer>("GunSFX");
		_scoreLbl = GetNode<Label>("ScoreLbl");		
		
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
	
	public override void _EnterTree()   //Lets to listen messages from IRecipient and the type of message emmited
	{
		base._EnterTree();
		StrongReferenceMessenger.Default.RegisterAll(this);
	}

	public override void _ExitTree()
	{
		base._ExitTree();

		StrongReferenceMessenger.Default.UnregisterAll(this); //Lets to unregister messages (For what idk)
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

	#region Events
	public void Receive(ScoreIncrease message)
	{
		_scoreLbl.Text = $"SCORE: {message.score}";
		GD.Print($"ScoreLbl Updated: {message.score}");
	}
	
	#endregion
}
