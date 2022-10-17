using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class AudioManager : Node2D
{
	public Dictionary<string, AudioStream> Sounds{get; set;} = new Dictionary<string, AudioStream>();
	
	public AudioStream this[string namePrefix, string s] {get => Sounds[$"{namePrefix}.{s}"]; set => Sounds[$"{namePrefix}.{s}"] = value;}
	public bool ContainsSound(string namePrefix, string s) => Sounds.ContainsKey($"{namePrefix}.{s}");
	public void AddSound(string namePrefix, string s, AudioStream stream) => Sounds.TryAdd($"{namePrefix}.{s}", stream);
	
	public List<AudioPlayer> Players{get; set;} = new List<AudioPlayer>();
	public Queue<AudioPlayer> Available{get; set;} = new Queue<AudioPlayer>();
	
	public const int INITIAL_LOAD_AMOUNT = 5;
	public AudioManager()
	{
		LoadPlayers(INITIAL_LOAD_AMOUNT);
	}
	
	public const int ADDITIONAL_LOAD_AMOUNT = 3;
	public void Play(AudioStream stream, Vector2 pos = default)
	{
		if(stream is null) return;
		
		if(Available.Count <= 0) LoadPlayers(ADDITIONAL_LOAD_AMOUNT);
		
		var use = Available.Dequeue();
		use.Position = pos;
		use.Play(stream);
	}
	
	public void Play(string namePrefix, string s, Vector2 pos = default)
	{
		if(s == "") return;
		AudioStream audio;
		if(Sounds.TryGetValue($"{namePrefix}.{s}", out audio)) Play(audio, pos);
		else GD.PushError($"[{nameof(AudioManager)}.cs]: Unknown sound {namePrefix}.{s}");
	}
	
	public void StreamFinished(AudioPlayer who, AudioStream what) => Available.Enqueue(who);
	
	public void LoadPlayers(int amount)
	{
		int start = Players.Count;
		for(int i = 0; i < amount; ++i)
		{
			var player = new AudioPlayer();
			player.Name = $"AudioPlayer{i+start}";
			player.Connect("FinishedPlaying", this, nameof(StreamFinished));
			AddChild(player);
			
			Players.Add(player);
			Available.Enqueue(player);
		}
	}
	
	public override string ToString()
	{
		var result = new StringBuilder();
		foreach(var player in Players)
		{
			if(player.Playing)
			{
				var filename = StringUtils.RemoveExtension(player.Stream.ResourcePath.SplitByLast('/')[1]);//TODO: Trim loop
				result.Append($"{filename}\n");
			}
		}
		return result.ToString();
	}
	
	public void Cleanup()
	{
		Players.Where(p=>p.Playing).ForEach(p=>p.Stop());
	}
	
	public override void _ExitTree()
	{
		Cleanup();
	}
}
