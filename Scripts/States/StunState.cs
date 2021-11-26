using Godot;
using System;

public class StunState : State
{
	public int stunLength = 0;
	public Vector2 force = Vector2.Zero;
	//public bool bounced = false;
	
	public StunState() : base() {}
	public StunState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		SetupCollisionParamaters();
		stunLength = 0;
		ch.vec = Vector2.Zero;
		//bounced = false;
		ch.PlayAnimation("Stun");
	}
	
	public Vector2 Force
	{
		get => force;
		set
		{
			force = value;
			ch.voc = force;
		}
	}
	
	protected override void RepeatActions()
	{
		var friction = ch.grounded?ch.groundFriction*ch.ffric:ch.airFriction;
		ch.voc.x *= (1f-friction);
		if(!ch.grounded) ch.voc.y.Lerp(ch.fallSpeed, ch.gravity);
		
		if(!bounced)
		{
			if(!ch.grounded || ch.voc.y > VCF)
			{
				var r = ch.voc.Bounce(ch.fnorm);
				r.y *= 0.95f;
				ch.voc = r;
				bounced = true;
			}
		}
	}
	
	public override void SetInputs()
	{
		SetHorizontalAlternatingInputs();
		SetUpHoldingInput();
		SetDownHoldingInput();
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= stunLength)
		{
			ch.framesSinceLastHit = 0;
			ch.ChangeState(ch.grounded?"Idle":ch.walled?"Wall":"Air");
		}
		else return false;
		
		return true;
	}
	
	public override void OnChange()
	{
		ch.vec = ch.voc;
		Force = Vector2.Zero;
	}
}
