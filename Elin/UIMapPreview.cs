using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIMapPreview : EMono
{
	private void Awake()
	{
		if (this.image)
		{
			this.texDefault = this.image.texture;
		}
	}

	public void GenerateMap(ZoneBlueprint bp)
	{
		if (this.thread == null)
		{
			this.thread = new UIMapPreview.GenThread();
			this.thread.Init();
		}
		else
		{
			if (!this.thread.done)
			{
				return;
			}
			this.image.texture = this.texDefault;
			this.thread.Reset();
		}
		this.thread.bp.genSetting = bp.genSetting;
		this.thread.bp.zoneProfile = bp.zoneProfile;
		this.thread.bp.genSetting.seed = EMono.rnd(20000);
		ThreadPool.QueueUserWorkItem(delegate(object a)
		{
			this.thread.Start();
		});
		base.CancelInvoke();
		base.InvokeRepeating("CheckThread", 0f, 0.1f);
	}

	public void CheckThread()
	{
		if (this.thread.done)
		{
			this.SetMap(this.thread.bp.map);
			base.CancelInvoke();
		}
	}

	public void SetMap(Map _map)
	{
		this.map = _map;
		if (_map == null)
		{
			this.image.texture = this.texDefault;
			return;
		}
		this.cells = this.map.cells;
		int num = this.limitBounds ? ((this.map.bounds.Width > this.map.bounds.Height) ? this.map.bounds.Width : this.map.bounds.Height) : this.map.Size;
		this.Size = num;
		this.offsetX = ((this.map.bounds.Width > this.map.bounds.Height) ? 0 : ((num - _map.bounds.Width) / 2));
		this.offsetZ = ((this.map.bounds.Width > this.map.bounds.Height) ? ((num - _map.bounds.Height) / 2) : 0);
		this.px = new Color[this.Size * this.Size];
		if (this.tex)
		{
			UnityEngine.Object.DestroyImmediate(this.tex);
		}
		this.tex = new Texture2D(this.Size, this.Size)
		{
			filterMode = this.filter,
			wrapMode = TextureWrapMode.Clamp
		};
		for (int i = 0; i < this.Size * this.Size; i++)
		{
			this.px[i].a = this.voidAlpha;
		}
		this.tex.SetPixels(this.px);
		if (this.limitBounds)
		{
			for (int j = 0; j < this.map.bounds.Height; j++)
			{
				for (int k = 0; k < this.map.bounds.Width; k++)
				{
					this._RefreshPoint(this.map.bounds.x + k, this.map.bounds.z + j, false);
				}
			}
		}
		else
		{
			for (int l = 0; l < this.Size; l++)
			{
				for (int m = 0; m < this.Size; m++)
				{
					this._RefreshPoint(m, l, false);
				}
			}
		}
		this.tex.SetPixels(this.px);
		this.tex.Apply();
		if (this.matMap)
		{
			this.matMap.SetTexture("_MainTex", this.tex);
		}
		if (this.image)
		{
			this.image.texture = this.tex;
		}
	}

	public void UpdateMap(int x, int z)
	{
		this._RefreshPoint(x, z, true);
		this.tex.Apply();
	}

	public void UpdateMap(List<Cell> newPoints)
	{
		foreach (Cell cell in newPoints)
		{
			this._RefreshPoint((int)cell.x, (int)cell.z, true);
		}
		this.tex.Apply();
	}

	public void _RefreshPoint(int x, int z, bool apply = true)
	{
		if (x >= EMono._map.Size || z >= EMono._map.Size)
		{
			return;
		}
		Cell cell = this.cells[x, z];
		int num = x;
		int num2 = z;
		if (this.limitBounds)
		{
			num = x - EMono._map.bounds.x + this.offsetX;
			num2 = z - EMono._map.bounds.z + this.offsetZ;
		}
		int num3 = num2 * this.Size + num;
		if (num3 >= this.px.Length || num3 < 0)
		{
			return;
		}
		if (this.monoColor)
		{
			if (!cell.isSeen)
			{
				this.px[num3] = this.colorSurround;
			}
			else if (cell.isSurrounded)
			{
				this.px[num3] = this.colorSurround;
			}
			else if (cell.IsTopWater)
			{
				this.px[num3] = this.colorWater;
			}
			else if (cell.HasBlock)
			{
				this.px[num3] = this.colorEdge;
			}
			else
			{
				this.px[num3] = this.colorDefault;
			}
		}
		else if (!cell.isSeen)
		{
			this.px[num3] = this.colorSurround;
		}
		else if (cell.HasZoneStairs(true))
		{
			this.px[num3] = this.colorStairs;
		}
		else if (cell.isSurrounded)
		{
			this.px[num3] = this.colorSurround;
		}
		else if (cell.HasBlock)
		{
			this.px[num3] = cell.matBlock.GetColor();
		}
		else
		{
			SourceMaterial.Row row = (cell.bridgeHeight != 0) ? cell.matBridge : cell.matFloor;
			if (!Application.isEditor && !EMono._zone.IsRegion && cell.IsSnowTile)
			{
				row = MATERIAL.sourceSnow;
			}
			Color color = cell.IsSky ? this.colorSky : row.GetColor();
			if (color.r > this.maxColor)
			{
				color.r = this.maxColor;
			}
			else if (color.r < this.minColor)
			{
				color.r = this.minColor;
			}
			if (color.g > this.maxColor)
			{
				color.g = this.maxColor;
			}
			else if (color.g < this.minColor)
			{
				color.g = this.minColor;
			}
			if (color.b > this.maxColor)
			{
				color.b = this.maxColor;
			}
			else if (color.b < this.minColor)
			{
				color.b = this.minColor;
			}
			this.px[num3] = color;
		}
		if (cell.isSeen)
		{
			if (cell.HasBlock)
			{
				this.px[num3] *= 0.4f;
			}
			else if (cell.room != null)
			{
				this.px[num3] *= 0.9f;
			}
		}
		if (!EMono._map.bounds.Contains(x, z))
		{
			this.px[num3].a = this.voidAlpha;
		}
		else
		{
			this.px[num3].a = 1f;
		}
		if (!apply)
		{
			return;
		}
		this.tex.SetPixel(num, num2, this.px[num3]);
	}

	public Color colorDefault;

	public Color colorWater;

	public Color colorSurround;

	public Color colorEdge;

	public Color colorFog;

	public Color colorSky;

	public Color colorStairs;

	public Material matMap;

	public RawImage image;

	public UIButton button;

	private Texture texDefault;

	private Texture2D tex;

	private Color[] px;

	private Cell[,] cells;

	[NonSerialized]
	public int Size;

	[NonSerialized]
	public int offsetX;

	[NonSerialized]
	public int offsetZ;

	public FilterMode filter = FilterMode.Bilinear;

	public Map map;

	public bool monoColor;

	public bool createNewMaterial;

	public bool limitBounds;

	public UIMapPreview.GenThread thread;

	public float voidAlpha;

	public float minColor;

	public float maxColor;

	public class GenThread : EClass
	{
		public void Init()
		{
			this.bp = new ZoneBlueprint();
			this.bp.Create();
			this.Reset();
		}

		public void Reset()
		{
			this.done = false;
		}

		public void Start()
		{
			this.done = true;
		}

		public ZoneBlueprint bp;

		public Map map;

		public bool done = true;
	}
}
