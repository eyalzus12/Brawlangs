using Godot;
using System;

public class JoshNSpecialShoot : AttackPart
{
	public override void OnStart()
	{
		var proj = new JoshNSpecialProjectile(ch);
		ch.EmitProjectile(proj);
	}
}
