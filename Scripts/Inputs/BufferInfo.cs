using Godot;
using System;

public class BufferInfo
{
	//bufferTimeLeft > 0: the input is currently buffered
	//bufferTimeLeft = 0: the input has just stopped being buffered
	//bufferTimeLeft < 0: the input isnt currently buffered

	public int BufferTimeLeft{get; set;} = 0;
	public int DefaultBufferTime{get; set;} = 1;
	public bool MarkedForDeletion{get; set;} = false;

	public BufferInfo() {}

	public BufferInfo(int buffer) {DefaultBufferTime = buffer;}

	public void Activate(int time = -1)
	{
		BufferTimeLeft = (time == -1)?DefaultBufferTime:time;
		BufferTimeLeft++;
	}

	public void Advance()
	{
		if(BufferTimeLeft > -1) --BufferTimeLeft;
	}

	public void Disable() => BufferTimeLeft = -1;

	public void Delete()
	{
		Disable();
		MarkedForDeletion = false;
	}

	public bool IsActive() => BufferTimeLeft > 0;
	public override string ToString() => $"{BufferTimeLeft}, {DefaultBufferTime}, {MarkedForDeletion}";
}
