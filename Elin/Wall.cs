using System;

public struct Wall
{
	public SourceBlock.Row source
	{
		get
		{
			return Cell.blockList[(int)this.id];
		}
	}

	public SourceMaterial.Row mat
	{
		get
		{
			return Cell.matList[(int)this.idMat];
		}
	}

	public static Map map;

	public byte id;

	public byte idMat;
}
