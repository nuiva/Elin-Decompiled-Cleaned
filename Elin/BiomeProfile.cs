using System;
using System.Collections.Generic;
using System.Linq;
using NoiseSystem;
using UnityEngine;

public class BiomeProfile : EScriptable
{
	public BiomeProfile.TileGroup exterior
	{
		get
		{
			return this._exterior;
		}
	}

	public BiomeProfile.TileGroup interior
	{
		get
		{
			if (this._interior.floor.id != 0)
			{
				return this._interior;
			}
			return this.exterior;
		}
	}

	public SourceMaterial.Row MatFloor
	{
		get
		{
			SourceMaterial.Row result;
			if ((result = this._mat) == null)
			{
				result = (this._mat = EClass.sources.materials.map[this.exterior.floor.mat]);
			}
			return result;
		}
	}

	public SourceMaterial.Row MatSub
	{
		get
		{
			SourceMaterial.Row result;
			if ((result = this._matSub) == null)
			{
				result = (this._matSub = ((this.exterior.floor.matSub == 0) ? this.MatFloor : EClass.sources.materials.map[this.exterior.floor.matSub]));
			}
			return result;
		}
	}

	public void Populate(Point point, bool interior = false)
	{
		Cell cell = point.cell;
		if (cell.obj != 0 || cell.Things.Count > 0)
		{
			return;
		}
		int count = this.cluster.obj.Count;
		for (int i = 0; i < count + this.cluster.thing.Count; i++)
		{
			BiomeProfile.Cluster cluster = (i >= count) ? this.cluster.thing[i - count] : this.cluster.obj[i];
			if (cluster.density == 0f)
			{
				return;
			}
			if (Rand.Range(0f, 1f) <= cluster.density)
			{
				BiomeProfile.Cluster.Type type = cluster.type;
				switch (type)
				{
				case BiomeProfile.Cluster.Type.ScatterExterior:
					if (interior)
					{
						goto IL_185;
					}
					break;
				case BiomeProfile.Cluster.Type.ScatterInterior:
					if (!interior)
					{
						goto IL_185;
					}
					break;
				case BiomeProfile.Cluster.Type.ScatterNonObstacle:
					if ((cell.Left.HasObstacle() ? 1 : 0) + (cell.Right.HasObstacle() ? 1 : 0) + (cell.Front.HasObstacle() ? 1 : 0) + (cell.Back.HasObstacle() ? 1 : 0) > 0)
					{
						goto IL_185;
					}
					break;
				default:
					if (type == BiomeProfile.Cluster.Type.ByWall)
					{
						if (cell.Left.hasDoor || cell.Right.hasDoor || cell.Front.hasDoor || cell.Back.hasDoor || (!cell.Left.HasBlock && !cell.Right.HasBlock && !cell.Front.HasBlock && !cell.Back.HasBlock))
						{
							goto IL_185;
						}
					}
					break;
				}
				if (cluster.TryCreate(point))
				{
					return;
				}
			}
			IL_185:;
		}
	}

	public static void Init()
	{
		if (!BiomeProfile.forceInitialize)
		{
			return;
		}
		Debug.Log("Initializing Clusters");
		foreach (BiomeProfile biomeProfile in EClass.core.refs.biomes.dict.Values)
		{
			biomeProfile.Reset();
		}
		BiomeProfile.forceInitialize = false;
	}

	public void Reset()
	{
		this._mat = null;
		this._matSub = null;
	}

	public static bool forceInitialize;

	public NoiseLayer layerBlock;

	public BiomeID id;

	public Color color;

	public FowProfile fowProfile;

	public BiomeProfile.TileGroup _exterior;

	public BiomeProfile.TileGroup _interior;

	public BiomeProfile.Style style;

	public BiomeProfile.Spawns spawn;

	public BiomeProfile.Clusters cluster;

	public string tags;

	public int floor_height;

	[NonSerialized]
	private SourceMaterial.Row _mat;

	[NonSerialized]
	private SourceMaterial.Row _matSub;

	public enum DoorStyle
	{
		simpleWood,
		FirmWood,
		Stone,
		Jail,
		Rune
	}

	public enum StairsStyle
	{
		Soil,
		Wood,
		Stone,
		Rock
	}

	public class BaseTile
	{
		protected string _GetThing(string id)
		{
			if (id.IsEmpty())
			{
				return "-";
			}
			SourceThing.Row row = EClass.editorSources.things.rows.First((SourceThing.Row a) => a.id == id);
			if (row != null)
			{
				return row.id + "-(" + row.name_JP + ")";
			}
			return "-";
		}

