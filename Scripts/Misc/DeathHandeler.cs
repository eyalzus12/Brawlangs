using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class DeathHandeler : Node
{
	[Signal]
	public delegate void MatchEnds();
	
	public List<Character> characters;
	public Stack<Character> winList;
	public int count;
	
	public DeathHandeler()
	{
		characters = new List<Character>();
		count = 0;
		winList = new Stack<Character>();
	}
	public DeathHandeler(IEnumerable<Character> cr)
	{
		characters = new List<Character>(cr);
		count = characters.Count;
		winList = new Stack<Character>();
		ConnectToDeath();
	}
	
	public void ConnectToDeath()
	{
		foreach(var c in characters)
			c.Connect("Dead", this, nameof(CharacterDead));
	}
	
	public void CharacterDead(Node2D who)
	{
		if(who is Character charGone)
		{
			characters.Remove(charGone);
			winList.Push(charGone);
			if(characters.Count <= 1)
			{
				if(characters.Count == 1) winList.Push(characters[0]);
				HandleEndOfMatch();
			}
		}
	}
	
	public virtual void HandleEndOfMatch()
	{
		EmitSignal(nameof(MatchEnds));
		var sb = new StringBuilder();
		sb.Append("\n");
		for(int i = 1; winList.Count > 0; ++i)
		{
			var ch = winList.Pop();
			sb.Append($"Character {ch.Name} of team {ch.teamNumber} is {IntToWord(i)} place\n");
		}
		GD.Print(sb.ToString());
	}
	
	private string IntToWord(int i)
	{
		string suffix(int num)
		{
			switch(num)
			{
				case 1: return "st";
				case 2: return "nd";
				case 3: return "rd";
				default: return "th";
			}
		}
		
		return i+suffix(i);
	}
}
