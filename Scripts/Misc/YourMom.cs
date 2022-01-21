using Godot;
using System;

public class YourMom : Hitbox
{	
	public override void OnHit(Area2D area)
	{
		Node n = area.GetParent();
		var c = n as Character;
		if(!(owner as Character).CanHit(c)) return;
		var me = new MomEffect(180);
		c.AttachEffect(me);
	}
}
