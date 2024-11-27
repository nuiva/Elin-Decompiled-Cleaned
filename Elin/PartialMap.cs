using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using Newtonsoft.Json;
using SFB;
using UnityEngine;

public class PartialMap : EClass
{
	public static string PathTemp
	{
		get
		{
			return CorePath.MapPieceSave + "_temp.mp";
		}
	}

	public bool altDirPosition
	{
		get
		{
			return true;
		}
	}

	public string ID
	{
		get
		{
			return new FileInfo(this.path).Name.Replace(".mp", "");
		}
	}

	public void Rotate()
	{
		this.dir++;
		if (this.dir > 3)
		{
			this.dir = 0;
		}
		SE.Rotate();
		this.localOffsetX = (this.localOffsetZ = 0);
	}

	public void ClearMarkedCells()
	{
		foreach (Cell cell in this.cellsMarked)
		{
			cell.skipRender = false;
		}
		this.cellsMarked.Clear();
	}

	public unsafe void Apply(Point _point, PartialMap.ApplyMode mode)
	{
		PartialMap.<>c__DisplayClass35_0 CS$<>8__locals1 = new PartialMap.<>c__DisplayClass35_0();
		CS$<>8__locals1.<>4__this = this;
		byte height = _point.cell.height;
		int num = _point.cell.IsFloorWater ? this.baseHeightWater : this.baseHeight;
		Point point = new Point(_point);
		point.x += this.localOffsetX;
		point.z += this.localOffsetZ;
		if (this.altDirPosition)
		{
			switch (this.dir)
			{
			case 1:
				point.z += this.w - 1;
				break;
			case 2:
				point.x += this.w - 1;
				point.z += this.h - 1;
				break;
			case 3:
				point.x += this.h - 1;
				break;
			}
		}
		this.destX = point.x;
		this.destZ = point.z;
		CS$<>8__locals1.p = new Point();
		if (mode == PartialMap.ApplyMode.Render)
		{
			this.ClearMarkedCells();
		}
		if (mode == PartialMap.ApplyMode.Evaluate)
		{
			this.value = 0;
		}
		int num2 = 0;
		int num3 = 0;
		CS$<>8__locals1.minX = point.x;
		CS$<>8__locals1.minZ = point.z;
		CS$<>8__locals1.maxX = point.x + this.w;
		CS$<>8__locals1.maxZ = point.z + this.h;
		switch (this.dir)
		{
		case 1:
			CS$<>8__locals1.minX = point.x;
			CS$<>8__locals1.maxX = point.x + this.h;
			CS$<>8__locals1.minZ = point.z - this.w + 1;
			CS$<>8__locals1.maxZ = point.z + 1;
			break;
		case 2:
			CS$<>8__locals1.minX = point.x - this.w + 1;
			CS$<>8__locals1.maxX = point.x + 1;
			CS$<>8__locals1.minZ = point.z - this.h + 1;
			CS$<>8__locals1.maxZ = point.z + 1;
			break;
		case 3:
			CS$<>8__locals1.minX = point.x - this.h + 1;
			CS$<>8__locals1.maxX = point.x + 1;
			CS$<>8__locals1.minZ = point.z;
			CS$<>8__locals1.maxZ = point.z + this.w;
			break;
		}
		EClass.pc.GetCurrency("money");
		BiomeProfile.TileGroup interior = EClass._zone.biome.interior;
		BiomeProfile.TileGroup exterior = EClass._zone.biome.exterior;
		this.result.ruined.Clear();
		for (int i = 0; i < this.w; i++)
		{
			for (int j = 0; j < this.h; j++)
			{
				Cell cell = this.map.cells[i, j];
				CS$<>8__locals1.p.Set(point.x + num2, point.z + num3);
				if (EClass._map.Contains(CS$<>8__locals1.p))
				{
					PartialMap.<>c__DisplayClass35_1 CS$<>8__locals2;
					CS$<>8__locals2.c1 = EClass._map.cells[CS$<>8__locals1.p.x, CS$<>8__locals1.p.z];
					CS$<>8__locals2.result = HitResult.Valid;
					if (CS$<>8__locals2.c1.isFloating)
					{
						CS$<>8__locals2.result = HitResult.Invalid;
					}
					if (CS$<>8__locals2.result == HitResult.Valid)
					{
						this.validPoints.Add(CS$<>8__locals1.p.index);
					}
					if (mode == PartialMap.ApplyMode.Render)
					{
						CS$<>8__locals2.c1.skipRender = true;
						this.cellsMarked.Add(CS$<>8__locals2.c1);
						if (CS$<>8__locals2.result != HitResult.Valid)
						{
							MeshPass guidePass = ActionMode.Copy.GetGuidePass(point);
							int num4 = (int)CS$<>8__locals2.result;
							Vector3 vector = *CS$<>8__locals1.p.Position();
							guidePass.Add(vector.x, vector.y, vector.z - 0.01f, (float)num4, 0.3f);
						}
						else
						{
							CS$<>8__locals1.<Apply>g__Render|1(cell.sourceFloor, cell.floorDir, 0, ref CS$<>8__locals2);
							CS$<>8__locals1.<Apply>g__Render|1(cell.sourceBlock, cell.HasWall ? this.FixWall(cell) : cell.blockDir, 0, ref CS$<>8__locals2);
							CS$<>8__locals1.<Apply>g__Render|1(cell.sourceObj, cell.objDir, 0, ref CS$<>8__locals2);
							CS$<>8__locals1.<Apply>g__Render|1(cell.sourceBridge, cell.floorDir, (int)((cell.bridgeHeight == 0) ? 0 : (cell.bridgeHeight - cell.height)), ref CS$<>8__locals2);
							foreach (SerializedCards.Data data in this.exportData.serializedCards.cards)
							{
								Recipe orCreate = Recipe.GetOrCreate(EClass.sources.cards.map[data.id].RecipeID);
								if (data.x - this.offsetX == i && data.z - this.offsetZ == j)
								{
									if (PartialMap.relative)
									{
										if (orCreate != null)
										{
											orCreate.OnRenderMarker(CS$<>8__locals1.p, true, CS$<>8__locals2.result, true, data.dir, (int)(height + cell.height) - num);
										}
									}
									else if (orCreate != null)
									{
										orCreate.OnRenderMarker(CS$<>8__locals1.p, true, CS$<>8__locals2.result, true, data.dir, (int)cell.height);
									}
								}
							}
							Critter.RebuildCritter(CS$<>8__locals2.c1);
						}
					}
					if (mode == PartialMap.ApplyMode.Apply && CS$<>8__locals2.result == HitResult.Valid)
					{
						if (Rand.Range(0f, 1f) < this.ruinChance)
						{
							this.result.ruined.Add(CS$<>8__locals1.p.index);
						}
						else if (cell._floor != 77 || this.editMode)
						{
							if (cell._block == 149 && !this.editMode)
							{
								CS$<>8__locals2.c1._block = (byte)interior.block.id;
								CS$<>8__locals2.c1._blockMat = (byte)interior.block.mat;
							}
							else if (cell._block == 150 && !this.editMode)
							{
								CS$<>8__locals2.c1._block = (byte)exterior.block.id;
								CS$<>8__locals2.c1._blockMat = (byte)exterior.block.mat;
							}
							else
							{
								CS$<>8__locals2.c1._block = cell._block;
								CS$<>8__locals2.c1._blockMat = cell._blockMat;
							}
							if (cell._floor == 2 && !this.editMode)
							{
								CS$<>8__locals2.c1._floor = (byte)interior.floor.id;
								CS$<>8__locals2.c1._floorMat = (byte)interior.floor.mat;
							}
							else if (cell._floor == 3 && !this.editMode)
							{
								CS$<>8__locals2.c1._floor = (byte)exterior.floor.id;
								CS$<>8__locals2.c1._floorMat = (byte)exterior.floor.mat;
							}
							else
							{
								CS$<>8__locals2.c1._floor = cell._floor;
								CS$<>8__locals2.c1._floorMat = cell._floorMat;
							}
							CS$<>8__locals2.c1._roofBlock = cell._roofBlock;
							CS$<>8__locals2.c1._roofBlockMat = cell._roofBlockMat;
							CS$<>8__locals2.c1.obj = cell.obj;
							CS$<>8__locals2.c1.objMat = cell.objMat;
							CS$<>8__locals2.c1.objVal = cell.objVal;
							CS$<>8__locals2.c1.decal = cell.decal;
							CS$<>8__locals2.c1._dirs = cell._dirs;
							CS$<>8__locals2.c1._bridge = cell._bridge;
							CS$<>8__locals2.c1._bridgeMat = cell._bridgeMat;
							CS$<>8__locals2.c1.bridgePillar = cell.bridgePillar;
							CS$<>8__locals2.c1.isModified = true;
							if (CS$<>8__locals2.c1.HasWall)
							{
								CS$<>8__locals2.c1.blockDir = this.FixWall(CS$<>8__locals2.c1);
							}
							if (PartialMap.relative || this.procedural)
							{
								CS$<>8__locals2.c1.height = (byte)Mathf.Clamp((int)(height + cell.height) - num, 0, 255);
								CS$<>8__locals2.c1.bridgeHeight = (byte)Mathf.Clamp((int)((cell.bridgeHeight == 0) ? 0 : (CS$<>8__locals2.c1.height + (cell.bridgeHeight - cell.height))), 0, 255);
							}
							else
							{
								CS$<>8__locals2.c1.height = cell.height;
								CS$<>8__locals2.c1.bridgeHeight = cell.bridgeHeight;
							}
						}
					}
				}
				switch (this.dir)
				{
				case 0:
					num3++;
					if (num3 >= this.h)
					{
						num3 = 0;
						num2++;
					}
					break;
				case 1:
					num2++;
					if (num2 >= this.h)
					{
						num2 = 0;
						num3--;
					}
					break;
				case 2:
					num3--;
					if (num3 <= -this.h)
					{
						num3 = 0;
						num2--;
					}
					break;
				case 3:
					num2--;
					if (num2 <= -this.h)
					{
						num2 = 0;
						num3++;
					}
					break;
				}
			}
		}
		if (mode != PartialMap.ApplyMode.Apply)
		{
			return;
		}
		CS$<>8__locals1.p.Set(point.x, point.z);
		EClass._map.SetReference();
		EClass._map.things.ForeachReverse(delegate(Thing t)
		{
			if (CS$<>8__locals1.<>4__this.validPoints.Contains(t.pos.index) && t.pos.x >= CS$<>8__locals1.minX && t.pos.z >= CS$<>8__locals1.minZ && t.pos.x < CS$<>8__locals1.maxX && t.pos.z < CS$<>8__locals1.maxZ && t.trait.CanBeDestroyed)
			{
				t.Destroy();
			}
		});
		if (this.exportData != null)
		{
			this.exportData.serializedCards.Restore(EClass._map, null, true, this);
		}
		EClass._map.RefreshAllTiles();
		EClass._map.RefreshAllTiles();
	}

