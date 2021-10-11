using Godot;
using System;
using System.Collections.Generic;

public class CollisionCreator
{
	public string path = "res://mario.ini";
	public IniFile inif = new IniFile();
	
	public CollisionCreator()
	{
		inif.Load(path);
	}
	
	public CollisionCreator(string path)
	{
		this.path = path;
		inif.Load(path);
	}
	
	public void Build(Character n)
	{
		var cs = new CollisionShape2D();
		n.AddChild(cs);
		cs.Shape = new RectangleShape2D();
		cs.Name = "Collision";
		
		var hr = new Hurtbox();
		n.AddChild(hr);
		hr.Name = "Hurtbox";
		hr.CreateCollision();
		
		n.collision = cs;
		n.hurtbox = hr;
		
		var oBase = inif["Main", "Bases", new List<string>()];
		if(oBase is string) BuildBase(n, oBase.s());
		else
		{
			var Bases = oBase.ls();
			foreach(var s in Bases) BuildBase(n, s);
		}
		
		//TODO: build platform dropping
		
		
		var platDrop = new Area2D();
		platDrop.Name = "PlatformDrop";
		n.AddChild(platDrop);
		platDrop.CollisionLayer = 0;
		platDrop.CollisionMask = 0b10;
		var droppos = inif["Main", "PlatDropPosition", new Vector2(0, 19)].v2();
		platDrop.Position = droppos;
		var dropc = new CollisionShape2D();
		platDrop.AddChild(dropc);
		dropc.Shape = new RectangleShape2D();
		dropc.Name = "DropCollision";
		var dropext = inif["Main", "PlatDropExtents", new Vector2(32, 11)].v2();
		(dropc.Shape as RectangleShape2D).Extents = dropext;
		platDrop.Connect("body_entered", n, "OnSemiSolidLeave");
		platDrop.Visible = false;
	}
	
	public void BuildBase(Character n, string section)
	{
		var pos = inif[section, "CollisionPosition", Vector2.Zero].v2();
		var xt = inif[section, "CollisionExtents", new Vector2(16f, 16f)].v2();
		var rd = inif[section, "HurtboxRadius", 30].f();
		var he = inif[section, "HurtboxHeight", 16].f();
		var hpos = inif[section, "HurtboxPosition", Vector2.Zero].v2();
		
		/*
		byte mask = 0;
		
		for(int i = 8; i >= 1; --i)
		{
			//GD.Print($"Fetching bit {i}");
			var bit = inif[section, i.ToString(), false].b();
			//GD.Print($"Current bit: {bit}");
			var b = Convert.ToByte(!bit);
			//flip and convert to byte
			//why you ask?
			//refer to this: https://discord.com/channels/789165471311462471/800276377387532328/884433912758149182
			//if you cant access it, you're not in the brawlangs discord server
			//so join it
			//if the server doesnt exist anymore... L
			//oh right the invite is https://discord.gg/ZaGfdm3bad
			//GD.Print($"As byte: {b}");
			mask |= b;
			mask <<= 1;
			//GD.Print($"Current layer: {mask}");
		}
		
		mask >>= 1;//undo last one
		UInt32 actualMask = (UInt32)mask;
		//GD.Print($"Full Layer: {actualMask}");
		actualMask <<= 3;//shift to match
		//GD.Print($"Stored Layer: {actualMask}");
		*/
		n.settings.Add(section, new CollisionSettings(xt, pos, rd, he, hpos/*, actualMask*/));
	}
}
