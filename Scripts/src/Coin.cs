using Godot;
using System;
using CoinShootingGame.Scripts.Communication;
using CoinShootingGame.Scripts.Communication.Listener;
using CoinShootingGame.Scripts.src.Globals;
using CommunityToolkit.Mvvm.Messaging;

public partial class Coin : RigidBody3D, HitListener
{
	#region Attributes
	
	[Export]
	public float impulseStrength = 10f;
	private float lateralMultiplier = 1.5f;

	public AudioStreamPlayer CoinHitSFX { get; set; }
	public Area3D Hitbox { get; set; }
	
	#endregion
	
	#region Overrides
	
	#region MVVM
	public override void _EnterTree()  
    {
        base._EnterTree();
        Hitbox = GetNode<Area3D>("Hitbox");
		StrongReferenceMessenger.Default.RegisterAll(this);
    }

	public override void _ExitTree()
    {
        base._ExitTree();

		StrongReferenceMessenger.Default.UnregisterAll(this);
    }
	
	#endregion
	public override void _Ready()
	{
		CoinHitSFX = GetNode<AudioStreamPlayer>("CoinHitSFX");
	}

	public override void _Process(double delta)
	{
	}
	
	#endregion

	#region Methods

	private void ApplyImpulse(Vector3 hitPosition, Vector3 hitDirection)
	{
		GD.Print($"RayCast hit Position: {hitPosition} / Coin Global Position: {GlobalPosition}");

		Vector3 displacement = hitPosition - GlobalPosition;
		Vector3 rightVector = hitDirection.Cross(Vector3.Up).Normalized();
		Vector3 forwardVector = rightVector.Cross(Vector3.Up).Normalized();

		float horizontalOffset = displacement.Dot(rightVector);
		float verticalOffset = displacement.Dot(forwardVector);

		float maxExtent = 0.5f;
		float lateralStrength = Mathf.Clamp(horizontalOffset / maxExtent, -1f, 1f) * impulseStrength * lateralMultiplier;
		float verticalStrength = Mathf.Clamp(verticalOffset / maxExtent, -1f, 1f) * impulseStrength * 0.75f;

		Vector3 impulseForce = (Vector3.Up * verticalStrength) + (-rightVector * lateralStrength);

		LinearVelocity += impulseForce;

		GD.Print($"Offset: Horizontal {horizontalOffset}, Vertical {verticalOffset}, Impulse: {impulseForce}");
	}

	#endregion

	#region Events

	public void Receive(Hit message)
	{
		if (message.rid == GetRid())
		{
			ApplyImpulse(message.hitPosition, message.hitDirection);
			ScoreSgt.Instance.AddPoints(100);
			impulseStrength += 1;
			lateralMultiplier += 0.2f;
			CoinHitSFX.Play();
		}
	}

	#endregion
	
	
}
