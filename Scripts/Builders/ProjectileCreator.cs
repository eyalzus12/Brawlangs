using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ProjectileCreator
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
	
	public Projectile BuildProjectile(IAttacker n, string section)
	{
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate projectile {section} as it is not a real section"); return null;}
		Projectile proj = new();
		
		proj.OwnerObject = n;
		var characterAudioManager = proj.OwnerObject.Audio;
		AudioManager am = new(2);
		am.Name = "AudioManager";
		am.Sounds = characterAudioManager.Sounds;
		proj.AddChild(am);
		proj.Audio = am;
		
		proj.Name = section;
		proj.Identifier = section;
		var sp = inif[section, "Position", Vector2.Zero].v2();
		proj.SpawningPosition = sp;
		var lt = inif[section, "LifeTime", 0].i();
		proj.MaxLifetime = lt;
		var smf = inif[section, "MovementFunction", ""].s();
		var movf = BuildMovementFunction(smf);
		proj.Movement = movf;
		
		var hitboxSections = inif[section, "Hitboxes", $"{section}Hitbox"].ls();
		foreach(var s in hitboxSections) BuildHitbox(proj, s);
		
		var hurtboxes = inif[section, "Hurtboxes", $"{section}Hurtbox"].ls();
		
		foreach(var hurtbox in hurtboxes)
		{
			Hurtbox hr = new();
			hr.Name = hurtbox;
			hr.OwnerObject = proj;
			proj.AddChild(hr);
			hr.Owner = proj;
			proj.Hurtboxes.Add(hr);
			BuildHurtbox(hr, hurtbox, "Default");
		}
		
		proj.LoadProperties();
		LoadExtraProperties(proj, section);
		
		return proj;
	}
	
	public ProjectileMovementFunction BuildMovementFunction(string section)
	{
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate movement function {section} as it is not a real section"); return null;}
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
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate hurtbox {section} as it is not a real section"); return;}
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
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate hitbox {section} as it is not a real section"); return;}
		var HitboxScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var h = TypeUtils.LoadScript<Hitbox>(HitboxScript, new Hitbox(), baseFolder);
		h.OwnerObject = p;
		h.Name = section;
		
		var sk = inif[section, "SetKnockback", Vector2.Zero].v2();
		h.SetKnockback = sk;
		var vk = inif[section, "VarKnockback", Vector2.Zero].v2();
		h.VarKnockback = vk;
		var sst = inif[section, "Stun", 0].f();
		h.SetStun = sst;
		var vst = inif[section, "VarStun", 0].f();
		h.VarStun = vst;
		var hp = inif[section, "HitPause", 0].f();
		h.SetHitpause = hp;
		var vhp = inif[section, "VarHitPause", 0].f();
		h.VarHitpause = vhp;
		var dm = inif[section, "Damage", 0f].f();
		h.Damage = dm;
		var pr = inif[section, "Priority", 0].i();
		h.HitPriority = pr;
		//var cm = inif[section, "MomentumCarry", Vector2.Zero].v2();
		//h.momentumCarry = cm;
		
		var hs = inif[section, "HitSound", "DefaultHit"].s();
		if(p.Audio.ContainsSound(hs)) h.HitSound = p.Audio[hs];
		else GD.PushError($"Hit sound {hs} for hitbox {section} in file at path {inif.FilePath} could not be found.");
		
		var hafs = inif[section, "HorizontalAngleFlipper", "Directional"].s();
		Hitbox.AngleFlipper haf;
		Enum.TryParse<Hitbox.AngleFlipper>(hafs, out haf);
		h.HorizontalAngleFlipper = haf;
		var vafs = inif[section, "VerticalAngleFlipper", "None"].s();
		Hitbox.AngleFlipper vaf;
		Enum.TryParse<Hitbox.AngleFlipper>(vafs, out vaf);
		h.VerticalAngleFlipper = vaf;
		
		var tkm = inif[section, "TeamKnockbackMultiplier", 1f].f();
		h.TeamKnockbackMult = tkm;
		var tdm = inif[section, "TeamDamageMultiplier", 1f].f();
		h.TeamDamageMult = tdm;
		var tsm = inif[section, "TeamStunMultiplier", 1].i();
		h.TeamStunMult = tsm;
		
		var whitelistedStates = inif[section, "WhitelistedStates", Enumerable.Empty<string>()].ls();
		h.WhitelistedStates = new(whitelistedStates);
		
		var blacklistedStates = inif[section, "BlacklistedStates", Enumerable.Empty<string>()].ls();
		h.BlacklistedStates = new(blacklistedStates);
		
		h.LoadProperties();
		LoadExtraProperties(h, section);
		
		//Build collision. no need for seperate function
		CollisionShape2D cs = new();
		
		var pos = inif[section, "Position", Vector2.Zero].v2();
		cs.Position = pos;
		h.OriginalPosition = pos;
		var rot = inif[section, "Rotation", 0f].f();
		var rotrad = (float)(rot*Math.PI/180f);
		cs.Rotation = rotrad;//turn to rads
		h.OriginalRotation = rotrad;
		
		CapsuleShape2D ps = new();
		
		var rd = inif[section, "Radius", 16f].f();
		ps.Radius = rd;
		var hg = inif[section, "Height", 16f].f();
		ps.Height = hg;
		
		cs.Shape = ps;
		cs.Disabled = true;
		cs.Name = section + "Collision";
		h.HitboxShape = cs;
		h.AddChild(cs);
		p.AddChild(h);
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
			loadTo.Set(objname, prop.ToVariant());
		}
	}
}
