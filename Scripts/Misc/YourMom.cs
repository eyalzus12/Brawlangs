using Godot;
using System;

public partial class YourMom : Hitbox
{	
	public override void OnHit(Hurtbox hurtbox)
	{
		var c = hurtbox.OwnerObject;
		if(!OwnerObject.CanHit(c)) return;
		var me = new MomEffect(180);
		((Character)c).AttachEffect(me);
	}
}
