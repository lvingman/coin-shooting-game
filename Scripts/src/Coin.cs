using Godot;
using System;
using CoinShootingGame.Scripts.Communication;
using CommunityToolkit.Mvvm.Messaging;

public partial class Coin : RigidBody3D, IRecipient<Hit>
{
	#region Attributes
	
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

	#region Events
	
	public void Receive(Hit message)
	{
		if (message.rid == GetRid())
		{
			CoinHitSFX.Play();
		}
	}
	
	#endregion
}
