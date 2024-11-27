using System;

public class ContentChronicle : EContent
{
	public override void OnSwitchContent(int idTab)
	{
		BaseList baseList = this.list;
		UIList.Callback<MsgLog.Data, UIItem> callback = new UIList.Callback<MsgLog.Data, UIItem>();
		callback.onInstantiate = delegate(MsgLog.Data a, UIItem b)
		{
			b.text1.text = a.text;
			if (!a.col.IsEmpty())
			{
				b.text1.SetColor(a.col.ToEnum(true));
			}
		};
		baseList.callbacks = callback;
		this.list.Clear();
		foreach (MsgLog.Data o in EClass.game.log.GetList(false))
		{
			this.list.Add(o);
		}
		this.list.Refresh(false);
		this.RebuildLayout(true);
	}

	public UIList list;
}
