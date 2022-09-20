using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public partial class DeathHandeler : Node
{
	[Signal]
	public delegate void MatchEndsEventHandler();
	
	public List<string> diedThisFrame = new();
	public List<List<string>> diedTotal = new();
	public List<Character> exists = new();
	public int count = 0;
	
	public DeathHandeler()
	{
		diedThisFrame = new();
		diedTotal = new();
		exists = new();
		count = 0;
	}
	
	public DeathHandeler(IEnumerable<Character> cr)
	{
		diedThisFrame = new();
		diedTotal = new();
		exists = new(cr);
		count = 0;
		foreach(var c in cr)
		{
			//c.Connect("Dead",new Callable(this,nameof(CharacterDead)));
			c.Dead += CharacterDead;
			++count;
		}
	}
	
	public void CharacterDead(Node2D who)
	{
		if(who is Character charGone)
		{
			var log = Log(charGone, count);
			diedThisFrame.Add(log);
			exists.Remove(charGone);
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		diedTotal.Add(new(diedThisFrame));
		count -= diedThisFrame.Count;
		diedThisFrame.Clear();
		if(count <= 1)
		{
			if(count == 1) 
			{
				var left = exists[0];
				var log = Log(left, 1);
				var toAdd = new List<string>{log};
				diedTotal.Add(toAdd);
			}
			HandleEndOfMatch();
		}
	}
	
	public virtual void HandleEndOfMatch()
	{
		diedTotal.Reverse();
		this.GetPublicData().Add("GameResults", diedTotal);
		EmitSignal(nameof(MatchEnds));
		/*var sb = new StringBuilder();
		sb.Append("\n");
		for(int i = 1; winList.Count > 0; ++i)
		{
			var ch = winList.Pop();
			if(!Godot.Object.IsInstanceValid(ch)) ch = null;
			sb.Append($"Character {ch?.Name} of team {ch?.teamNumber} is {StringUtils.IntToWord(i)} place\n");
		}
		GD.Print(sb.ToString());*/
	}
	
	public string Log(Character c, int logcount) => $"Character {c.Name} from team {c.TeamNumber} placed {StringUtils.IntToWord(logcount)}";
}