		protected void _SetThing(ref string id, string value)
		{
			id = EClass.editorSources.things.rows.First((SourceThing.Row a) => a.id == value.Split('-', StringSplitOptions.None)[0]).id;
		}

		protected IEnumerable<string> ThingRows()
		{
			return EClass.editorSources.things.GetListString();
		}

		protected string _GetObj(int id)
		{
			SourceObj.Row row = EClass.editorSources.objs.rows.First((SourceObj.Row a) => a.id == id);
			if (row != null)
			{
				return string.Concat(new string[]
				{
					row.id.ToString(),
					"-",
					row.alias,
					"(",
					row.name_JP,
					")"
				});
			}
			return "-";
		}

		protected void _SetObj(ref int id, string value)
		{
			id = EClass.editorSources.objs.rows.First((SourceObj.Row a) => a.id == int.Parse(value.Split('-', StringSplitOptions.None)[0])).id;
		}

		protected IEnumerable<string> ObjRows()
		{
			return EClass.editorSources.objs.GetListString();
		}

		protected string _GetFloor(int id)
		{
			SourceFloor.Row row = EClass.editorSources.floors.rows.First((SourceFloor.Row a) => a.id == id);
			if (row != null)
			{
				return string.Concat(new string[]
				{
					row.id.ToString(),
					"-",
					row.alias,
					"(",
					row.name_JP,
					")"
				});
			}
			return "-";
		}

		protected void _SetFloor(ref int id, string value)
		{
			id = EClass.editorSources.floors.rows.First((SourceFloor.Row a) => a.id == int.Parse(value.Split('-', StringSplitOptions.None)[0])).id;
		}

		protected IEnumerable<string> FloorRows()
		{
			return EClass.editorSources.floors.GetListString();
		}

		protected string _GetBlock(int id)
		{
			SourceBlock.Row row = EClass.editorSources.blocks.rows.First((SourceBlock.Row a) => a.id == id);
			if (row != null)
			{
				return string.Concat(new string[]
				{
					row.id.ToString(),
					"-",
					row.alias,
					"(",
					row.name_JP,
					")"
				});
			}
			return "-";
		}

		protected void _SetBlock(ref int id, string value)
		{
			id = EClass.editorSources.blocks.rows.First((SourceBlock.Row a) => a.id == int.Parse(value.Split('-', StringSplitOptions.None)[0])).id;
		}

		protected IEnumerable<string> BlockRows()
		{
			return EClass.editorSources.blocks.GetListString();
		}

		protected string _GetMat(int id)
		{
			if (id == -1)
			{
				id = 0;
			}
			SourceMaterial.Row row = EClass.editorSources.materials.rows.First((SourceMaterial.Row a) => a.id == id);
			if (row != null)
			{
				return string.Concat(new string[]
				{
					row.id.ToString(),
					"-",
					row.alias,
					"(",
					row.name_JP,
					")"
				});
			}
			return "-";
		}

		protected void _SetMat(ref int id, string value)
		{
			id = EClass.editorSources.materials.rows.First((SourceMaterial.Row a) => a.id == int.Parse(value.Split('-', StringSplitOptions.None)[0])).id;
		}

		protected IEnumerable<string> MatRows()
		{
			return EClass.editorSources.materials.GetListString();
		}

		protected string _GetSpawnList(string id)
		{
			if (id.IsEmpty() || !EClass.editorSources.spawnLists.map.ContainsKey(id))
			{
				return "-";
			}
			SourceSpawnList.Row row = EClass.editorSources.spawnLists.rows.First((SourceSpawnList.Row a) => a.id == id);
			if (row != null)
			{
				return row.id;
			}
			return "-";
		}

		protected void _SetSpawnList(ref string id, string value)
		{
			id = ((value == null) ? "" : EClass.editorSources.spawnLists.rows.First((SourceSpawnList.Row a) => a.id == value).id);
		}

		protected IEnumerable<string> _SpawnList()
		{
			return EClass.editorSources.spawnLists.GetListString();
		}

		public enum SubType
		{
			None,
			Rnd5 = 10,
			Rnd10,
			Rnd20,
			Pattern = 20
		}

		public enum DirType
		{
			Random8,
			RandomSqrt8,
			RandomSqrtSqrt8
		}
	}

	[Serializable]
	public class Tile : BiomeProfile.BaseTile
	{
		public string _mat
		{
			get
			{
				return base._GetMat(this.mat);
			}
			set
			{
				base._SetMat(ref this.mat, value);
			}
		}

