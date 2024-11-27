using System;
using System.Collections.Generic;

public class PropSet : Dictionary<int, Card>
{
	public void Add(Card c)
	{
		this.ModNum(c.Num);
		base.Add(c.uid, c);
	}

	public void Remove(Card c)
	{
		this.ModNum(-c.Num);
		base.Remove(c.uid);
	}

	public bool Contains(Card c)
	{
		return base.ContainsKey(c.uid);
	}

	public virtual void ModNum(int a)
	{
		this.num += a;
	}

	public void OnChangeNum(int a)
	{
		this.ModNum(a);
	}

	public int num;
}
