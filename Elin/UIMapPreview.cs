using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIMapPreview : EMono
{
	public class GenThread : EClass
	{
		public ZoneBlueprint bp;

		public Map map;

		public bool done = true;

		public void Init()
		{
			bp = new ZoneBlueprint();
			bp.Create();
			Reset();
		}

		public void Reset()
		{
			done = false;
		}

		public void Start()
		{
			done = true;
		}
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

	public GenThread thread;

	public float voidAlpha;

	public float minColor;

	public float maxColor;

	private void Awake()
	{
		if ((bool)image)
		{
			texDefault = image.texture;
		}
	}

	public void GenerateMap(ZoneBlueprint bp)
	{
		if (thread == null)
		{
			thread = new GenThread();
			thread.Init();
		}
		else
		{
			if (!thread.done)
			{
				return;
			}
			image.texture = texDefault;
			thread.Reset();
		}
		thread.bp.genSetting = bp.genSetting;
		thread.bp.zoneProfile = bp.zoneProfile;
		thread.bp.genSetting.seed = EMono.rnd(20000);
		ThreadPool.QueueUserWorkItem(delegate
		{
			thread.Start();
		});
		CancelInvoke();
		InvokeRepeating("CheckThread", 0f, 0.1f);
	}

	public void CheckThread()
	{
		if (thread.done)
		{
			SetMap(thread.bp.map);
			CancelInvoke();
		}
	}

	public void SetMap(Map _map)
	{
		map = _map;
		if (_map == null)
		{
			image.texture = texDefault;
			return;
		}
		cells = map.cells;
		int num = (Size = ((!limitBounds) ? map.Size : ((map.bounds.Width > map.bounds.Height) ? map.bounds.Width : map.bounds.Height)));
		offsetX = ((map.bounds.Width <= map.bounds.Height) ? ((num - _map.bounds.Width) / 2) : 0);
		offsetZ = ((map.bounds.Width > map.bounds.Height) ? ((num - _map.bounds.Height) / 2) : 0);
		px = new Color[Size * Size];
		if ((bool)tex)
		{
			UnityEngine.Object.DestroyImmediate(tex);
		}
		tex = new Texture2D(Size, Size)
		{
			filterMode = filter,
			wrapMode = TextureWrapMode.Clamp
		};
		for (int i = 0; i < Size * Size; i++)
		{
			px[i].a = voidAlpha;
		}
		tex.SetPixels(px);
		if (limitBounds)
		{
			for (int j = 0; j < map.bounds.Height; j++)
			{
				for (int k = 0; k < map.bounds.Width; k++)
				{
					_RefreshPoint(map.bounds.x + k, map.bounds.z + j, apply: false);
				}
			}
		}
		else
		{
			for (int l = 0; l < Size; l++)
			{
				for (int m = 0; m < Size; m++)
				{
					_RefreshPoint(m, l, apply: false);
				}
			}
		}
		tex.SetPixels(px);
		tex.Apply();
		if ((bool)matMap)
		{
			matMap.SetTexture("_MainTex", tex);
		}
		if ((bool)image)
		{
			image.texture = tex;
		}
	}

	public void UpdateMap(int x, int z)
	{
		_RefreshPoint(x, z);
		tex.Apply();
	}

	public void UpdateMap(List<Cell> newPoints)
	{
		foreach (Cell newPoint in newPoints)
		{
			_RefreshPoint(newPoint.x, newPoint.z);
		}
		tex.Apply();
	}

	public void _RefreshPoint(int x, int z, bool apply = true)
	{
		if (x >= EMono._map.Size || z >= EMono._map.Size)
		{
			return;
		}
		Cell cell = cells[x, z];
		int num = x;
		int num2 = z;
		if (limitBounds)
		{
			num = x - EMono._map.bounds.x + offsetX;
			num2 = z - EMono._map.bounds.z + offsetZ;
		}
		int num3 = num2 * Size + num;
		if (num3 >= px.Length || num3 < 0)
		{
			return;
		}
		if (monoColor)
		{
			if (!cell.isSeen)
			{
				px[num3] = colorSurround;
			}
			else if (cell.isSurrounded)
			{
				px[num3] = colorSurround;
			}
			else if (cell.IsTopWater)
			{
				px[num3] = colorWater;
			}
			else if (cell.HasBlock)
			{
				px[num3] = colorEdge;
			}
			else
			{
				px[num3] = colorDefault;
			}
		}
		else if (!cell.isSeen)
		{
			px[num3] = colorSurround;
		}
		else if (cell.HasZoneStairs())
		{
			px[num3] = colorStairs;
		}
		else if (cell.isSurrounded)
		{
			px[num3] = colorSurround;
		}
		else if (cell.HasBlock)
		{
			px[num3] = cell.matBlock.GetColor();
		}
		else
		{
			SourceMaterial.Row row = ((cell.bridgeHeight != 0) ? cell.matBridge : cell.matFloor);
			if (!Application.isEditor && !EMono._zone.IsRegion && cell.IsSnowTile)
			{
				row = MATERIAL.sourceSnow;
			}
			Color color = (cell.IsSky ? colorSky : row.GetColor());
			if (color.r > maxColor)
			{
				color.r = maxColor;
			}
			else if (color.r < minColor)
			{
				color.r = minColor;
			}
			if (color.g > maxColor)
			{
				color.g = maxColor;
			}
			else if (color.g < minColor)
			{
				color.g = minColor;
			}
			if (color.b > maxColor)
			{
				color.b = maxColor;
			}
			else if (color.b < minColor)
			{
				color.b = minColor;
			}
			px[num3] = color;
		}
		if (cell.isSeen)
		{
			if (cell.HasBlock)
			{
				px[num3] *= 0.4f;
			}
			else if (cell.room != null)
			{
				px[num3] *= 0.9f;
			}
		}
		if (!EMono._map.bounds.Contains(x, z))
		{
			px[num3].a = voidAlpha;
		}
		else
		{
			px[num3].a = 1f;
		}
		if (apply)
		{
			tex.SetPixel(num, num2, px[num3]);
		}
	}
}
