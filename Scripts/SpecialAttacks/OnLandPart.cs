using Godot;
using System;

public class OnLandPart : AttackPart
{
	public override void Loop()
	{
		if(ch.grounded) ChangePart("Land");
	}
}
