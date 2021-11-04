using Godot;
using System;

public class ChargeableAttack : Attack
{
	public ChargePart charge;
	//public bool isCharging = false;
	
	public override void Start()
	{
		frameCount = 0;
		EmitSignal(nameof(AttackStarts), this);
		if(connected != null) Disconnect("AttackEnds", connected, "SetEnd");
		Connect("AttackEnds", ch.currentState, "SetEnd");
		connected = ch.currentState;
		//currentPart = start;
		charge.Activate();
		OnStart();
	}
	
	public override void Stop()
	{
		OnEnd();
		if(currentPart != null) currentPart.Stop();
		else charge.Stop();
		EmitSignal(nameof(AttackEnds), this);
		currentPart = null;
	}
}
