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
	
	/*public List<(string,Projectile)> Build(Node2D n)
	{
		var packs = new List<(string,Projectile)>();
		var oProjectiles = inif["", "Projectiles", new List<string>()];
		if(oProjectiles is string sproj)
		{
			var pack = BuildProjectile(n, sproj);
			packs.Add((sproj,pack));
		}
		else
		{
			var Projectiles = oProjectiles.ls();
			foreach(var s in Projectiles)
			{
				var pack = BuildProjectile(n, s);
				packs.Add((s,pack));
			}
		}
		
		return packs;
	}*/
	
	public Projectile BuildProjectile(Node2D n, string section)
	{
		var proj = new Projectile();
		
		proj.OwnerObject = (IAttacker)n;
		var characterAudioManager = n.Get("audioManager") as AudioManager;
		var am = new AudioManager(2);
		am.Name = "AudioManager";
		am.sounds = characterAudioManager.sounds;
		proj.AddChild(am);
		proj.audioManager = am;
		
		proj.Name = section;
		proj.identifier = section;
		var sp = inif[section, "Position", Vector2.Zero].v2();
		proj.spawningPosition = sp;
		var lt = inif[section, "LifeTime", 0].i();
		proj.maxLifetime = lt;
		var smf = inif[section, "MovementFunction", ""].s();
		var movf = BuildMovementFunction(smf);
		proj.Movement = movf;
		
		var oHitboxSections = inif[section, "Hitboxes", $"{section}Hitbox"];
		if(oHitboxSections is string hitboxSection)
			BuildHitbox(proj, hitboxSection);
		else if(oHitboxSections is object)//not null
		{
			var HitboxSections = oHitboxSections.ls();
			foreach(var s in HitboxSections) BuildHitbox(proj, s);
		}
		
		var oHurtboxes = inif[section, "Hurtboxes", $"{section}Hurtbox"];
		
		if(oHurtboxes is string hurtbox)
		{
			var hr = new Hurtbox();
			hr.Name = hurtbox;
			hr.owner = proj;
			proj.AddChild(hr);
			hr.Owner = proj;
			proj.Hurtboxes.Add(hr);
			BuildHurtbox(hr, hurtbox, "Default");
		}
		else foreach(var hurtboox in oHurtboxes.ls())
		{
			var hr = new Hurtbox();
			hr.Name = hurtboox;
			hr.owner = proj;
			proj.AddChild(hr);
			hr.Owner = proj;
			proj.Hurtboxes.Add(hr);
			BuildHurtbox(hr, hurtboox, "Default");
		}
		
		proj.LoadProperties();
		LoadExtraProperties(proj, section);
		
		return proj;
	}
	
	public ProjectileMovementFunction BuildMovementFunction(string section)
	{
		var movementScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var movf = TypeUtils.LoadScript<ProjectileMovementFunction>(movementScript, new ProjectileMovementFunction(), baseFolder);
		if(section == "") return movf;
		movf.LoadProperties();
		LoadExtraProperties(movf, section);
		return movf;
	}
	
	public void BuildHurtbox(Hurtbox hr, string section, string state)
	{
		var rd = inif[section, "Radius", 0f].f();
		var he = inif[section, "Height", 0f].f();
		var pos = inif[section, "Position", Vector2.Zero].v2();
		var rot = inif[section, "Rotation", 0f].f();
		rot = (float)(rot*Math.PI/180f);//to rads
		var hurtboxState = new HurtboxCollisionState(state, rd, he, pos, rot);
		hr.AddState(hurtboxState);
	}
	
	public void BuildHitbox(Projectile p, string section)
	{
		var HitboxScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var h = TypeUtils.LoadScript<Hitbox>(HitboxScript, new Hitbox(), baseFolder);
		h.owner = p;
		h.Name = section;
		
		var sk = inif[section, "SetKnockback", Vector2.Zero].v2();
		h.setKnockback = sk;
		var vk = inif[section, "VarKnockback", Vector2.Zero].v2();
		h.varKnockback = vk;
		var st = inif[section, "Stun", 0].i();
		h.stun = st;
		var hp = inif[section, "HitPause", 0].i();
		h.hitpause = hp;
		var dm = inif[section, "Damage", 0f].f();
		h.damage = dm;
		var pr = inif[section, "Priority", 0].i();
		h.hitPriority = pr;
		//var cm = inif[section, "MomentumCarry", Vector2.Zero].v2();
		//h.momentumCarry = cm;
		
		var hs = inif[section, "HitSound", "DefaultHit"].s();
		if(p.audioManager.sounds.ContainsKey(hs)) h.hitSound = p.audioManager.sounds[hs];
		else GD.Print($"Hit sound {hs} for hitbox {section} in file at path {inif.filePath} could not be found.");
		
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
		
		var oWhitelistedStates = inif[section, "WhitelistedStates", null];
		if(oWhitelistedStates is string)
			h.whitelistedStates.Add(oWhitelistedStates.s());
		else if(oWhitelistedStates is object)//not null
		{
			var whitelistedStates = oWhitelistedStates.ls();
			foreach(var s in whitelistedStates) h.whitelistedStates.Add(s);
		}
		
		var oBlacklistedStates = inif[section, "BlacklistedStates", null];
		if(oBlacklistedStates is string)
			h.blacklistedStates.Add(oBlacklistedStates.s());
		else if(oBlacklistedStates is object)//not null
		{
			var blacklistedStates = oBlacklistedStates.ls();
			foreach(var s in blacklistedStates) h.blacklistedStates.Add(s);
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
		//cs.Owner = h;//for scene packing
		p.AddChild(h);
		//h.Owner = p;//for scene packing
		p.Hitboxes.Add(h);
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
			var prop = prop_obj.cast(type, $"loading extra properties for {loadTo.GetType().Name} {section}");
			loadTo.Set(objname, prop);
		}
	}
}
