using Godot;
using System;

public partial class testdamagelabelplacement : Node2D
{
	public override void _Ready()
	{
		var margin = 50f;
		var windowsize = DisplayServer.WindowGetSize();
		var topleft = Vector2.Zero;
		var bottomright = windowsize;
		var bottomleft = new Vector2(topleft.x, bottomright.y);
		var topright = new Vector2(bottomright.x, topleft.y);
		var midleft = GeometryUtils.CenterBetween(topright, bottomright);
		var midright = GeometryUtils.CenterBetween(topleft, bottomleft);
		var counts = new int[]{8};
		var locations = counts.GetLabelLocations(midleft,midright,margin);
		foreach(var v in locations)
		{
			var l = new Label();
			l.Text = "h";
			AddChild(l);
			l.Position = v;
		}
		
		counts = new int[]{4,4};
		midleft.y += 100f;
		midright.y += 100f;
		locations = counts.GetLabelLocations(midleft,midright,margin);
		foreach(var v in locations)
		{
			var l = new Label();
			l.Text = "g";
			AddChild(l);
			l.Position = v;
		}
		
		counts = new int[]{2,2,2,2};
		midleft.y += 100f;
		midright.y += 100f;
		locations = counts.GetLabelLocations(midleft,midright,margin);
		foreach(var v in locations)
		{
			var l = new Label();
			l.Text = "j";
			AddChild(l);
			l.Position = v;
		}
	}
}
