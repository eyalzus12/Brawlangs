using Godot;
using System;
using System.Collections.Generic;

public partial class AudioPlayer : AudioStreamPlayer2D
{
	[Signal]
	public delegate void FinishedPlayingEventHandler(AudioPlayer who);
	
	public int ID{get; set;} = 0;
	
	public AudioPlayer() {}
	public AudioPlayer(int id) {ID = id;}
	
	public override void _Ready()
	{
		Finished += OnFinish;
	}
	
	public void Play(AudioStream stream)
	{
		Stream = stream;
		Play();
	}
	
	public void OnFinish() => EmitSignal(nameof(FinishedPlaying), this);
	
	public override string ToString() => $"AudioPlayer with ID {ID}";
}
