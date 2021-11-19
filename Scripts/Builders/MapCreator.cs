using Godot;
using System;
using System.Collections.Generic;

public class MapCreator
{
	public const string BASE_SECTION = "Base";
	
	public string path = "res://IcerMap.ini";
	public IniFile inif = new IniFile();
	
	public MapCreator() {}
	public MapCreator(string path)
	{
		this.path = path;
		inif.Load(path);
	}
	
	public bool Build(Node n)
	{
		GD.Print(inif.ToString());
		
		var list = inif[BASE_SECTION, "Platforms", null];
		if(list is null) return false;
		
		foreach(string str in list.ls()) BuildPlatform(str, n);
		
		var camera = new MatchCamera();
		camera.Name = "Camera";
		var cbounds = inif[BASE_SECTION, "CameraBounds", new Vector2(700, 500)].v2();
		camera.limits = cbounds;
		var center = inif[BASE_SECTION, "Center", new Vector2(512, 300)].v2();
		camera.middle = center;
		var zoom = inif[BASE_SECTION, "BaseZoom", 1.5f].f();
		camera.baseZoom = zoom;
		var zoff = inif[BASE_SECTION, "ZoomOffset", 0.5f].f();
		camera.zoomOffset = zoff;
		var inter = inif[BASE_SECTION, "Interpolation", 0.01f].f();
		camera.interpolationWeight = inter;
		var debug = inif[BASE_SECTION, "Debug", false].b();
		n.AddChild(camera);
		camera.Current = true;
		
		var blast = inif[BASE_SECTION, "BlastZones", new Vector2(2000, 1200)].v2();
		var bz = new BlastZone(center, blast);
		n.AddChild(bz);
		
		return true;
	}
	
	public bool BuildPlatform(String section, Node n)
	{
		section = section.Trim();
		var sp = new StaticPlatform2D();
		
		var pos = inif[section, "Position", Vector2.Zero].v2();
		sp.Position = pos;
		var rot = inif[section, "Rotation", 0f].f();
		sp.Rotation = rot;
		var fr = inif[section, "Friction", 1f].f();
		sp.PlatformFriction = fr;
		var bn = inif[section, "Bounce", 0f].f();
		sp.PlatformBounce = bn;
		var vl = inif[section, "Velocity", Vector2.Zero].v2();
		sp.PlatformLinearVelocity = vl;
		var cl = inif[section, "Clingable", true].b();
		sp.Clingable = cl;
		var ft = inif[section, "FallThrough", false].b();
		sp.FallThroughPlatform = ft;
		
		n.AddChild(sp);
		
		var cp = inif[section, "Collision", section+"col"];
		if(cp is string) BuildCollision(cp.s(), sp);
		else foreach(var s in cp.ls()) BuildCollision(s, sp);
		var sr = inif[section, "Sprite", section+"sprite"];
		if(sr is string) BuildSprite(sr.s(), sp);
		else foreach(var ss in sr.ls()) BuildCollision(ss, sp);
		
		return true;
	}
	
	public bool BuildCollision(string section, Node n)
	{
		section = section.Trim();
		var cs = new CollisionShape2D();
		
		var pos = inif[section, "Position", Vector2.Zero].v2();
		cs.Position = pos;
		var rot = inif[section, "Rotation", 0f].f();
		cs.Rotation = rot;
		var ds = inif[section, "Disabled", false].b();
		cs.Disabled = ds;
		var ow = inif[section, "OneWay", false].b();
		cs.OneWayCollision = ow;
		var type = inif[section, "Type", ""].s();
		cs.Shape = t2s(type, section);
		
		n.AddChild(cs);
		
		return true;
	}
	
	public bool BuildSprite(string section, Node n)
	{
		section = section.Trim();
		var sp = new Sprite();
		
		var pos = inif[section, "Position", Vector2.Zero].v2();
		sp.Position = pos;
		var rot = inif[section, "Rotation", 0f].f();
		sp.Rotation = rot;
		var scl = inif[section, "Scale", new Vector2(1f,1f)].v2();
		sp.Scale = scl;
		var te = inif[section, "Texture", "res://icon.png"].s();
		sp.Texture = ResourceLoader.Load(te) as Texture;
		var zz = inif[section, "Z", 1].i();
		sp.ZIndex = zz;
		
		n.AddChild(sp);
		
		return true;
	}
	
	private Shape2D t2s(string type, string section)
	{
		switch(type)
		{
			case "Rectangle":
				return s2r(section);
			case "Circle":
				return s2c(section);
			case "Capsule":
				return s2p(section);
			case "Line":
				return s2l(section);
			case "ConvexPolygon":
				return s2xp(section);
			case "ConvcavePolygon":
				return s2vp(section);
			case "Segment":
				return s2s(section);
			default:
				return null;
		}
	}
	
	private RectangleShape2D s2r(string section)
	{
		var rs = new RectangleShape2D();
		
		var ex = inif[section, "Extents", new Vector2(16f,16f)].v2();
		rs.Extents = ex;
		
		return rs;
	}
	
	private CircleShape2D s2c(string section)
	{
		var cs = new CircleShape2D();
		
		var rd = inif[section, "Radius", 16f].f();
		cs.Radius = rd;
		
		return cs;
	}
	
	private CapsuleShape2D s2p(string section)
	{
		var ps = new CapsuleShape2D();
		
		var rd = inif[section, "Radius", 16f].f();
		ps.Radius = rd;
		var hg = inif[section, "Height", 16f].f();
		ps.Height = hg;
		
		return ps;
	}
	
	private LineShape2D s2l(string section)
	{
		var ls = new LineShape2D();
		
		var nr = inif[section, "Normal", new Vector2(0, -1)].v2();
		ls.Normal = nr;
		var ds = inif[section, "Distance", 0f].f();
		ls.D = ds;
		
		return ls;
	}
	
	private ConvexPolygonShape2D s2xp(string section)
	{
		var xps = new ConvexPolygonShape2D();
		
		var vl = inif[section, "Points", null].lv2();
		xps.Points = vl.ToArray();
		
		return xps;
	}
	
	private ConcavePolygonShape2D s2vp(string section)
	{
		var vps = new ConcavePolygonShape2D();
		
		var vl = inif[section, "Segments", null].lv2();
		vps.Segments = vl.ToArray();
		
		return vps;
	}
	
	private SegmentShape2D s2s(string section)
	{
		var ss = new SegmentShape2D();
		
		var a = inif[section, "PointA", Vector2.Zero].v2();
		ss.A = a;
		var b = inif[section, "PointB", Vector2.Zero].v2();
		ss.B = b;
		
		return ss;
	}
}
