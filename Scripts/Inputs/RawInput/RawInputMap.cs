using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Linearstar.Windows.RawInput;
using System.Threading.Tasks;
using RawInputDeviceHandle = Linearstar.Windows.RawInput.Native.RawInputDeviceHandle;

public class RawInputMap : Node
{
	public RawInputMap() {}
	
	#if MULTI_KEYBOARD
	public Dictionary<RawInputDeviceHandle, int> KeyboardDevices{get; set;} = new Dictionary<RawInputDeviceHandle, int>();
	public Dictionary<int, uint> KeycodeDict{get; set;} = new Dictionary<int, uint>();
	public HashSet<(int,uint)> HeldEvents{get; set;} = new HashSet<(int,uint)>();
	
	public RawInputReceiverForm InputForm{get; set;}
	
	public bool DebugInput{get; set;} = false;
	
	public const string ACTION_WITH_PROFILE_PATTERN = @"^P(?:[0-9]+)_.*?$";
	public static readonly Regex ACTION_WITH_PROFILE_REGEX = new Regex(ACTION_WITH_PROFILE_PATTERN, RegexOptions.Compiled);
	
	public override void _Ready()
	{
		ReadKeyMap();
		
		var keyboards = RawInputDevice.GetDevices().OfType<RawInputKeyboard>();
		
		int i = 0;
		foreach(var device in keyboards)
		{
			KeyboardDevices.Add(device.Handle, i);
			foreach(string action in InputMap.GetActions())
			{
				if(ACTION_WITH_PROFILE_REGEX.IsMatch(action))
					InputMap.AddAction($"D{i}_{action}");
				else foreach(var e in InputMap.GetActionList(action).FilterType<InputEventKey>().ToList())
				{
					var newe = e.Copy();
					newe.Device = i;
					InputMap.ActionAddEvent(action, newe);
				}
			}
			
			++i;
		}
		
		#if DEBUG_INPUT_MAP
		foreach(var h in InputMap.GetActions()) GD.Print(h);
		#endif
		
		InputForm = new RawInputReceiverForm();
		InputForm.KeycodePress += (sender, t) => KeycodePress(t);
		
		RawInputDevice.RegisterDevice(HidUsageAndPage.Keyboard, RawInputDeviceFlags.NoLegacy, InputForm.Window.Handle);
		
		new Task(RunForm).Start();
	}
	
	
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("toggle_input_debug")) DebugInput = !DebugInput;
		
		if(Input.IsActionJustPressed("list_devices"))
		{
			int i = 0;
			foreach(var device in RawInputDevice.GetDevices())
			{
				var type = device.DeviceType.ToString();
				var handle = device.Handle.ToString();
				var ids = $"{device.VendorId:X4}:{device.ProductId:X4}";
				
				string productName;
				try{productName = device.ProductName;}
				catch(InvalidOperationException){productName = "ERROR";}
				
				string manufacturerName;
				try{manufacturerName = device.ManufacturerName;}
				catch(InvalidOperationException){manufacturerName = "ERROR";}
				
				var path = device.DevicePath;
				
				if(device is RawInputKeyboard)
				{
					GD.PrintT(i.ToString(), type, handle, ids, $"Product: {productName}", $"Manufacturer: {manufacturerName}");
					++i;
				}
				else
					GD.PrintT("", type, "", handle, ids, $"Product: {productName}", $"Manufacturer: {manufacturerName}");
				
				GD.PrintT("", $"Path: {path}");
			}
		}
	}
	
	public void RunForm() => Application.Run(InputForm);
	public void CloseForm() => InputForm.Close();
	
	public override void _ExitTree()
	{
		RawInputDevice.UnregisterDevice(HidUsageAndPage.Keyboard);
		CloseForm();
	}
	
	public void KeycodePress(RawInputKeyEvent rawInput)
	{
		var device = KeyboardDevices.GetValueOrDefault(rawInput.Handle, 0);
		
		if(!KeycodeDict.ContainsKey(rawInput.Key))
		{
			if(DebugInput) GD.PrintT("Unknwon raw input", $"Keycode: {rawInput.Key}", $"Pressed: {rawInput.Pressed}", $"Device: {device}");
			return;
		}
		
		var scancode = KeycodeDict[rawInput.Key];
		
		var data = (device, scancode);
		
		var input = new InputEventKey();
		
		input.Device = device;
		input.Scancode = scancode;
		input.Pressed = rawInput.Pressed;
		input.Echo = rawInput.Pressed && HeldEvents.Contains(data);
		
		
		
		//hack to manually set the unicode for the input
		var s = input.AsText();
		if(s.Length == 1) input.Unicode = s[0];
		
		if(input.Pressed) HeldEvents.Add(data);
		else HeldEvents.Remove(data);
		
		if(DebugInput) GD.PrintT($"Raw Input: {input.AsText()}", $"Keycode: {rawInput.Key}", $"Mapped to: {input.Scancode}", $"Pressed: {rawInput.Pressed}", $"Device: {input.Device}");
		
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
