using Godot;
using System;
using System.Collections.Generic;

public class ProjectileCreator
{
	public string path;
	public IniFile inif = new IniFile();
	
	public ProjectileCreator() {}
	
	public ProjectileCreator(string path)
	{
		this.path = path;
		inif.Load(path);
	}
	
	public List<PackedScene> Build(Node2D n)
	{
		var packs = new List<PackedScene>();
		var oProjectiles = inif["", "ProjectileSections", new List<string>()];
		if(oProjectiles is string)
		{
			var pack = BuildProjectile(n, oProjectiles.s());
			packs.Add(pack);
		}
		else
		{
			var Projectiles = oProjectiles.ls();
			foreach(var s in Projectiles)
			{
				var pack = BuildProjectile(n, s);
				packs.Add(pack);
			}
		}
		
		return packs;
	}
	
	public PackedScene BuildProjectile(Node2D n, string section)
	{
		var proj = new Projectile();
		proj.owner = n;
		proj.Name = section;
		var sp = inif[section, "Position", Vector2.Zero].v2();
		proj.spawningPosition = sp;
		var lt = inif[section, "LifeTime", 1].i();
		proj.maxLifetime = lt;
		var dr = inif[section, "Direction", 1].i();//should be -1 or 1
		if(dr != 1 && dr != -1) dr = 1;//fallback
		proj.direction = dr;
		var smf = inif[section, "MovementFunction", ""].s();
		var movf = BuildMovementFunction(smf);
		proj.Movement = movf;
		
		proj.LoadProperties();
		LoadExtraProperties(proj, section);
		
		var pack = new PackedScene();
		var err = pack.Pack(proj);
		if(err != Error.Ok)
		{
			GD.Print($"Error {err} while packing projectile defined at section {section} of file {path}");
			return null;
		}
		else return pack;
	}
	
	public ProjectileMovementFunction BuildMovementFunction(string section)
	{
		var movf = new ProjectileMovementFunction();
		if(section == "") return movf;
		movf.LoadProperties();
		LoadExtraProperties(movf, section);
		return movf;
	}
	
