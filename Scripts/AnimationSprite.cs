using Godot;
using System;
using System.Collections.Generic;

public class AnimationSprite : Sprite
{
	public Dictionary<string, AnimationSheet> animations = new Dictionary<string, AnimationSheet>();
	
	public AnimationSheet currentSheet;
	public AnimationPlayer framePlayer;
	
	public void InitFramePlayer()
	{
		var fplayer = new AnimationPlayer();
		fplayer.PlaybackProcessMode = AnimationPlayer.AnimationProcessMode.Physics;
		fplayer.Name = "FramePlayer";
		AddChild(fplayer);
		framePlayer = GetNode<AnimationPlayer>("FramePlayer");
		foreach(var a in animations)
		{
			var animationName = a.Key;
			var animationSheet = a.Value;
			
			var anm = new Animation();
			var frameCount = animationSheet.HFrames * animationSheet.VFrames;
			anm.Length = frameCount * anm.Step;
			anm.Loop = animationSheet.loop;
			framePlayer.AddAnimation(animationName, anm);
			int trc = anm.AddTrack(Animation.TrackType.Value);
			var path = GetPath() + ":frame";
			anm.TrackSetPath(trc, path);
			
			for(int i = 0; i < frameCount; ++i)
				anm.TrackInsertKey(trc, i/10f, i);
		}
	}
	
	public void Play(string animation)
	{
		try
		{
			SetSheet(animations[animation]);
		}
		catch(KeyNotFoundException)
		{
		}
	}
	
	public void AddSheet(Texture texture, string name, int HFrames, int VFrames, bool loop)
	{
		AddSheet(new AnimationSheet(texture, name, HFrames, VFrames, loop));
	}
	
	public void AddSheet(AnimationSheet sheet)
	{
		animations.Add(sheet.name, sheet);
	}
	
	public void SetSheet(AnimationSheet sheet)
	{
		currentSheet = sheet;
		this.Texture = currentSheet.texture;
		this.Hframes = currentSheet.HFrames;
		this.Vframes = currentSheet.VFrames;
		framePlayer.Play(currentSheet.name);
	}
}

public readonly struct AnimationSheet
{
	public readonly Texture texture;
	public readonly string name;
	public readonly int HFrames;
	public readonly int VFrames;
	public readonly bool loop;
	
	public AnimationSheet(Texture texture, string name, int HFrames, int VFrames, bool loop)
	{
		this.texture = texture;
		this.name = name;
		this.HFrames = HFrames;
		this.VFrames = VFrames;
		this.loop = loop;
	}
}
