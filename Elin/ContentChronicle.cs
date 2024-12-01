public class ContentChronicle : EContent
{
	public UIList list;

	public override void OnSwitchContent(int idTab)
	{
		list.callbacks = new UIList.Callback<MsgLog.Data, UIItem>
		{
			onInstantiate = delegate(MsgLog.Data a, UIItem b)
			{
				b.text1.text = a.text;
				if (!a.col.IsEmpty())
				{
					b.text1.SetColor(a.col.ToEnum<FontColor>());
				}
			}
		};
		list.Clear();
		foreach (MsgLog.Data item in EClass.game.log.GetList())
		{
			list.Add(item);
		}
		list.Refresh();
		this.RebuildLayout(recursive: true);
	}
}
