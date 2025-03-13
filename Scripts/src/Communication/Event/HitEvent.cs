using Godot;

namespace CoinShootingGame.Scripts.Communication;

public record Hit(Rid rid, Vector3 hitPosition, Vector3 hitDirection);