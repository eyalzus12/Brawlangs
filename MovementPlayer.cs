using Godot;
using System;

public class MovementPlayer : AnimationPlayer
{
	bool paused = false;
	bool frameStep = false;
	
	//TOOD: wtf is this. comment the heck out of it
	public override void _PhysicsProcess(float delta)
	{
		if(Input.IsActionJustPressed("pause"))
		{
			paused = !paused;
			
			if(paused) Stop(false);
			else Play();
		}
		
		if(Input.IsActionJustPressed("reset"))
		{
			Stop();
			if(!paused) Play();
		}
		
		if(paused)
		{
			if(frameStep)
			{
				Stop(false);
				frameStep = false;
			}
			
			if(Input.IsActionJustPressed("next_frame") && !frameStep)
			{
				Play();
				frameStep = true;
			}
		}
	}
}
