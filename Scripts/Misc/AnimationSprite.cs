using Godot;
using System;
using System.Collections.Generic;

public class AnimationSprite : Sprite
{
	public Dictionary<string, AnimationSheet> animations = new Dictionary<string, AnimationSheet>();
	
	public AnimationSheet currentSheet;
	public AnimationSheet? queuedSheet;
	public AnimationPlayer framePlayer;
	
	public AnimationSprite()
	{
		currentSheet = new AnimationSheet(null, "", 0, false);
	}
	
	public override void _Ready()
	{
		InitFramePlayer();
	}
	
	public void InitFramePlayer()
	{
		framePlayer = new AnimationPlayer();
		framePlayer.PlaybackProcessMode = AnimationPlayer.AnimationProcessMode.Physics;
		framePlayer.Name = "FramePlayer";
		AddChild(framePlayer);
		foreach(var a in animations)
		{
			var animationName = a.Key;
			var animationSheet = a.Value;
			
			var anm = new Animation();
			anm.Step = 1/24f;
			var frameCount = animationSheet.frames;
			anm.Length = frameCount * anm.Step;
			anm.Loop = animationSheet.loop;
			framePlayer.AddAnimation(animationName, anm);
			int trc = anm.AddTrack(Animation.TrackType.Value);
			var path = GetPath() + ":frame";
			anm.TrackSetPath(trc, path);
			
			for(int i = 0; i < frameCount; ++i)
				anm.TrackInsertKey(trc, i*anm.Step, i);
		}
		
		framePlayer.Connect("animation_finished", this, nameof(AnimationFinished));
	}
	
	public void Play(string animation)
	{
		if(animations.ContainsKey(animation)) SetSheet(animations[animation]);
		else if(animations.ContainsKey("Default")) SetSheet(animations["Default"]);
	}
	
	public void Queue(string animation)
	{
		if(animations.ContainsKey(animation)) QueueSheet(animations[animation]);
		else if(animations.ContainsKey("Default")) QueueSheet(animations["Default"]);
	}
	
	public void AnimationFinished(string anm = "")
	{
		if(queuedSheet is null) return;
		SetSheet((AnimationSheet)queuedSheet);
		queuedSheet = null;
	}
	
	public void AddSheet(Texture texture, string name, int frames, bool loop)
	{
		AddSheet(new AnimationSheet(texture, name, frames, loop));
	}
	
	public void AddSheet(AnimationSheet sheet)
	{
		animations.Add(sheet.name, sheet);
	}
	
	public void SetSheet(AnimationSheet sheet)
	{
		if(framePlayer is null) return;
		currentSheet = sheet;
		this.Texture = currentSheet.texture;
		this.Hframes = currentSheet.frames;
		framePlayer.Play(currentSheet.name);
		framePlayer.Advance(0);
	}
	
	public void QueueSheet(AnimationSheet sheet)
	{
		queuedSheet = sheet;
	}
	
	public void Pause() => framePlayer?.Stop(false);
	public void Continue() => framePlayer?.Play();
}

public readonly struct AnimationSheet
{
	public readonly Texture texture;
	public readonly string name;
	public readonly int frames;
	public readonly bool loop;
	
	public AnimationSheet(Texture texture, string name, int frames, bool loop)
	{
		this.texture = texture;
		this.name = name;
		this.frames = frames;
		this.loop = loop;
	}
}
