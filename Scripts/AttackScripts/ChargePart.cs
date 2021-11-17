using Godot;
using System;

public class ChargePart : AttackPart
{
	public string ChargeInput = "heavy";
	
	public override void Init()
	{
		LoadExtraProperty<string>("ChargeInput");
	}
	
	public override void Loop()
	{
		if(!ch.Inputs.IsActionPressed($"player_{ChargeInput}_attack"))
		{
			ChangePart(GetNextPart());
		}
	}
}
