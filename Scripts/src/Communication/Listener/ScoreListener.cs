using CommunityToolkit.Mvvm.Messaging;

namespace CoinShootingGame.Scripts.Communication.Listener;

public interface ScoreListener : IRecipient<ScoreIncrease>
{
    
}