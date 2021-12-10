using Godot;
using System;

public class ChargePart : AttackPart
{
	public string ChargeInput = "heavy";
	public float FullChargeDamageMult;
	public float FullChargeKnockbackMult;
	
	public override void Init()
	{
		LoadExtraProperty<string>("ChargeInput");
		LoadExtraProperty<float>("FullChargeDamageMult");
		LoadExtraProperty<float>("FullChargeKnockbackMult");
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
		globalDamageMult *= CalculateDamageMult();
		globalKnockbackMult *= CalculateKnockbackMult();
	}
	
	public virtual float ChargeFraction() => (float)frameCount/(float)length;
	public virtual float CalculateDamageMult() => ChargeFraction()*FullChargeDamageMult + 1f;
	public virtual float CalculateKnockbackMult() => ChargeFraction()*FullChargeKnockbackMult + 1f;
}
