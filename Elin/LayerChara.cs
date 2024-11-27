using System;

public class LayerChara : ELayer
{
	public override bool blockWidgetClick
	{
		get
		{
			return false;
		}
	}

	public LayerChara SetChara(Chara c)
	{
		this.windowChara.SetChara(c);
		LayerChara.Instance = this;
		return this;
	}

	public static void Refresh()
	{
		if (LayerChara.Instance)
		{
			LayerChara.Instance.windowChara.Refresh();
		}
	}

	public static LayerChara Instance;

	public WindowChara windowChara;
}
