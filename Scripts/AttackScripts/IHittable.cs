using Godot;
using System;

public interface IHittable
{
	int TeamNumber{get; set;}
	void HandleGettingHit(HitData data);
}