		public string _matSub
		{
			get
			{
				return base._GetMat(this.matSub);
			}
			set
			{
				base._SetMat(ref this.matSub, value);
			}
		}

		public int GetDir()
		{
			switch (this.dirType)
			{
			case BiomeProfile.BaseTile.DirType.Random8:
				return EScriptable.rnd(8);
			case BiomeProfile.BaseTile.DirType.RandomSqrt8:
				return EScriptable.rnd(EScriptable.rnd(8) + 1);
			case BiomeProfile.BaseTile.DirType.RandomSqrtSqrt8:
				return EScriptable.rnd(EScriptable.rnd(EScriptable.rnd(8) + 1) + 1);
			default:
				return EScriptable.rnd(8);
			}
		}

		[HideInInspector]
		public int id;

		[HideInInspector]
		public int mat;

		[HideInInspector]
		public int idSub;

		[HideInInspector]
		public int matSub;

		public BiomeProfile.BaseTile.SubType subType;

		public BiomeProfile.BaseTile.DirType dirType;
	}

	[Serializable]
	public class TileFloor : BiomeProfile.Tile
	{
		public string _id
		{
			get
			{
				return base._GetFloor(this.id);
			}
			set
			{
				base._SetFloor(ref this.id, value);
			}
		}

		public string _idSub
		{
			get
			{
				return base._GetFloor(this.idSub);
			}
			set
			{
				base._SetFloor(ref this.idSub, value);
			}
		}
	}

	[Serializable]
	public class TileBlock : BiomeProfile.Tile
	{
		public string _id
		{
			get
			{
				return base._GetBlock(this.id);
			}
			set
			{
				base._SetBlock(ref this.id, value);
			}
		}

		public string _idSub
		{
			get
			{
				return base._GetBlock(this.idSub);
			}
			set
			{
				base._SetBlock(ref this.idSub, value);
			}
		}
	}

	[Serializable]
	public class TileGroup
	{
		public BiomeProfile.TileFloor floor;

		public BiomeProfile.TileBlock block;
	}

	[Serializable]
	public class Style : BiomeProfile.BaseTile
	{
		public string _matDoor
		{
			get
			{
				return base._GetMat(this.matDoor);
			}
			set
			{
				base._SetMat(ref this.matDoor, value);
			}
		}

		public string _matStairs
		{
			get
			{
				return base._GetMat(this.matStairs);
			}
			set
			{
				base._SetMat(ref this.matStairs, value);
			}
		}

		public string GetIdLight(bool wall)
		{
			if (this.lights.Count > 0)
			{
				foreach (BiomeProfile.Cluster.ItemThing itemThing in this.lights)
				{
					if (EClass.sources.cards.map[itemThing.id].tileType.UseMountHeight == wall)
					{
						return itemThing.id;
					}
				}
			}
			if (!wall)
			{
				return "torch";
			}
			return "torch_wall";
		}

		public string GetIdStairs(bool upstairs)
		{
			switch (this.stairsStyle)
			{
			case BiomeProfile.StairsStyle.Wood:
				if (!upstairs)
				{
					return "381";
				}
				return "376";
			case BiomeProfile.StairsStyle.Stone:
				if (!upstairs)
				{
					return "932";
				}
				return "379";
			case BiomeProfile.StairsStyle.Rock:
				if (!upstairs)
				{
					return "380";
				}
				return "377";
			default:
				if (!upstairs)
				{
					return "stairsDown_cave";
				}
				return "stairs_cave";
			}
		}

		public string GetIdDoor()
		{
			switch (this.doorStyle)
			{
			case BiomeProfile.DoorStyle.FirmWood:
				return "46";
			case BiomeProfile.DoorStyle.Stone:
				return "42";
			case BiomeProfile.DoorStyle.Jail:
				return "40";
			case BiomeProfile.DoorStyle.Rune:
				return "43";
			default:
				return "45";
			}
		}

		public float doorChance = 0.9f;

		public BiomeProfile.DoorStyle doorStyle;

		[HideInInspector]
		public int matDoor;

		public BiomeProfile.StairsStyle stairsStyle;

		[HideInInspector]
		public int matStairs;

		public List<BiomeProfile.Cluster.ItemThing> lights;
	}

	[Serializable]
	public class Spawns
	{
		public string GetRandomCharaId()
		{
			if (this.chara.Count != 0)
			{
				return this.chara.RandomItemWeighted((BiomeProfile.SpawnListChara a) => a.chance).id;
			}
			return "c_dungeon";
		}

		public string GetRandomThingId()
		{
			if (this.thing.Count != 0)
			{
				return this.thing.RandomItemWeighted((BiomeProfile.SpawnListThing a) => a.chance).id;
			}
			return "dungeon";
		}

