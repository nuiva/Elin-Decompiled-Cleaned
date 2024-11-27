using System;

public class TileRow : RenderRow
{
	public void Init()
	{
		this.tileType = TileType.dict[this._tileType];
		base.SetRenderData();
		this.OnInit();
	}

	public virtual void OnInit()
	{
	}

	[NonSerialized]
	public bool ignoreSnow;

	public int id;

	public int hp;

	public string alias;

	public string soundFoot;
}
