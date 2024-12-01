using UnityEngine;

public class RenderParam : MeshPassParam
{
	public static RenderParam shared = new RenderParam();

	public int liquidLv;

	public int tile2;

	public int halfBlockColor;

	public int dir;

	public Cell cell;

	public SourceMaterial.Row mat;

	public Vector3 v;

	public float shadowFix;

	public Vector3 NewVector3 => new Vector3(x, y, z);

	public RenderParam()
	{
	}

	public RenderParam(RenderParam p)
	{
		liquid = p.liquid;
		liquidLv = p.liquidLv;
		tile2 = p.tile2;
		halfBlockColor = p.halfBlockColor;
		dir = p.dir;
		mat = p.mat;
		v = p.v;
		shadowFix = p.shadowFix;
		x = p.x;
		y = p.y;
		z = p.z;
		color = p.color;
		tile = p.tile;
		matColor = p.matColor;
		liquid = p.liquid;
		cell = p.cell;
		snow = p.snow;
	}
}
