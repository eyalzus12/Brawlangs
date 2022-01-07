using Godot;
using System;
using System.Text;
using System.Collections.Generic;

public class ResultsLabel : Label
{
	public const string RESULT_STRING = "GameResults";
	
	public override void _Ready()
	{
		var data = this.GetPublicData();
		var sb = new StringBuilder("Results:\n");
		data[RESULT_STRING].lo().ForEach(rl=>rl.ls().ForEach(r=>sb.Append(r+"\n")));
		Text = sb.ToString();
		data.Remove(RESULT_STRING);
	}
}
