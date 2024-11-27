using System;
using Dungen;
using UnityEngine;

public class GenRoom : GenBounds
{
	public int Index
	{
		get
		{
			return this.x + this.y * this.Size;
		}
	}

	public virtual bool PopulateCluster
	{
		get
		{
			return true;
		}
	}

	public virtual bool IsBigRoom
	{
		get
		{
			return false;
		}
	}

	public void Init(Dungen.Room room)
	{
		this.x = room.x + 1;
		this.y = room.y + 1;
		this.width = room.width;
		this.height = room.height;
		this.Size = this.width * this.height;
	}

	public void Init(int _x, int _y, int w, int h)
	{
		this.x = _x;
		this.y = _y;
		this.width = w;
		this.height = h;
		this.Size = this.width * this.height;
	}

	public void Populate()
	{
		PartialMap partialMap = base.TryAddMapPiece(MapPiece.Type.Any, -1f, null, null);
		this.OnPopulate();
		BiomeProfile biome = this.zone.biome;
		if (biome.style.lights.Count > 0 && (partialMap == null || !partialMap.result.hasLight))
		{
			foreach (BiomeProfile.Cluster.ItemThing itemThing in biome.style.lights)
			{
				if (Rand.Range(0f, 1f) <= itemThing.chance)
				{
					Thing thing = ThingGen.Create(itemThing.id, -1, -1);
					Point point = new Point();
					int i = 0;
					while (i < 100)
					{
						if (!thing.TileType.UseMountHeight)
						{
							int a = Mathf.Min(i / 3 + 2, this.width / 2);
							int a2 = Mathf.Min(i / 3 + 2, this.height / 2);
							point.Set(this.x + this.width / 2 + EClass.rnd(a) - EClass.rnd(a), this.y + this.height / 2 + EClass.rnd(a2) - EClass.rnd(a2));
							goto IL_1E0;
						}
						point.Set(this.x + EClass.rnd(this.width), this.y + EClass.rnd(this.height));
						global::Cell cell = point.cell;
						if (!cell.Left.hasDoor && !cell.Right.hasDoor && !cell.Front.hasDoor && !cell.Back.hasDoor)
						{
							if (cell.Left.HasBlock || cell.Right.HasBlock || cell.Front.HasBlock || cell.Back.HasBlock)
							{
								goto IL_1E0;
							}
						}
						IL_23F:
						i++;
						continue;
						IL_1E0:
						if (!point.cell.blocked && !point.HasBlock && !point.HasObj && point.Installed == null)
						{
							int desiredDir = thing.TileType.GetDesiredDir(point, 0);
							if (desiredDir != -1)
							{
								thing.dir = desiredDir;
							}
							this.zone.AddCard(thing, point).Install();
							break;
						}
						goto IL_23F;
					}
					break;
				}
			}
		}
		if (this.PopulateCluster)
		{
			Point point2 = new Point();
			for (int j = this.x; j < this.x + this.width; j++)
			{
				for (int k = this.y; k < this.y + this.height; k++)
				{
					point2.Set(j, k);
					if (!point2.cell.isModified && !point2.cell.blocked)
					{
						biome.Populate(point2, true);
					}
				}
			}
		}
	}

	public void SpawnMob(Point p)
	{
		if (EClass.rnd(100) > 55 + this.zone.DangerLv * 3)
		{
			return;
		}
		this.zone.SpawnMob(p, null);
	}

	public virtual void OnPopulate()
	{
		for (int i = 0; i < EClass.rnd(3 + this.Size / 8) + 1; i++)
		{
			this.SetRandomPoint(delegate(Point p)
			{
				int num = EClass.rnd(7);
				if (num > 1)
				{
					if (num != 2)
					{
						this.SpawnMob(p);
					}
					else
					{
						if (EClass.rnd(5) != 0)
						{
							this.map.SetObj(p.x, p.z, GenRoom.ListWrecks.RandomItem<int>(), 1, 0);
							return;
						}
						this.map.SetObj(p.x, p.z, 82, 1, 0);
						if (EClass.rnd(2) == 0)
						{
							this.map.ApplyBackerObj(p, -1);
							return;
						}
					}
				}
				else if (this.zone.biome.spawn.thing.Count > 0)
				{
					Thing thing = ThingGen.CreateFromFilter(this.zone.biome.spawn.GetRandomThingId(), EClass._zone.DangerLv);
					if (thing != null)
					{
						this.zone.AddCard(thing, p);
						return;
					}
				}
			});
		}
		if (this.Size > 25)
		{
			for (int j = 0; j < this.Size / 25 + EClass.rnd(this.Size / 25); j++)
			{
				this.SetRandomPoint(delegate(Point p)
				{
					if (EClass.rnd(3) == 0)
					{
						Thing thing = ThingGen.CreateFromFilter(this.zone.biome.spawn.GetRandomThingId(), EClass._zone.DangerLv);
						if (thing != null)
						{
							this.zone.AddCard(thing, p);
							return;
						}
					}
					else
					{
						this.SpawnMob(p);
					}
				});
			}
		}
	}

	public void Fill()
	{
		BiomeProfile.TileFloor floor = this.group.floor;
		BiomeProfile.TileBlock block = this.group.block;
		for (int i = this.x; i < this.x + this.width; i++)
		{
			for (int j = this.y; j < this.y + this.height; j++)
			{
				if (this.map.cells[i, j]._block != 0)
				{
					base.SetBlock(i, j, block.mat, block.id, EClass.rnd(EClass.rnd(4) + 1));
				}
				base.SetFloor(i, j, floor.mat, floor.id, EClass.rnd(EClass.rnd(8) + 1));
			}
		}
	}

	public void SetRandomPoint(Action<Point> action)
	{
		int a = Mathf.Max(this.width - 2, 1);
		int a2 = Mathf.Max(this.height - 2, 1);
		for (int i = 0; i < 100; i++)
		{
			this._p.x = this.x + 1 + EClass.rnd(a);
			this._p.z = this.y + 1 + EClass.rnd(a2);
			this._c = this.map.cells[this._p.x, this._p.z];
			if (!this._c.blocked && !this._c.HasBlock && !this._p.HasThing && !this._p.HasChara)
			{
				action(this._p);
				return;
			}
		}
	}

	public void SetRandomPointCentered(Action<Point> action)
	{
		int a = Mathf.Max(this.width / 2 - 1, 1);
		int a2 = Mathf.Max(this.height / 2 - 1, 1);
		for (int i = 0; i < 100; i++)
		{
			this._p.x = this.x + this.width / 2 + EClass.rnd(EClass.rnd(a) + 1) - EClass.rnd(EClass.rnd(a) + 1);
			this._p.z = this.y + this.height / 2 + EClass.rnd(EClass.rnd(a2) + 1) - EClass.rnd(EClass.rnd(a2) + 1);
			this._c = this.map.cells[this._p.x, this._p.z];
			if (!this._c.blocked && !this._c.HasBlock && !this._p.HasThing && !this._p.HasChara)
			{
				action(this._p);
				return;
			}
		}
	}

	public BiomeProfile.TileGroup group;

	public MapGenDungen gen;

	public static int[] ListWrecks = new int[]
	{
		34,
		46,
		83,
		85,
		86,
		87
	};

	private Point _p = new Point();

	private global::Cell _c;
}
