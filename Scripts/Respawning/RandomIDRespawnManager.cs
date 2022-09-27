using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class RandomIDRespawnManager : FallbackRespawnManager
{
	public RandomIDRespawnManager(IEnumerable<Node2D> nodes, IEnumerable<RespawnPoint> points) : base(nodes, points) {}
	public override IEnumerable<Vector2> InitializeRespawns(IEnumerable<Node2D> nodes) => RespawnPositions.CircularTakeOrDefault(nodes.Count(), Vector2.Zero);
	public override (Vector2, int) DecideRespawn(Node2D killer, Node2D dead) => (RespawnPositions.ChoiceOrDefault(RNG, Vector2.Zero),0);
}
