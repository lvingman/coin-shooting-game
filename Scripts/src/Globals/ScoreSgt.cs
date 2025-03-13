using CoinShootingGame.Scripts.Communication;
using CommunityToolkit.Mvvm.Messaging;

namespace CoinShootingGame.Scripts.src.Globals;

using Godot;

public partial class ScoreSgt : Node
{
    private static ScoreSgt _instance;
    public static ScoreSgt Instance
    {
        get
        {
            if (_instance == null)
                GD.PrintErr("ScoreSgt instance is null! Did you forget to add it to AutoLoad?");
            return _instance;
        }
    }

    public int Score { get; private set; }

    public override void _EnterTree()
    {
        if (_instance != null)
        {
            GD.PrintErr("Multiple instances of ScoreSgt detected! Ensure it's set as AutoLoad.");
            QueueFree(); // Destroy the duplicate instance
            return;
        }

        _instance = this;
    }

    public void AddPoints(int points)
    {
        Score += points;
        GD.Print($"Score Updated: {Score}");
        StrongReferenceMessenger.Default.Send<ScoreIncrease>(new(Score));
    }
}
