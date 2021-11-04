using Godot;
using System;

public class JumpAttack : Attack
{
	public override void OnHit(Hitbox hitbox, Area2D hurtbox)
	{
		if(ch.jumpCounter > 0) --ch.jumpCounter;
	}
}
