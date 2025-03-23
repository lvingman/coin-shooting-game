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

	#region MVVM
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

		Vector3 targetPos = Target.GlobalPosition;
		Vector3 cameraPos = GlobalPosition;

		// Check the distance in the X and Z axes
		float distanceX = Mathf.Abs(targetPos.X - cameraPos.X);
		float distanceZ = Mathf.Abs(targetPos.Z - cameraPos.Z);

		// Move the camera if it's too far
		if (distanceX > 100 || distanceZ > 100)
		{
			Vector3 newPos = new Vector3(targetPos.X, cameraPos.Y, targetPos.Z);
			GlobalPosition = GlobalPosition.Lerp(newPos, (float)delta * RotationSpeed);
		}

		// Keep looking at the target
		_currentLookAtPoint = _currentLookAtPoint.Lerp(targetPos, (float)delta * RotationSpeed);
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

	#region Methods
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
			var collider = _rayCast.GetCollider();

			if (collider is Area3D area) // Check if it's an Area3D
			{
				var coin = area.GetParent<Coin>(); // Get the parent Coin node
				if (coin != null)
				{
					// Send a hit message to the Coin script
					StrongReferenceMessenger.Default.Send<Hit>(new(coin.GetRid(), _rayCast.GetCollisionPoint(), _rayCast.GetCollisionNormal()));
				}
			}
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
