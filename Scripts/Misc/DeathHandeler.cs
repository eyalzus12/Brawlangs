using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class DeathHandeler : Node
{
	[Signal]
	public delegate void MatchEnds();
	
	public List<string> diedThisFrame;
	public List<List<string>> diedTotal;
	public List<Character> exists;
	public int count = 0;
	
	public DeathHandeler()
	{
		diedThisFrame = new List<string>();
		diedTotal = new List<List<string>>();
		exists = new List<Character>();
		count = 0;
	}
	
	public DeathHandeler(IEnumerable<Character> cr)
	{
		diedThisFrame = new List<string>();
		diedTotal = new List<List<string>>();
		exists = new List<Character>(cr);
		count = 0;
		foreach(var c in cr)
		{
			c.Connect("Dead", this, nameof(CharacterDead));
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
	
	public override void _PhysicsProcess(float delta)
	{
		diedTotal.Add(new List<string>(diedThisFrame));
		count -= diedThisFrame.Count;
		diedThisFrame.Clear();
		if(count <= 1)
		{
			if(count == 1) 
			{
				var left = exists[0];
				var log = Log(left, 1);
				var toAdd = new List<string>(new string[]{log});
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
	
	public string Log(Character c, int logcount) => $"Character {c.Name} from team {c.teamNumber} placed {StringUtils.IntToWord(logcount)}";
}
