using Godot;
using System;
using System.Collections.Generic;

public class CharacterAudioManager : AudioStreamPlayer2D
{
	public Dictionary<string, AudioStream> sounds = new Dictionary<string, AudioStream>();
	
	public void Play(AudioStream stream)
	{
		Stream = stream;
		Play();
	}
	
	public void Play(string sound)
	{
		try {Play(sounds[sound]);}
		catch(KeyNotFoundException) {}
	}
	
	public void AddSound(string name, AudioStream audio) => sounds.Add(name, audio);
}
