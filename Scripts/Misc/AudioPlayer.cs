using Godot;
using System;
using System.Collections.Generic;

public class AudioPlayer : AudioStreamPlayer2D
{
	[Signal]
	public delegate void FinishedPlaying(AudioPlayer who, AudioStream what);
	
	public override void _Ready()
	{
		Connect("finished", this, nameof(OnFinish));
		Attenuation = 0.5f;
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
