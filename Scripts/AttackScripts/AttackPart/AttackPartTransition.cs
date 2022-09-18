using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AttackPartTransition
{
	public List<Vector2> Frames{get; set;} = new();
	public AttackPartTransitionTagExpression TagExpression{get; set;} = new();
	public string NextPart{get; set;} = "";
	
	public AttackPartTransition() {}
	
	public AttackPartTransition(IEnumerable<Vector2> frames, AttackPartTransitionTagExpression tagExpression, string nextPart)
	{
		tagExpression.Get(null);//test expression
		TagExpression = tagExpression;
		Frames = new(frames);
		NextPart = nextPart;
	}
	
	public bool ContainsFrame(int frame) => Frames.Any(v => v.x <= frame && frame <= v.y);
	public bool MatchesTag(StateTagsManager tagsManager, int currentFrame) =>
		(currentFrame == -1 || ContainsFrame(currentFrame)) &&
		TagExpression.Get(tagsManager);
}
