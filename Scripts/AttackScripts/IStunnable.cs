using Godot;
using System;

public interface IStunnable : IHittable
{
	int StunFrames{get; set;}
	float StunTakenMult{get; set;}
}
