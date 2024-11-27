using System;
using System.Collections.Generic;

public class CellDetail
{
	public static CellDetail Empty
	{
		get
		{
			return new CellDetail();
		}
	}

	public static CellDetail Spawn()
	{
		if (CellDetail.cache.Count > 0)
		{
			return CellDetail.cache.Pop();
		}
		CellDetail.count++;
		return new CellDetail();
	}

	public bool TryDespawn()
	{
		if (this.things.Count > 0 || this.charas.Count > 0 || this.area != null || this.footmark != null || this.designation != null || this.anime != null)
		{
			return false;
		}
		CellDetail.cache.Push(this);
		return true;
	}

	public void MoveThingToBottom(Thing t)
	{
		if (this.things.Count == 1)
		{
			return;
		}
		this.things.Remove(t);
		this.things.Insert(0, t);
		int num = 0;
		while (num < this.things.Count - 1 && this.things[num].IsInstalled)
		{
			t.stackOrder = num;
			num++;
		}
	}

	public void MoveThingToTop(Thing t)
	{
		if (this.things.Count == 1)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		while (num2 < this.things.Count - 1 && this.things[num2].IsInstalled)
		{
			num = num2 + 1;
			num2++;
		}
		if (num >= this.things.Count)
		{
			num--;
		}
		this.things.Remove(t);
		this.things.Insert(num, t);
		t.stackOrder = num;
	}

	public static int count;

	public static Map map;

	public static Stack<CellDetail> cache = new Stack<CellDetail>();

	public List<Thing> things = new List<Thing>(1);

	public List<Chara> charas = new List<Chara>(1);

	public Area area;

	public Footmark footmark;

	public TaskDesignation designation;

	public TransAnime anime;
}
