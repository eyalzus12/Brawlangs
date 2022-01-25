using Godot;
using System;
using System.Collections.Generic;

public class ObjectPool : Node
{
	public const int LoadAmount = 3;
	public Dictionary<string, Queue<Node2D>> ObjectDict;
	public Queue<Tuple<string,Node2D>> ReturnQueue;
	public HashSet<Node2D> ReturnQueueSet;
	
	public ObjectPool()
	{
		ObjectDict = new Dictionary<string, Queue<Node2D>>();
	}
	
	public override void _PhysicsProcess(float delta)
	{
		CleanReturnQueue();
	}
	
	public Node2D GetObject(string path)
	{
		Node2D obj;
		Queue<Node2D> poolQueue;
		if(!ObjectDict.TryGetValue(path, out poolQueue) || poolQueue.Count <= 0)//no available objects
		{
			CreateNewObject(path);//load new ones
		}
		
		obj = poolQueue.Dequeue();//get available object
		return obj;
	}
	
	public void CreateNewObject(string path)
	{
		var resource = ResourceLoader.Load<PackedScene>(path);
		if(resource is null)
		{
			GD.Print($"Cannot pool objects from path {path} as it does not exist");
			return;
		}
		
		if(!ObjectDict.ContainsKey(path))//no queue exists
			ObjectDict.Add(path, new Queue<Node2D>());//make a queue
		
		for(int i = 0; i < LoadAmount; ++i)
		{
			var obj = resource.Instance<Node2D>();//instance new object
			ObjectDict[path].Enqueue(obj);//put in the pool
		}
	}
	
	public bool InsertObject(Node2D n, string path = "")
	{
		if(path == "") path = n.Filename;
		
		if(path == "")
		{
			GD.Print($"Cannot pool object {n} because it does not have a given file path (or wasnt generated from one)");
			return false;
		}
		
		if(ReturnQueueSet.Contains(n)) return false;
		ReturnQueue.Enqueue(new Tuple<string,Node2D>(path,n));
		ReturnQueueSet.Add(n);
		return true;
	}
	
	public void CleanReturnQueue()
	{
		while(ReturnQueue.Count > 0)
		{
			var h = ReturnQueue.Dequeue();
			var obj = h.Item2;
			var path = h.Item1;
			if(obj is null || !Godot.Object.IsInstanceValid(obj)) continue;
			var parent = obj.GetParent();
			parent.RemoveChild(obj);
			
			if(!ObjectDict.ContainsKey(path))
				ObjectDict.Add(path, new Queue<Node2D>());//make a queue
			ObjectDict[path].Enqueue(obj);
			
			ReturnQueueSet.Remove(obj);
		}
	}
	
	public void DisposePool()
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
	}
	
	public override void _ExitTree()
	{
		DisposePool();
		ReturnQueue.Clear();
		ReturnQueueSet.Clear();
	}
}
