using Godot;
using System;

public class ChargePart : AttackPart
{
	public string ChargeInput = "heavy";
	public float FullChargeDamageMult = 1f;
	public float FullChargeKnockbackMult = 1f;
	public int MinimumChargeForBoost = 1;
	public string ChargeSound = "";
	
	public override void LoadProperties()
	{
		LoadExtraProperty<string>("ChargeInput", "heavy");
		LoadExtraProperty<float>("FullChargeDamageMult", 1f);
		LoadExtraProperty<float>("FullChargeKnockbackMult", 1f);
		LoadExtraProperty<string>("ChargeSound", "");
	}
	
	public override void OnStart()
	{
		ch.PlaySound(ChargeSound);
	}
	
	public override void Loop()
	{
		if(!ch.Inputs.IsActionPressed(ChargeInput)) ChangePart(GetNextPart());
	}
	
	public override void OnEnd()
	{
		if(frameCount < MinimumChargeForBoost) return;
		att.damageMult *= CalculateDamageMult();
		att.knockbackMult *= CalculateKnockbackMult();
	}
	
	public virtual float ChargeFraction()
	{
		var timeHeld = Math.Max(0, frameCount-startup);
		return (float)timeHeld/(float)length;
	}
	
	public virtual float CalculateDamageMult() => ChargeFraction()*(FullChargeDamageMult-1f) + 1f;
	public virtual float CalculateKnockbackMult() => ChargeFraction()*(FullChargeKnockbackMult-1f) + 1f;
}
