using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MapGen : BaseMapGen
{
	public static MapGen Instance
	{
		get
		{
			MapGen result;
			if ((result = MapGen._Instance) == null)
			{
				result = (MapGen._Instance = new MapGen());
			}
			return result;
		}
	}

	protected override void GenerateTerrain()
	{
		base.SetSize(this.zp.useRootSize ? this.zone.GetTopZone().bounds.Size : this.zp.size, 10);
		if (this.biomes == null || this.Size != this.biomes.GetLength(0))
		{
			this.biomes = new BiomeProfile[this.Size, this.Size];
			this.subBiomes = new bool[this.Size, this.Size];
			this.heights1 = new float[this.Size, this.Size];
			this.heights2 = new float[this.Size, this.Size];
			this.heights3 = new float[this.Size, this.Size];
			this.waters = new float[this.Size, this.Size];
			this.heights3d = new float[this.Size, this.Size];
			this.lastSize = this.Size;
		}
		this.map.biomes = this.biomes;
		this.layerHeight.SaveSettings();
		this.layerStratum.SaveSettings();
		this.layerBiome.SaveSettings();
		this.layerRiver.SaveSettings();
		this.layerBiome.SaveSettings();
		this.skipWater = (this.zp.noWater || (this.bp.tileCenter != null && (this.bp.tileCenter.IsNeighborRoad || this.bp.tileCenter.isRoad)));
		for (int i = 0; i < 100; i++)
		{
			this.seed = (this.map.seed = this.bp.genSetting.seed + i);
			Rand.SetSeed(this.seed);
			if (this.OnGenerateTerrain())
			{
				break;
			}
			Debug.Log("Failed map generation:" + i.ToString() + " / " + BaseMapGen.err);
			this.skipWater = true;
		}
		this.map.SetZone(EClass._zone);
		for (int j = 0; j < this.Size; j++)
		{
			for (int k = 0; k < this.Size; k++)
			{
				this.map.QuickRefreshTile(j, k);
			}
		}
		Rand.SetSeed(-1);
	}

	protected override bool OnGenerateTerrain()
	{
		int idMat = 66;
		if (this.map.isGenerated)
		{
			if (this.map.Size != this.Size)
			{
				this.map.Resize(this.Size);
			}
			this.map.Reset();
		}
		else
		{
			this.map.CreateNew(this.Size, true);
		}
		this.map.poiMap.Reset();
		if (this.bp.zoneProfile.useRootSize)
		{
			this.map.SetBounds(this.zone.GetTopZone().bounds);
		}
		else
		{
			this.map.SetBounds((this.bp.zoneProfile.sizeBounds == 0) ? EClass.setting.defaultMapSize : this.bp.zoneProfile.sizeBounds);
		}
		ZoneProfile.GenType genType = this.zp.genType;
		if (genType != ZoneProfile.GenType.Sky)
		{
			if (genType == ZoneProfile.GenType.Underground)
			{
				this.map.config.idBiome = "Underground";
				this.zone._biome = null;
			}
			this.waterCount = 0f;
			if (!this.skipWater)
			{
				float num = (float)(this.Size * this.Size) * this.variation.maxWaterRatio;
				this.layerRiver.FillHeightMap(this.waters, this.OX, 0, this.OZ, this.Size, this.Size, this.seed, 1f);
				for (int i = 0; i < this.Size; i++)
				{
					for (int j = 0; j < this.Size; j++)
					{
						this.waters[i, j] += (float)this.zp.water;
						if (this.waters[i, j] > 0f)
						{
							this.waterCount += 1f;
						}
					}
					if (this.waterCount > num)
					{
						BaseMapGen.err = "Too many water";
						return false;
					}
				}
			}
			this.layerBiome.FillHeightMap(this.heights1, this.OX, 0, this.OZ, this.Size, this.Size, this.zp.seeds.biome, 1f);
			this.layerBiome.FillHeightMap(this.heights2, this.OX, 0, this.OZ, this.Size, this.Size, this.zp.seeds.biome + 1, 1f);
			if (base.extraBiome)
			{
				this.layerBiome.FillHeightMap(this.heights3, this.OX, 0, this.OZ, this.Size, this.Size, this.zp.seeds.biome + 2, 1f);
			}
			for (int k = 0; k < this.Size; k++)
			{
				for (int l = 0; l < this.Size; l++)
				{
					if (this.heights1[k, l] != 0f)
					{
						this.biomes[k, l] = this.biomeProfiles[1];
					}
					else if (this.heights2[k, l] != 0f)
					{
						this.biomes[k, l] = this.biomeProfiles[2];
					}
					else if (base.extraBiome && this.heights3[k, l] != 0f)
					{
						this.biomes[k, l] = this.biomeProfiles[3];
					}
					else
					{
						this.biomes[k, l] = this.biomeProfiles[0];
					}
				}
			}
			this.layerBiome.FillHeightMap(this.heights1, this.OX, 0, this.OZ, this.Size, this.Size, this.zp.seeds.biomeSub, this.zp.biomeSubScale);
			for (int m = 0; m < this.Size; m++)
			{
				for (int n = 0; n < this.Size; n++)
				{
					this.subBiomes[m, n] = (this.heights1[m, n] != 0f);
				}
			}
			this.layerBiome.FillHeightMap(this.heights1, this.OX, 0, this.OZ, this.Size, this.Size, this.zp.seeds.biome, 1f);
			for (int num2 = 0; num2 < this.biomeProfiles.Length; num2++)
			{
				if (num2 == 0 || !(this.biomeProfiles[num2 - 1] == this.biomeProfiles[num2]))
				{
					this.biomeProfiles[num2].layerBlock.FillHeightMap(this.heights1, this.OX, 0, this.OZ, this.Size, this.Size, this.zp.seeds.biome, 1f);
					bool flag = EClass.sources.floors.rows[this.biomeProfiles[num2].exterior.floor.id].tag.Contains("soil");
					for (int num3 = 0; num3 < this.Size; num3++)
					{
						if (!this.zp.clearEdge || ((num3 <= this.map.bounds.x - 4 || num3 > this.map.bounds.x + 4) && (num3 <= this.map.bounds.maxX - 4 || num3 > this.map.bounds.maxX + 4)))
						{
							for (int num4 = 0; num4 < this.Size; num4++)
							{
								if ((!this.zp.clearEdge || ((num4 <= this.map.bounds.z - 4 || num4 > this.map.bounds.z + 4) && (num4 <= this.map.bounds.maxZ - 4 || num4 > this.map.bounds.maxZ + 4))) && !(this.biomes[num3, num4] != this.biomeProfiles[num2]) && this.heights1[num3, num4] > (float)this.blockHeight)
								{
									SourceBlock.Row row = EClass.sources.blocks.rows[this.biomeProfiles[num2].exterior.block.id];
									if (!flag || row.alias == "block_ice")
									{
										SourceFloor.Row row2 = EClass.sources.floors.alias[row.autoFloor];
										base.SetFloor(num3, num4, row2.DefaultMaterial.id, row2.id, 0);
									}
									base.SetBlock(num3, num4, this.biomeProfiles[num2].exterior.block.mat, this.biomeProfiles[num2].exterior.block.id, 0);
								}
							}
						}
					}
				}
			}
			bool flag2 = false;
			for (int num5 = this.Size / 2 - 4; num5 < this.Size / 2 + 4; num5++)
			{
				for (int num6 = this.Size / 2 - 4; num6 < this.Size / 2 + 4; num6++)
				{
					if (num5 >= 0 && num6 >= 0 && num5 < this.Size && num6 < this.Size && this.map.cells[num5, num6]._block != 0)
					{
						flag2 = true;
						break;
					}
				}
			}
			if (flag2)
			{
				for (int num7 = 0; num7 < this.Size; num7++)
				{
					for (int num8 = 0; num8 < this.Size; num8++)
					{
						base.SetBlock(num7, num8, 0, 0, 0);
					}
				}
			}
			this.layerHeight.FillHeightMap(this.heights1, this.OX, 0, this.OZ, this.Size, this.Size, this.seed, 1f);
			this.layerHeight.FillHeightMap(this.heights2, this.OX, 0, this.OZ, this.Size, this.Size, this.zp.seeds.bush, 1f);
			this.layerHeight.FillHeightMap(this.heights3, this.OX, 0, this.OZ, this.Size, this.Size, this.seed + 1, 1f);
			BiomeProfile biomeWater = this.biomeWater;
			byte b = (byte)EClass.setting.maxGenHeight;
			for (int num9 = 0; num9 < this.Size; num9++)
			{
				for (int num10 = 0; num10 < this.Size; num10++)
				{
					int num11 = (int)this.heights1[num9, num10];
					Cell cell = this.map.cells[num9, num10];
					if (cell._block == 0 || cell._floor == 0)
					{
						BiomeProfile biomeProfile = this.biomes[num9, num10];
						SourceMaterial.Row row3 = this.subBiomes[num9, num10] ? biomeProfile.MatSub : biomeProfile.MatFloor;
						int dir = biomeProfile.exterior.floor.GetDir();
						byte b2 = (byte)(this.heights1[num9, num10] * (float)this.hSetting.heightMod + (float)this.hSetting.baseHeight);
						if (this.hSetting.step > 0)
						{
							b2 = (byte)((int)b2 / this.hSetting.step * this.hSetting.step);
						}
						if (b2 > b)
						{
							b2 = b;
						}
						cell.height = b2;
						if (!this.skipWater && this.waters[num9, num10] > 0f && cell._block == 0)
						{
							cell.height = 0;
							if (biomeWater)
							{
								bool flag3 = false;
								for (int num12 = num10 - 2; num12 < num10 + 3; num12++)
								{
									for (int num13 = num9 - 2; num13 < num9 + 3; num13++)
									{
										if (num12 >= 0 && num13 >= 0 && num13 < this.Size && num12 < this.Size && this.waters[num13, num12] <= 0f)
										{
											flag3 = true;
											break;
										}
									}
								}
								base.SetFloor(num9, num10, this.biomeWater.MatFloor.id, flag3 ? 44 : 43, 0);
							}
							else
							{
								base.SetFloor(num9, num10, idMat, (this.waters[num9, num10] < 5f) ? 44 : 43, 0);
							}
						}
						else if (this.zp.shoreHeight > 0f && (float)num11 < this.zp.shoreHeight)
						{
							base.SetFloor(num9, num10, this.biomeShore.MatFloor.id, this.biomeShore.MatFloor.defFloor, 0);
							this.biomes[num9, num10] = this.biomeShore;
						}
						else if (biomeProfile.floor_height != 0 && (int)this.heights2[num9, num10] + this.zp.bushMod >= 4)
						{
							base.SetFloor(num9, num10, (int)((byte)row3.id), biomeProfile.floor_height, dir);
						}
						else
						{
							base.SetFloor(num9, num10, (int)((byte)row3.id), (biomeProfile.exterior.floor.id != 0) ? biomeProfile.exterior.floor.id : row3.defFloor, dir);
						}
					}
				}
			}
			if (this.hSetting.heightMod > 0)
			{
				this.ModifyHeight(this.map);
			}
			this.MakeNeighbor();
			for (int num14 = 0; num14 < this.Size; num14++)
			{
				for (int num15 = 0; num15 < this.Size; num15++)
				{
					Cell cell2 = this.map.cells[num14, num15];
					if (this.zp.setShore && !cell2.HasBlock && !cell2.IsTopWater && cell2.height <= 20)
					{
						bool flag4 = false;
						for (int num16 = num14 - 1; num16 < num14 + 2; num16++)
						{
							if (num16 >= 0 && num16 < this.Size)
							{
								for (int num17 = num15 - 1; num17 < num15 + 2; num17++)
								{
									if (num17 >= 0 && num17 < this.Size && (num16 != num14 || num17 != num15) && this.map.cells[num16, num17].IsTopWater)
									{
										flag4 = true;
										break;
									}
								}
								if (flag4)
								{
									break;
								}
							}
						}
						if (flag4)
						{
							base.SetFloor(num14, num15, this.biomeShore.MatFloor.id, this.biomeShore.MatFloor.defFloor, 0);
							this.biomes[num14, num15] = this.biomeShore;
							if (this.zp.extraShores > 0)
							{
								int extraShores = this.zp.extraShores;
								for (int num18 = num14 - extraShores; num18 < num14 + extraShores + 1; num18++)
								{
									if (num18 >= 0 && num18 < this.Size)
									{
										for (int num19 = num15 - extraShores; num19 < num15 + extraShores + 1; num19++)
										{
											if (num19 >= 0 && num19 < this.Size && !(this.biomes[num18, num19] == this.biomeShore) && !this.map.cells[num18, num19].IsTopWater && !this.map.cells[num18, num19].HasBlock)
											{
												base.SetFloor(num18, num19, this.biomeShore.MatFloor.id, this.biomeShore.MatFloor.defFloor, 0);
												this.biomes[num18, num19] = this.biomeShore;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return true;
		}
		this.map.config.idBiome = "Sky";
		this.zone._biome = null;
		for (int num20 = 0; num20 < this.Size; num20++)
		{
			for (int num21 = 0; num21 < this.Size; num21++)
			{
				base.SetFloor(num20, num21, 0, 90, 0);
			}
		}
		return true;
	}

	public void MakeNeighbor()
	{
		MapGen.<>c__DisplayClass5_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (this.bp.surrounding == null)
		{
			return;
		}
		CS$<>8__locals1.thisInfo = this.bp.surrounding[1, 1];
		CS$<>8__locals1.seaDir = (CS$<>8__locals1.thisInfo.sea ? (1 + EClass.rnd(4)) : 0);
		this.<MakeNeighbor>g___MakeNeighbor|5_0(this.bp.surrounding[1, 2], this.Size / 3 + 7, new Point(0, this.Size - 1), 0, -1, 3, 1, ref CS$<>8__locals1);
		this.<MakeNeighbor>g___MakeNeighbor|5_0(this.bp.surrounding[1, 0], this.Size / 3 + 7, new Point(0, 0), 0, 1, 1, 3, ref CS$<>8__locals1);
		this.<MakeNeighbor>g___MakeNeighbor|5_0(this.bp.surrounding[2, 1], this.Size / 3 + 7, new Point(this.Size - 1, 0), -1, 0, 2, 4, ref CS$<>8__locals1);
		this.<MakeNeighbor>g___MakeNeighbor|5_0(this.bp.surrounding[0, 1], this.Size / 3 + 7, new Point(0, 0), 1, 0, 4, 2, ref CS$<>8__locals1);
		EClass._map.config.seaDir = CS$<>8__locals1.seaDir;
	}

	public void MakeNeighbor_old()
	{
		if (this.bp.surrounding == null)
		{
			return;
		}
		this.<MakeNeighbor_old>g___MakeNeighbor|6_0(this.bp.surrounding[1, 2], new Point(this.Size / 2, this.Size / 3 * 2 - 5), 0, 1);
		this.<MakeNeighbor_old>g___MakeNeighbor|6_0(this.bp.surrounding[1, 0], new Point(this.Size / 2, this.Size / 3 + 6), 0, -1);
		this.<MakeNeighbor_old>g___MakeNeighbor|6_0(this.bp.surrounding[2, 1], new Point(this.Size / 3 * 2 - 5, this.Size / 2), 1, 0);
		this.<MakeNeighbor_old>g___MakeNeighbor|6_0(this.bp.surrounding[0, 1], new Point(this.Size / 3 + 6, this.Size / 2), -1, 0);
	}

	public void MakeRoad()
	{
		if (this.bp.tileCenter == null || this.bp.ignoreRoad)
		{
			return;
		}
		EloMap.TileInfo tileCenter = this.bp.tileCenter;
		if (tileCenter.roadLeft)
		{
			this.<MakeRoad>g___MakeRoad|7_0(-1, 0);
		}
		if (tileCenter.roadRight)
		{
			this.<MakeRoad>g___MakeRoad|7_0(1, 0);
		}
		if (tileCenter.roadUp)
		{
			this.<MakeRoad>g___MakeRoad|7_0(0, 1);
		}
		if (tileCenter.roadDown)
		{
			this.<MakeRoad>g___MakeRoad|7_0(0, -1);
		}
	}

	public void ModifyHeight(Map _map)
	{
		this.map = _map;
		Cell[,] cells = this.map.cells;
		bool flag = false;
		for (int i = this.Size - 1; i > 0; i--)
		{
			for (int j = 0; j < this.Size - 1; j++)
			{
				Cell cell = cells[i, j];
				byte height = cell.height;
				Cell cell2 = cells[i - 1, j];
				Cell cell3 = cells[i, j + 1];
				if (!cell2.IsTopWater && !cell3.IsTopWater)
				{
					if (EClass.rnd(500) == 0)
					{
						flag = !flag;
					}
					if (EClass.rnd(3) == 0)
					{
						if (flag)
						{
							cell2.height = height;
						}
						else
						{
							cell3.height = height;
						}
					}
					if (this.hSetting.smoothDownhill && EClass.rnd(3) == 0)
					{
						if (cell2.height < height - 2)
						{
							cell2.height = height - 2;
						}
						if (cell3.height < height - 2)
						{
							cell3.height = height - 2;
						}
					}
					else
					{
						if (this.hSetting.mod1 && (cell2.height >= height - 1 || cell3.height >= height - 1))
						{
							if (EClass.rnd(4) == 0 && height != 0)
							{
								cell2.height = (cell3.height = height);
							}
							else
							{
								cell.height = (cell2.height = cell3.height);
							}
						}
						if (this.hSetting.mod2 && (cell2.height < height - 1 || cell3.height < height - 1))
						{
							if (EClass.rnd(2) == 0)
							{
								cell2.height = (cell3.height = height);
							}
							else
							{
								cells[i, j].height = (cell2.height = cell3.height);
							}
						}
						if (this.hSetting.mod3 && (cell2.height >= height - 1 || cell3.height >= height - 1))
						{
							if (EClass.rnd(2) == 0)
							{
								cell2.height = (cell3.height = height);
							}
							else
							{
								cells[i, j].height = (cell2.height = cell3.height);
							}
						}
					}
				}
			}
		}
		if (this.hSetting.smoothDownhill)
		{
			for (int k = this.Size - 1; k > 0; k--)
			{
				for (int l = 0; l < this.Size - 1; l++)
				{
					byte height2 = cells[k, l].height;
					Cell cell4 = cells[k - 1, l];
					Cell cell5 = cells[k, l + 1];
					if (height2 != 0)
					{
						if (cell4.height < height2 - 2 && !cell4.IsTopWater)
						{
							cell4.height = height2 - 2;
						}
						if (cell5.height < height2 - 2 && !cell5.IsTopWater)
						{
							cell5.height = height2 - 2;
						}
					}
				}
			}
		}
	}

	public void MakeRiver(Map _map)
	{
		int num = 0;
		for (int i = 0; i < 100; i++)
		{
			Point point = null;
			Point point2 = null;
			for (int j = 0; j < 1000; j++)
			{
				point = _map.GetRandomEdge(1);
				if (!point.cell.blocked)
				{
					break;
				}
			}
			for (int k = 0; k < 1000; k++)
			{
				point2 = _map.GetRandomEdge(1);
				if (point.x != point2.x && point.z != point2.z && !point2.cell.blocked && point.Distance(point2) >= _map.Size / 2)
				{
					break;
				}
			}
			if (this.TryMakeRiver(point, point2))
			{
				num += 1 + EClass.rnd(2);
				if (num > 10)
				{
					break;
				}
			}
		}
	}

	public bool TryMakeRiver(Point p1, Point p2)
	{
		List<Point> list = new List<Point>();
		int num = 30;
		for (int i = 0; i < num; i++)
		{
			list.Add(new Point
			{
				x = p1.x + (p2.x - p1.x) * i / num,
				z = p1.z + (p2.z - p1.z) * i / num
			});
		}
		for (int j = 0; j < num; j++)
		{
			Point point = list[j];
			if (!p1.Equals(point))
			{
				while (!p1.IsWater)
				{
					base.SetBlock(p1.x, p1.z, 0, 0, 0);
					base.SetFloor(p1.x, p1.z, 67, 44, 0);
					if (EClass.rnd(2) == 0)
					{
						if (p1.x != point.x)
						{
							p1.x += ((p1.x > point.x) ? -1 : 1);
						}
						else if (p1.z != point.z)
						{
							p1.z += ((p1.z > point.z) ? -1 : 1);
						}
					}
					else if (p1.z != point.z)
					{
						p1.z += ((p1.z > point.z) ? -1 : 1);
					}
					else if (p1.x != point.x)
					{
						p1.x += ((p1.x > point.x) ? -1 : 1);
					}
					if (p1.Equals(point))
					{
						break;
					}
				}
				if (p1.IsWater || p1.Equals(p2))
				{
					return true;
				}
			}
		}
		return true;
	}

	public void Populate(Map _map)
	{
		this.map = _map;
		EClass._zone.isShore = this.bp.zoneProfile.isShore;
		Rand.SetSeed(this.zp.seeds.poi);
		if (this.zp.river)
		{
			this.MakeRiver(_map);
		}
		this.map.RefreshAllTiles();
		this.layerStratum.FillHeightMap(this.heights1, this.OX, 0, this.OZ, this.Size, this.Size, this.seed, 1f);
		this.layerStratum.FillHeightMap(this.heights2, this.OX, 0, this.OZ, this.Size, this.Size, this.seed + 1, 1f);
		this.layerStratum.FillHeightMap(this.heights3, this.OX, 0, this.OZ, this.Size, this.Size, this.seed + 2, 1f);
		this.layerStratum.FillHeightMap(this.heights3d, this.OX, 0, this.OZ, this.Size, this.Size, this.seed, 1f);
		Point point = new Point();
		for (int i = 0; i < this.Size; i++)
		{
			for (int j = 0; j < this.Size; j++)
			{
				point.Set(i, j);
				Cell cell = point.cell;
				if (cell.IsTopWater && this.biomeWater)
				{
					if (cell.Left.IsTopWater && cell.Right.IsTopWater && cell.Front.IsTopWater && cell.Back.IsTopWater)
					{
						this.biomeWater.Populate(point, false);
					}
				}
				else
				{
					BiomeProfile biome = cell.biome;
					if (!cell.HasBlock)
					{
						biome.Populate(point, false);
					}
				}
			}
		}
		if (this.zp.name == "R_Plain")
		{
			Crawler crawler = Crawler.Create("pasture");
			int tries = 10;
			crawler.CrawlUntil(_map, () => _map.GetRandomPoint(), tries, delegate(Crawler.Result r)
			{
				int id = (EClass.rnd(3) == 0) ? 108 : 105;
				foreach (Point point2 in r.points)
				{
					this.map.SetObj(point2.x, point2.z, id, 1, 0);
					int num = 3;
					if (EClass.rnd(6) == 0)
					{
						num++;
					}
					point2.growth.SetStage(num, false);
				}
				return false;
			}, null);
		}
		if (this.crawlers != null)
		{
			Crawler[] crawlers = this.crawlers;
			for (int k = 0; k < crawlers.Length; k++)
			{
				crawlers[k].Crawl(this.map);
			}
		}
		this.MakeRoad();
		Rand.SetSeed(-1);
	}

	public void Output()
	{
		Debug.Log(this.zp.name + "/" + this.variation.name);
		Debug.Log(string.Concat(new string[]
		{
			"seed:",
			EClass._map.seed.ToString(),
			"  offset: ",
			this.OX.ToString(),
			"/",
			this.OZ.ToString()
		}));
	}

	[CompilerGenerated]
	private void <MakeNeighbor>g___MakeNeighbor|5_0(EloMap.TileInfo info, int _s, Point p, int vx, int vz, int _seaDir1, int _seaDir2, ref MapGen.<>c__DisplayClass5_0 A_8)
	{
		int num = _s;
		int num2 = 4;
		Point point = new Point();
		while (p.IsValid)
		{
			point.Set(p);
			num2--;
			if (num2 < 0)
			{
				num += ((EClass.rnd(2) == 0) ? 1 : -1);
				num = Mathf.Clamp(num, _s - 3, _s + 3);
				num2 = 2 + EClass.rnd(4);
			}
			for (int i = 0; i < num; i++)
			{
				if (vx != 0)
				{
					point.x = p.x + i * vx;
				}
				else
				{
					point.z = p.z + i * vz;
				}
				if (point.IsValid)
				{
					if (info.sea)
					{
						if (!A_8.thisInfo.sea)
						{
							A_8.seaDir = _seaDir1;
						}
						base.SetFloor(point.x, point.z, 66, (i >= num - 3) ? 44 : 43, 0);
					}
					else if (info.rock)
					{
						base.SetBlock(point.x, point.z, 45, 1, 0);
					}
					else if (info.shore)
					{
						if (!A_8.thisInfo.shore)
						{
							A_8.seaDir = _seaDir2;
						}
						base.SetFloor(point.x, point.z, 97, 33, 0);
					}
				}
			}
			if (vx == 0)
			{
				p.x++;
			}
			else
			{
				p.z++;
			}
		}
	}

	[CompilerGenerated]
	private void <MakeNeighbor_old>g___MakeNeighbor|6_0(EloMap.TileInfo info, Point p, int vx, int vz)
	{
		Point point = new Point();
		while (p.IsValid)
		{
			point.Set(p);
			int num = EClass.rnd(2) - EClass.rnd(2);
			if (vx != 0)
			{
				p.x += num;
			}
			else
			{
				p.z += num;
			}
			for (int i = -this.Size / 2; i < this.Size / 2; i++)
			{
				if (vx != 0)
				{
					point.z = p.z + i;
				}
				else
				{
					point.x = p.x + i;
				}
				if (point.IsValid)
				{
					if (info.sea)
					{
						base.SetFloor(point.x, point.z, 66, 43, 0);
					}
					else if (info.rock)
					{
						base.SetBlock(point.x, point.z, 45, 1, 0);
					}
					else if (info.shore)
					{
						base.SetFloor(point.x, point.z, 97, 33, 0);
					}
				}
			}
			p.x += vx;
			p.z += vz;
		}
	}

	[CompilerGenerated]
	private void <MakeRoad>g___MakeRoad|7_0(int vx, int vz)
	{
		Point point = new Point(this.Size / 2, this.Size / 2);
		Point point2 = new Point();
		while (point.IsValid)
		{
			point2.Set(point);
			for (int i = -1; i < 2; i++)
			{
				if (vx != 0)
				{
					point2.z = point.z + i;
				}
				else
				{
					point2.x = point.x + i;
				}
				if (point2.IsValid)
				{
					base.SetFloor(point2.x, point2.z, 45, 40, 0);
					base.SetBlock(point2.x, point2.z, 0, 0, 0);
					point2.cell.obj = 0;
				}
			}
			point.x += vx;
			point.z += vz;
			if (EClass.rnd(30) == 0)
			{
				point.x += ((vx == 0) ? (EClass.rnd(3) - 1) : 0);
				point.z += ((vz == 0) ? (EClass.rnd(3) - 1) : 0);
			}
		}
	}

	private static MapGen _Instance;
}
