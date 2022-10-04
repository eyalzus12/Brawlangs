using Godot;
using System;

public interface IHitPausable : IHittable
{
	int HitPauseFrames{get; set;}
}
