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
	private void ApplyImpulseAtGlobalPosition(Vector3 direction, Vector3 position)
	{
		// Apply the impulse to the coin in the direction of the hit
		ApplyImpulse(position, direction.Normalized() * impulseStrength);
	}
	
	#endregion
	
	#region Events
	
	public void Receive(Hit message)
	{
		if (message.rid == GetRid())
		{
			// Get the direction of the raycast hit (opposite of the collision normal)
			Vector3 hitDirection = message.hitDirection;

			// Calculate the upward direction (Y-axis) and combine it with the hit direction
			Vector3 upwardForce = new Vector3(0, 5, 0); // Upward direction
			Vector3 oppositeForce = hitDirection * -1;  // Opposite direction of where we shot

			// Combine the forces
			Vector3 forceToApply = upwardForce + oppositeForce;

			// Apply an impulse at the collision point
			ApplyImpulseAtGlobalPosition(forceToApply, message.hitPosition);

			// Add points to the score
			ScoreSgt.Instance.AddPoints(100);

			// Play hit sound effect
			CoinHitSFX.Play();
		}
	}

	
	#endregion
}
