using Godot;
using System;
using System.Collections.Generic;

public class CharacterCreator
{
	public const string BASE_SECTION = "Main";
	
	//public string path = "res://mario.ini";
	public string path;
	public IniFile inif = new IniFile();
	
	public CharacterCreator() {}
	
	public CharacterCreator(string path)
	{
		this.path = path;
	}
	
	public Character Build(Node2D n)
	{
		//create actual character
		var ch = new Character();
		//open ini file
		var charinif = new IniFile();
		charinif.Load(path);
		var name = charinif[BASE_SECTION, "Name", ""].s();
		//load name
		ch.Name = name;
		n.AddChild(ch);
		
		ch.CollisionLayer = 0b100;
		ch.CollisionMask = 0b011;
		
		/*var oCanHitSlots = charinif[BASE_SECTION, "CanHit", new List<int>()];
		if(oCanHitSlots is int)
		{
			ch.CollisionLayer = 0;
			ch.SetCollisionLayerBit(oCanHitSlots.i() + 2)
		}
		else
		{
			
		}*/
		
		//find stat file path
		var statPath = charinif[BASE_SECTION, "Stats", ""].s();
		//load properties from stat file
		var prop = new PropertyMap();
		prop.ReadFromConfigFile(statPath, "Stats");
		prop.LoadProperties(ch);
		
		//find collision file path
		var collisionPath = charinif[BASE_SECTION, "Collision", ""].s();
		//create collision
		var collCreator = new CollisionCreator(collisionPath);
		collCreator.Build(ch);
		
		//find attack file path
		var attackPath = charinif[BASE_SECTION, "Attacks", ""].s();
		//create attacks
		var attCreator = new AttackCreator(attackPath);
		attCreator.Build(ch);
		
		/*var input = charinif[BASE_SECTION, "Input", "Buffer"].s();
		var inputs = (InputManager)ch.GetRootNode(input + "Input");
		ch.Inputs = inputs;
		foreach(var s in ch.states.Values) s.Inputs = inputs;*/
		
		var cl = new CanvasLayer();
		cl.Name = "UI";
		ch.AddChild(cl);
		var lb = new DebugLabel();
		cl.AddChild(lb);
		var dl = new DamageLabel();
		dl.ch = ch;
		dl.MarginLeft = 1000;
		dl.MarginTop = 200;
		cl.AddChild(dl);
		
		var spr = new Sprite();
		spr.Scale = new Vector2(0.25f, 0.25f);
		ch.AddChild(spr);
		var texture = ResourceLoader.Load<Texture>("res://Untitled_04-10-2021_10-51-27sus.png");
		spr.Texture = texture;
		var shader = ResourceLoader.Load<Shader>("res://palletetest.shader");
		spr.Material = new ShaderMaterial();
		(spr.Material as ShaderMaterial).Shader = shader;
		var pall = ResourceLoader.Load<Texture>("res://empty pallete.png");
		(spr.Material as ShaderMaterial).SetShaderParam("pallete", pall);
		
		return ch;
	}
}
