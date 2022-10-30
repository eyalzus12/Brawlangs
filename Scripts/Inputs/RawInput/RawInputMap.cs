using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;
using RawInput_dll;

public class RawInputMap : Node
{
	public RawInputMap() {}
	
	#if MULTI_KEYBOARD
	public Dictionary<IntPtr, int> KeyboardDevices{get; set;} = new Dictionary<IntPtr, int>();
	public Dictionary<int, uint> KeycodeDict{get; set;} = new Dictionary<int, uint>();
	public HashSet<(int,uint)> HeldEvents{get; set;} = new HashSet<(int,uint)>();
	
	public RawInputReceiverForm InputForm{get; set;}
	
	public bool DebugInput{get; set;} = false;
	
	public const string ACTION_WITH_PROFILE_PATTERN = @"^P(?:[0-9]+)_.*?$";
	public static readonly Regex ACTION_WITH_PROFILE_REGEX = new Regex(ACTION_WITH_PROFILE_PATTERN, RegexOptions.Compiled);
	
	public override void _Ready()
	{
		ReadKeyMap();
		
		InputForm = new RawInputReceiverForm();
		InputForm.KeyPressed += KeycodePress;
		
		for(int i = 0; i <= InputForm.RawInputWindow.NumberOfKeyboards; ++i)
			PrepareInputMapForDevice(i);
		
		new Task(RunForm).Start();
	}
	
	public void PrepareInputMapForDevice(int device)
	{
		foreach(string action in InputMap.GetActions())
		{
			var newaction = $"D{device}_{action}";
			if(!InputMap.HasAction(newaction) && ACTION_WITH_PROFILE_REGEX.IsMatch(action))
				InputMap.AddAction(newaction);
		}
	}
	
	public int RegisterKeyboard(IntPtr handle)
	{
		var device = KeyboardDevices.Count+1;
		PrepareInputMapForDevice(device);
		KeyboardDevices.Add(handle, device);
		return device;
	}
	
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("toggle_input_debug")) DebugInput = !DebugInput;

		if(Input.IsActionJustPressed("list_devices"))
		{
			int i = 1;
			foreach(var keyboard in InputForm.RawInputWindow.KeyboardList)
			{
				GD.Print($"Device {i}\nDeviceName: {keyboard.DeviceName}\nDeviceType: {keyboard.DeviceType}\nDeviceHandle: {keyboard.DeviceHandle.ToInt64().ToString("X")}\nName: {keyboard.Name}\n");
				++i;
			}
		}
	}
	
	public void RunForm() => Application.Run(InputForm);
	public void CloseForm() => InputForm.Close();
	
	public override void _ExitTree() => CloseForm();
	
	public void KeycodePress(object sender, RawInputEventArg e)
	{
		var ev = e.KeyPressEvent;
		var device = ev.DeviceHandle;
		var key = ev.VKey;
		var pressed = (ev.KeyPressState == "MAKE");
		
		int deviceNum;
		if(device == IntPtr.Zero) deviceNum = 0;
		else if(!KeyboardDevices.ContainsKey(device)) deviceNum = RegisterKeyboard(device);
		else deviceNum = KeyboardDevices[device];
		
		if(!KeycodeDict.ContainsKey(key))
		{
			if(DebugInput) GD.PrintT("Unknwon raw input", $"Keycode: {key}", $"Pressed: {pressed}", $"Device: {deviceNum}");
			return;
		}
		
		var scancode = KeycodeDict[key];
		
		var data = (deviceNum, scancode);
		
		var input = new InputEventKey();
		
		input.Device = deviceNum;
		input.Scancode = scancode;
		input.Pressed = pressed;
		input.Echo = pressed && HeldEvents.Contains(data);
		
		if(input.Pressed) HeldEvents.Add(data);
		else HeldEvents.Remove(data);
		
		if(DebugInput) GD.PrintT($"Raw Input: {ev.VKeyName}", $"Keycode: {key}", $"Mapped to: {scancode}", $"Pressed: {pressed}", $"Device: {deviceNum}");
		
		Input.ParseInputEvent(input);
	}
	
	private const string KEY_MAP_PATH = "res://Scripts/Inputs/RawInput/KeyToScancode.keys";
	private void ReadKeyMap()
	{
		string content;
		var er = Utils.ReadFile(KEY_MAP_PATH, out content);
		if(er != Error.Ok) return;
		ParseKeyMap(content);
	}
	
	private void ParseKeyMap(string str)
	{
		str
			.Split("\n")
			.Select(s => s.Substring(0,s.IndexOf(';')).Trim().Split(" "))
			.ForEach(t => KeycodeDict.Add(int.Parse(t[0].Trim()), uint.Parse(t[1].Trim())));
	}
	#endif
}
