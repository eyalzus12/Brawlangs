using Godot;
using System;

public class DamageLabel : Label
{
	public const string PATH = "res://damage.shader";
	
	public Character ch;
	protected ShaderMaterial mat;
	
	public override void _Ready()
	{
		var shaderResource = ResourceLoader.Load(PATH);
		if(shaderResource is null)
		{
			GD.Print($"Failed to load shader at position {PATH} because it does not exist");
			return;
		}
		
		var shader = shaderResource as Shader;
		if(shader is null)
		{
			GD.Print($"Failed to load shader at position {PATH} because it is not a shader");
			return;
		}
		
		this.Material = new ShaderMaterial();
		mat = this.Material as ShaderMaterial;
		mat.Shader = shader;
	}
	
	public override void _Process(float delta)
	{
		if(ch is null) return;
		mat.SetShaderParam("damage", ch.damage);
		Text = $"{ch.damage.ToString()} / {ch.stocks}";
		Visible = (this.GetDataOrDefault("CurrentInfoLabelCharacter",0).i() == ch.teamNumber);
	}
}
