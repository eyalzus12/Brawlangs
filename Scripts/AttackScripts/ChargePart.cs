using Godot;
using System;

public class ChargePart : AttackPart
{
	public string ChargeInput = "heavy";
	public float FullChargeDamageMult = 1f;
	public float FullChargeKnockbackMult = 1f;
	public int MinimumChargeForBoost = 1;
	
	public override void Init()
	{
		LoadExtraProperty<string>("ChargeInput", "heavy");
		LoadExtraProperty<float>("FullChargeDamageMult", 1f);
		LoadExtraProperty<float>("FullChargeKnockbackMult", 1f);
		LoadExtraProperty<int>("MinimumChargeForBoost", 1);
	}
	
	public override void Loop()
	{
		var inputsuffix = (ChargeInput == "taunt")?"":"_attack";
		var inputname = $"player_{ChargeInput}{inputsuffix}"; 
		if(!ch.Inputs.IsActionPressed(inputname))
		{
			ChangePart(GetNextPart());
		}
	}
	
	public override void OnEnd()
	{
		if(frameCount < MinimumChargeForBoost) return;
		globalDamageMult *= CalculateDamageMult();
		globalKnockbackMult *= CalculateKnockbackMult();
	}
	
	public virtual float ChargeFraction()
	{
		var timeTook = Math.Max(0, frameCount-MinimumChargeForBoost);
		var possibleTime = Math.Max(0, length-MinimumChargeForBoost);
		return (float)timeTook/(float)possibleTime;
	}
	
	public virtual float CalculateDamageMult() => ChargeFraction()*(FullChargeDamageMult-1f) + 1f;
	public virtual float CalculateKnockbackMult() => ChargeFraction()*(FullChargeKnockbackMult-1f) + 1f;
}
