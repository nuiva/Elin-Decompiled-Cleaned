using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Algorithms;
using FloodSpill;
using Ionic.Zip;
using Newtonsoft.Json;
using UnityEngine;

public class Map : MapBounds, IPathfindGrid
{
	public bool isBreakerDown
	{
		get
		{
			return this.bits[0];
		}
		set
		{
			this.bits[0] = value;
		}
	}

	public PropsStocked Stocked
	{
		get
		{
			return this.props.stocked;
		}
	}

	public PropsInstalled Installed
	{
		get
		{
			return this.props.installed;
		}
	}

	public PropsRoaming Roaming
	{
		get
		{
			return this.props.roaming;
		}
	}

	public float sizeModifier
	{
		get
		{
			return 1f / (16384f / (float)this.SizeXZ);
		}
	}

	public bool isGenerated
	{
		get
		{
			return this.Size != 0;
		}
	}

	public bool IsIndoor
	{
		get
		{
			return this.config.indoor;
		}
	}

	public int SizeXZ
	{
		get
		{
			return this.Size * this.Size;
		}
	}

	public IEnumerable<Card> Cards
	{
		get
		{
			return this.things.Concat(this.charas);
		}
	}

	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		this._bits = (int)this.bits.Bits;
	}

	protected virtual void OnSerializing()
	{
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		this.bits.Bits = (uint)this._bits;
	}

	public void CreateNew(int size, bool setReference = true)
	{
		Debug.Log("Map CreateNew:");
		this.Size = size;
		this.cells = new Cell[this.Size, this.Size];
		this.bounds = new MapBounds
		{
			x = 0,
			z = 0,
			maxX = this.Size - 1,
			maxZ = this.Size - 1,
			Size = this.Size
		};
		base.SetBounds(0, 0, this.Size - 1, this.Size - 1);
		this.ForeachXYZ(delegate(int x, int z)
		{
			this.cells[x, z] = new Cell
			{
				x = (byte)x,
				z = (byte)z
			};
		});
		if (setReference)
		{
			this.SetReference();
		}
	}

	public void SetZone(Zone _zone)
	{
		this.zone = _zone;
		this.zone.bounds = this.bounds;
		this.bounds.Size = this.Size;
		this.SetReference();
		this.props.Init();
		EClass.scene.profile = SceneProfile.Load(this.config.idSceneProfile.IsEmpty("default"));
		if (!this.config.idFowProfile.IsEmpty())
		{
			this.fowProfile = FowProfile.Load(this.config.idFowProfile);
		}
	}

	public void SetReference()
	{
		CellDetail.map = this;
		Point.map = this;
		Wall.map = this;
		Cell.map = this;
		Fov.map = this;
		Cell.Size = this.Size;
		Cell.cells = this.cells;
		IPathfinder pathfinder = PathManager.Instance.pathfinder;
		WeightCell[,] weightMap = this.cells;
		pathfinder.Init(this, weightMap, this.Size);
	}

	public void OnDeactivate()
	{
		this.charas.ForeachReverse(delegate(Chara c)
		{
			c.ai = new NoGoal();
			if (c.IsGlobal)
			{
				this.zone.RemoveCard(c);
				c.currentZone = this.zone;
			}
		});
		foreach (Thing thing in this.things)
		{
			if (thing.renderer.hasActor)
			{
				thing.renderer.KillActor();
			}
		}
		EClass.player.ClearMapHighlights();
	}

	public void Resize(int newSize)
	{
		Point p = new Point(0, 0);
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.pos.x >= newSize || thing.pos.z >= newSize)
			{
				this.MoveCard(p, thing);
			}
		}
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.pos.x >= newSize || chara.pos.z >= newSize)
			{
				this.MoveCard(p, chara);
			}
		}
		this.bounds.Size = newSize;
		this.Size = newSize;
		this.maxX = (this.maxZ = this.Size);
		this.cells = Util.ResizeArray<Cell>(EClass._map.cells, newSize, newSize, (int x, int y) => new Cell
		{
			x = (byte)x,
			z = (byte)y,
			isSeen = true
		});
		this.Reload();
	}

	public void Shift(Vector2Int offset)
	{
		TweenUtil.Tween(0.1f, null, delegate()
		{
			foreach (Card card in EClass._map.things.Concat(EClass._map.charas))
			{
				card.pos.x += offset.x;
				card.pos.z += offset.y;
				card.pos.Clamp(false);
			}
			Cell[,] array = new Cell[this.Size, this.Size];
			for (int i = 0; i < this.Size; i++)
			{
				int num = i - offset.y;
				for (int j = 0; j < this.Size; j++)
				{
					int num2 = j - offset.x;
					if (num2 >= 0 && num2 < this.Size && num >= 0 && num < this.Size)
					{
						array[j, i] = this.cells[num2, num];
					}
					else
					{
						array[j, i] = new Cell
						{
							x = (byte)j,
							z = (byte)i
						};
					}
				}
			}
			this.cells = array;
			this.bounds.x += offset.x;
			this.bounds.z += offset.y;
			this.bounds.maxX += offset.x;
			this.bounds.maxZ += offset.y;
			if (this.bounds.x < 0)
			{
				this.bounds.x = 0;
			}
			if (this.bounds.z < 0)
			{
				this.bounds.z = 0;
			}
			if (this.bounds.maxX > this.Size)
			{
				this.bounds.maxX = this.Size;
			}
			if (this.bounds.maxZ > this.Size)
			{
				this.bounds.maxZ = this.Size;
			}
			this.Reload();
		});
	}

	public void Reload()
	{
		this.rooms = new RoomManager();
		this.SetReference();
		string id = Game.id;
		EClass.game.Save(false, null, false);
		EClass.scene.Init(Scene.Mode.None);
		Game.Load(id);
		this.RevealAll(true);
		TweenUtil.Tween(0.1f, null, delegate()
		{
			this.ReloadRoom();
		});
	}

	public void ReloadRoom()
	{
		List<Thing> list = new List<Thing>();
		foreach (Thing thing in this.things)
		{
			if (thing.trait.IsDoor)
			{
				list.Add(thing);
			}
		}
		foreach (Thing thing2 in list)
		{
			Point pos = thing2.pos;
			EClass._zone.RemoveCard(thing2);
			EClass._zone.AddCard(thing2, pos);
			thing2.Install();
		}
		this.rooms.RefreshAll();
	}

	public void Reset()
	{
		for (int i = 0; i < this.Size; i++)
		{
			for (int j = 0; j < this.Size; j++)
			{
				this.cells[i, j].Reset();
			}
		}
		this.SetReference();
	}

	public void ResetEditorPos()
	{
		EClass._zone.Revive();
		foreach (Chara chara in this.charas)
		{
			if (chara.isPlayerCreation && chara.orgPos != null)
			{
				this.MoveCard(chara.orgPos, chara);
			}
		}
		foreach (Thing thing in this.things)
		{
			if (thing.trait is TraitDoor)
			{
				(thing.trait as TraitDoor).ForceClose();
			}
		}
	}

	public void Save(string path, ZoneExportData export = null, PartialMap partial = null)
	{
		this.version = EClass.core.version;
		Debug.Log("#io saving map:" + path);
		IO.CreateDirectory(path);
		int num = 0;
		int num2 = 0;
		int num3 = this.Size;
		int num4 = this.Size;
		if (partial != null)
		{
			num = partial.offsetX;
			num2 = partial.offsetZ;
			num3 = partial.w;
			num4 = partial.h;
		}
		int num5 = num3 * num4;
		byte[] array = new byte[num5];
		byte[] array2 = new byte[num5];
		byte[] array3 = new byte[num5];
		byte[] array4 = new byte[num5];
		byte[] array5 = new byte[num5];
		byte[] array6 = new byte[num5];
		byte[] array7 = new byte[num5];
		byte[] array8 = new byte[num5];
		byte[] array9 = new byte[num5];
		byte[] array10 = new byte[num5];
		byte[] array11 = new byte[num5];
		byte[] array12 = new byte[num5];
		byte[] array13 = new byte[num5];
		byte[] array14 = new byte[num5];
		byte[] array15 = new byte[num5];
		byte[] array16 = new byte[num5];
		byte[] array17 = new byte[num5];
		byte[] array18 = new byte[num5];
		byte[] array19 = new byte[num5];
		this.cellEffects.Clear();
		int num6 = 0;
		for (int i = num; i < num + num3; i++)
		{
			for (int j = num2; j < num2 + num4; j++)
			{
				Cell cell = this.cells[i, j];
				array[num6] = cell.objVal;
				array3[num6] = cell._blockMat;
				array2[num6] = cell._block;
				array5[num6] = cell._floorMat;
				array4[num6] = cell._floor;
				array6[num6] = cell.obj;
				array7[num6] = cell.objMat;
				array8[num6] = cell.decal;
				array9[num6] = cell._dirs;
				array12[num6] = cell.height;
				array13[num6] = cell._bridge;
				array14[num6] = cell._bridgeMat;
				array15[num6] = cell.bridgeHeight;
				array17[num6] = cell._roofBlockDir;
				array18[num6] = cell._roofBlock;
				array19[num6] = cell._roofBlockMat;
				array16[num6] = cell.bridgePillar;
				array10[num6] = array10[num6].SetBit(1, cell.isSeen);
				array10[num6] = array10[num6].SetBit(2, cell.isHarvested);
				array10[num6] = array10[num6].SetBit(3, cell.impassable);
				array10[num6] = array10[num6].SetBit(4, cell.isModified);
				array10[num6] = array10[num6].SetBit(5, cell.isClearSnow);
				array10[num6] = array10[num6].SetBit(6, cell.isForceFloat);
				array10[num6] = array10[num6].SetBit(7, cell.isToggleWallPillar);
				array11[num6] = array11[num6].SetBit(0, cell.isWatered);
				array11[num6] = array11[num6].SetBit(1, cell.isObjDyed);
				array11[num6] = array11[num6].SetBit(2, cell.crossWall);
				if (cell.effect != null)
				{
					this.cellEffects[j * num4 + i] = cell.effect;
				}
				num6++;
			}
		}
		this.compression = IO.Compression.None;
		IO.WriteLZ4(path + "objVals", array);
		IO.WriteLZ4(path + "blocks", array2);
		IO.WriteLZ4(path + "blockMats", array3);
		IO.WriteLZ4(path + "floors", array4);
		IO.WriteLZ4(path + "floorMats", array5);
		IO.WriteLZ4(path + "objs", array6);
		IO.WriteLZ4(path + "objMats", array7);
		IO.WriteLZ4(path + "decal", array8);
		IO.WriteLZ4(path + "flags", array10);
		IO.WriteLZ4(path + "flags2", array11);
		IO.WriteLZ4(path + "dirs", array9);
		IO.WriteLZ4(path + "heights", array12);
		IO.WriteLZ4(path + "bridges", array13);
		IO.WriteLZ4(path + "bridgeMats", array14);
		IO.WriteLZ4(path + "bridgeHeights", array15);
		IO.WriteLZ4(path + "bridgePillars", array16);
		IO.WriteLZ4(path + "roofBlocks", array18);
		IO.WriteLZ4(path + "roofBlockMats", array19);
		IO.WriteLZ4(path + "roofBlockDirs", array17);
		this.things.Sort((Thing a, Thing b) => a.stackOrder - b.stackOrder);
		if (export == null)
		{
			foreach (Chara chara in this.charas)
			{
				if (!chara.IsGlobal)
				{
					this.serializedCharas.Add(chara);
				}
			}
			GameIO.SaveFile(path + "map", this);
		}
		else
		{
			export.serializedCards.cards.Clear();
			if (partial == null)
			{
				foreach (Chara chara2 in this.charas)
				{
					if (export.usermap)
					{
						if (!chara2.trait.IsUnique && !chara2.IsPC)
						{
							export.serializedCards.Add(chara2);
						}
					}
					else if (!chara2.IsGlobal && chara2.isPlayerCreation)
					{
						export.serializedCards.Add(chara2);
					}
				}
				using (List<Thing>.Enumerator enumerator2 = this.things.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Thing thing = enumerator2.Current;
						if (thing.isPlayerCreation && thing.c_altName != "DebugContainer")
						{
							export.serializedCards.Add(thing);
						}
					}
					goto IL_68A;
				}
			}
			foreach (Thing thing2 in this.things)
			{
				if ((ActionMode.Copy.IsActive || thing2.trait.CanCopyInBlueprint) && thing2.pos.x >= num && thing2.pos.z >= num2 && thing2.pos.x < num + num3 && thing2.pos.z < num2 + num4)
				{
					export.serializedCards.Add(thing2);
				}
			}
			IL_68A:
			List<Thing> list = this.things;
			this.things = new List<Thing>();
			GameIO.SaveFile(path + "map", this);
			this.things = list;
		}
		this.serializedCharas.Clear();
	}

	public byte[] TryLoadFile(string path, string s, int size)
	{
		byte[] array = IO.ReadLZ4(path + s, size, this.compression);
		if (array == null)
		{
			Debug.Log("Couldn't load:" + s);
			return new byte[size];
		}
		return array;
	}

	public void Load(string path, bool import = false, PartialMap partial = null)
	{
		Map.<>c__DisplayClass69_0 CS$<>8__locals1 = new Map.<>c__DisplayClass69_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.path = path;
		if (partial == null)
		{
			Debug.Log("Map Load:" + this.compression.ToString() + ": " + CS$<>8__locals1.path);
		}
		int num = this.Size;
		int num2 = this.Size;
		if (partial != null)
		{
			num = partial.w;
			num2 = partial.h;
			Debug.Log(string.Concat(new string[]
			{
				this.compression.ToString(),
				": ",
				num.ToString(),
				"/",
				num2.ToString()
			}));
		}
		CS$<>8__locals1.size = num * num2;
		this.cells = new Cell[num, num2];
		if (this.bounds.maxX == 0)
		{
			this.bounds.SetBounds(0, 0, num - 1, num2 - 1);
		}
		base.SetBounds(0, 0, num - 1, num2 - 1);
		byte[] array = CS$<>8__locals1.<Load>g__TryLoad|0("objVals");
		byte[] array2 = CS$<>8__locals1.<Load>g__TryLoad|0("blockMats");
		byte[] array3 = CS$<>8__locals1.<Load>g__TryLoad|0("blocks");
		byte[] array4 = CS$<>8__locals1.<Load>g__TryLoad|0("floorMats");
		byte[] array5 = CS$<>8__locals1.<Load>g__TryLoad|0("floors");
		byte[] array6 = CS$<>8__locals1.<Load>g__TryLoad|0("objs");
		byte[] array7 = CS$<>8__locals1.<Load>g__TryLoad|0("objMats");
		byte[] array8 = CS$<>8__locals1.<Load>g__TryLoad|0("decal");
		byte[] array9 = CS$<>8__locals1.<Load>g__TryLoad|0("dirs");
		byte[] array10 = CS$<>8__locals1.<Load>g__TryLoad|0("flags");
		byte[] array11 = CS$<>8__locals1.<Load>g__TryLoad|0("flags2");
		byte[] array12 = CS$<>8__locals1.<Load>g__TryLoad|0("heights");
		byte[] array13 = CS$<>8__locals1.<Load>g__TryLoad|0("bridges");
		byte[] array14 = CS$<>8__locals1.<Load>g__TryLoad|0("bridgeMats");
		byte[] array15 = CS$<>8__locals1.<Load>g__TryLoad|0("bridgeHeights");
		byte[] array16 = CS$<>8__locals1.<Load>g__TryLoad|0("bridgePillars");
		byte[] array17 = CS$<>8__locals1.<Load>g__TryLoad|0("roofBlocks");
		byte[] array18 = CS$<>8__locals1.<Load>g__TryLoad|0("roofBlockMats");
		byte[] array19 = CS$<>8__locals1.<Load>g__TryLoad|0("roofBlockDirs");
		if (array16.Length < CS$<>8__locals1.size)
		{
			array16 = new byte[CS$<>8__locals1.size];
		}
		if (array.Length < CS$<>8__locals1.size)
		{
			array = new byte[CS$<>8__locals1.size];
		}
		if (array11.Length < CS$<>8__locals1.size)
		{
			array11 = new byte[CS$<>8__locals1.size];
		}
		CS$<>8__locals1.<Load>g__Validate|1(ref array, "objVals");
		CS$<>8__locals1.<Load>g__Validate|1(ref array2, "blockMats");
		CS$<>8__locals1.<Load>g__Validate|1(ref array3, "blocks");
		CS$<>8__locals1.<Load>g__Validate|1(ref array4, "floorMats");
		CS$<>8__locals1.<Load>g__Validate|1(ref array5, "floors");
		CS$<>8__locals1.<Load>g__Validate|1(ref array6, "objs");
		CS$<>8__locals1.<Load>g__Validate|1(ref array7, "objMats");
		CS$<>8__locals1.<Load>g__Validate|1(ref array8, "decal");
		CS$<>8__locals1.<Load>g__Validate|1(ref array9, "dirs");
		CS$<>8__locals1.<Load>g__Validate|1(ref array10, "flags");
		CS$<>8__locals1.<Load>g__Validate|1(ref array11, "flags2");
		CS$<>8__locals1.<Load>g__Validate|1(ref array12, "heights");
		CS$<>8__locals1.<Load>g__Validate|1(ref array13, "bridges");
		CS$<>8__locals1.<Load>g__Validate|1(ref array14, "bridgeMats");
		CS$<>8__locals1.<Load>g__Validate|1(ref array15, "bridgeHeights");
		CS$<>8__locals1.<Load>g__Validate|1(ref array16, "bridgePillars");
		CS$<>8__locals1.<Load>g__Validate|1(ref array17, "roofBlocks");
		CS$<>8__locals1.<Load>g__Validate|1(ref array18, "roofBlockMats");
		CS$<>8__locals1.<Load>g__Validate|1(ref array19, "roofBlockDirs");
		int num3 = 0;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				this.cells[i, j] = new Cell
				{
					x = (byte)i,
					z = (byte)j,
					objVal = array[num3],
					_blockMat = array2[num3],
					_block = array3[num3],
					_floorMat = array4[num3],
					_floor = array5[num3],
					obj = array6[num3],
					objMat = array7[num3],
					decal = array8[num3],
					_dirs = array9[num3],
					height = array12[num3],
					_bridge = array13[num3],
					_bridgeMat = array14[num3],
					bridgeHeight = array15[num3],
					bridgePillar = array16[num3],
					_roofBlock = array17[num3],
					_roofBlockMat = array18[num3],
					_roofBlockDir = array19[num3],
					isSeen = array10[num3].GetBit(1),
					isHarvested = array10[num3].GetBit(2),
					impassable = array10[num3].GetBit(3),
					isModified = array10[num3].GetBit(4),
					isClearSnow = array10[num3].GetBit(5),
					isForceFloat = array10[num3].GetBit(6),
					isToggleWallPillar = array10[num3].GetBit(7),
					isWatered = array11[num3].GetBit(0),
					isObjDyed = array11[num3].GetBit(1),
					crossWall = array11[num3].GetBit(2)
				};
				Critter.RebuildCritter(this.cells[i, j]);
				num3++;
			}
		}
		this.things.ForeachReverse(delegate(Thing t)
		{
			if (t.Num <= 0 || t.isDestroyed)
			{
				Debug.Log(string.Concat(new string[]
				{
					"[bug] Removing bugged thing:",
					t.Num.ToString(),
					"/",
					t.isDestroyed.ToString(),
					"/",
					(t != null) ? t.ToString() : null
				}));
				CS$<>8__locals1.<>4__this.things.Remove(t);
			}
		});
		foreach (KeyValuePair<int, CellEffect> keyValuePair in this.cellEffects)
		{
			int key = keyValuePair.Key;
			int num4 = key % this.Size;
			int num5 = key / this.Size;
			this.cells[num4, num5].effect = keyValuePair.Value;
			if (keyValuePair.Value.IsFire)
			{
				this.effectManager.GetOrCreate(new Point(num4, num5));
			}
		}
		this.cellEffects.Clear();
		this.ValidateVersion();
	}

	public void ValidateVersion()
	{
		this.version = EClass.core.version;
	}

	public void OnLoad()
	{
		this.rooms.OnLoad();
		this.tasks.OnLoad();
	}

	public void OnImport(ZoneExportData data)
	{
		this.tasks = new TaskManager();
		data.serializedCards.Restore(this, data.orgMap, false, null);
	}

	public void ExportMetaData(string _path, string id, PartialMap partial = null)
	{
		if (this.custom == null)
		{
			this.custom = new CustomData();
		}
		MapMetaData mapMetaData = new MapMetaData
		{
			name = EClass._zone.Name,
			version = BaseCore.Instance.version.GetInt(),
			partial = partial
		};
		CustomData customData = this.custom;
		mapMetaData.id = id;
		customData.id = id;
		IO.SaveFile(_path + "meta", mapMetaData, false, null);
	}

	public static MapMetaData GetMetaData(string pathZip)
	{
		try
		{
			using (ZipFile zipFile = ZipFile.Read(pathZip))
			{
				ZipEntry zipEntry = zipFile["meta"];
				if (zipEntry != null)
				{
					Debug.Log(zipEntry);
					using (MemoryStream memoryStream = new MemoryStream())
					{
						zipEntry.Extract(memoryStream);
						MapMetaData mapMetaData = IO.LoadStreamJson<MapMetaData>(memoryStream, null);
						Debug.Log(mapMetaData);
						mapMetaData.path = pathZip;
						return mapMetaData;
					}
				}
			}
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
		return null;
	}

	public static void UpdateMetaData(string pathZip, PartialMap partial = null)
	{
		IO.CreateTempDirectory(null);
		ZipFile zipFile = ZipFile.Read(pathZip);
		zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
		zipFile.ExtractAll(IO.TempPath);
		zipFile.Dispose();
		EClass._map.ExportMetaData(IO.TempPath + "/", Path.GetFileNameWithoutExtension(pathZip), partial);
		ZipFile zipFile2;
		zipFile = (zipFile2 = new ZipFile());
		try
		{
			zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
			zipFile.AddDirectory(IO.TempPath);
			zipFile.Save(pathZip);
			zipFile.Dispose();
		}
		finally
		{
			if (zipFile2 != null)
			{
				((IDisposable)zipFile2).Dispose();
			}
		}
		IO.DeleteTempDirectory(null);
	}

	public void AddCardOnActivate(Card c)
	{
		c.parent = this.zone;
		this.props.OnCardAddedToZone(c);
		Chara chara = c.Chara;
		if (chara != null)
		{
			chara.currentZone = EClass._zone;
		}
		if (c.isChara)
		{
			if (!c.pos.IsInBounds)
			{
				c.pos.Set(EClass._map.GetCenterPos());
			}
		}
		else if (!c.pos.IsValid)
		{
			c.pos.Set(EClass._map.GetCenterPos());
		}
		this._AddCard(c.pos.x, c.pos.z, c, true);
	}

	public void OnCardAddedToZone(Card t, int x, int z)
	{
		if (t.isChara)
		{
			this.charas.Add(t.Chara);
		}
		else
		{
			this.things.Add(t.Thing);
		}
		this.props.OnCardAddedToZone(t);
		if (t.isChara && new Point(x, z).cell.HasFullBlock)
		{
			this.DestroyBlock(x, z);
		}
		this._AddCard(x, z, t, true);
		t.trait.OnAddedToZone();
	}

	public void OnCardRemovedFromZone(Card t)
	{
		t.trait.OnRemovedFromZone();
		t.SetPlaceState(PlaceState.none, false);
		this._RemoveCard(t);
		t.parent = null;
		if (t.isChara)
		{
			this.charas.Remove(t.Chara);
			return;
		}
		this.things.Remove(t.Thing);
	}

	public void MoveCard(Point p, Card t)
	{
		this._AddCard(p.x, p.z, t, false);
	}

	public void _AddCard(int x, int z, Card t, bool onAddToZone)
	{
		if (!onAddToZone)
		{
			this._RemoveCard(t);
		}
		t.pos.Set(x, z);
		if (t.IsMultisize)
		{
			t.ForeachPoint(delegate(Point p, bool main)
			{
				p.cell.AddCard(t);
			});
		}
		else
		{
			t.Cell.AddCard(t);
		}
		t.CalculateFOV();
		if (t.isThing && !t.trait.IDActorEx.IsEmpty())
		{
			EClass.scene.AddActorEx(t, null);
		}
	}

	public void _RemoveCard(Card t)
	{
		if (t.IsMultisize)
		{
			t.ForeachPoint(delegate(Point p, bool main)
			{
				p.cell.RemoveCard(t);
			});
		}
		else
		{
			t.Cell.RemoveCard(t);
		}
		t.ClearFOV();
	}

	public Cell GetCell(int index)
	{
		return this.cells[index % this.Size, index % this.SizeXZ / this.Size];
	}

	public void SetSeen(int x, int z, bool seen = true, bool refresh = true)
	{
		if (this.cells[x, z].isSeen == seen)
		{
			return;
		}
		this.cells[x, z].isSeen = seen;
		if (refresh)
		{
			WidgetMinimap.UpdateMap(x, z);
		}
		EClass._map.RefreshNeighborTiles(x, z);
	}

	public void RevealAll(bool reveal = true)
	{
		for (int i = 0; i < this.Size; i++)
		{
			for (int j = 0; j < this.Size; j++)
			{
				EClass._map.SetSeen(i, j, reveal, false);
			}
		}
		this.revealed = !this.revealed;
		WidgetMinimap.UpdateMap();
	}

	public void Reveal(Point center, int power = 100)
	{
		this.ForeachSphere(center.x, center.z, (float)(10 + power / 5), delegate(Point p)
		{
			if (EClass.rnd(power) >= Mathf.Min(p.Distance(center) * 10, power - 10))
			{
				EClass._map.SetSeen(p.x, p.z, true, true);
			}
		});
	}

	public void RefreshFOV(int x, int z, int radius = 6, bool recalculate = false)
	{
		this.ForeachSphere(x, z, (float)radius, delegate(Point p)
		{
			List<Card> list = p.ListCards(false);
			if (recalculate)
			{
				using (List<Card>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Card card = enumerator.Current;
						card.RecalculateFOV();
					}
					return;
				}
			}
			foreach (Card card2 in list)
			{
				card2.CalculateFOV();
			}
		});
	}

	public void RefreshFOVAll()
	{
		foreach (Card card in EClass._map.things.Concat(EClass._map.charas))
		{
			card.RecalculateFOV();
		}
	}

	public void SetFloor(int x, int z, int idMat = 0, int idFloor = 0)
	{
		this.SetFloor(x, z, idMat, idFloor, 0);
	}

	public void SetFloor(int x, int z, int idMat, int idFloor, int dir)
	{
		Cell cell = this.cells[x, z];
		cell._floorMat = (byte)idMat;
		cell._floor = (byte)idFloor;
		cell.floorDir = dir;
		Critter.RebuildCritter(cell);
		this.RefreshNeighborTiles(x, z);
	}

	public void SetBridge(int x, int z, int height = 0, int idMat = 0, int idBridge = 0, int dir = 0)
	{
		Cell cell = this.cells[x, z];
		cell.bridgeHeight = (byte)height;
		cell._bridgeMat = (byte)idMat;
		cell._bridge = (byte)idBridge;
		cell.bridgePillar = 0;
		cell.floorDir = dir;
		if (cell.room != null)
		{
			cell.room.SetDirty();
		}
		this.RefreshNeighborTiles(x, z);
	}

	public void SetRoofBlock(int x, int z, int idMat, int idBlock, int dir, int height)
	{
		Cell cell = this.cells[x, z];
		cell._roofBlockMat = (byte)idMat;
		cell._roofBlock = (byte)idBlock;
		cell._roofBlockDir = (byte)(dir + height * 4);
		this.RefreshSingleTile(x, z);
	}

	public void SetBlock(int x, int z, int idMat = 0, int idBlock = 0)
	{
		this.SetBlock(x, z, idMat, idBlock, 0);
	}

	public void SetBlock(int x, int z, int idMat, int idBlock, int dir)
	{
		Cell cell = this.cells[x, z];
		bool hasFloodBlock = cell.HasFloodBlock;
		cell._blockMat = (byte)idMat;
		cell._block = (byte)idBlock;
		cell.blockDir = dir;
		if (cell.effect == null || !cell.effect.IsFire)
		{
			cell.effect = null;
		}
		cell.isToggleWallPillar = false;
		if (cell.room != null)
		{
			cell.room.SetDirty();
		}
		if (hasFloodBlock || cell.HasFloodBlock)
		{
			this.OnSetBlockOrDoor(x, z);
		}
		this.RefreshNeighborTiles(x, z);
	}

	public void OnSetBlockOrDoor(int x, int z)
	{
		new Point(x, z);
		this.TryRemoveRoom(x, z);
		if (x > 0)
		{
			this.TryRemoveRoom(x - 1, z);
		}
		if (x < this.Size - 1)
		{
			this.TryRemoveRoom(x + 1, z);
		}
		if (z > 0)
		{
			this.TryRemoveRoom(x, z - 1);
		}
		if (z < this.Size - 1)
		{
			this.TryRemoveRoom(x, z + 1);
		}
		if (x > 0 && z < this.Size - 1)
		{
			this.TryRemoveRoom(x - 1, z + 1);
		}
		this.roomHash.Clear();
		this.TryAddRoom(x, z);
		if (x > 0)
		{
			this.TryAddRoom(x - 1, z);
		}
		if (x < this.Size - 1)
		{
			this.TryAddRoom(x + 1, z);
		}
		if (z > 0)
		{
			this.TryAddRoom(x, z - 1);
		}
		if (z < this.Size - 1)
		{
			this.TryAddRoom(x, z + 1);
		}
		if (x > 0 && z < this.Size - 1)
		{
			this.TryAddRoom(x - 1, z + 1);
		}
	}

	public void TryRemoveRoom(int x, int z)
	{
		if (this.cells[x, z].HasFloodBlock)
		{
			return;
		}
		Room room = this.cells[x, z].room;
		if (room != null)
		{
			this.rooms.RemoveRoom(room);
		}
	}

	public void TryAddRoom(int x, int z)
	{
		if (EClass._zone.DisableRooms)
		{
			return;
		}
		if (this.roomHash.Contains(x + z * this.Size) || this.cells[x, z].HasFloodBlock)
		{
			return;
		}
		FloodSpiller floodSpiller = this.flood;
		IFloodCell[,] array = this.cells;
		FloodSpiller.Result result = floodSpiller.SpillFlood(array, x, z);
		if (!result.IsValid)
		{
			return;
		}
		bool flag = false;
		foreach (IFloodCell floodCell in result.visited)
		{
			if (floodCell.hasDoor)
			{
				flag = true;
				break;
			}
			Cell cell = floodCell as Cell;
			if (cell.sourceBlock.tileType.IsFloodDoor || cell.Front.hasDoor || cell.Right.hasDoor || cell.FrontRight.hasDoor || cell.Back.hasDoor || cell.Left.hasDoor || cell.BackLeft.hasDoor)
			{
				flag = true;
				break;
			}
		}
		if (!flag && this.IsIndoor)
		{
			foreach (IFloodCell floodCell2 in result.visited)
			{
				Cell cell2 = floodCell2 as Cell;
				if (cell2.detail != null && cell2.detail.things.Count != 0)
				{
					foreach (Thing thing in cell2.detail.things)
					{
						if (thing.trait is TraitRoomPlate || thing.trait is TraitHouseBoard)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}
		if (!flag)
		{
			return;
		}
		Room room = this.rooms.AddRoom(new Room());
		foreach (IFloodCell floodCell3 in result.visited)
		{
			Cell cell3 = floodCell3 as Cell;
			byte x2 = cell3.x;
			byte z2 = cell3.z;
			room.AddPoint(new Point((int)x2, (int)z2));
			this.roomHash.Add((int)x2 + (int)z2 * this.Size);
			if (z2 > 0 && cell3.Front.HasFloodBlock && cell3.Front.room == null)
			{
				room.AddPoint(new Point((int)x2, (int)(z2 - 1)));
				this.roomHash.Add((int)x2 + (int)(z2 - 1) * this.Size);
			}
			if ((int)x2 < this.Size - 1 && cell3.Right.HasFloodBlock && cell3.Right.room == null)
			{
				room.AddPoint(new Point((int)(x2 + 1), (int)z2));
				this.roomHash.Add((int)(x2 + 1) + (int)z2 * this.Size);
			}
			if (z2 > 0 && (int)x2 < this.Size - 1 && cell3.FrontRight.HasFloodBlock && cell3.FrontRight.room == null)
			{
				room.AddPoint(new Point((int)(x2 + 1), (int)(z2 - 1)));
				this.roomHash.Add((int)(x2 + 1) + (int)(z2 - 1) * this.Size);
			}
		}
	}

	public void SetBlockDir(int x, int z, int dir)
	{
		Cell cell = this.cells[x, z];
		cell._block = (byte)cell.sourceBlock.id;
		cell.blockDir = dir;
	}

	public void ModFire(int x, int z, int amount)
	{
		Cell cell = this.cells[x, z];
		if (cell.IsTopWaterAndNoSnow || cell.IsSnowTile)
		{
			return;
		}
		if (cell.effect == null && amount > 0)
		{
			SE.Play("fire");
		}
		CellEffect effect = cell.effect;
		int num = amount + ((effect != null) ? effect.FireAmount : 0);
		if (num > 20)
		{
			num = 20;
		}
		if (num <= 0)
		{
			cell.effect = null;
			return;
		}
		cell.effect = new CellEffect
		{
			id = 3,
			amount = num
		};
		this.effectManager.GetOrCreate(new Point(x, z));
	}

	public void TryShatter(Point pos, int ele, int power)
	{
		Element element = Element.Create(ele, 0);
		List<Card> list = new List<Card>();
		Map.<>c__DisplayClass101_0 CS$<>8__locals1;
		CS$<>8__locals1.fire = (ele == 910);
		bool fire = CS$<>8__locals1.fire;
		List<Card> list2 = pos.ListCards(false);
		if (CS$<>8__locals1.fire && (pos.cell.IsSnowTile || pos.cell.IsTopWater))
		{
			return;
		}
		foreach (Card card in list2)
		{
			if (card.ResistLvFrom(ele) < 3 && !(card.trait is TraitBlanket) && (EClass.rnd(3) != 0 || Map.<TryShatter>g__CanCook|101_0(card, ref CS$<>8__locals1)) && (!card.IsPCFaction || EClass.rnd(3) != 0))
			{
				if (EClass._zone.IsPCFaction && EClass.Branch.lv >= 3)
				{
					Card rootCard = card.GetRootCard();
					if (!rootCard.isChara || rootCard.Chara.IsPCFaction)
					{
						if (pos.IsSync)
						{
							Msg.Say("damage_protectCore", card, null, null, null);
							continue;
						}
						continue;
					}
				}
				if (card.isThing && card.things.Count == 0)
				{
					list.Add(card);
				}
				else
				{
					Thing thing;
					if (ele != 910)
					{
						thing = card.things.FindBest<TraitBlanketColdproof>((Thing t) => -t.c_charges);
					}
					else
					{
						thing = card.things.FindBest<TraitBlanketFireproof>((Thing t) => -t.c_charges);
					}
					Thing thing2 = thing;
					if (thing2 != null)
					{
						thing2.ModCharge(-1, false);
						Card rootCard2 = card.GetRootCard();
						if (pos.IsSync)
						{
							Msg.Say((card.isChara ? "blanketInv_" : "blanketGround_") + element.source.alias, thing2, card, null, null);
						}
						if (thing2.c_charges <= 0)
						{
							thing2.Die(element, null, AttackSource.None);
							if (rootCard2.IsPCParty)
							{
								ActionMode.Adv.itemLost++;
							}
						}
					}
					else
					{
						foreach (Thing thing3 in card.things.List((Thing a) => a.things.Count == 0, false))
						{
							Card parentCard = thing3.parentCard;
							if (parentCard == null || (!(parentCard.trait is TraitChestMerchant) && !parentCard.HasEditorTag(EditorTag.PreciousContainer)))
							{
								list.Add(thing3);
							}
						}
					}
				}
			}
		}
		list.Shuffle<Card>();
		int num = 0;
		foreach (Card card2 in list)
		{
			if (card2.trait.CanBeDestroyed && !card2.category.IsChildOf("currency") && !(card2.trait is TraitDoor) && !(card2.trait is TraitFigure) && !(card2.trait is TraitTrainingDummy) && card2.rarity < Rarity.Legendary)
			{
				Card rootCard3 = card2.GetRootCard();
				if (!card2.IsEquipmentOrRanged || (EClass.rnd(4) == 0 && ((!card2.IsRangedWeapon && !card2.Thing.isEquipped) || !rootCard3.IsPCFaction || EClass.rnd(4) == 0)))
				{
					if (ele != 910)
					{
						if (ele == 911)
						{
							if (!card2.category.IsChildOf("drink") && EClass.rnd(5) == 0)
							{
								continue;
							}
						}
					}
					else
					{
						if (card2.isFireproof)
						{
							continue;
						}
						if (!card2.category.IsChildOf("book") && EClass.rnd(2) == 0)
						{
							continue;
						}
					}
					if (EClass.rnd(num * num) == 0)
					{
						bool flag = Map.<TryShatter>g__CanCook|101_0(card2, ref CS$<>8__locals1);
						string text = "";
						if (flag)
						{
							List<SourceThing.Row> list3 = new List<SourceThing.Row>();
							foreach (RecipeSource recipeSource in RecipeManager.list)
							{
								SourceThing.Row row = recipeSource.row as SourceThing.Row;
								if (row != null && !row.isOrigin && !row.components.IsEmpty() && (row.components.Length < 3 || row.components[2].StartsWith('+')) && row.Category.IsChildOf("meal"))
								{
									if (!row.factory.IsEmpty())
									{
										string a2 = row.factory[0];
										if (a2 == "chopper" || a2 == "mixer" || a2 == "camppot" || a2 == "cauldron")
										{
											continue;
										}
									}
									if (row.components[0] == card2.id || row.components[0] == card2.sourceCard._origin)
									{
										list3.Add(row);
									}
								}
							}
							if (list3.Count > 0)
							{
								text = list3.RandomItem<SourceThing.Row>().id;
							}
						}
						if (flag && !text.IsEmpty())
						{
							card2.GetRoot();
							Thing thing4 = card2.Split(1);
							List<Thing> list4 = new List<Thing>();
							list4.Add(thing4);
							Thing thing5 = ThingGen.Create(text, -1, -1);
							CraftUtil.MakeDish(thing5, list4, 999, null);
							thing5.elements.ModBase(2, EClass.curve(power / 10, 50, 10, 75));
							if (pos.IsSync)
							{
								Msg.Say((rootCard3 == card2) ? "cook_groundItem" : "cook_invItem", thing4, rootCard3, thing5.Name, null);
							}
							if (rootCard3 == card2)
							{
								EClass._zone.AddCard(thing5, card2.pos);
							}
							else if (rootCard3.isChara)
							{
								rootCard3.Chara.Pick(thing5, false, true);
							}
							else
							{
								rootCard3.AddThing(thing5, true, -1, -1);
							}
							thing4.Destroy();
						}
						else
						{
							int num2 = EClass.rnd(card2.Num) / 2 + 1;
							if (card2.Num > num2)
							{
								Thing thing6 = card2.Split(num2);
								if (pos.IsSync)
								{
									Msg.Say((rootCard3 == card2) ? "damage_groundItem" : "damage_invItem", thing6, rootCard3, null, null);
								}
								thing6.Destroy();
								if (rootCard3.IsPCFaction)
								{
									WidgetPopText.Say("popDestroy".lang(thing6.Name, rootCard3.Name, null, null, null), FontColor.Default, null);
								}
							}
							else
							{
								card2.Die(element, null, AttackSource.None);
								if (rootCard3.IsPCFaction)
								{
									WidgetPopText.Say("popDestroy".lang(card2.Name, rootCard3.Name, null, null, null), FontColor.Default, null);
								}
							}
						}
						if (rootCard3.IsPCParty)
						{
							ActionMode.Adv.itemLost++;
						}
						num++;
					}
				}
			}
		}
		this._ValidateInstalled(pos.x, pos.z);
	}

	public void Burn(int x, int z, bool instant = false)
	{
		Cell cell = this.cells[x, z];
		Point sharedPoint = cell.GetSharedPoint();
		if ((instant || EClass.rnd(10) == 0) && cell.HasObj)
		{
			if (cell.sourceObj.tileType is TileTypeTree)
			{
				this.SetObj(x, z, cell.matObj_fixed.id, 59, 0, EClass.rnd(4));
			}
			else
			{
				this.SetObj(x, z, 0, 1, 0);
				if (EClass.rnd(2) == 0)
				{
					EClass._zone.AddCard(ThingGen.Create((EClass.rnd(2) == 0) ? "ash" : "ash2", -1, -1), sharedPoint);
				}
			}
		}
		if ((instant || EClass.rnd(5) == 0) && cell._block != 0 && cell._block != 96)
		{
			if (EClass.rnd(10) == 0 || !cell.sourceBlock.tileType.IsWall)
			{
				cell._block = 0;
			}
			else
			{
				cell._block = 96;
			}
			this.SetObj(x, z, 0, 1, 0);
			if (cell.room != null)
			{
				cell.room.SetDirty();
			}
			this.RefreshNeighborTiles(x, z);
		}
		if (instant || EClass.rnd(10) == 0)
		{
			if (EClass.rnd(4) != 0)
			{
				cell._floor = 49;
			}
			if (cell._bridge != 0 && EClass.rnd(5) != 0)
			{
				cell._bridge = 49;
			}
		}
		foreach (Card card in sharedPoint.ListCards(false))
		{
			if (card.trait.CanBeDestroyed && !card.trait.IsDoor && card.isThing)
			{
				if (instant)
				{
					card.Destroy();
					EClass._zone.AddCard(ThingGen.Create((EClass.rnd(2) == 0) ? "ash" : "ash2", -1, -1), sharedPoint);
				}
				else
				{
					card.DamageHP(30, 910, 100, AttackSource.None, null, true);
				}
			}
		}
		if (instant)
		{
			cell.effect = null;
		}
	}

	public void SetLiquid(int x, int z, CellEffect effect = null)
	{
		Cell cell = this.cells[x, z];
		if (cell.IsTopWaterAndNoSnow && effect != null)
		{
			return;
		}
		cell.effect = effect;
	}

	public void SetLiquid(int x, int z, int id, int value = 1)
	{
		Cell cell = this.cells[x, z];
		if (cell.IsTopWaterAndNoSnow && value != 0)
		{
			return;
		}
		if (value > 3)
		{
			value = 3;
		}
		if (value <= 0)
		{
			cell.effect = null;
			return;
		}
		cell.effect = new CellEffect
		{
			id = id,
			amount = value
		};
	}

	public void SetEffect(int x, int z, CellEffect effect = null)
	{
		this.cells[x, z].effect = effect;
	}

	public void ModLiquid(int x, int z, int amount)
	{
		Cell cell = this.cells[x, z];
		if (cell.IsTopWaterAndNoSnow || cell.effect == null)
		{
			return;
		}
		cell.effect.amount += amount;
		if (cell.effect.amount <= 0)
		{
			cell.effect = null;
		}
	}

	public void ClearRainAndDecal()
	{
		this.ForeachCell(delegate(Cell c)
		{
			c.effect = null;
			c.decal = 0;
		});
	}

	public void SetObj(int x, int z, int id = 0, int value = 1, int dir = 0)
	{
		this.SetObj(x, z, (int)((byte)EClass.sources.objs.rows[id].DefaultMaterial.id), id, value, dir);
	}

	public void SetObj(int x, int z, int idMat, int idObj, int value, int dir)
	{
		Cell cell = this.cells[x, z];
		cell.obj = (byte)idObj;
		cell.objVal = (byte)value;
		cell.objMat = (byte)idMat;
		cell.objDir = dir;
		cell.isHarvested = false;
		cell.isObjDyed = false;
		SourceObj.Row sourceObj = cell.sourceObj;
		if (!sourceObj.matCategory.IsEmpty())
		{
			int num = EClass._zone.DangerLv;
			if (sourceObj.tag.Contains("spot"))
			{
				num += EClass.pc.Evalue(1656) * 5;
			}
			cell.objMat = (byte)MATERIAL.GetRandomMaterialFromCategory(num, sourceObj.matCategory.Split(',', StringSplitOptions.None), cell.matObj).id;
		}
		if (this.backerObjs.ContainsKey(cell.index))
		{
			this.backerObjs.Remove(cell.index);
		}
		if (this.plants.ContainsKey(cell.index))
		{
			this.plants.Remove(cell.index);
		}
		Critter.RebuildCritter(cell);
		this.RefreshNeighborTiles(x, z);
	}

	public void AddBackerTree(bool draw)
	{
		SourceBacker.Row item = EClass.sources.backers.listTree.NextItem(ref BackerContent.indexTree);
		int num = draw ? 2 : 1;
		EClass._map.bounds.ForeachCell(delegate(Cell c)
		{
			int num;
			if (num <= 0 || c.growth == null || !c.growth.IsTree || !c.growth.IsMature || (EClass.player.doneBackers.Contains(item.id) && !EClass.core.config.test.ignoreBackerDestoryFlag))
			{
				return;
			}
			if (c.sourceObj.alias == item.tree || item.tree == "random")
			{
				this.backerObjs[c.index] = item.id;
				Debug.Log(string.Concat(new string[]
				{
					c.index.ToString(),
					"/",
					c.x.ToString(),
					"/",
					c.z.ToString(),
					"/",
					item.id.ToString(),
					"/",
					item.Name,
					"/",
					item.tree
				}));
				num = num;
				num--;
				item = EClass.sources.backers.listTree.NextItem(ref BackerContent.indexTree);
			}
		});
	}

	public SourceBacker.Row GetBackerObj(Point p)
	{
		if (!this.backerObjs.ContainsKey(p.index))
		{
			return null;
		}
		return EClass.sources.backers.map.TryGetValue(this.backerObjs[p.index], null);
	}

	public void ApplyBackerObj(Point p, int id = -1)
	{
		if (!p.HasObj)
		{
			return;
		}
		bool flag = p.sourceObj.id == 82;
		SourceBacker.Row row = (id != -1) ? EClass.sources.backers.map.TryGetValue(id, null) : (flag ? EClass.sources.backers.listRemain.NextItem(ref BackerContent.indexRemain) : EClass.sources.backers.listTree.NextItem(ref BackerContent.indexTree));
		if (row != null)
		{
			if (!EClass.player.doneBackers.Contains(row.id) || EClass.core.config.test.ignoreBackerDestoryFlag)
			{
				this.backerObjs[p.index] = row.id;
				if (flag)
				{
					p.cell.objDir = row.skin;
					return;
				}
			}
			else
			{
				this.backerObjs.Remove(p.index);
			}
		}
	}

	public void DropBlockComponent(Point point, TileRow r, SourceMaterial.Row mat, bool recoverBlock, bool isPlatform = false, Chara c = null)
	{
		if (r.components.Length == 0)
		{
			return;
		}
		RecipeManager.BuildList();
		Thing thing;
		if (recoverBlock)
		{
			if (r is SourceFloor.Row)
			{
				thing = ThingGen.CreateFloor(r.id, mat.id, isPlatform);
			}
			else
			{
				thing = ThingGen.CreateBlock(r.id, mat.id);
			}
		}
		else
		{
			RecipeSource recipeSource = RecipeManager.Get(r.RecipeID + (isPlatform ? "-b" : ""));
			if (recipeSource == null)
			{
				return;
			}
			string idingredient = recipeSource.GetIDIngredient();
			if (idingredient == null)
			{
				return;
			}
			thing = ThingGen.Create(idingredient, -1, -1);
			thing.ChangeMaterial(mat.alias);
		}
		if (EClass.scene.actionMode.IsBuildMode && EClass.debug.godBuild)
		{
			this.PutAway(thing);
			return;
		}
		this.TrySmoothPick(point, thing, c);
	}

	public void MineBlock(Point point, bool recoverBlock = false, Chara c = null)
	{
		bool flag = ActionMode.Mine.IsRoofEditMode(null) && point.cell._roofBlock > 0;
		if (!point.IsValid || (!flag && !point.cell.HasBlock))
		{
			return;
		}
		SourceMaterial.Row row = flag ? point.matRoofBlock : point.matBlock;
		byte b = flag ? point.cell._roofBlock : point.cell._block;
		SourceBlock.Row row2 = EClass.sources.blocks.rows[(int)b];
		Effect.Get("smoke").Play(point, 0f, null, null);
		Effect.Get("mine").Play(point, 0f, null, null).SetParticleColor(row.GetColor()).Emit(10 + EClass.rnd(10));
		point.PlaySound(row.GetSoundDead(row2), true, 1f, true);
		row.AddBlood(point, 8);
		bool flag2 = c == null || c.IsAgent || c.IsPCFactionOrMinion;
		if (flag)
		{
			point.cell._roofBlock = 0;
			this.RefreshSingleTile(point.x, point.z);
		}
		else
		{
			if (point.cell.HasFullBlock)
			{
				this.RemoveLonelyRamps(point.cell);
			}
			point.SetBlock(0, 0);
			if (flag2 && point.sourceObj.tileType.IsBlockMount)
			{
				this.MineObj(point, null, c);
			}
		}
		if (flag2)
		{
			this.DropBlockComponent(point, row2, row, recoverBlock, false, c);
		}
		this.RefreshShadow(point.x, point.z);
		this.RefreshShadow(point.x, point.z - 1);
		this.ValidateInstalled(point);
		this.RefreshFOV(point.x, point.z, 6, false);
		if (flag2 && !point.cell.isModified && !flag)
		{
			if (b == 17 || EClass.rnd(100) == 0)
			{
				this.zone.AddCard(ThingGen.Create("money2", -1, -1), point);
			}
			if (EClass._zone.DangerLv >= 10 && EClass.rnd(200) == 0)
			{
				this.zone.AddCard(ThingGen.Create("crystal_earth", -1, -1), point);
			}
			if (EClass._zone.DangerLv >= 25 && EClass.rnd(200) == 0)
			{
				this.zone.AddCard(ThingGen.Create("crystal_sun", -1, -1), point);
			}
			if (EClass._zone.DangerLv >= 40 && EClass.rnd(200) == 0)
			{
				this.zone.AddCard(ThingGen.Create("crystal_mana", -1, -1), point);
			}
			point.cell.isModified = true;
		}
	}

	public void MineRamp(Point point, int ramp, bool recoverBlock = false)
	{
		if (!point.IsValid || !point.cell.HasFullBlock)
		{
			return;
		}
		SourceMaterial.Row matBlock = point.matBlock;
		byte block = point.cell._block;
		Effect.Get("smoke").Play(point, 0f, null, null);
		Effect.Get("mine").Play(point, 0f, null, null).SetParticleColor(point.matBlock.GetColor()).Emit(10 + EClass.rnd(10));
		this.MineObj(point, null, null);
		int rampDir = EClass._map.GetRampDir(point.x, point.z, EClass.sources.blocks.rows[ramp].tileType);
		this.RemoveLonelyRamps(point.cell);
		this.SetBlock(point.x, point.z, (int)point.cell._blockMat, ramp, rampDir);
		this.DropBlockComponent(point, EClass.sources.blocks.rows[(int)block], matBlock, recoverBlock, false, null);
	}

	public void MineFloor(Point point, Chara c = null, bool recoverBlock = false, bool removePlatform = true)
	{
		if (!point.IsValid || (!point.HasFloor && !point.HasBridge))
		{
			return;
		}
		SourceMaterial.Row row = point.cell.HasBridge ? point.matBridge : point.matFloor;
		SourceFloor.Row c2 = point.cell.HasBridge ? point.sourceBridge : point.sourceFloor;
		Effect.Get("mine").Play(point, 0f, null, null).SetParticleColor(row.GetColor()).Emit(10 + EClass.rnd(10));
		point.PlaySound(row.GetSoundDead(c2), true, 1f, true);
		this.MineObj(point, null, c);
		if (point.cell.HasBridge && removePlatform)
		{
			this.DropBlockComponent(EClass.pc.pos, point.sourceBridge, point.matBridge, recoverBlock, true, c);
			EClass._map.SetBridge(point.x, point.z, 0, 0, 0, 0);
			if (point.IsSky)
			{
				EClass.pc.Kick(point, true);
			}
			return;
		}
		if (EClass._zone.IsSkyLevel)
		{
			this.DropBlockComponent(EClass.pc.pos, point.sourceFloor, row, recoverBlock, false, c);
			this.SetFloor(point.x, point.z, 0, 90);
			if (point.IsSky)
			{
				EClass.pc.Kick(point, true);
			}
			return;
		}
		if (this.zone.IsRegion || point.cell._floor == 40)
		{
			Thing thing = ThingGen.CreateRawMaterial(row);
			thing.ChangeMaterial(row.alias);
			this.TrySmoothPick(point, thing, c);
		}
		else
		{
			this.DropBlockComponent(point, point.sourceFloor, row, recoverBlock, false, c);
		}
		if (EClass._zone.IsRegion || point.sourceFloor.components[0].Contains("chunk@soil"))
		{
			return;
		}
		point.SetFloor(EClass.sources.floors.rows[1].DefaultMaterial.id, 40);
	}

	public void RefreshShadow(int x, int z)
	{
	}

	public void TrySmoothPick(Cell cell, Thing t, Chara c)
	{
		this.TrySmoothPick(cell.GetPoint(), t, c);
	}

	public void TrySmoothPick(Point p, Thing t, Chara c)
	{
		if (c != null && c.IsAgent)
		{
			EClass.pc.PickOrDrop(p, t, true);
			return;
		}
		if (c != null && (c.pos.Equals(p) || EClass.core.config.game.smoothPick || EClass._zone.IsRegion))
		{
			c.PickOrDrop(p, t, true);
			return;
		}
		EClass._zone.AddCard(t, p);
	}

	public void DestroyObj(Point point)
	{
		Cell cell = point.cell;
		RenderRow sourceObj = cell.sourceObj;
		SourceMaterial.Row matObj = cell.matObj;
		if (sourceObj.tileType.IsBlockPass)
		{
			Effect.Get("smoke").Play(point, 0f, null, null);
		}
		Effect.Get("mine").Play(point, 0f, null, null).SetParticleColor(cell.matObj.GetColor()).Emit(10 + EClass.rnd(10));
		point.PlaySound(matObj.GetSoundDead(null), true, 1f, true);
		matObj.AddBlood(point, 3);
	}

	public void MineObj(Point point, Task task = null, Chara c = null)
	{
		Map.<>c__DisplayClass121_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.point = point;
		CS$<>8__locals1.c = c;
		if (!CS$<>8__locals1.point.IsValid || !CS$<>8__locals1.point.HasObj)
		{
			return;
		}
		Cell cell = CS$<>8__locals1.point.cell;
		SourceObj.Row sourceObj = cell.sourceObj;
		bool flag = false;
		if (CS$<>8__locals1.c == null && task != null)
		{
			CS$<>8__locals1.c = task.owner;
		}
		bool flag2 = CS$<>8__locals1.c == null || CS$<>8__locals1.c.IsAgent || CS$<>8__locals1.c.IsPCFactionOrMinion;
		this.DestroyObj(CS$<>8__locals1.point);
		if (flag2)
		{
			SourceMaterial.Row matObj_fixed = cell.matObj_fixed;
			TaskHarvest taskHarvest = task as TaskHarvest;
			if (taskHarvest != null && taskHarvest.IsReapSeed)
			{
				int num = 1 + EClass.rnd(2) + ((EClass.rnd(3) == 0) ? 1 : 0);
				if (EClass._zone.IsPCFaction)
				{
					int soilCost = EClass._zone.GetSoilCost();
					int maxSoil = EClass.Branch.MaxSoil;
					if (soilCost > maxSoil)
					{
						num -= EClass.rnd(2 + (soilCost - maxSoil) / 10);
					}
				}
				if (num <= 0)
				{
					Msg.Say("seedSpoiled", cell.GetObjName(), null, null, null);
				}
				else
				{
					Thing t = TraitSeed.MakeSeed(sourceObj, this.TryGetPlant(cell)).SetNum(num);
					EClass.pc.PickOrDrop(CS$<>8__locals1.point, t, true);
				}
				if (cell.growth.IsTree)
				{
					cell.isHarvested = true;
					return;
				}
			}
			else if (sourceObj.HasGrowth)
			{
				cell.growth.PopMineObj(CS$<>8__locals1.c);
			}
			else
			{
				if (cell.HasBlock && (sourceObj.id == 18 || sourceObj.id == 19))
				{
					this.MineBlock(CS$<>8__locals1.point, false, CS$<>8__locals1.c);
				}
				string alias = sourceObj.alias;
				uint num2 = <PrivateImplementationDetails>.ComputeStringHash(alias);
				if (num2 <= 529228496U)
				{
					if (num2 != 414349311U)
					{
						if (num2 != 489301296U)
						{
							if (num2 == 529228496U)
							{
								if (!(alias == "wreck_junk"))
								{
								}
							}
						}
						else if (!(alias == "wreck_precious"))
						{
						}
					}
					else if (alias == "nest_bird")
					{
						if (EClass.rnd(5) <= 1)
						{
							this.<MineObj>g__Pop|121_0(ThingGen.Create((EClass.rnd(10) == 0) ? "egg_fertilized" : "_egg", -1, -1).TryMakeRandomItem(-1), ref CS$<>8__locals1);
						}
					}
				}
				else if (num2 <= 1575653919U)
				{
					if (num2 != 1117262783U)
					{
						if (num2 == 1575653919U)
						{
							if (!(alias == "wreck_scrap"))
							{
							}
						}
					}
					else if (!(alias == "wreck_stone"))
					{
					}
				}
				else if (num2 != 3603235706U)
				{
					if (num2 == 3937420123U)
					{
						if (!(alias == "wreck_wood"))
						{
						}
					}
				}
				else if (!(alias == "wreck_cloth"))
				{
				}
				int num3 = EClass.rnd(EClass.rnd(sourceObj.components.Length) + 1);
				string[] array = sourceObj.components[num3].Split('/', StringSplitOptions.None);
				Thing thing = ThingGen.Create(array[0], matObj_fixed.alias);
				if (array.Length > 1)
				{
					thing.SetNum(EClass.rnd(array[1].ToInt()) + 1);
				}
				this.<MineObj>g__Pop|121_0(thing, ref CS$<>8__locals1);
			}
		}
		this.SetObj(CS$<>8__locals1.point.x, CS$<>8__locals1.point.z, 0, 1, 0);
		cell.gatherCount = 0;
		if (flag)
		{
			this.RefreshFOV(CS$<>8__locals1.point.x, CS$<>8__locals1.point.z, 6, false);
		}
	}

	public void MineObjSound(Point point)
	{
		point.PlaySound(point.cell.matObj.GetSoundDead(point.cell.sourceObj), true, 1f, true);
	}

	public PlantData TryGetPlant(Point p)
	{
		return this.plants.TryGetValue(p.index, null);
	}

	public PlantData TryGetPlant(Cell c)
	{
		return this.plants.TryGetValue(c.index, null);
	}

	public PlantData AddPlant(Point pos, Thing seed)
	{
		PlantData plantData = new PlantData
		{
			seed = seed
		};
		this.plants[pos.index] = plantData;
		return plantData;
	}

	public void RemovePlant(Point pos)
	{
		this.plants.Remove(pos.index);
	}

	public void ValidateInstalled(Point p)
	{
		this._ValidateInstalled(p.x, p.z);
		this._ValidateInstalled(p.x + 1, p.z);
		this._ValidateInstalled(p.x - 1, p.z);
		this._ValidateInstalled(p.x, p.z + 1);
		this._ValidateInstalled(p.x, p.z - 1);
	}

	public void _ValidateInstalled(int x, int y)
	{
		Point point = Point.shared.Set(x, y);
		if (!point.IsValid)
		{
			return;
		}
		List<Card> list = point.ListCards(false);
		CellDetail detail = point.cell.detail;
		if (detail == null)
		{
			return;
		}
		foreach (Card card in list)
		{
			if (card.isThing && card.trait.CanBeDestroyed && card.IsInstalled)
			{
				HitResult hitResult = card.TileType._HitTest(point, card.Thing, false);
				if (card.Thing.stackOrder != detail.things.IndexOf(card.Thing) || (hitResult != HitResult.Valid && hitResult != HitResult.Warning))
				{
					if (EClass._zone.IsPCFaction)
					{
						card.SetPlaceState(PlaceState.roaming, false);
					}
					else
					{
						card.Die(null, null, AttackSource.None);
					}
				}
			}
		}
	}

	public void RemoveLonelyRamps(Cell cell)
	{
		for (int i = 0; i < 4; i++)
		{
			Cell dependedRamp = this.GetDependedRamp(cell);
			if (dependedRamp == null)
			{
				break;
			}
			this.MineBlock(dependedRamp.GetPoint(), false, null);
		}
	}

	public void DestroyBlock(int x, int z)
	{
		this.SetBlock(x, z, 0, 0);
	}

	public void AddDecal(int x, int z, int id, int amount = 1, bool refresh = true)
	{
		if (x < 0 || z < 0 || x >= this.Size || z >= this.Size)
		{
			return;
		}
		Cell cell = this.cells[x, z];
		if (!cell.sourceFloor.tileType.AllowBlood)
		{
			return;
		}
		if ((int)(cell.decal / 8) != id && (int)(cell.decal % 8) > amount)
		{
			return;
		}
		if ((int)(cell.decal / 8) != id && cell.decal % 8 == 0)
		{
			amount--;
		}
		int num = Mathf.Clamp((int)(((int)(cell.decal / 8) == id) ? (cell.decal % 8) : 0) + amount, 0, 7);
		cell.decal = (byte)(id * 8 + num);
		if (refresh)
		{
			this.RefreshNeighborTiles(x, z);
		}
	}

	public void SetDecal(int x, int z, int id = 0, int amount = 1, bool refresh = true)
	{
		this.cells[x, z].decal = (byte)(id * 8 + ((id == 0) ? 0 : amount));
		if (refresh)
		{
			this.RefreshNeighborTiles(x, z);
		}
	}

	public void SetFoormark(Point pos, int id, int angle, int offset = 0)
	{
		Cell cell = pos.cell;
		int tile = this.AngleToIndex(angle) + offset;
		Footmark footmark = new Footmark
		{
			tile = tile,
			remaining = 10
		};
		footmark.pos.Set(pos);
		this.footmarks.Add(footmark);
		cell.GetOrCreateDetail().footmark = footmark;
	}

	public int AngleToIndex(int a)
	{
		if (EClass._zone.IsRegion)
		{
			if (a == 135)
			{
				return 7;
			}
			if (a == 180)
			{
				return 0;
			}
			if (a == 225)
			{
				return 1;
			}
			if (a == -90)
			{
				return 2;
			}
			if (a == -45)
			{
				return 3;
			}
			if (a == 0)
			{
				return 4;
			}
			if (a == 45)
			{
				return 5;
			}
			return 6;
		}
		else
		{
			if (a == 135)
			{
				return 0;
			}
			if (a == 180)
			{
				return 1;
			}
			if (a == 225)
			{
				return 2;
			}
			if (a == -90)
			{
				return 3;
			}
			if (a == -45)
			{
				return 4;
			}
			if (a == 0)
			{
				return 5;
			}
			if (a == 45)
			{
				return 6;
			}
			return 7;
		}
	}

	public void RefreshSingleTile(int x, int z)
	{
		this.cells[x, z].Refresh();
	}

	public void RefreshAllTiles()
	{
		for (int i = 0; i < this.Size; i++)
		{
			for (int j = 0; j < this.Size; j++)
			{
				this.cells[i, j].Refresh();
			}
		}
	}

	public void RefreshNeighborTiles(int x, int z)
	{
		this.cells[x, z].Refresh();
		for (int i = x - 2; i < x + 3; i++)
		{
			if (i >= 0 && i < this.Size)
			{
				for (int j = z - 2; j < z + 3; j++)
				{
					if (j >= 0 && j < this.Size && (x != i || z != j))
					{
						this.cells[i, j].Refresh();
					}
				}
			}
		}
	}

	public void QuickRefreshTile(int x, int z)
	{
		Cell cell = this.cells[x, z];
		Cell cell2 = (x > 0) ? this.cells[x - 1, z] : Cell.Void;
		Cell cell3 = (x + 1 < this.Size) ? this.cells[x + 1, z] : Cell.Void;
		Cell cell4 = (z > 0) ? this.cells[x, z - 1] : Cell.Void;
		Cell cell5 = (z + 1 < this.Size) ? this.cells[x, z + 1] : Cell.Void;
		Cell cell6 = (x > 0 && z > 0) ? this.cells[x - 1, z - 1] : Cell.Void;
		Cell cell7 = (x + 1 < this.Size && z > 0) ? this.cells[x + 1, z - 1] : Cell.Void;
		Cell cell8 = (x > 0 && z + 1 < this.Size) ? this.cells[x - 1, z + 1] : Cell.Void;
		Cell cell9 = (x + 1 < this.Size && z + 1 < this.Size) ? this.cells[x + 1, z + 1] : Cell.Void;
		cell.isSurrounded4d = (cell2.HasFullBlock && cell3.HasFullBlock && cell4.HasFullBlock && cell5.HasFullBlock);
		cell.isSurrounded = (cell.isSurrounded4d && cell6.HasFullBlock && cell7.HasFullBlock && cell8.HasFullBlock && cell9.HasFullBlock);
	}

	public int GetRampDir(int x, int z, TileType blockType = null)
	{
		Cell cell = this.cells[x, z];
		if (cell.HasFullBlock)
		{
			if (blockType == null)
			{
				blockType = cell.sourceBlock.tileType;
			}
			Cell right = cell.Right;
			Cell front = cell.Front;
			Cell left = cell.Left;
			Cell back = cell.Back;
			if (!right.HasBlock && !right.IsVoid && left.HasFullBlock && front.CanBuildRamp(1) && back.CanBuildRamp(1))
			{
				return 1;
			}
			if (!front.HasBlock && !front.IsVoid && back.HasFullBlock && left.CanBuildRamp(0) && right.CanBuildRamp(0))
			{
				return 0;
			}
			if (!left.HasBlock && !left.IsVoid && right.HasFullBlock && front.CanBuildRamp(3) && back.CanBuildRamp(3))
			{
				return 3;
			}
			if (!back.HasBlock && !back.IsVoid && front.HasFullBlock && left.CanBuildRamp(2) && right.CanBuildRamp(2))
			{
				return 2;
			}
			if (!blockType.IsRamp)
			{
				return 0;
			}
		}
		return -1;
	}

	public Cell GetDependedRamp(Cell cell)
	{
		Cell right = cell.Right;
		if (right.HasRamp && !right.HasStairs && right.blockDir == 1)
		{
			return right;
		}
		Cell front = cell.Front;
		if (front.HasRamp && !front.HasStairs && front.blockDir == 0)
		{
			return front;
		}
		Cell left = cell.Left;
		if (left.HasRamp && !left.HasStairs && left.blockDir == 3)
		{
			return left;
		}
		Cell back = cell.Back;
		if (back.HasRamp && !back.HasStairs && back.blockDir == 2)
		{
			return back;
		}
		return null;
	}

	public Point GetRandomPoint(Point center, int radius, int tries = 100, bool mustBeWalkable = true, bool requireLos = true)
	{
		Point point = new Point();
		for (int i = 0; i < tries; i++)
		{
			point.x = center.x + EClass.rnd(radius * 2 + 1) - radius;
			point.z = center.z + EClass.rnd(radius * 2 + 1) - radius;
			point.Clamp(false);
			if ((!mustBeWalkable || !point.cell.blocked) && (!requireLos || Los.IsVisible(center, point, null)))
			{
				return point;
			}
		}
		Debug.Log("GetRandomPoint failed center:" + ((center != null) ? center.ToString() : null) + " rad:" + radius.ToString());
		point.IsValid = false;
		return point;
	}

	public new Point GetRandomEdge(int r = 3)
	{
		for (int i = 0; i < 10000; i++)
		{
			int x;
			int z;
			if (EClass.rnd(2) == 0)
			{
				x = ((EClass.rnd(2) == 0) ? EClass.rnd(r) : (this.Size - 1 - EClass.rnd(r)));
				z = EClass.rnd(this.Size);
			}
			else
			{
				z = ((EClass.rnd(2) == 0) ? EClass.rnd(r) : (this.Size - 1 - EClass.rnd(r)));
				x = EClass.rnd(this.Size);
			}
			Point surface = base.GetSurface(x, z, false);
			if (surface.IsValid)
			{
				return surface;
			}
		}
		return base.GetSurface(this.Size / 2, this.Size / 2, false);
	}

	public Point GetNearbyResourcePoint(Point center)
	{
		Point point = new Point();
		int num = (EClass.rnd(2) == 0) ? 1 : -1;
		int num2 = (EClass.rnd(2) == 0) ? 1 : -1;
		for (int i = 0; i < 3; i++)
		{
			point.x = center.x - num + i * num;
			for (int j = 0; j < 3; j++)
			{
				point.z = center.z - num2 + j * num2;
				if (point.IsValid && point.area == null && point.cell.CanHarvest())
				{
					return point;
				}
			}
		}
		return Point.Invalid;
	}

	public List<Point> ListPointsInCircle(Point center, float radius, bool mustBeWalkable = true, bool los = true)
	{
		List<Point> list = new List<Point>();
		this.ForeachSphere(center.x, center.z, radius, delegate(Point p)
		{
			if ((!mustBeWalkable || !p.cell.blocked) && (!los || Los.IsVisible(center, p, null)))
			{
				list.Add(p.Copy());
			}
		});
		return list;
	}

	public List<Chara> ListCharasInCircle(Point center, float radius, bool los = true)
	{
		List<Chara> list = new List<Chara>();
		foreach (Point point in this.ListPointsInCircle(center, radius, false, los))
		{
			CellDetail detail = point.detail;
			if (detail != null && detail.charas.Count > 0)
			{
				foreach (Chara item in point.detail.charas)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	public List<Point> ListPointsInArc(Point center, Point to, int radius, float angle)
	{
		Point to2 = new Point((to.x > center.x) ? 1 : ((to.x < center.x) ? -1 : 0), (to.z > center.z) ? 1 : ((to.z < center.z) ? -1 : 0));
		Point point = new Point(0, 0);
		List<Point> list = new List<Point>();
		float diff = point.GetAngle2(to2);
		this.ForeachSphere(center.x, center.z, (float)radius, delegate(Point p)
		{
			float angle2 = center.GetAngle2(p);
			if ((Mathf.Abs(diff - angle2) < angle || Mathf.Abs(diff - angle2 + 360f) < angle || Mathf.Abs(360f - diff + angle2) < angle) && Los.IsVisible(center, p, null) && !p.IsBlocked)
			{
				list.Add(p.Copy());
			}
		});
		return list;
	}

	public List<Point> ListPointsInLine(Point center, Point to, int radius)
	{
		return Los.ListVisible(center, to, radius, null);
	}

	public void SetBounds(int size)
	{
		if (size > this.Size / 2 + 1)
		{
			size = this.Size / 2 - 1;
		}
		this.bounds.SetBounds(this.Size / 2 - size, this.Size / 2 - size, this.Size / 2 + size, this.Size / 2 + size);
	}

	public void SetBounds(MapBounds b)
	{
		this.bounds.SetBounds(b.x, b.z, b.maxX, b.maxZ);
		this.bounds.Size = b.Size;
	}

	public new void ForeachCell(Action<Cell> action)
	{
		for (int i = 0; i < this.Size; i++)
		{
			for (int j = 0; j < this.Size; j++)
			{
				action(this.cells[i, j]);
			}
		}
	}

	public new void ForeachPoint(Action<Point> action)
	{
		Point point = new Point();
		for (int i = 0; i < this.Size; i++)
		{
			for (int j = 0; j < this.Size; j++)
			{
				action(point.Set(i, j));
			}
		}
	}

	public new void ForeachXYZ(Action<int, int> action)
	{
		for (int i = 0; i < this.Size; i++)
		{
			for (int j = 0; j < this.Size; j++)
			{
				action(i, j);
			}
		}
	}

	public void ForeachSphere(int _x, int _z, float r, Action<Point> action)
	{
		Point point = new Point();
		int num = (int)Mathf.Ceil(r);
		for (int i = _x - num; i < _x + num + 1; i++)
		{
			if (i >= 0 && i < this.Size)
			{
				for (int j = _z - num; j < _z + num + 1; j++)
				{
					if (j >= 0 && j < this.Size && (float)((i - _x) * (i - _x) + (j - _z) * (j - _z)) < r * r)
					{
						point.Set(i, j);
						action(point);
					}
				}
			}
		}
	}

	public void ForeachNeighbor(Point center, Action<Point> action)
	{
		int x = center.x;
		int z = center.z;
		Point point = new Point();
		for (int i = x - 1; i < x + 2; i++)
		{
			if (i >= 0 && i < this.Size)
			{
				for (int j = z - 1; j < z + 2; j++)
				{
					if (j >= 0 && j < this.Size)
					{
						point.Set(i, j);
						action(point);
					}
				}
			}
		}
	}

	public void Quake()
	{
		Point point = new Point();
		for (int i = 0; i < this.Size; i++)
		{
			for (int j = 0; j < this.Size; j++)
			{
				point.x = i;
				point.z = j;
				point.Copy().Animate(AnimeID.Quake, true);
				j += EClass.rnd(2);
			}
			i += EClass.rnd(2);
		}
	}

	public int CountChara(Faction faction)
	{
		int num = 0;
		using (List<Chara>.Enumerator enumerator = this.charas.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.faction == faction)
				{
					num++;
				}
			}
		}
		return num;
	}

	public int CountGuest()
	{
		int num = 0;
		using (List<Chara>.Enumerator enumerator = this.charas.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsGuest())
				{
					num++;
				}
			}
		}
		return num;
	}

	public int CountHostile()
	{
		int num = 0;
		foreach (Chara chara in this.charas)
		{
			if (!chara.IsPCFaction && chara.IsHostile())
			{
				num++;
			}
		}
		return num;
	}

	public int CountWildAnimal()
	{
		int num = 0;
		foreach (Chara chara in this.charas)
		{
			if (!chara.IsPCFaction && chara.race.IsAnimal)
			{
				num++;
			}
		}
		return num;
	}

	public int CountNonHostile()
	{
		int num = 0;
		foreach (Chara chara in this.charas)
		{
			if (!chara.IsPCFaction && !chara.IsHostile())
			{
				num++;
			}
		}
		return num;
	}

	public List<Chara> ListChara(Faction faction)
	{
		List<Chara> list = new List<Chara>();
		foreach (Chara chara in this.charas)
		{
			if (chara.faction == faction)
			{
				list.Add(chara);
			}
		}
		return list;
	}

	public List<Thing> ListThing<T>() where T : Trait
	{
		List<Thing> list = new List<Thing>();
		foreach (Thing thing in this.things)
		{
			if (thing.IsInstalled && thing.trait is T)
			{
				list.Add(thing);
			}
		}
		return list;
	}

	public bool PutAway(Card c)
	{
		if (c.IsPC || (c.trait.CanOnlyCarry && !EClass.debug.ignoreBuildRule))
		{
			return false;
		}
		if (c.isChara)
		{
			if (!EClass.debug.ignoreBuildRule)
			{
				return false;
			}
			c.Destroy();
			return true;
		}
		else
		{
			Thing thing = c.Thing;
			if (thing.parent != null)
			{
				thing.parent.RemoveCard(thing);
			}
			thing.isMasked = false;
			thing.isRoofItem = false;
			if (EClass._zone.IsPCFaction && EClass._map.props.installed.traits.GetRandomThing<TraitSpotStockpile>() != null)
			{
				EClass._zone.TryAddThingInSpot<TraitSpotStockpile>(thing, true, true);
				return true;
			}
			if (EClass.debug.enable)
			{
				EClass.debug.GetOrCreateDebugContainer().AddThing(thing, true, -1, -1);
				return true;
			}
			EClass.pc.Pick(thing, false, true);
			return true;
		}
	}

	public Chara FindChara(string id)
	{
		foreach (Chara chara in this.charas)
		{
			if (chara.id == id)
			{
				return chara;
			}
		}
		return null;
	}

	public Chara FindChara(int uid)
	{
		foreach (Chara chara in this.charas)
		{
			if (chara.uid == uid)
			{
				return chara;
			}
		}
		return null;
	}

	public Thing FindThing(Func<Thing, bool> func)
	{
		foreach (Thing thing in this.things)
		{
			if (func(thing))
			{
				return thing;
			}
		}
		return null;
	}

	public Thing FindThing(int uid)
	{
		foreach (Thing thing in this.things)
		{
			if (thing.uid == uid)
			{
				return thing;
			}
		}
		return null;
	}

	public T FindThing<T>() where T : Trait
	{
		foreach (Thing thing in this.things)
		{
			if (thing.trait is T)
			{
				return thing.trait as T;
			}
		}
		return default(T);
	}

	public Thing FindThing(Type type, Chara c = null)
	{
		this._things.Clear();
		foreach (Thing thing in EClass._map.props.installed.things)
		{
			if (type.IsAssignableFrom(thing.trait.GetType()) && thing.pos.IsPublicSpace())
			{
				this._things.Add(thing);
			}
		}
		if (this._things.Count <= 0)
		{
			return null;
		}
		return this._things.RandomItem<Thing>();
	}

	public Thing FindThing(Type type, BaseArea area1, BaseArea area2 = null)
	{
		Map.<>c__DisplayClass171_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.type = type;
		if (area1 == null && area2 == null)
		{
			return null;
		}
		Thing thing = this.<FindThing>g__Find|171_0(area1, ref CS$<>8__locals1);
		if (thing == null && area2 != null)
		{
			thing = this.<FindThing>g__Find|171_0(area2, ref CS$<>8__locals1);
		}
		return thing;
	}

	public Thing FindThing(string workTag, BaseArea area1 = null, BaseArea area2 = null)
	{
		if (area1 == null && area2 == null)
		{
			return null;
		}
		Thing thing = null;
		PropSet orCreate = EClass._map.Installed.workMap.GetOrCreate(workTag, null);
		if (area1 != null)
		{
			IEnumerable<Card> enumerable = from a in orCreate.Values
			where a.pos.HasRoomOrArea(area1)
			select a;
			if (enumerable.Count<Card>() > 0)
			{
				thing = (enumerable.RandomItem<Card>() as Thing);
			}
		}
		if (thing == null && area2 != null)
		{
			IEnumerable<Card> enumerable2 = from a in orCreate.Values
			where a.pos.HasRoomOrArea(area2)
			select a;
			if (enumerable2.Count<Card>() > 0)
			{
				thing = (enumerable2.RandomItem<Card>() as Thing);
			}
		}
		return thing;
	}

	public Thing FindThing(string workTag, Chara c)
	{
		Thing result = null;
		IEnumerable<Card> enumerable = from a in EClass._map.Installed.workMap.GetOrCreate(workTag, null).Values
		where a.pos.IsPublicSpace()
		select a;
		if (enumerable.Count<Card>() > 0)
		{
			result = (enumerable.RandomItem<Card>() as Thing);
		}
		return result;
	}

	public BaseArea FindPublicArea()
	{
		return (from a in this.rooms.listArea.Concat(this.rooms.listRoom)
		where a.type.IsPublicArea
		select a).RandomItem<BaseArea>();
	}

	public void RefreshSunMap()
	{
		if (Map.isDirtySunMap)
		{
			Map.sunMap.Clear();
			foreach (Trait trait in EClass._map.props.installed.traits.suns.Values)
			{
				foreach (Point point in trait.ListPoints(null, false))
				{
					Map.sunMap.Add(point.index);
				}
			}
			Map.isDirtySunMap = false;
		}
	}

	[CompilerGenerated]
	internal static bool <TryShatter>g__CanCook|101_0(Card c, ref Map.<>c__DisplayClass101_0 A_1)
	{
		return A_1.fire && c.IsFood && c.category.IsChildOf("foodstuff");
	}

	[CompilerGenerated]
	private void <MineObj>g__Pop|121_0(Thing t, ref Map.<>c__DisplayClass121_0 A_2)
	{
		if (EClass.scene.actionMode.IsBuildMode && EClass.debug.godBuild)
		{
			EClass._map.PutAway(t);
			return;
		}
		this.TrySmoothPick(A_2.point, t, A_2.c);
	}

	[CompilerGenerated]
	private Thing <FindThing>g__Find|171_0(BaseArea area, ref Map.<>c__DisplayClass171_0 A_2)
	{
		this._things.Clear();
		foreach (Thing thing in EClass._map.props.installed.things)
		{
			if (A_2.type.IsAssignableFrom(thing.trait.GetType()) && thing.pos.HasRoomOrArea(area))
			{
				this._things.Add(thing);
			}
		}
		if (this._things.Count <= 0)
		{
			return null;
		}
		return this._things.RandomItem<Thing>();
	}

	public static HashSet<int> sunMap = new HashSet<int>();

	public static bool isDirtySunMap;

	[JsonProperty]
	public int seed;

	[JsonProperty]
	public int _bits;

	[JsonProperty]
	public IO.Compression compression;

	[JsonProperty]
	public global::Version version;

	[JsonProperty]
	public RoomManager rooms = new RoomManager();

	[JsonProperty]
	public TaskManager tasks = new TaskManager();

	[JsonProperty]
	public MapConfig config = new MapConfig();

	[JsonProperty]
	public CustomData custom;

	[JsonProperty]
	public List<Chara> serializedCharas = new List<Chara>();

	[JsonProperty]
	public List<Chara> deadCharas = new List<Chara>();

	[JsonProperty]
	public List<Thing> things = new List<Thing>();

	[JsonProperty]
	public MapBounds bounds = new MapBounds();

	[JsonProperty]
	public List<int> _plDay = new List<int>();

	[JsonProperty]
	public List<int> _plNight = new List<int>();

	[JsonProperty]
	public Dictionary<int, int> gatherCounts = new Dictionary<int, int>();

	[JsonProperty]
	public Dictionary<int, CellEffect> cellEffects = new Dictionary<int, CellEffect>();

	[JsonProperty]
	public Dictionary<int, int> backerObjs = new Dictionary<int, int>();

	[JsonProperty]
	public Dictionary<int, PlantData> plants = new Dictionary<int, PlantData>();

	public BitArray32 bits;

	public Playlist plDay;

	public Playlist plNight;

	public List<Chara> charas = new List<Chara>();

	public List<TransAnime> pointAnimes = new List<TransAnime>();

	public Cell[,] cells;

	public Zone zone;

	public CellEffectManager effectManager = new CellEffectManager();

	public PropsManager props = new PropsManager();

	public FloodSpiller flood = new FloodSpiller();

	public BiomeProfile[,] biomes;

	public POIMap poiMap;

	public List<Footmark> footmarks = new List<Footmark>();

	public FowProfile fowProfile;

	public bool revealed;

	private HashSet<int> roomHash = new HashSet<int>();

	private List<Thing> _things = new List<Thing>();
}
