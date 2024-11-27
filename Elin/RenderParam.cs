using System;
using UnityEngine;

public class RenderParam : MeshPassParam
{
	public Vector3 NewVector3
	{
		get
		{
			return new Vector3(this.x, this.y, this.z);
		}
	}

	public RenderParam()
	{
	}

	public RenderParam(RenderParam p)
	{
		this.liquid = p.liquid;
		this.liquidLv = p.liquidLv;
		this.tile2 = p.tile2;
		this.halfBlockColor = p.halfBlockColor;
		this.dir = p.dir;
		this.mat = p.mat;
		this.v = p.v;
		this.shadowFix = p.shadowFix;
		this.x = p.x;
		this.y = p.y;
		this.z = p.z;
		this.color = p.color;
		this.tile = p.tile;
		this.matColor = p.matColor;
		this.liquid = p.liquid;
		this.cell = p.cell;
		this.snow = p.snow;
	}

	public static RenderParam shared = new RenderParam();

	public int liquidLv;

	public int tile2;

	public int halfBlockColor;

	public int dir;

	public Cell cell;

	public SourceMaterial.Row mat;

	public Vector3 v;

	public float shadowFix;
}
