using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

using GArray = Godot.Collections.Array;

public class RespawnPoint : Position2D, IComparable<RespawnPoint>
{
	[Export]
	public int ID;
	[Export]
	public GArray Tags;
	
	public HashSet<string> TagSet{get; set;} = new HashSet<string>();
	
	public RespawnPoint() {}
	
	public override void _Ready()
	{
		TagSet = new HashSet<string>(Tags.ToFilteredEnumerable<string>());
	}
	
	public int CompareTo(RespawnPoint point) => ID.CompareTo(point.ID);
}
