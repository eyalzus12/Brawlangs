using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class CharacterAudioManager : Node
{
	public Dictionary<string, AudioStream> sounds = new Dictionary<string, AudioStream>();
	public List<CharacterAudioPlayer> players = new List<CharacterAudioPlayer>();
	public Queue<CharacterAudioPlayer> available = new Queue<CharacterAudioPlayer>();
	
	public CharacterAudioManager() {}
	
	public CharacterAudioManager(int capacity)
	{
		for(int i = 0; i < capacity; ++i)
		{
			var player = new CharacterAudioPlayer();
			players.Add(player);
			available.Enqueue(player);
		}
	}
	
	public override void _Ready()
	{
		foreach(var player in players) AddChild(player);
	}
	
	public void Play(AudioStream stream)
	{
		if(available.Count <= 0) return;//TODO: add a waiting queue for sounds, that plays them at the fitting time
		var use = available.Dequeue();
		use.Play(stream);
		use.Connect("FinishedPlaying", this, nameof(StreamFinished));
	}
	
	public void Play(string sound)
	{
		try {Play(sounds[sound]);}
		catch(KeyNotFoundException) {}
	}
	
	public void AddSound(string name, AudioStream audio) => sounds.Add(name, audio);
	
	public void StreamFinished(CharacterAudioPlayer who, AudioStream what)
	{
		available.Enqueue(who);
		who.Disconnect("FinishedPlaying", this, nameof(StreamFinished));
	}
	
	public override string ToString()
	{
		var result = new StringBuilder();
		foreach(var player in players)
		{
			if(player.Playing)
			{
				var filename = StringUtils.RemoveExtension(player.Stream.ResourcePath.SplitByLast('/')[1]);//TODO: Trim loop
				result.Append(filename + " ");
			}
		}
		return result.ToString();
	}
}
