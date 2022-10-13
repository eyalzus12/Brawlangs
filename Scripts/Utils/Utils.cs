using Godot;
using System;
using System.Collections.Generic;

public static class Utils
{
	public static Node Root(this Node n) => n.GetTree().Root;
	public static Node GetRootNode(this Node n, string s) => n.Root().GetNode(s);
	public static T GetRootNode<T>(this Node n, string s) where T : Node => n.Root().GetNode<T>(s);
	
	public static void ChangeScene(this Node n, string path) => n.GetTree().ChangeScene(path);//n.GetTree().CallDeferred("change_scene", path);
	
	public static float Index(this Vector2 v, int i) => (i==0)?v.x:v.y;
	public static float Index(this Vector3 v, int i) => (i==0)?v.x:(i==1)?v.y:v.z;
	public static float Index(this Quat q, int i) => (i==0)?q.x:(i==1)?q.y:(i==2)?q.z:q.w;
	
	public static T GetProp<T>(this Godot.Object o, string s, T @default = default(T)) => (T)(o.Get(s)??@default);
	
	public static Action<T> Chain<T>(this Action<T> a1, Action<T> a2) => (T t) => {a1(t); a2(t);};
	
	public static Action<T> Chain<T>(this IEnumerable<Action<T>> ae) => (T t) => {foreach(var a in ae) a(t);};
	
	public static Action<T> Chain<T>(this Action<T> a1, params Action<T>[] ae) => (T t) => {a1(t); ae.Chain<T>()(t);};
	public static Action<T> Chain<T>(this Action<T> a1, IEnumerable<Action<T>> ae) => (T t) => {a1(t); ae.Chain<T>()(t);};
	
	public static bool IsActionIgnoreDevice(this InputEvent e, string action)
	{
		var temp = e.Device;
		e.Device = (e is InputEventKey)?0:-1;
		var result = e.IsAction(action);
		e.Device = temp;
		return result;
	}
	
	public static InputEvent Copy(this InputEvent e)
	{
		if(e is InputEventJoypadMotion j)
		{
			var r = new InputEventJoypadMotion();
			r.Axis = j.Axis;
			r.AxisValue = j.AxisValue;
			r.Device = e.Device;
			return r;
		}
		else if(e is InputEventJoypadButton b)
		{
			var r = new InputEventJoypadButton();
			r.ButtonIndex = b.ButtonIndex;
			r.Pressure = b.Pressure;
			r.Pressed = b.Pressed;
			r.Device = e.Device;
			return r;
		}
		else if(e is InputEventMIDI mid)
		{
			var r = new InputEventMIDI();
			r.Channel = mid.Channel;
			r.Message = mid.Message;
			r.Pitch = mid.Pitch;
			r.Velocity = mid.Velocity;
			r.Instrument = mid.Instrument;
			r.Pressure = mid.Pressure;
			r.ControllerNumber = mid.ControllerNumber;
			r.ControllerValue = mid.ControllerValue;
			r.Device = e.Device;
			return r;
		}
		else if(e is InputEventScreenDrag d)
		{
			var r = new InputEventScreenDrag();
			r.Index = d.Index;
			r.Position = d.Position;
			r.Relative = d.Relative;
			r.Speed = d.Speed;
			r.Device = e.Device;
			return r;
		}
		else if(e is InputEventScreenTouch t)
		{
			var r = new InputEventScreenTouch();
			r.Index = t.Index;
			r.Position = t.Position;
			r.Pressed = t.Pressed;
			r.Device = e.Device;
			return r;
		}
		else if(e is InputEventMagnifyGesture mg)
		{
			var r = new InputEventMagnifyGesture();
			r.Alt = mg.Alt;
			r.Shift = mg.Shift;
			r.Control = mg.Control;
			r.Meta = mg.Meta;
			r.Command = mg.Command;
			r.Position = mg.Position;
			r.Factor = mg.Factor;
			r.Device = e.Device;
			return r;
		}
		else if(e is InputEventPanGesture pg)
		{
			var r = new InputEventPanGesture();
			r.Alt = pg.Alt;
			r.Shift = pg.Shift;
			r.Control = pg.Control;
			r.Meta = pg.Meta;
			r.Command = pg.Command;
			r.Position = pg.Position;
			r.Delta = pg.Delta;
			r.Device = e.Device;
			return r;
		}
		else if(e is InputEventKey k)
		{
			var r = new InputEventKey();
			r.Alt = k.Alt;
			r.Shift = k.Shift;
			r.Control = k.Control;
			r.Meta = k.Meta;
			r.Command = k.Command;
			r.Pressed = k.Pressed;
			r.Scancode = k.Scancode;
			r.PhysicalScancode = k.PhysicalScancode;
			r.Unicode = k.Unicode;
			r.Echo = k.Echo;
			r.Device = e.Device;
			return r;
		}
		else if(e is InputEventMouseButton mb)
		{
			var r = new InputEventMouseButton();
			r.Alt = mb.Alt;
			r.Shift = mb.Shift;
			r.Control = mb.Control;
			r.Meta = mb.Meta;
			r.Command = mb.Command;
			r.ButtonMask = mb.ButtonMask;
			r.Position = mb.Position;
			r.GlobalPosition = mb.GlobalPosition;
			r.Factor = mb.Factor;
			r.ButtonIndex = mb.ButtonIndex;
			r.Pressed = mb.Pressed;
			r.Doubleclick = mb.Doubleclick;
			r.Device = e.Device;
			return r;
		}
		else if(e is InputEventMouseMotion mm)
		{
			var r = new InputEventMouseMotion();
			r.Alt = mm.Alt;
			r.Shift = mm.Shift;
			r.Control = mm.Control;
			r.Meta = mm.Meta;
			r.Command = mm.Command;
			r.ButtonMask = mm.ButtonMask;
			r.Position = mm.Position;
			r.GlobalPosition = mm.GlobalPosition;
			r.Tilt = mm.Tilt;
			r.Pressure = mm.Pressure;
			r.Relative = mm.Relative;
			r.Speed = mm.Speed;
			r.Device = e.Device;
			return r;
		}
		
		return null;
	}
	
	public static Error ReadFile(string path, out string content)
	{
		var f = new File();//create new file
		var er = f.Open(path, File.ModeFlags.Read);//open file
		
		if(er != Error.Ok)//if error, return
		{
			content = "";
			return er;
		}
		
		content = f.GetAsText();//read text
		f.Close();//flush buffer
		return Error.Ok;
	}
	
	public static Error SaveFile(string path, string content)
	{
		var f = new File();
		var er = f.Open(path, File.ModeFlags.Write);
		
		if(er != Error.Ok) return er;
		
		f.StoreString(content);
		f.Close();
		return Error.Ok;
	}
}
