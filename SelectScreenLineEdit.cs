using Godot;
using System;

public class SelectScreenLineEdit : LineEdit
{
	public CheckBox check;
	
	public int number = 1;
	public const string PREFIX = "LoadedCharacter";
	
	private PublicData data;
	
	public override void _Ready()
	{
		number = int.Parse(GetParent().Name.Substring("Load".Length));
		//take parent name
		//remove "Load"
		//parse
		
		data = this.GetPublicData();
		object o = null;
		if(data.TryGet($"{PREFIX}{number}", out o)) Text = o.s();
		
		//check = GetParent().GetNode<Label>("Label").GetNode<CheckBox>("CheckBox");
		//if(data.TryGet($"{PREFIX}{number}Dummy", out o))
		//	check.Pressed = o.b();
		
		GetParent().GetParent().GetParent().GetNode<Button>("ExitButton").Connect("pressed", this, nameof(OnExit));
		
	}
	
	public void OnExit()
	{
		if(Text == "") return;
		data.AddOverride($"{PREFIX}{number}", Text);
		//data.AddOverride($"{PREFIX}{number}Dummy", check.Pressed);
	}
}
