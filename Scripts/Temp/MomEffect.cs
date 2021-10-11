using Godot;
using System;

public class MomEffect : Effect
{
	public MomEffect():base() {}
	public MomEffect(int length):base(length) {}
	
	Label l;
	
	public override void Init()
	{
		l = new Label();
		l.Text = "Your Mom";
		ch.AddChild(l);
	}
	
	public override void OnEnd()
	{
		l.QueueFree();
		GetTree().Quit();
	}
}
