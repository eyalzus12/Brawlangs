using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public partial class AudioManager : Node
{
	public Dictionary<string, AudioStream> Sounds{get; set;} = new();
	public AudioStream this[string s] {get => Sounds[s]; set => Sounds[s] = value;}
	public bool ContainsSound(string s) => Sounds.ContainsKey(s);
	public List<AudioPlayer> Players{get; set;} = new();
	public Queue<AudioPlayer> Available{get; set;} = new();
	
	public AudioManager() {}
	
	public AudioManager(int capacity)
	{
		for(int i = 0; i < capacity; ++i)
		{
			var player = new AudioPlayer();
			Players.Add(player);
			Available.Enqueue(player);
		}
	}
	
	public override void _Ready()
	{
		Players.ForEach(a => AddChild(a));
	}
	
	public void Play(AudioStream stream)
	{
		if(stream is null || !Godot.Object.IsInstanceValid(stream) || Available.Count <= 0) return;//TODO: add a waiting queue for sounds, that plays them at the fitting time
		
		var use = Available.Dequeue();
		use.Play(stream);
		//use.Connect("FinishedPlaying",new Callable(this,nameof(StreamFinished)));
		use.FinishedPlaying += StreamFinished;
	}
	
	public void Play(string sound)
	{
		if(Sounds.ContainsKey(sound)) Play(this[sound]);
		else if(sound != "") GD.PushError($"Could not play sound {sound} as it does not exist");
	}
	
	public void AddSound(string name, AudioStream audio)
	{
		//audio.Changed += Temp;
		Sounds.Add(name, audio);
	}
	
	//public void Temp() => GD.Print("amogus");
	
	public void StreamFinished(AudioPlayer who, AudioStream what)
	{
		GD.Print(what);
		Available.Enqueue(who);
		//who.Disconnect("FinishedPlaying",new Callable(this,nameof(StreamFinished)));
		who.FinishedPlaying -= StreamFinished;
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
		Players.Where(p=>p.Playing).ForEach(CleanAfter);
	}
	
	public void CleanAfter(AudioPlayer player)
	{
		//player.Disconnect("FinishedPlaying",new Callable(this,nameof(StreamFinished)));
		player.FinishedPlaying -= StreamFinished;
		player.Stop();
		Available.Enqueue(player);
	}
	
	public override void _ExitTree()
	{
		Cleanup();
	}
}
