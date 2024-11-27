using System;
using System.Collections.Generic;

public class ThingStack : EClass
{
	public void Add(Thing t)
	{
		this.list.Add(t);
		this.count += t.Num;
		if (t.Num > this.max)
		{
			this.max = t.Num;
		}
	}

	public List<Thing> list = new List<Thing>();

	public int count;

	public int val;

	public int max;
}
