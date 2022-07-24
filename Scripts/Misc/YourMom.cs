using Godot;
using System;

public class YourMom : Hitbox
{	
	public override void OnHit(Hurtbox hurtbox)
	{
		var c = (Character)hurtbox.owner;
		if(!owner.CanHit(c)) return;
		var me = new MomEffect(180);
		c.AttachEffect(me);
	}
}
