using Godot;
using System;

public class JoshSSpecial : AttackPart
{
	public override void OnStart()
	{
		ch.EmitProjectile("SSpecialBullet");
	}
}
