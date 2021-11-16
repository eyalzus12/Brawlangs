using Godot;
using System;

public class TestAttack : Hitbox
{
	public override void Init()
	{
		setKnockback = new Vector2(0f, 1300f);
		varKnockback = new Vector2(0f, 500f);
		stun = 20;
		hitpause = 2;
		hitlag = 2;
		damage = 20;
	}
}