	public void BuildHitbox(Node2D n, string section)
	{
		/*var HitboxScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var h = TypeUtils.LoadScript<Hitbox>(HitboxScript, new Hitbox(), baseFolder);*/
		var h = new Hitbox();
		h.Name = section;
		//var ch = (Character)ap.Get("ch");
		h.owner = n;
		
		var sk = inif[section, "SetKnockback", Vector2.Zero].v2();
		h.setKnockback = sk;
		var vk = inif[section, "VarKnockback", Vector2.Zero].v2();
		h.varKnockback = vk;
		var st = inif[section, "Stun", 0].i();
		h.stun = st;
		var hp = inif[section, "HitPause", 0].i();
		h.hitpause = hp;
		var hl = inif[section, "HitLag", 0].i();
		h.hitlag = hl;
		var dm = inif[section, "Damage", 0f].f();
		h.damage = dm;
		var pr = inif[section, "Priority", 0].i();
		h.hitPriority = pr;
		var cm = inif[section, "MomentumCarry", Vector2.Zero].v2();
		h.momentumCarry = cm;
		/*var hs = inif[section, "HitSound", "DefaultHit"].s();
		try
		{
			var ahs = ch.audioManager.sounds[hs];
			h.hitSound = ahs;
		}
		catch(KeyNotFoundException)
		{
			GD.Print($"Hit sound {hs} for hitbox {section} in file at path {inif.filePath} could not be found.");
		}*/
		
		/*var af = inif[section, "ActiveFrames", new List<Vector2>()];
		if(af is Vector2) h.activeFrames = new List<Vector2> {af.v2()};
		else h.activeFrames = af.lv2();*/
		
		var hafs = inif[section, "HorizontalAngleFlipper", "Directional"].s();
		Hitbox.AngleFlipper haf;
		Enum.TryParse<Hitbox.AngleFlipper>(hafs, out haf);
		h.horizontalAngleFlipper = haf;
		var vafs = inif[section, "VerticalAngleFlipper", "None"].s();
		Hitbox.AngleFlipper vaf;
		Enum.TryParse<Hitbox.AngleFlipper>(vafs, out vaf);
		h.verticalAngleFlipper = vaf;
		
		var tkm = inif[section, "TeamKnockbackMultiplier", 1f].f();
		h.teamKnockbackMult = tkm;
		var tdm = inif[section, "TeamDamageMultiplier", 1f].f();
		h.teamDamageMult = tdm;
		var tsm = inif[section, "TeamStunMultiplier", 1].i();
		h.teamStunMult = tsm;
		
		var kms = inif[section, "KnockbackStateMultipliers", ""].s();
		if(kms != "")
		{
			h.stateKnockbackMult = new Dictionary<string, float>();
			foreach(var entry in inif[kms])
			{
				var stateName = entry.Key;
				var mult = entry.Value.f();
				h.stateKnockbackMult.Add(stateName, mult);
			}
		}
		
		var dms = inif[section, "DamageStateMultipliers", ""].s();
		if(dms != "")
		{
			h.stateDamageMult = new Dictionary<string, float>();
			foreach(var entry in inif[dms])
			{
				var stateName = entry.Key;
				var mult = entry.Value.f();
				h.stateDamageMult.Add(stateName, mult);
			}
		}
		
		var sms = inif[section, "StunStateMultipliers", ""].s();
		if(sms != "")
		{
			h.stateStunMult = new Dictionary<string, float>();
			foreach(var entry in inif[sms])
			{
				var stateName = entry.Key;
				var mult = entry.Value.i();
				h.stateStunMult.Add(stateName, (float)mult);
			}
		}
		
		h.LoadProperties();
		LoadExtraProperties(h, section);
		
		//Build collision. no need for seperate function
		var cs = new CollisionShape2D();
		
		var pos = inif[section, "Position", Vector2.Zero].v2();
		cs.Position = pos;
		h.originalPosition = pos;
		var rot = inif[section, "Rotation", 0f].f();
		var rotrad = (float)(rot*Math.PI/180f);
		cs.Rotation = rotrad;//turn to rads
		h.originalRotation = rotrad;
		
		var ps = new CapsuleShape2D();
		
		var rd = inif[section, "Radius", 16f].f();
		ps.Radius = rd;
		var hg = inif[section, "Height", 16f].f();
		ps.Height = hg;
		
		cs.Shape = ps;
		cs.Disabled = true;
		cs.Name = section + "Collision";
		h.shape = cs;
		h.AddChild(cs);
		cs.Owner = h;//for scene packing
		n.AddChild(h);
		h.Owner = n;//for scene packing
	}
	
	public void LoadExtraProperties(Projectile loadTo, string section) => LoadExtraProperties(loadTo, loadTo.LoadExtraProperties, section);
	public void LoadExtraProperties(ProjectileMovementFunction loadTo, string section) => LoadExtraProperties(loadTo, loadTo.LoadExtraProperties, section);
	public void LoadExtraProperties(Hitbox loadTo, string section) => LoadExtraProperties(loadTo, loadTo.LoadExtraProperties, section);
	
	public void LoadExtraProperties(Node2D loadTo, Dictionary<string, ParamRequest> load, string section)
	{
		foreach(var entry in load)
		{
			var ininame = entry.Key;
			var request = entry.Value;
			var type = request.ParamType;
			var objname = request.ParamName;
			var @default = request.ParamDefault;
			
			var prop_obj = inif[section, ininame, @default];
			
			if(type == typeof(Hitbox))
			{
				BuildHitbox(loadTo, prop_obj.s());
			}
			else if(type == typeof(List<Hitbox>))
			{
				foreach(var h in prop_obj.ls()) BuildHitbox(loadTo, h);
			}
			else
			{
				var prop = prop_obj.cast(type, $"loading extra properties for {loadTo.GetType().Name} {section}");
				loadTo.Set(objname, prop);
			}
		}
	}
}
