using System;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : EScriptable
{
	public bool CrawlUntil(Map map, Func<Point> onStart, int tries, Func<Crawler.Result, bool> canComplete, Action onFail = null)
	{
		if (tries <= 0)
		{
			return false;
		}
		for (int i = 0; i < tries; i++)
		{
			bool flag = false;
			for (int j = 0; j < 100; j++)
			{
				Point point = onStart();
				Crawler.StartType startType = this.startType;
				if (startType != Crawler.StartType.Surface)
				{
					if (startType == Crawler.StartType.Block)
					{
						if (point.sourceBlock.tileType.IsFullBlock && point.Installed == null)
						{
							flag = true;
						}
					}
				}
				else if (!point.HasBlock)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Crawler.Result arg = this.Crawl(map, onStart(), null);
				if (canComplete(arg))
				{
					if (i > 0)
					{
						Debug.Log("CrawlUntil Complete:" + i.ToString());
					}
					return true;
				}
			}
		}
		if (onFail != null)
		{
			Debug.Log("CrawlUntil Fail" + tries.ToString());
			if (onFail != null)
			{
				onFail();
			}
		}
		return false;
	}

	public void Crawl(Map map)
	{
		int num = 0;
		while ((float)num < map.sizeModifier * (float)this.repeat + 1f)
		{
			this.Crawl(map, map.GetRandomSurface(false, true, false), delegate(Point p)
			{
				p.cell.biome.Populate(p, false);
			});
			num++;
		}
	}

	public Crawler.Result Crawl(Map map, Point _start, Action<Point> onNewVisit = null)
	{
		if (this.members.Count == 0)
		{
			for (int i = 0; i < this.member; i++)
			{
				this.members.Add(new Crawler.Member());
			}
		}
		Point point = _start.Copy();
		point.Clamp(false);
		Crawler.Result result = new Crawler.Result();
		BiomeProfile biome = point.cell.biome;
		Crawler.sync++;
		this.Size = map.Size;
		int num = this.skipBorder ? 2 : 0;
		int num2 = this.skipBorder ? (this.Size - 2) : this.Size;
		if (Crawler.mapping.Length != this.Size)
		{
			Crawler.mapping = new int[map.Size, map.Size];
		}
		foreach (Crawler.Member member in this.members)
		{
			member.pos.Set(point);
			member.life = this.life;
		}
		for (int j = 0; j < this.life; j++)
		{
			for (int k = 0; k < this.members.Count; k++)
			{
				Crawler.Member member2 = this.members[k];
				int num3 = EScriptable.rnd(3) - 1;
				int num4 = EScriptable.rnd(3) - 1;
				int num5 = member2.pos.x + num3;
				int num6 = member2.pos.z + num4;
				if (point.Distance(num5, num6) <= this.radius && num5 >= num && num6 >= num && num5 < num2 && num6 < num2)
				{
					Cell cell = map.cells[num5, num6];
					if (!this.stickToStartBiome || !(cell.biome != biome))
					{
						Crawler.MoveType moveType = this.moveType;
						if (moveType != Crawler.MoveType.Surface)
						{
							if (moveType == Crawler.MoveType.Block)
							{
								if (cell._block == 0 || !cell.sourceBlock.tileType.IsFullBlock || cell.Installed != null)
								{
									goto IL_269;
								}
							}
						}
						else
						{
							if (cell._block != 0)
							{
								goto IL_269;
							}
							if (cell.IsTopWater)
							{
								goto IL_269;
							}
						}
						member2.pos.Set(num5, num6);
						if (Crawler.mapping[num5, num6] != Crawler.sync)
						{
							Crawler.mapping[num5, num6] = Crawler.sync;
							Point point2 = member2.pos.Copy();
							result.points.Add(point2);
							if (onNewVisit != null)
							{
								onNewVisit(point2);
							}
						}
					}
				}
				IL_269:;
			}
		}
		return result;
	}

	public static Crawler Create(string id)
	{
		return ResourceCache.Load<Crawler>("World/Map/Crawler/crawler " + id);
	}

	public static int[,] mapping = new int[0, 0];

	public static int sync;

	public int life;

	public int member;

	public int radius;

	public int repeat;

	public bool stickToStartBiome;

	public bool skipBorder;

	public Crawler.StartType startType;

	public Crawler.MoveType moveType;

	[NonSerialized]
	public int Size;

	[NonSerialized]
	public List<Crawler.Member> members = new List<Crawler.Member>();

	public class Member
	{
		public Point pos = new Point();

		public int life;
	}

	public class Result
	{
		public List<Point> points = new List<Point>();
	}

	public enum MoveType
	{
		Surface,
		Block
	}

	public enum StartType
	{
		Surface,
		Block
	}
}
