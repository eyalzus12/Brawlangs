using Godot;
using System;
using System.Collections.Generic;
using Conn = System.Collections.Generic.Dictionary<string, (string, Attack)>;

public class AttackCreator
{
	public string path;
	public IniFile inif = new IniFile();
	
	private Conn cn = new Conn();
	//part name - connection section name
	
	public AttackCreator()
	{
		cn = new Conn();
	}
	
	public AttackCreator(string path)
	{
		this.path = path;
		inif.Load(path);
		cn = new Conn();
	}
	
	public void Build(Node2D n)
	{
		cn = new Conn();
		var oAttacks = inif["", "AttackSections", new List<string>()];
		if(oAttacks is string)
			BuildAttack(n, oAttacks.s());
		else
		{
			var Attacks = oAttacks.ls();
			foreach(var s in Attacks)
				BuildAttack(n, s);
		}
		
		BuildConnections();
	}
	
	public void BuildAttack(Node n, string section)
	{
		var AttackScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var a = TypeUtils.LoadScript<Attack>(AttackScript, new Attack(), baseFolder);
		var ch = n as Character;
		a.OwnerObject = ch;
		a.Name = section;
		
		var fric = inif[section, "Friction", 1f].f();
		a.AttackFriction = fric;
		
		object oSharesCooldownWith = inif[section, "SharesCooldownWith", new List<string>()];
		if(oSharesCooldownWith is string CooldownShareStr) a.SharesCooldownWith.Add(CooldownShareStr);
		else a.SharesCooldownWith.AddRange(oSharesCooldownWith.ls());
		
		var StartPartSection = inif[section, "StartPart", ""].s();
		object oPartSections = inif[section, "Parts", new List<string>()];
		if(oPartSections is string PartSection)
			BuildPart(a, PartSection, StartPartSection);
		else
		{
			var PartSections = oPartSections.ls();
			foreach(var s in PartSections)
				BuildPart(a, s, StartPartSection);
		}
		
		a.LoadProperties();
		LoadExtraProperties(a, section);
		
		n.AddChild(a);
		a.Owner = n;//for scene packing
		//ch.attacks.Add(a);
		ch.Attacks.Add(a.Name, a);
		//ch.cooldowns.Add(a.Name, 0);
	}
	
	public AttackPart BuildPart(Attack a, string section, string start)
	{
		var PartScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var ap = TypeUtils.LoadScript<AttackPart>(PartScript, new AttackPart(), baseFolder);
		ap.att = a;
		ap.OwnerObject = a.OwnerObject;
		ap.Name = section;
		a.Parts.Add(section, ap);
		if(section == start) a.StartPart = ap;
		
		var su = inif[section, "Startup", 0].i();
		ap.Startup = su;
		var ln = inif[section, "Length", 0].i();
		ap.Length = ln;
		var el = inif[section, "Endlag", 0].i();
		ap.Endlag = el;
		var cd = inif[section, "Cooldown", 0].i();
		ap.Cooldown = cd;
		var mv = inif[section, "Movement", Vector2.Zero].v2();
		ap.Movement = mv;
		var ox = inif[section, "OverwriteXMovement", false].b();
		ap.OverwriteXMovement = ox;
		var oy = inif[section, "OverwriteYMovement", false].b();
		ap.OverwriteYMovement = oy;
		var dfa = inif[section, "DriftForwardAcceleration", 0f].f();
		ap.DriftForwardAcceleration = dfa;
		var dfs = inif[section, "DriftForwardSpeed", 0f].f();
		ap.DriftForwardSpeed = dfs;
		var dba = inif[section, "DriftBackwardsAcceleration", 0f].f();
		ap.DriftBackwardsAcceleration = dba;
		var dbs = inif[section, "DriftBackwardsSpeed", 0f].f();
		ap.DriftBackwardsSpeed = dbs;
		var me = inif[section, "MissEndlag", 0].i();
		ap.MissEndlag = me;
		var mc = inif[section, "MissCooldown", 0].i();
		ap.MissCooldown = mc;
		var gm = inif[section, "GravityMultiplier", 1f].f();
		ap.GravityMultiplier = gm;
		var dm = inif[section, "DamageMult", 1f].f();
		ap.DamageMult = dm;
		var km = inif[section, "KnockbackMult", 1f].f();
		ap.KnockbackMult = km;
		var sm = inif[section, "StunMult", 1f].f();
		ap.StunMult = sm;
		var sa = inif[section, "StartupAnimation", "Default"].s();
		ap.StartupAnimation = sa;
		var aa = inif[section, "AttackAnimation", "Default"].s();
		ap.AttackAnimation = aa;
		var ea = inif[section, "EndlagAnimation", "Default"].s();
		ap.EndlagAnimation = ea;
		var ps = inif[section, "Sound", ""].s();
		ap.AttackSound = ps;
		var ep = inif[section, "EmittedProjectiles", new List<string>()];
		if(ep is string sep) ap.EmittedProjectiles = new List<string>{sep};
		else ap.EmittedProjectiles = ep.ls();
		
		var oHitboxSections = inif[section, "Hitboxes", null];
		if(oHitboxSections is string)
			BuildHitbox(ap, oHitboxSections.s());
		else if(oHitboxSections is object)//not null
		{
			var HitboxSections = oHitboxSections.ls();
			foreach(var s in HitboxSections) BuildHitbox(ap, s);
		}
		
		var ConnectionSection = inif[section, "Connections", ""].s();
		//get connection section
		if(ConnectionSection != "") cn.Add(section, (ConnectionSection, a));
		//request connection for later
		
		ap.LoadProperties();
		LoadExtraProperties(ap, section);
		
		a.AddChild(ap);
		ap.Owner = a;//for scene packing
		
		return ap;
	}
	
