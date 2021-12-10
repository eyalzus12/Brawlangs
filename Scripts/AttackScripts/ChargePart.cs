using Godot;
using System;

public class ChargePart : AttackPart
{
	public string ChargeInput = "heavy";
	public float FullChargeDamageMult = 0f;
	public float FullChargeKnockbackMult = 0f;
	public int MinimumChargeForBoost = 1;
	
	public override void Init()
	{
		LoadExtraProperty<string>("ChargeInput");
		LoadExtraProperty<float>("FullChargeDamageMult");
		LoadExtraProperty<float>("FullChargeKnockbackMult");
		LoadExtraProperty<int>("MinimumChargeForBoost");
	}
	
	public override void Loop()
	{
		if(!ch.Inputs.IsActionPressed($"player_{ChargeInput}_attack"))
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
	
	public virtual float CalculateDamageMult() => ChargeFraction()*FullChargeDamageMult + 1f;
	public virtual float CalculateKnockbackMult() => ChargeFraction()*FullChargeKnockbackMult + 1f;
}
