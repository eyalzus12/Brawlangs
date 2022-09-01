using Godot;
using System;

public class GenericInvincibleState : State
{
	public GenericInvincibleState() : base() {}
	public GenericInvincibleState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public virtual int Startup{get;}
	public virtual int IFrames{get;}
	public virtual int Endlag{get;}
	public virtual string ActionName{get;}
	public virtual string StateAnimation{get;}
	
	public bool IFramesStarted = false;
	public bool IsInEndlag = false;
	
	public override void Init()
	{
		IFramesStarted = false;
		IsInEndlag = false;
		ch.Resources.Give(ActionName, -1);
		ch.PlayAnimation(StateAnimation);
		ch.Uncrouch();
	}
	
	protected override void LoopActions()
	{
		if(!IFramesStarted && frameCount >= Startup)
		{
			ch.IFrames.Add(ActionName, IFrames);
			OnIFramesStart();
			IFramesStarted = true;
		}
		
		if(!IsInEndlag && frameCount >= IFrames+Startup)
		{
			ch.IFrames.Remove(ActionName);
			OnEndlagStart();
			IsInEndlag = true;
		}
	}
	
	protected virtual void OnIFramesStart() {}
	protected virtual void OnEndlagStart() {}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= Startup+IFrames+Endlag) DecideNextState();
		else return false;
		
		return true;
	}
	
	protected virtual void DecideNextState()
	{
		ch.States.Change("Air");
	}
}
