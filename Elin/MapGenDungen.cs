using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Dungen;
using UnityEngine;

public class MapGenDungen : BaseMapGen
{
	public static MapGenDungen Instance
	{
		get
		{
			MapGenDungen result;
			if ((result = MapGenDungen._Instance) == null)
			{
				result = (MapGenDungen._Instance = new MapGenDungen());
			}
			return result;
		}
	}

	protected override bool OnGenerateTerrain()
	{
		MapGenDungen.<>c__DisplayClass3_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		DunGen.Init();
		MapGenerator generator = this.zone.Generator;
		MapData mapData = DunGen.Generate(generator, 0);
		this.width = mapData.size_X + 2;
		this.height = mapData.size_Y + 2;
		base.SetSize(Mathf.Max(this.width, this.height), 10);
		this.map.CreateNew(this.Size, true);
		this.map.poiMap.Reset();
		this.map.SetZone(this.zone);
		this.map.config.blockHeight = EClass.core.gameSetting.gen.defaultBlockHeight;
		CS$<>8__locals1.biome = this.zone.biome;
		if (this.zone is Zone_Void)
		{
			CS$<>8__locals1.biome = CS$<>8__locals1.biome.Instantiate<BiomeProfile>();
			CS$<>8__locals1.biome.interior.block.mat = MATERIAL.GetRandomMaterialFromCategory(this.zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			CS$<>8__locals1.biome.interior.block.matSub = MATERIAL.GetRandomMaterialFromCategory(this.zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			CS$<>8__locals1.biome.interior.floor.mat = MATERIAL.GetRandomMaterialFromCategory(this.zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			CS$<>8__locals1.biome.interior.floor.matSub = MATERIAL.GetRandomMaterialFromCategory(this.zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			CS$<>8__locals1.biome.exterior.block.mat = MATERIAL.GetRandomMaterialFromCategory(this.zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			CS$<>8__locals1.biome.exterior.block.matSub = MATERIAL.GetRandomMaterialFromCategory(this.zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			CS$<>8__locals1.biome.exterior.floor.mat = MATERIAL.GetRandomMaterialFromCategory(this.zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			CS$<>8__locals1.biome.exterior.floor.matSub = MATERIAL.GetRandomMaterialFromCategory(this.zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
		}
		BiomeProfile.TileFloor floor = CS$<>8__locals1.biome.exterior.floor;
		BiomeProfile.TileBlock block = CS$<>8__locals1.biome.exterior.block;
		bool flag = this.zone.lv <= 0;
		bool flag2 = false;
		bool flag3 = false;
		Point point = new Point();
		Thing thing = null;
		Thing thing2 = null;
		for (int i = 0; i < mapData.size_X + 2; i++)
		{
			for (int j = 0; j < mapData.size_Y + 2; j++)
			{
				if (i == 0 || j == 0 || i >= mapData.size_X || j >= mapData.size_Y)
				{
					base.SetFloor(floor, i, j);
					base.SetBlock(block, i, j);
				}
				else
				{
					Dungen.Cell cell = mapData.cellsOnMap[i - 1, j - 1];
					CellType type = cell.type;
					point.Set(i, j);
					base.SetFloor(floor, i, j);
					string name = type.name;
					if (!(name == "Entrance"))
					{
						if (!(name == "Exit"))
						{
							if (!(name == "Door"))
							{
								if (!(name == "Abyss"))
								{
									if (type.passable == generator.reversePassage)
									{
										base.SetBlock(block, i, j);
									}
								}
								else
								{
									base.SetBlock(block, i, j);
								}
							}
							else if (CS$<>8__locals1.biome.style.doorChance >= Rand.Range(0f, 1f))
							{
								base.SetBlock(block, i, j);
								Thing t3 = ThingGen.Create(CS$<>8__locals1.biome.style.GetIdDoor(), CS$<>8__locals1.biome.style.matDoor, -1);
								this.zone.AddCard(t3, i, j).Install();
							}
						}
						else if (this.zone.ShouldMakeExit)
						{
							if (flag3)
							{
								Debug.LogError("exception: already created exit");
							}
							else
							{
								flag3 = true;
								Thing thing3 = ThingGen.Create(EClass._zone.LockExit ? "stairs_locked" : CS$<>8__locals1.biome.style.GetIdStairs(!flag), this.zone.LockExit ? -1 : CS$<>8__locals1.biome.style.matStairs, -1);
								if (flag)
								{
									thing = thing3;
								}
								else
								{
									thing2 = thing3;
								}
								this.zone.AddCard(thing3, i, j).Install();
							}
						}
					}
					else if (flag2)
					{
						Debug.LogError("exception: already created entrance");
					}
					else
					{
						flag2 = true;
						Thing thing4 = ThingGen.Create(CS$<>8__locals1.biome.style.GetIdStairs(flag), CS$<>8__locals1.biome.style.matStairs, -1);
						if (!flag)
						{
							thing = thing4;
						}
						else
						{
							thing2 = thing4;
						}
						this.zone.AddCard(thing4, i, j).Install();
					}
					if (!cell.isRoomCell)
					{
						CS$<>8__locals1.biome.Populate(point, false);
					}
				}
			}
		}
		if (!flag2)
		{
			Debug.LogError("exception: Failed to create entrance:");
			return false;
		}
		if (this.zone.ShouldMakeExit && !flag3)
		{
			Debug.LogError("exception: Failed to create exist:");
			return false;
		}
		if (thing2 != null)
		{
			thing2.pos.cell._block = 0;
			thing2.pos.cell.obj = 0;
		}
		if (thing != null)
		{
			thing.pos.cell._block = 0;
			thing.pos.cell.obj = 0;
		}
		CS$<>8__locals1.rooms = new Dictionary<int, GenRoom>();
		CS$<>8__locals1.count = 0;
		int num = 0;
		foreach (Dungen.Room room in mapData.rooms)
		{
			if (room.width != 0 && room.height != 0)
			{
				num++;
			}
		}
		if (num == 0)
		{
			mapData.rooms.Clear();
		}
		if (mapData.rooms.Count == 0)
		{
			GenRoomBig genRoomBig = new GenRoomBig();
			genRoomBig.Init(1, 1, this.width - 1, this.height - 1);
			this.<OnGenerateTerrain>g__SetRoom|3_0(genRoomBig, ref CS$<>8__locals1);
		}
		else
		{
			foreach (Dungen.Room room2 in mapData.rooms)
			{
				GenRoom genRoom = this.ChooseRoom();
				genRoom.Init(room2);
				this.<OnGenerateTerrain>g__SetRoom|3_0(genRoom, ref CS$<>8__locals1);
			}
		}
		this.map.RefreshAllTiles();
		foreach (GenRoom genRoom2 in CS$<>8__locals1.rooms.Values)
		{
			genRoom2.Populate();
		}
		this.zone.OnGenerateRooms(this);
		this.map.ReloadRoom();
		Debug.Log(string.Concat(new string[]
		{
			"Dungen: room:",
			CS$<>8__locals1.rooms.Count.ToString(),
			"/",
			mapData.rooms.Count.ToString(),
			" width:",
			this.width.ToString(),
			" height:",
			this.height.ToString()
		}));
		int num2 = EClass.rnd(this.Size * this.Size / 50 + EClass.rnd(20)) + 5;
		num2 = num2 * Mathf.Min(20 + this.zone.DangerLv * 5, 100) / 100;
		for (int k = 0; k < num2; k++)
		{
			point = EClass._map.GetRandomPoint();
			if (!point.cell.isModified && !point.HasThing && !point.HasBlock && !point.HasObj)
			{
				Thing t2 = ThingGen.CreateFromCategory("trap", this.zone.DangerLv);
				EClass._zone.AddCard(t2, point).Install();
			}
		}
		this.map.things.ForeachReverse(delegate(Thing t)
		{
			TraitDoor traitDoor = t.trait as TraitDoor;
			if (traitDoor == null)
			{
				return;
			}
			if (!traitDoor.IsValid())
			{
				string str = "Purging Door:";
				string name2 = t.Name;
				string str2 = "/";
				Point pos = t.pos;
				Debug.Log(str + name2 + str2 + ((pos != null) ? pos.ToString() : null));
				t.Destroy();
			}
		});
		if (thing != null)
		{
			MapGenDungen.<OnGenerateTerrain>g__ClearPos|3_2(thing);
		}
		if (thing2 != null)
		{
			MapGenDungen.<OnGenerateTerrain>g__ClearPos|3_2(thing2);
		}
		return true;
	}

	public GenRoom ChooseRoom()
	{
		if (EClass.rnd(100) < 5)
		{
			return new GenRoomMonsterHouse();
		}
		return new GenRoom();
	}

	[CompilerGenerated]
	private void <OnGenerateTerrain>g__SetRoom|3_0(GenRoom room, ref MapGenDungen.<>c__DisplayClass3_0 A_2)
	{
		room.map = this.map;
		room.zone = this.zone;
		room.gen = this;
		room.group = A_2.biome.interior;
		A_2.rooms[room.Index] = room;
		room.Fill();
		Debug.Log(string.Concat(new string[]
		{
			"Room",
			A_2.count.ToString(),
			" ",
			room.width.ToString(),
			"*",
			room.height.ToString(),
			" ",
			(room != null) ? room.ToString() : null
		}));
		int count = A_2.count;
		A_2.count = count + 1;
	}

	[CompilerGenerated]
	internal static void <OnGenerateTerrain>g__ClearPos|3_2(Thing t)
	{
		foreach (Card card in t.pos.ListCards(false))
		{
			if (card != t && card.isThing)
			{
				card.Destroy();
			}
		}
		t.pos.SetObj(0, 1, 0);
		t.pos.SetBlock(0, 0);
		t.pos.cell.height = 0;
	}

	private static MapGenDungen _Instance;
}
