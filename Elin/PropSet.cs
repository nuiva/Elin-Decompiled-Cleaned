using System.Collections.Generic;

public class PropSet : Dictionary<int, Card>
{
	public int num;

	public void Add(Card c)
	{
		ModNum(c.Num);
		Add(c.uid, c);
	}

	public void Remove(Card c)
	{
		ModNum(-c.Num);
		Remove(c.uid);
	}

	public bool Contains(Card c)
	{
		return ContainsKey(c.uid);
	}

	public virtual void ModNum(int a)
	{
		num += a;
	}

	public void OnChangeNum(int a)
	{
		ModNum(a);
	}
}
