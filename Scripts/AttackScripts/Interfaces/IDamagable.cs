using Godot;
using System;

public interface IDamagable : IHittable
{
	float Damage{get; set;}
	float DamageTakenMult{get; set;}
}
