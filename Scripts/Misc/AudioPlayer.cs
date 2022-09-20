using Godot;
using System;
using System.Collections.Generic;

public partial class AudioPlayer : AudioStreamPlayer2D
{
	[Signal]
	public delegate void FinishedPlayingEventHandler(AudioPlayer who, AudioStream what);
	
	public override void _Ready()
	{
		//Connect("finished",new Callable(this,nameof(OnFinish)));
		Finished += OnFinish;
	}
	
	public void Play(AudioStream stream)
	{
		Stream = stream;
		Play();
	}
	
	public void OnFinish()
	{
		EmitSignal(nameof(FinishedPlaying), this, Stream);
	}
}
