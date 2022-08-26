using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class AttackPartTransition
{
	public List<Vector2> Frames{get; set;} = new List<Vector2>();
	public List<(string, StateTag)> Tags{get; set;} = new List<(string, StateTag)>();
	public string NextPart{get; set;} = "";
	
	public AttackPartTransition() {}
	
	public AttackPartTransition(List<Vector2> frames, List<(string,StateTag)> tags, string nextPart)
	{
		Frames = new List<Vector2>(frames);
		Tags = new List<(string,StateTag)>(tags);
		NextPart = nextPart;
	}
	
	public bool ContainsFrame(int frame) => Frames.Any(v => v.x <= frame && frame <= v.y);
	public bool MatchesTag(StateTagsManager tagsManager, int currentFrame) =>
		(currentFrame == -1 || Frames.Any(v => v.x <= currentFrame && currentFrame <= v.y)) &&
		(Tags.Count == 0 || Tags.Any(t => tagsManager[t.Item1] == t.Item2));
}
