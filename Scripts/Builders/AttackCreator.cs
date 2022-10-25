using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class AttackCreator
{
	public string path;
	public IniFile inif = new IniFile();
	
	public Dictionary<string,TagExpression> TopLevelExpressions{get; set;} = new Dictionary<string,TagExpression>();
	public Dictionary<string,TagExpression> AttackLevelExpressions{get; set;} = new Dictionary<string,TagExpression>();
	public Dictionary<string,TagExpression> AttackPartLevelExpressions{get; set;} = new Dictionary<string,TagExpression>();
	
	public AttackCreator() {}
	
	public AttackCreator(string path)
	{
		this.path = path;
		inif.Load(path);
	}
	
	public const string TAG_EXPRESSION_PATTERN = @"^TagExpression_(?<name>.*?)$";
	public static readonly Regex TAG_EXPRESSION_REGEX = new Regex(TAG_EXPRESSION_PATTERN, RegexOptions.Compiled);
	public void Build(Node2D n)
	{
		foreach(var key in inif[""].Keys)
		{
			var match = TAG_EXPRESSION_REGEX.Match(key);
			if(!match.Success) continue;
			var name = match.Groups["name"].Value;
			var expression = new TagExpression(inif["",key,""].s());
			TopLevelExpressions.Add(name,expression);
		}
		
		var attacks = inif["", "AttackSections", Enumerable.Empty<string>()].ls();
		foreach(var s in attacks) BuildAttack(n, s);
	}
	
	public void BuildAttack(Node n, string section)
	{
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate attack {section} as it is not a real section"); return;}
		
		foreach(var key in inif[section].Keys)
		{
			var match = TAG_EXPRESSION_REGEX.Match(key);
			if(!match.Success) continue;
			var name = match.Groups["name"].Value;
			var expression = new TagExpression(inif["",key,""].s());
			AttackLevelExpressions.Add(name,expression);
		}
		
		var AttackScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var a = TypeUtils.LoadScript<Attack>(AttackScript, new Attack(), baseFolder);
		var ch = n as Character;
		a.OwnerObject = ch;
		a.Name = section;
		
		var fric = inif[section, "Friction", 1f].f();
		a.AttackFriction = fric;
		
		var sharesCooldownWith = inif[section, "SharesCooldownWith", Enumerable.Empty<string>()].ls();
		a.SharesCooldownWith.AddRange(sharesCooldownWith);
		
		var startPartSection = inif[section, "StartPart", ""].s();
		var partSections = inif[section, "Parts", Enumerable.Empty<string>()].ls();
		foreach(var s in partSections) BuildPart(a, s, startPartSection);
		
		a.LoadProperties();
		LoadExtraProperties(a, section);
		
		n.AddChild(a);
		a.Owner = n;//for scene packing
		ch.Attacks.Add(a.Name, a);
		
		AttackLevelExpressions.Clear();
	}
	
	public const string PROPERTY_FRAME_PATTERN = @"^PropertyFrame_(?<name>.*?)$";
	public static readonly Regex PROPERTY_FRAME_REGEX = new Regex(PROPERTY_FRAME_PATTERN, RegexOptions.Compiled);
	public const string CONDITION_PATTERN = @"^Condition_(?<type>Trans|Tag|State)_(?<expression>.*?)_(?<frames>.*?)$";
	public static readonly Regex CONDITION_REGEX = new Regex(CONDITION_PATTERN, RegexOptions.Compiled);
	public AttackPart BuildPart(Attack a, string section, string start)
	{
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate attack part {section} as it is not a real section"); return null;}
		
		foreach(var key in inif[section].Keys)
		{
			var match = TAG_EXPRESSION_REGEX.Match(key);
			if(!match.Success) continue;
			var name = match.Groups["name"].Value;
			var expression = new TagExpression(inif["",key,""].s());
			AttackPartLevelExpressions.Add(name,expression);
		}
		
		var PartScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var ap = TypeUtils.LoadScript<AttackPart>(PartScript, new AttackPart(), baseFolder);
		ap.OwnerAttack = a;
		ap.OwnerObject = a.OwnerObject;
		ap.Name = section;
		a.Parts.Add(section, ap);
		if(section == start) a.StartPart = ap;
		
		foreach(var key in inif[section].Keys)
		{
			var match = PROPERTY_FRAME_REGEX.Match(key);
			if(match.Success) ApplyAttackPartPropertyFrame(ap, section, key, match);
			else
			{
				match = CONDITION_REGEX.Match(key);
				if(match.Success) ApplyAttackPartCondition(ap, section, key, match);
			}
		}
		
		var su = inif[section, "Startup", 0].i();
		ap.Startup = su;
		var ln = inif[section, "Length", 0].i();
		ap.Length = ln;
		var cd = inif[section, "Cooldown", 0].i();
		ap.Cooldown = cd;
		var mv = inif[section, "Movement", Vector2.Zero].v2();
		ap.Movement = mv;
		var mp = inif[section, "MomentumPreservation", Vector2.One].v2();
		ap.MomentumPreservation = mp;
		var bmp = inif[section, "BurstMomentumPreservation", Vector2.One].v2();
		ap.BurstMomentumPreservation = bmp;
		var mmmd = inif[section, "MakeMomentumMatchDirection", false].b();
		ap.MakeMomentumMatchDirection = mmmd;
		var mbmmd = inif[section, "MakeBurstMomentumMatchDirection", false].b();
		ap.MakeBurstMomentumMatchDirection = mbmmd;
		var dfa = inif[section, "DriftForwardAcceleration", 0f].f();
		ap.DriftForwardAcceleration = dfa;
		var dfs = inif[section, "DriftForwardSpeed", 0f].f();
		ap.DriftForwardSpeed = dfs;
		var dba = inif[section, "DriftBackwardsAcceleration", 0f].f();
		ap.DriftBackwardsAcceleration = dba;
		var dbs = inif[section, "DriftBackwardsSpeed", 0f].f();
		ap.DriftBackwardsSpeed = dbs;
		var ws = inif[section, "SlowOnWalls", true].b();
		ap.SlowOnWalls = ws;
		var ffl = inif[section, "FastFallLocked", false].b();
		ap.FastFallLocked = ffl;
		var gm = inif[section, "GravityMultiplier", 1f].f();
		ap.GravityMultiplier = gm;
		var dm = inif[section, "DamageDoneMult", 1f].f();
		ap.DamageDoneMult = dm;
		var km = inif[section, "KnockbackDoneMult", 1f].f();
		ap.KnockbackDoneMult = km;
		var sm = inif[section, "StunDoneMult", 1f].f();
		ap.StunDoneMult = sm;
		var aa = inif[section, "Animation", ""].s();
		ap.AttackAnimation = aa;
		var ps = inif[section, "Sound", ""].s();
		ap.AttackSound = ps;
		
		var ep = inif[section, "EmittedProjectiles", Enumerable.Empty<string>()].ls();
		ap.EmittedProjectiles = ep;
		
		var hitboxSections = inif[section, "Hitboxes", Enumerable.Empty<string>()].ls();
		foreach(var s in hitboxSections) BuildHitbox(ap, s);
		
		ap.LoadProperties();
		LoadExtraProperties(ap, section);
		
		a.AddChild(ap);
		ap.Owner = a;//for scene packing
		
		AttackPartLevelExpressions.Clear();
		
		return ap;
	}
	
	public void ApplyAttackPartPropertyFrame(AttackPart ap, string section, string key, Match match)
	{
		var name = match.Groups["name"].Value;
		var frames = inif[section, key, Enumerable.Empty<Vector2>()].lv2();
		foreach(var frame in frames)
			ap.FramePropertyManager.Add((long)frame.x, (long)frame.y, name);
	}
	
	public void ApplyAttackPartCondition(AttackPart ap, string section, string key, Match match)
	{
		var groups = match.Groups;
		var type = groups["type"].Value;
		var expression = groups["expression"].Value;
		var frames = groups["frames"].Value;
		var result = inif[section, key, ""].s();
		
		TagExpression t;
		if(
			!AttackPartLevelExpressions.TryGetValue(expression, out t) &&
			!AttackLevelExpressions.TryGetValue(expression, out t) &&
			!TopLevelExpressions.TryGetValue(expression, out t)
		) t = new TagExpression();
		
		var apc = new AttackPartCondition(frames, t, result);
		
		switch(type)
		{
			case "Tag":
				ap.TagConditionManager.Add(apc);
				break;
			case "Trans":
				ap.TransConditionManager.Add(apc);
				break;
			case "State":
				ap.StateConditionManager.Add(apc);
				break;
			default:
				GD.PushError($"Unkown condition type {type} in {inif.FilePath} section {section} key {key}");
				break;
		}
	}
	
	public void BuildHitbox(AttackPart ap, string section)
	{
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate hitbox {section} as it is not a real section"); return;}
		var HitboxScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var h = TypeUtils.LoadScript<Hitbox>(HitboxScript, new Hitbox(), baseFolder);
		h.Name = section;
		h.OwnerObject = ap;
		
		var sk = inif[section, "SetKnockback", Vector2.Zero].v2();
		h.SetKnockback = sk;
		var vk = inif[section, "VarKnockback", Vector2.Zero].v2();
		h.VarKnockback = vk;
		var sst = inif[section, "Stun", 0f].f();
		h.SetStun = sst;
		var vst = inif[section, "VarStun", 0f].f();
		h.VarStun = vst;
		var shl = inif[section, "HitLag", 0].f();
		h.SetHitLag = shl;
		var vhl = inif[section, "VarHitLag", 0].f();
		h.VarHitLag = vhl;
		var hp = inif[section, "ExtraOpponentHitLag", 0].f();
		h.SetHitPause = shl+hp;
		var ehp = inif[section, "ExtraOpponentVarHitLag", 0].f();
		h.VarHitPause = ehp+vhl;
		var dm = inif[section, "Damage", 0f].f();
		h.Damage = dm;
		var pr = inif[section, "Priority", 0].i();
		h.HitPriority = pr;
		var cm = inif[section, "MomentumCarry", Vector2.Zero].v2();
		h.MomentumCarry = cm;
		var hs = inif[section, "HitSound", "DefaultHit"].s();
		var ch = ap.OwnerObject;
		if(ch.Audio.ContainsSound(ch.AudioPrefix, hs)) h.HitSound = ap.OwnerObject.Audio[ch.AudioPrefix, hs];
		else GD.PushError($"Hit sound {hs} for hitbox {section} in file at path {inif.FilePath} could not be found.");
		
		var af = inif[section, "ActiveFrames", Enumerable.Empty<Vector2>()].lv2();
		h.ActiveFrames = af;
		
		var hafs = inif[section, "HorizontalAngleFlipper", "Directional"].s();
		Hitbox.AngleFlipper haf;
		Enum.TryParse<Hitbox.AngleFlipper>(hafs, out haf);
		h.HorizontalAngleFlipper = haf;
		var vafs = inif[section, "VerticalAngleFlipper", "None"].s();
		Hitbox.AngleFlipper vaf;
		Enum.TryParse<Hitbox.AngleFlipper>(vafs, out vaf);
		h.VerticalAngleFlipper = vaf;
		
		var shcs = inif[section, "SelfHitCondition", "DecideByFriendlyFire"].s();
		HitCondition shc;
		Enum.TryParse<HitCondition>(shcs, out shc);
		h.SelfHitCondition = shc;
		var thcs = inif[section, "TeamHitCondition", "DecideByFriendlyFire"].s();
		HitCondition thc;
		Enum.TryParse<HitCondition>(thcs, out thc);
		h.TeamHitCondition = thc;
		
		var tkm = inif[section, "TeamKnockbackMultiplier", 1f].f();
		h.TeamKnockbackMult = tkm;
		var tdm = inif[section, "TeamDamageMultiplier", 1f].f();
		h.TeamDamageMult = tdm;
		var tsm = inif[section, "TeamStunMultiplier", 1f].f();
		h.TeamStunMult = tsm;
		
		var whitelistedStates = inif[section, "WhitelistedStates", Enumerable.Empty<string>()].ls();
		h.WhitelistedStates = new HashSet<string>(whitelistedStates);
		
		var blacklistedStates = inif[section, "BlacklistedStates", Enumerable.Empty<string>()].ls();
		h.BlacklistedStates = new HashSet<string>(blacklistedStates);
		
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
		ap.Hitboxes.Add(h);
		h.Owner = ap;//for scene packing
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
