using System;
using UnityEngine;

public class ZoneInspector : EMono
{
	public string idExport
	{
		get
		{
			Zone zone = this.zone;
			if (zone == null)
			{
				return null;
			}
			return zone.idExport;
		}
	}

	public MapConfig mapConfig
	{
		get
		{
			Zone zone = this.zone;
			if (zone == null)
			{
				return null;
			}
			return zone.map.config;
		}
		set
		{
			this.zone.map.config = value;
		}
	}

	public MapBounds bounds
	{
		get
		{
			Zone zone = this.zone;
			if (zone == null)
			{
				return null;
			}
			return zone.map.bounds;
		}
		set
		{
			this.zone.map.bounds = value;
		}
	}

	private void Awake()
	{
		ZoneInspector.Instance = this;
	}

	public void RefreshAll()
	{
		EMono._map.RefreshAllTiles();
	}

	public void RefreshScreen()
	{
		if (!EMono.core.IsGameStarted)
		{
			return;
		}
		EMono.scene.profile = SceneProfile.Load(EMono._map.config.idSceneProfile.IsEmpty("default"));
		EMono.screen.Deactivate();
		if (EMono.player.zone is Region)
		{
			ActionMode.Region.Activate(true, false);
		}
		else
		{
			ActionMode.Adv.Activate(true, false);
		}
		EMono.screen.SetZoom(0.5f);
		EMono.screen.RefreshScreenSize();
	}

	public void SetAllPlayerCreation()
	{
		foreach (Thing thing in EMono._map.things)
		{
			thing.isPlayerCreation = true;
		}
		foreach (Chara chara in EMono._map.charas)
		{
			chara.isPlayerCreation = true;
		}
	}

	public void Export()
	{
		EMono._zone.Export();
	}

	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			this.RefreshScreen();
		}
	}

	public static ZoneInspector Instance;

	public ZoneInspector.ResizeUtil resize = new ZoneInspector.ResizeUtil();

	public ZoneInspector.FillUtil fill = new ZoneInspector.FillUtil();

	public Zone zone;

	public class FillUtil
	{
		public void FlattenHeight()
		{
			EMono._map.ForeachCell(delegate(Cell c)
			{
				c.height = (byte)this.flatHeight;
			});
			EMono._map.RefreshAllTiles();
		}

		public void FillFloor()
		{
			EMono._map.ForeachCell(delegate(Cell c)
			{
				int dir = this.randomFillDir ? EMono.rnd(4) : 0;
				EMono._map.SetFloor((int)c.x, (int)c.z, EMono.sources.floors.rows[this.fillFloor].DefaultMaterial.id, this.fillFloor, dir);
			});
		}

		public void FillBlock()
		{
			EMono._map.ForeachCell(delegate(Cell c)
			{
				int dir = this.randomFillDir ? EMono.rnd(4) : 0;
				EMono._map.SetBlock((int)c.x, (int)c.z, EMono.sources.blocks.rows[this.fillBlock].DefaultMaterial.id, this.fillBlock, dir);
			});
		}

		public void ClearBlock()
		{
			EMono._map.ForeachCell(delegate(Cell c)
			{
				EMono._map.SetBlock((int)c.x, (int)c.z, 0, 0);
			});
		}

		public void ClearObj()
		{
			EMono._map.ForeachCell(delegate(Cell c)
			{
				EMono._map.SetObj((int)c.x, (int)c.z, 0, 1, 0);
			});
		}

		public void ClearBridge()
		{
			EMono._map.ForeachCell(delegate(Cell c)
			{
				EMono._map.SetObj((int)c.x, (int)c.z, 0, 1, 0);
			});
		}

		public int flatHeight;

		public int fillFloor = 5;

		public int fillBlock = 1;

		public bool randomFillDir;
	}

	public class ResizeUtil
	{
		public void Apply()
		{
			if (EMono._map.Size != this.newSize)
			{
				EMono._map.Resize(this.newSize);
			}
			if (this.offset.x != 0 || this.offset.y != 0)
			{
				EMono._map.Shift(this.offset);
			}
		}

		public int newSize;

		public Vector2Int offset;
	}
}
