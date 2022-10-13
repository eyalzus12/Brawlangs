using Godot;
using System;
using System.Collections.Generic;

public interface IRespawnManager
{
	//TODO: IMortal interface
	List<Node2D> HandledNodes{get; set;}
	List<RespawnPoint> RespawnPoints{get; set;}
	
	IEnumerable<Vector2> InitializeRespawns(IEnumerable<Node2D> node);
	(Vector2, int) DecideRespawn(Node2D killer, Node2D dead);
	void ConnectSignals();
	void OnNodeDie(Node2D who);
}
