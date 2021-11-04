using Godot;
using System;

public class TestAttack2 : Hitbox
{
	public override void Init()
	{
		setKnockback = new Vector2(700f, -700f);
		varKnockback = new Vector2(0f, 0f);
		stun = 50;
		hitpause = 2;
		hitlag = 2;
		damage = 10;
	}
}
