using Godot;
using System;

public class GenericInvincibleState : State
{
	public GenericInvincibleState() : base() {}
	public GenericInvincibleState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public virtual int Startup() => 0;
	public int startup => Startup();
	public virtual int InvincibilityLength() => 0;
	public int iframes => InvincibilityLength();
	public virtual int Endlag() => 0;
	public int endlag => Endlag();
	public virtual string ActionName() => ToString();
	public string actionName => ActionName();
	public virtual string Animation() => "Default";
	public string animation => Animation();
	
	public bool IFramesStarted = false;
	public bool IsInEndlag = false;
	
	public override void Init()
	{
		IFramesStarted = false;
		IsInEndlag = false;
		ch.GiveResource(ActionName(), -1);
		ch.PlayAnimation(animation);
	}
	
	protected override void LoopActions()
	{
		if(!IFramesStarted && frameCount >= startup)
		{
			ch.AddInvincibility(actionName, iframes);
			OnIFramesStart();
			IFramesStarted = true;
		}
		
		if(!IsInEndlag && frameCount >= iframes+startup)
		{
			ch.RemoveInvincibility(actionName);
			OnEndlagStart();
			IsInEndlag = true;
		}
	}
	
	protected virtual void OnIFramesStart() {}
	protected virtual void OnEndlagStart() {}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= startup+iframes+endlag) DecideNextState();
		else return false;
		
		return true;
	}
	
	protected virtual void DecideNextState()
	{
		ch.ChangeState<AirState>();
	}
}