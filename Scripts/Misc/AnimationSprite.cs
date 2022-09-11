using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class AnimationSprite : Sprite
{
	public Dictionary<string, AnimationSheet> Animations{get; set;} = new Dictionary<string, AnimationSheet>();
	public AnimationPlayer FramePlayer{get; private set;} = new AnimationPlayer();
	public AnimationSheet? Current{get; private set;} = null;
	public Queue<AnimationSheet> Queued{get; private set;} = new Queue<AnimationSheet>();
	public bool Playing => FramePlayer.IsPlaying();
	public bool Looping => Current?.Loop ?? false;
	
	public override void _Ready()
	{
		InitFramePlayer();
	}
	
	public void Add(Texture texture, string name, int length, bool loop)
	{
		Animations.Add(name, new AnimationSheet(texture, name, length, loop));
	}
	
	public void InitFramePlayer()
	{
		FramePlayer.PlaybackProcessMode = AnimationPlayer.AnimationProcessMode.Physics;
		FramePlayer.Name = "FramePlayer";
		AddChild(FramePlayer);
		foreach(var a in Animations)
		{
			var animationName = a.Key;
			var animationSheet = a.Value;
			
			var anm = new Animation();
			anm.Step = 1/24f;
			var frameCount = animationSheet.Length;
			anm.Length = frameCount * anm.Step;
			anm.Loop = animationSheet.Loop;
			FramePlayer.AddAnimation(animationName, anm);
			int trc = anm.AddTrack(Animation.TrackType.Value);
			var path = GetPath() + ":frame";
			anm.TrackSetPath(trc, path);
			
			for(int i = 0; i < frameCount; ++i)
				anm.TrackInsertKey(trc, i*anm.Step, i);
		}
		
		FramePlayer.Connect("animation_finished", this, nameof(AnimationFinished));
	}
	
	public void Play(string anm, bool overwriteQueue)
	{
		if(overwriteQueue) ClearQueue();
		AnimationSheet sheet; if(Animations.TryGetValue(anm, out sheet)) Play(sheet, false);
	}
	
	public void Play(AnimationSheet sheet, bool overwriteQueue)
	{
		if(overwriteQueue) ClearQueue();
		if(FramePlayer is null) return;
		this.Texture = sheet.Texture;
		this.Hframes = sheet.Length;
		FramePlayer.Play(sheet.Name);
		FramePlayer.Advance(0);
		Current = sheet;
	}
	
	public void Queue(string anm, bool goNext, bool overwriteQueue)
	{
		if(overwriteQueue) ClearQueue();
		AnimationSheet sheet; if(Animations.TryGetValue(anm, out sheet)) Queue(sheet, false, false);
		if(goNext) GoNext();
	}
	
	public void Queue(AnimationSheet sheet, bool goNext, bool overwriteQueue)
	{
		if(overwriteQueue) ClearQueue();
		Queued.Enqueue(sheet);
		if(goNext || !Playing) GoNext();
	}
	
	public void GoNext() => AnimationFinished(Current?.Name ?? "");
	
	public void ClearQueue() => Queued.Clear();
	
	public void AnimationFinished(string anm = "")
	{
		if(Queued.Count > 0)
		{
			Current = Queued.Peek();
			Play(Queued.Dequeue(), false);
		}
	}
	
	public void Pause() => FramePlayer?.Stop(false);
	public void Resume() => FramePlayer?.Play();
	
	public string QueueToString => string.Join(" ", Queued.Select(a=>a.Name).ToArray());
}

public readonly struct AnimationSheet
{
	public Texture @Texture {get;}
	public string Name {get;}
	public int Length {get;}
	public bool Loop {get;}
	
	public AnimationSheet(Texture texture, string name, int length, bool loop)
	{
		@Texture = texture;
		Name = name;
		Length = length;
		Loop = loop;
	}
}
