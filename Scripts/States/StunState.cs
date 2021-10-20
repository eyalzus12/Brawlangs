using Godot;
using System;

public class StunState : State
{
	public int stunLength = 0;
	public Vector2 force = Vector2.Zero;
	public bool bounced = false;
	//public Vector2 forceDumping = Vector2.Zero;
	
	public StunState() : base() {}
	public StunState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		SetupCollisionParamaters();
		stunLength = 0;
		ch.vec = Vector2.Zero;
		bounced = false;
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
		ch.voc.x *= (1f-ch.airFriction);
		ch.voc.y.Lerp(ch.fallSpeed, ch.gravity);
		
		if(bounced){}
		else if(ch.grounded)
		{
			if(ch.voc.y > VCF)
			{
				var r = ch.voc.Bounce(ch.fnorm);
				r.y *= 0.95f;
				ch.voc = r;
				bounced = true;
			}
		}
		else if(ch.walled)
		{
			var r = ch.voc.Bounce(ch.wnorm);
			r.y *= 0.95f;
			ch.voc = r;
			bounced = true;
		}
		else if(ch.ceilinged)
		{
			var r = ch.voc.Bounce(ch.cnorm);
			r.y *= 0.95f;
			ch.voc = r;
			bounced = true;
		}
		
		ch.framesSinceLastHit = 0;
	}
	
	private float prec()
	{
		float fc = frameCount.f() - 1f;
		float sl = stunLength.f() - 1f;
		
		if(sl <= 0f) return fc;
		else return fc/sl;
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
			ch.vec = ch.voc;
			Force = Vector2.Zero;
			
			ch.ChangeState(ch.grounded?"Idle":ch.walled?"Wall":"Air");
		}
		else return false;
		
		return true;
	}
}
