using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class WorkSummary : EClass
{
	public int progress
	{
		get
		{
			return this.ints[0];
		}
		set
		{
			this.ints[0] = value;
		}
	}

	public int money
	{
		get
		{
			return this.ints[5];
		}
		set
		{
			this.ints[5] = value;
		}
	}

	public int food
	{
		get
		{
			return this.ints[6];
		}
		set
		{
			this.ints[6] = value;
		}
	}

	public int knowledge
	{
		get
		{
			return this.ints[7];
		}
		set
		{
			this.ints[7] = value;
		}
	}

	public int material
	{
		get
		{
			return this.ints[8];
		}
		set
		{
			this.ints[8] = value;
		}
	}

	public void Reset()
	{
		this.progress = (this.money = (this.food = (this.knowledge = (this.material = 0))));
	}

	public void AddThing(Thing t)
	{
		this.things.Add(t);
	}

	[JsonProperty]
	public int[] ints = new int[10];

	[JsonProperty]
	public WorkSession work;

	[JsonProperty]
	public List<WorkSession> hobbies = new List<WorkSession>();

	[JsonProperty]
	public List<Thing> things = new List<Thing>();
}
