using Godot;
using System;
using System.Collections.Generic;

public class CollisionCreator
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
	}
	
	public void BuildHurtbox(Hurtbox hr, string section, string state)
	{
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate hurtbox {section} as it is not a real section"); return;}
		var rd = inif[section, "Radius", 0f].f();
		var he = inif[section, "Height", 0f].f();
		var pos = inif[section, "Position", Vector2.Zero].v2();
		var rot = inif[section, "Rotation", 0f].f();
		rot = (float)(rot*Math.PI/180f);//to rads
		
		var shcs = inif[section, "SelfHitCondition", "DecideByFriendlyFire"].s();
		HitCondition shc;
		Enum.TryParse<HitCondition>(shcs, out shc);
		
		var thcs = inif[section, "TeamHitCondition", "DecideByFriendlyFire"].s();
		HitCondition thc;
		Enum.TryParse<HitCondition>(thcs, out thc);
		
		var hurtboxState = new HurtboxCollisionState(state, rd, he, pos, rot, shc, thc);
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
}
