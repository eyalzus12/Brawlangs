using Godot;
using System;

public interface IHitLaggable : IAttacker
{
	int HitLagFrames{get; set;}
}