	public void BuildHitbox(AttackPart ap, string section)
	{
		var HitboxScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var h = TypeUtils.LoadScript<Hitbox>(HitboxScript, new Hitbox(), baseFolder);
		h.Name = section;
		h.OwnerObject = ap;
		
		var sk = inif[section, "SetKnockback", Vector2.Zero].v2();
		h.SetKnockback = sk;
		var vk = inif[section, "VarKnockback", Vector2.Zero].v2();
		h.VarKnockback = vk;
		var st = inif[section, "Stun", 0f].f();
		h.Stun = st;
		var hl = inif[section, "HitLag", 0].i();
		h.Hitlag = hl;
		var hp = inif[section, "ExtraOpponentHitlag", 0].i();
		h.Hitpause = hl+hp;
		var dm = inif[section, "Damage", 0f].f();
		h.Damage = dm;
		var pr = inif[section, "Priority", 0].i();
		h.HitPriority = pr;
		var cm = inif[section, "MomentumCarry", Vector2.Zero].v2();
		h.MomentumCarry = cm;
		var hs = inif[section, "HitSound", "DefaultHit"].s();
		if(ap.OwnerObject.Audio.ContainsSound(hs)) h.HitSound = ap.OwnerObject.Audio[hs];
		else GD.Print($"Hit sound {hs} for hitbox {section} in file at path {inif.filePath} could not be found.");
		
		var af = inif[section, "ActiveFrames", new List<Vector2>()];
		if(af is Vector2) h.ActiveFrames = new List<Vector2>{af.v2()};
		else h.ActiveFrames = af.lv2();
		
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
		var tsm = inif[section, "TeamStunMultiplier", 1f].f();
		h.TeamStunMult = tsm;
		
		var oWhitelistedStates = inif[section, "WhitelistedStates", null];
		if(oWhitelistedStates is string)
			h.WhitelistedStates.Add(oWhitelistedStates.s());
		else if(oWhitelistedStates is object)//not null
		{
			var whitelistedStates = oWhitelistedStates.ls();
			foreach(var s in whitelistedStates) h.WhitelistedStates.Add(s);
		}
		
		var oBlacklistedStates = inif[section, "BlacklistedStates", null];
		if(oBlacklistedStates is string)
			h.BlacklistedStates.Add(oBlacklistedStates.s());
		else if(oBlacklistedStates is object)//not null
		{
			var blacklistedStates = oBlacklistedStates.ls();
			foreach(var s in blacklistedStates) h.BlacklistedStates.Add(s);
		}
		
		h.LoadProperties();
		LoadExtraProperties(h, section);
		
		//Build collision. no need for seperate function
		var cs = new CollisionShape2D();
		
		var pos = inif[section, "Position", Vector2.Zero].v2();
		cs.Position = pos;
		h.OriginalPosition = pos;
		var rot = inif[section, "Rotation", 0f].f();
		var rotrad = (float)(rot*Math.PI/180f);
		cs.Rotation = rotrad;//turn to rads
		h.OriginalRotation = rotrad;
		
		var ps = new CapsuleShape2D();
		
		var rd = inif[section, "Radius", 0f].f();
		ps.Radius = rd;
		var hg = inif[section, "Height", 0f].f();
		ps.Height = hg;
		
		cs.Shape = ps;
		cs.Disabled = true;
		cs.Name = section + "Collision";
		h.HitboxShape = cs;
		h.AddChild(cs);
		cs.Owner = h;//for scene packing
		ap.AddChild(h);
		h.Owner = ap;//for scene packing
	}
	
	public void BuildConnections()
	{
		foreach(var entry in cn)//go over requests
		{
			Attack a = entry.Value.Item2;
			AttackPart ap = a.Parts[entry.Key];//get requester
			var conndict = inif.dict[entry.Value.Item1];//get dictionary of connection
			foreach(var connection in conndict)//go over it
			{
				//get requested to connect to
				AttackPart toConnect = a.Parts[connection.Value.s()];
				//connect
				ap.Connect(connection.Key, toConnect);
			}
		}
	}
	
	/*
	the following is a quick fix for being unable to get the load request dictionary
	when using the Get method for godot objects, it refuses to be casted into the dictionary
	oh well. this works ok.
	*/
	
	public void LoadExtraProperties(Attack loadTo, string section) => LoadExtraProperties(loadTo, loadTo.LoadExtraProperties, section);
	public void LoadExtraProperties(AttackPart loadTo, string section) => LoadExtraProperties(loadTo, loadTo.LoadExtraProperties, section);
	public void LoadExtraProperties(Hitbox loadTo, string section) => LoadExtraProperties(loadTo, loadTo.LoadExtraProperties, section);
	
	public void LoadExtraProperties(Godot.Object loadTo, Dictionary<string, ParamRequest> load, string section)
	{
		foreach(var entry in load)
		{
			var ininame = entry.Key;
			var request = entry.Value;
			var type = request.ParamType;
			var objname = request.ParamName;
			var @default = request.ParamDefault;
			var prop = inif[section, ininame, @default].cast(type, $"loading extra properties for {loadTo.GetType().Name} {section}");
			loadTo.Set(objname, prop);
		}
	}
}