	public int FixWall(Cell c)
	{
		switch (this.dir)
		{
		case 1:
			if (c.blockDir == 0)
			{
				return 1;
			}
			if (c.blockDir == 1)
			{
				return 0;
			}
			break;
		case 3:
			if (c.blockDir == 0)
			{
				return 1;
			}
			if (c.blockDir == 1)
			{
				return 0;
			}
			if (c.blockDir == 2)
			{
				return 0;
			}
			break;
		}
		return c.blockDir;
	}

	public void Update()
	{
		if (this.path.IsEmpty())
		{
			return;
		}
		Map.UpdateMetaData(this.path, this);
	}

	public void Save(int _x, int _z, int _w, int _h)
	{
		this.offsetX = _x;
		this.offsetZ = _z;
		this.w = _w;
		this.h = _h;
		this.value = 0;
		for (int i = _x; i < _x + this.w; i++)
		{
			for (int j = _z; j < _z + this.h; j++)
			{
				Cell cell = EClass._map.cells[i, j];
				this.value += Card.GetTilePrice(cell.sourceBlock, cell.matBlock);
				this.value += Card.GetTilePrice(cell.sourceFloor, cell.matFloor);
				this.value += Card.GetTilePrice(cell.sourceObj, cell.matObj);
				this.value += Card.GetTilePrice(cell.sourceBridge, cell.matBridge);
				this.value += Card.GetTilePrice(cell.sourceRoofBlock, cell.matRoofBlock);
			}
		}
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.trait.CanCopyInBlueprint && thing.pos.Within(_x, _z, _w, _h) && thing.IsInstalled)
			{
				this.value += thing.GetPrice(CurrencyType.Money, false, PriceType.Default, null) * thing.Num;
			}
		}
		EClass._zone.Export(PartialMap.PathTemp, this, false);
	}

	public static PartialMap Apply(string path, Point pos)
	{
		PartialMap partialMap = PartialMap.Load(CorePath.MapPieceSave + path);
		partialMap.ruinChance = 0f;
		partialMap.Apply(pos, PartialMap.ApplyMode.Apply);
		return partialMap;
	}

	public static PartialMap Load(string path = null)
	{
		bool flag = path.IsEmpty();
		if (flag)
		{
			path = PartialMap.PathTemp;
		}
		if (!File.Exists(path))
		{
			return null;
		}
		MapMetaData metaData = Map.GetMetaData(path);
		if (metaData == null || metaData.partial == null)
		{
			return null;
		}
		PartialMap partial = metaData.partial;
		if (!flag)
		{
			partial.path = path;
		}
		partial.exportData = EClass._zone.Import(path);
		partial._Load();
		return partial;
	}

	public void _Load()
	{
		this.map = GameIO.LoadFile<Map>(EClass._zone.pathTemp + "map");
		this.map.Load(EClass._zone.pathTemp, false, this);
		if (this.map == null)
		{
			Debug.Log("Map is null:" + this.name + "/" + PartialMap.PathTemp);
			return;
		}
		EClass._map.SetReference();
		this.baseHeight = 255;
		this.baseHeightWater = 255;
		for (int i = 0; i < this.w; i++)
		{
			for (int j = 0; j < this.h; j++)
			{
				Cell cell = this.map.cells[i, j];
				if ((int)cell.height < this.baseHeight)
				{
					this.baseHeightWater = (int)cell.height;
				}
				if ((int)cell.height < this.baseHeight && !cell.IsFloorWater)
				{
					this.baseHeight = (int)cell.height;
				}
			}
		}
		if (this.baseHeight == 255)
		{
			this.baseHeight = 0;
		}
	}

	public static void Delete(string path)
	{
		string fullFileNameWithoutExtension = path.GetFullFileNameWithoutExtension();
		IO.DeleteFile(path);
		IO.DeleteFile(fullFileNameWithoutExtension + ".jpg");
		IO.DeleteFile(fullFileNameWithoutExtension + ".txt");
	}

	public static void ExportDialog(string dir = null)
	{
		EClass.core.WaitForEndOfFrame(delegate
		{
			string pathDest = StandaloneFileBrowser.SaveFilePanel("Export Map Piece", dir ?? CorePath.MapPieceSaveUser, "new map piece", "mp");
			if (!string.IsNullOrEmpty(pathDest))
			{
				PartialMap.SavePreview(PartialMap.PathTemp, pathDest);
				Msg.SayRaw("Exported Zone");
			}
		});
	}

	public static void SavePreview(string path, string pathDest)
	{
		EClass.core.actionsNextFrame.Add(delegate
		{
			string fileName = CorePath.Temp + "preview.jpg";
			Texture2D texture2D = ScreenCapture.CaptureScreenshotAsTexture();
			int num = 200;
			int num2 = 100;
			RenderTexture renderTexture = new RenderTexture(num, num2, 0);
			renderTexture.Create();
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = renderTexture;
			Graphics.Blit(texture2D, renderTexture);
			Texture2D texture2D2 = new Texture2D(num, num2, texture2D.format, false);
			texture2D2.ReadPixels(new Rect(0f, 0f, (float)num, (float)num2), 0, 0);
			texture2D2.Apply();
			RenderTexture.active = active;
			renderTexture.Release();
			File.WriteAllBytes(fileName, texture2D2.EncodeToJPG());
			ZipFile zipFile = ZipFile.Read(path);
			zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
			zipFile.AddFile(fileName, "");
			zipFile.Save(pathDest);
			zipFile.Dispose();
			IO.DeleteFile(fileName);
			UnityEngine.Object.Destroy(texture2D2);
			UnityEngine.Object.Destroy(texture2D);
			UnityEngine.Object.Destroy(renderTexture);
		});
	}

	public static bool relative;

	[JsonProperty]
	public string name;

	[JsonProperty]
	public int offsetX;

	[JsonProperty]
	public int offsetZ;

	[JsonProperty]
	public int w;

	[JsonProperty]
	public int h;

	[JsonProperty]
	public int dir;

	[JsonProperty]
	public int baseHeight;

	[JsonProperty]
	public int baseHeightWater;

	[JsonProperty]
	public int value;

	[JsonProperty]
	public bool allowRotate;

	[JsonProperty]
	public bool ignoreBlock;

	public int localOffsetX;

	public int localOffsetZ;

	public int destX;

	public int destZ;

	public string path;

	public bool editMode;

	public bool procedural;

	public float ruinChance;

	public Map map;

	public ZoneExportData exportData;

	public List<Cell> cellsMarked = new List<Cell>();

	public HashSet<int> validPoints = new HashSet<int>();

	public PartialMap.Result result = new PartialMap.Result();

	public enum ApplyMode
	{
		Render,
		HitTest,
		Apply,
		Evaluate
	}

	public class Result
	{
		public bool hasLight;

		public List<int> ruined = new List<int>();
	}
}
