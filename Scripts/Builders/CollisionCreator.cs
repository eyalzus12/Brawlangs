using Godot;
using System;
using System.Collections.Generic;

public partial class CollisionCreator
{
	public const string BrawlangsServer_JoinOrBad = "https://discord.gg/ZaGfdm3bad";
	
	public IniFile inif = new IniFile();
	public string path;
	
	public CollisionCreator() {}
	
	public CollisionCreator(string path)
	{
		this.path = path;
		inif.Load(path);
	}
	
	public void Build(Character ch)
	{
		var states = inif["", "States", "Default"].ls();
		var hurtboxes = inif["", "Hurtboxes", "Hurtbox"].ls();
		
		var collision = inif["", "Collision", "Collision"].s();
		var cs = new CharacterCollision();
		cs.Name = "Collision";
		cs.owner = ch;
		ch.collision = cs;
		ch.AddChild(cs);
		
		foreach(var state in states) BuildCollision(cs, state+collision, state);
			
		foreach(var hurtbox in hurtboxes)
		{
			var hr = new Hurtbox();
			hr.Name = hurtbox;
			hr.OwnerObject = ch;
			ch.AddChild(hr);
			ch.Hurtboxes.Add(hr);
			foreach(var state in states) BuildHurtbox(hr, state+hurtbox, state);
		}
		
		/*var platDrop = new PlatformDropDetector();
		platDrop.Name = "DropDetector";
		platDrop.owner = ch;
		ch.DropDetector = platDrop;
		ch.AddChild(platDrop);*/
	}
	
	public void BuildHurtbox(Hurtbox hr, string section, string state)
	{
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate hurtbox {section} as it is not a real section"); return;}
		var rd = inif[section, "Radius", 0f].f();
		var he = inif[section, "Height", 0f].f();
		var pos = inif[section, "Position", Vector2.Zero].v2();
		var rot = inif[section, "Rotation", 0f].f();
		rot = (float)(rot*Math.PI/180f);//to rads
		var hurtboxState = new HurtboxCollisionState(state, rd, he, pos, rot);
		hr.AddState(hurtboxState);
	}
	
	public void BuildCollision(CharacterCollision cs, string section, string state)
	{
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate character collision {section} as it is not a real section"); return;}
		var ext = inif[section, "Extents", Vector2.Zero].v2();
		var pos = inif[section, "Position", Vector2.Zero].v2();
		var collisionState = new CollisionShapeState(state, ext, pos);
		cs.AddState(collisionState);
	}
	
	/*public void BuildBase(Character ch, string section)
	{
		var pos = inif[section, "CollisionPosition", Vector2.Zero].v2();
		var xt = inif[section, "CollisionExtents", new Vector2(16f, 16f)].v2();
		var rd = inif[section, "HurtboxRadius", 30].f();
		var he = inif[section, "HurtboxHeight", 16].f();
		var hpos = inif[section, "HurtboxPosition", Vector2.Zero].v2();
		var rot = inif[section, "HurtboxRotation", 0f].f();
		rot = (float)(rot*Math.PI/180f);//to rads
		ch.settings.Add(section, new CollisionSettings(section, xt, pos, rd, he, hpos, rot));
	}*/
}
