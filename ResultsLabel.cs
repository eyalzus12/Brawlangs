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
		var results = data[RESULT_STRING].lo();
		
		var sb = new StringBuilder("Results:\n");
		foreach(var rlo in results)
		{
			var rl = rlo.ls();
			foreach(var r in rl)
			{
				sb.Append(r + "\n");
			}
		}
		
		Text = sb.ToString();
		data.Remove(RESULT_STRING);
	}
}