		public List<BiomeProfile.SpawnListChara> chara;

		public List<BiomeProfile.SpawnListThing> thing;
	}

	[Serializable]
	public class SpawnList : BiomeProfile.BaseTile
	{
		public string _id
		{
			get
			{
				return base._GetSpawnList(this.id);
			}
			set
			{
				base._SetSpawnList(ref this.id, value);
			}
		}

		public float chance = 1f;

		[HideInInspector]
		public string id;
	}

	[Serializable]
	public class SpawnListChara : BiomeProfile.SpawnList
	{
	}

	[Serializable]
	public class SpawnListThing : BiomeProfile.SpawnList
	{
	}

	[Serializable]
	public class Clusters
	{
		public List<BiomeProfile.ClusterObj> obj;

		public List<BiomeProfile.ClusterThing> thing;
	}

	[Serializable]
	public class Cluster
	{
		public virtual bool TryCreate(Point p)
		{
			return false;
		}

		public BiomeProfile.Cluster.Type type;

		public float density;

		public enum Type
		{
			Scatter,
			ScatterExterior,
			ScatterInterior,
			ScatterNonObstacle,
			ByWall = 10
		}

		[Serializable]
		public class BaseItem : BiomeProfile.BaseTile
		{
			public virtual bool IsSpawnOnBlock
			{
				get
				{
					return false;
				}
			}

			public virtual bool IsSpawnOnWater
			{
				get
				{
					return false;
				}
			}

			public float chance = 1f;
		}

		[Serializable]
		public class Item : BiomeProfile.Cluster.BaseItem
		{
			public override bool IsSpawnOnBlock
			{
				get
				{
					return EClass.sources.objs.rows[this.idObj].tileType.IsBlockMount;
				}
			}

			public override bool IsSpawnOnWater
			{
				get
				{
					return EClass.sources.objs.rows[this.idObj].tileType.CanSpawnOnWater;
				}
			}

			public string obj
			{
				get
				{
					return base._GetObj(this.idObj);
				}
				set
				{
					base._SetObj(ref this.idObj, value);
				}
			}

			public int idObj;
		}

		[Serializable]
		public class ItemThing : BiomeProfile.Cluster.BaseItem
		{
			public string _id
			{
				get
				{
					return base._GetThing(this.id);
				}
				set
				{
					base._SetThing(ref this.id, value);
				}
			}

			public string id;
		}
	}

	[Serializable]
	public class ClusterObj : BiomeProfile.Cluster
	{
		public override bool TryCreate(Point p)
		{
			BiomeProfile.Cluster.Item item = this.items.RandomItem<BiomeProfile.Cluster.Item>();
			if (Rand.Range(0f, 1f) > item.chance)
			{
				return false;
			}
			Cell cell = p.cell;
			if (cell.HasBlock)
			{
				if (!item.IsSpawnOnBlock)
				{
					return false;
				}
			}
			else if (item.IsSpawnOnBlock)
			{
				return false;
			}
			p.SetObj(item.idObj, 1, 0);
			cell.objDir = EScriptable.rnd(8);
			if (cell.sourceObj.HasGrowth && cell.sourceObj.id != 103)
			{
				cell.growth.SetRandomStage();
			}
			return true;
		}

		public List<BiomeProfile.Cluster.Item> items;
	}

	[Serializable]
	public class ClusterThing : BiomeProfile.Cluster
	{
		public override bool TryCreate(Point p)
		{
			BiomeProfile.Cluster.ItemThing itemThing = this.items.RandomItem<BiomeProfile.Cluster.ItemThing>();
			if (Rand.Range(0f, 1f) > itemThing.chance)
			{
				return false;
			}
			Cell cell = p.cell;
			if (cell.HasBlock)
			{
				if (!itemThing.IsSpawnOnBlock)
				{
					return false;
				}
			}
			else if (itemThing.IsSpawnOnBlock)
			{
				return false;
			}
			if (cell.IsTopWater)
			{
				if (!itemThing.IsSpawnOnWater)
				{
					return false;
				}
			}
			else if (itemThing.IsSpawnOnWater)
			{
				return false;
			}
			Thing thing = ThingGen.Create(itemThing.id, -1, -1);
			int desiredDir = thing.TileType.GetDesiredDir(p, 0);
			if (desiredDir != -1)
			{
				thing.dir = desiredDir;
			}
			EClass._zone.AddCard(thing, p).Install();
			return true;
		}

		public List<BiomeProfile.Cluster.ItemThing> items;
	}
}
