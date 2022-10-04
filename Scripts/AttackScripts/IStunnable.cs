using Godot;
using System;

public interface IStunnable : IHittable
{
	int StunFrames{get; set;}
}
