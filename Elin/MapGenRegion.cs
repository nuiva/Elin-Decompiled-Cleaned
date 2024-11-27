using System;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

public class MapGenRegion : BaseMapGen
{
	public static MapGenRegion Instance
	{
		get
		{
			MapGenRegion result;
			if ((result = MapGenRegion._Instance) == null)
			{
				result = (MapGenRegion._Instance = new MapGenRegion());
			}
			return result;
		}
	}

	protected override bool OnGenerateTerrain()
	{
		EClass.scene.elomapActor.Initialize(this.zone.Region.elomap);
		EloMap elomap = EClass.scene.elomapActor.elomap;
		base.SetSize(Mathf.Max(elomap.w, elomap.h), 10);
		this.map.CreateNew(this.Size, true);
		for (int i = 0; i < elomap.h; i++)
		{
			for (int j = 0; j < elomap.w; j++)
			{
				elomap.cells[j, i];
				int gridX = elomap.minX + j;
				int gridY = elomap.minY + i;
				this.map.cells[j, i].isSeen = true;
				base.SetFloor(j, i, 66, 43, 0);
				foreach (STETilemap stetilemap in elomap.group.Tilemaps)
				{
					if (!(stetilemap == elomap.fogmap) && !(stetilemap == elomap.cloudmap))
					{
						int tileIdFromTileData = Tileset.GetTileIdFromTileData(stetilemap.GetTileData(gridX, gridY));
						SourceGlobalTile.Row row = EClass.sources.globalTiles.tileAlias.TryGetValue(tileIdFromTileData, null);
						if (row != null)
						{
							this.map.cells[j, i].impassable = (row.attribs[0] == 0);
							if (row.floor != 0)
							{
								base.SetFloor(j, i, EClass.sources.floors.rows[row.floor].DefaultMaterial.id, row.floor, 0);
							}
						}
					}
				}
				if (Tileset.GetTileIdFromTileData(elomap.cloudmap.GetTileData(gridX, gridY)) < 1000)
				{
					this.map.cells[j, i].impassable = true;
				}
			}
		}
		this.map.poiMap.Reset();
		this.map.SetZone(this.zone);
		this.map.config.idSceneProfile = "region";
		this.map.RefreshAllTiles();
		return true;
	}

	private static MapGenRegion _Instance;
}
