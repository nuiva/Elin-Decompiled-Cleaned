public class LayerChara : ELayer
{
	public static LayerChara Instance;

	public WindowChara windowChara;

	public override bool blockWidgetClick => false;

	public LayerChara SetChara(Chara c)
	{
		windowChara.SetChara(c);
		Instance = this;
		return this;
	}

	public static void Refresh()
	{
		if ((bool)Instance)
		{
			Instance.windowChara.Refresh();
		}
	}
}
