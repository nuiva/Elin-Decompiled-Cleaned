using UnityEngine;
using UnityEngine.UI;

public class ContentKeyItem : EContent
{
	public UIList list;

	public UIText textClient;

	public UIText textTitle;

	public UIText textDetail;

	public Image imageItem;

	public override void OnSwitchContent(int idTab)
	{
		list.sortMode = UIList.SortMode.ByValue;
		list.callbacks = new UIList.Callback<SourceKeyItem.Row, ItemGeneral>
		{
			onInstantiate = delegate(SourceKeyItem.Row a, ItemGeneral b)
			{
				b.SetSound();
				b.button1.mainText.SetText(a.GetName());
				b.SetSubText(EClass.player.CountKeyItem(a.id).ToString() ?? "", 260, FontColor.Default, TextAnchor.MiddleRight);
			},
			onClick = delegate(SourceKeyItem.Row a, ItemGeneral b)
			{
				SelectItem(a);
			},
			onList = delegate
			{
				foreach (int key in EClass.player.keyItems.Keys)
				{
					if (EClass.player.CountKeyItem(key) > 0)
					{
						list.Add(EClass.sources.keyItems.map[key]);
					}
				}
			}
		};
		list.List();
		SelectItem(EClass.sources.keyItems.rows[0]);
	}

	public void SelectItem(SourceKeyItem.Row q)
	{
		textTitle.SetText(q.GetName());
		string detail = q.GetDetail();
		textDetail.SetText(detail);
		Sprite sprite = Resources.Load<Sprite>("Media/Graphics/Image/KeyItem/" + q.alias);
		if ((bool)sprite)
		{
			imageItem.sprite = sprite;
		}
		imageItem.SetNativeSize();
		imageItem.SetActive(sprite);
		this.RebuildLayout(recursive: true);
	}
}
