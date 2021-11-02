using Godot;
using System;

public class DamageLabel : Label
{
	public const string PATH = "res://damage.shader";
	
	public Character ch;
	public bool DynamicText = true;
	
	public DamageLabel(): base() {}
	public DamageLabel(bool @dynamic): base() {DynamicText = @dynamic;}
	public DamageLabel(Character c): base() {ch = c;}
	public DamageLabel(Character c, bool @dynamic): base() {ch = c; DynamicText = @dynamic;}
	
	public override void _Process(float delta)
	{
		if(ch is null) return;
		AddColorOverride("font_color", DamageCalculator.DamageToColor(ch.damage));
		if(DynamicText)
		{
			Text = $"{ch.damage.ToString()} / {ch.stocks}";
			Visible = (this.GetDataOrDefault("CurrentInfoLabelCharacter",0).i() == ch.teamNumber);
		}
	}
}
