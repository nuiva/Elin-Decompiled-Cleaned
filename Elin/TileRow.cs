using System;

public class TileRow : RenderRow
{
	[NonSerialized]
	public bool ignoreSnow;

	public int id;

	public int hp;

	public string alias;

	public string soundFoot;

	public void Init()
	{
		tileType = TileType.dict[_tileType];
		SetRenderData();
		OnInit();
	}

	public virtual void OnInit()
	{
	}
}
