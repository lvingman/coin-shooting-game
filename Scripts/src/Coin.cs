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
	public AudioStreamPlayer CoinHitSFX { get; set; }
	
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
		CoinHitSFX = GetNode<AudioStreamPlayer>("CoinHitSFX");
	}

	public override void _Process(double delta)
	{
	}
	
	#endregion

	#region Methods
	
	// Method to apply an impulse based on the hit direction
	private void ApplyImpulse(Vector3 hitPosition, Vector3 hitDirection)
	{
		GD.Print($"RayCast hit Position: {hitPosition} / Coin Global Position: {GlobalPosition}");

		// Calculate displacement vector
		Vector3 displacement = hitPosition - GlobalPosition;

		// Compute a right vector (perpendicular to hitDirection)
		Vector3 worldUp = Vector3.Up;
		if (hitDirection.Abs().DistanceTo(Vector3.Up) < 0.1f) // If hitDirection is almost vertical, use another axis
			worldUp = Vector3.Forward;

		Vector3 rightVector = hitDirection.Cross(worldUp).Normalized();

		// Calculate normalized offset (-1 to 1)
		float maxExtent = 0.5f; // Adjust based on object size
		float offset = Mathf.Clamp(displacement.Dot(rightVector) / maxExtent, -1f, 1f);

		// Gradually scale the lateral force
		float lateralStrength = impulseStrength * offset;

		// Compute force direction
		Vector3 impulseForce = Vector3.Up * impulseStrength; // Always apply upward force
		impulseForce += rightVector * -lateralStrength; // Stronger push if near an edge

		// Apply the impulse
		LinearVelocity += impulseForce;

		GD.Print($"Offset: {offset}, Impulse: {impulseForce}");
	}

	
	#endregion
	
	#region Events
	
	public void Receive(Hit message)
	{
		if (message.rid == GetRid())
		{
			ApplyImpulse(message.hitPosition, message.hitDirection);

			// Add points to the score
			ScoreSgt.Instance.AddPoints(100);

			// Play hit sound effect
			CoinHitSFX.Play();
		}
	}

	
	#endregion
}
