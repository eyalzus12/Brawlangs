using Godot;
using System;
using System.Collections.Generic;

public class ObjectPool : Node
{
	public const int LoadAmount = 3;
	public Dictionary<string, Queue<Node2D>> ObjectDict;
	public Dictionary<string, PackedScene> LoadDict;
	public Queue<Tuple<string, Node2D>> ReturnQueue;
	public HashSet<Node2D> ReturnQueueSet;
	
	public ObjectPool()
	{
		ObjectDict = new Dictionary<string, Queue<Node2D>>();
		LoadDict = new Dictionary<string, PackedScene>();
	}
	
	public override void _PhysicsProcess(float delta)
	{
		CleanReturnQueue();
	}
	
	public Node2D GetObject(string identifier, PackedScene loader = null)
	{
		Queue<Node2D> poolQueue;
		if(!ObjectDict.TryGetValue(identifier, out poolQueue) || poolQueue.Count <= 0)//no available objects
		{
			poolQueue = CreateNewObject(identifier, loader);//load new ones
		}
		
		return poolQueue?.Dequeue();//get available object if exists
	}
	
	public Queue<Node2D> CreateNewObject(string identifier, PackedScene source = null)
	{
		if(!ObjectDict.ContainsKey(identifier))//no queue exists
		{
			if(source is null)//cant instance 
			{
				GD.PushError($"Cannot pool object with identifier {identifier} because given packed scene is null");
				return null;
			}
			
			ObjectDict.Add(identifier, new Queue<Node2D>());//make a queue
			LoadDict.Add(identifier, source);
		}
		
		for(int i = 0; i < LoadAmount; ++i)
		{
			var loader = source??LoadDict[identifier];
			var obj = source.Instance<Node2D>();//instance new object
			ObjectDict[identifier].Enqueue(obj);//put in the pool
		}
		
		return ObjectDict[identifier];
	}
	
	public bool InsertObject(Node2D n, string identifier = "")
	{
		if(identifier == "") identifier = n.Filename;
		
		if(identifier == "")
		{
			GD.PushError($"Cannot pool object {n} because it does not have a given identifier (or wasnt generated from one)");
			return false;
		}
		
		if(ReturnQueueSet.Contains(n)) return false;
		ReturnQueue.Enqueue(new Tuple<string,Node2D>(identifier,n));
		ReturnQueueSet.Add(n);
		return true;
	}
	
	public void CleanReturnQueue()
	{
		while(ReturnQueue.Count > 0)
		{
			var h = ReturnQueue.Dequeue();
			var obj = h.Item2;
			var identifier = h.Item1;
			if(obj is null || !Godot.Object.IsInstanceValid(obj)) continue;
			var parent = obj.GetParent();
			parent.RemoveChild(obj);
			
			if(!ObjectDict.ContainsKey(identifier))
				ObjectDict.Add(identifier, new Queue<Node2D>());//make a queue
			ObjectDict[identifier].Enqueue(obj);
			
			ReturnQueueSet.Remove(obj);
		}
	}
	
	public void ClearPool()
	{
		foreach(var entry in ObjectDict)
		{
			var queue = entry.Value;
			while(queue.Count > 0)
			{
				var obj = queue.Dequeue();
				obj.QueueFree();
			}
		}
		
		ObjectDict.Clear();
		LoadDict.Clear();
	}
	
	public override void _ExitTree()
	{
		ClearPool();
		ReturnQueue.Clear();
		ReturnQueueSet.Clear();
	}
}
