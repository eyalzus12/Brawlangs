using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class FallbackRespawnManager : Node2D, IRespawnManager
{
	public List<Node2D> HandledNodes{get; set;} = new List<Node2D>();
	public List<RespawnPoint> RespawnPoints{get; set;} = new List<RespawnPoint>();
	public IEnumerable<Vector2> RespawnPositions => RespawnPoints.Select(r => r.Position);
	public Randomizer RNG{get; set;}
	
	public FallbackRespawnManager()
	{
		RNG = this.GetRootNode<Randomizer>("Randomizer");
	}
	
	public FallbackRespawnManager(IEnumerable<Node2D> nodes, IEnumerable<RespawnPoint> points)
	{
		HandledNodes = new List<Node2D>(nodes);
		RespawnPoints = new List<RespawnPoint>(points);
		RespawnPoints.Sort();
		RNG = this.GetRootNode<Randomizer>("Randomizer");
	}
	
	public override void _Ready()
	{
		ConnectSignals();
	}
	
	public virtual IEnumerable<Vector2> InitializeRespawns(IEnumerable<Node2D> nodes) => Vector2.Zero.RepeatListOf(nodes.Count());
	public virtual (Vector2, int) DecideRespawn(Node2D killer, Node2D dead) => (Vector2.Zero, 0);
	
	public virtual void ConnectSignals() {foreach(var n in HandledNodes) n.Connect("Dead", this, nameof(OnNodeDie));}
	public virtual void OnNodeDie(Node2D who) => HandledNodes.Remove(who);
}
