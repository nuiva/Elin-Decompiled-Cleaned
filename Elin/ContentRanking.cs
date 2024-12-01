using UnityEngine;

public class ContentRanking : EContent
{
	public UIList list;

	public UIText textTitle;

	public UIText textFactionName;

	public Sprite[] spriteTrophies;

	public GameObject comingSoon;

	public GameObject wet;

	public override void OnSwitchContent(int idTab)
	{
		SwitchRanking("contribution");
	}

	public void SwitchRanking(string id)
	{
		textTitle.text = Lang.Get("rank_" + id);
		textFactionName.text = EClass.Home.name;
		switch (id)
		{
		}
		list.callbacks = new UIList.Callback<Chara, ButtonChara>
		{
			onInstantiate = delegate(Chara a, ButtonChara b)
			{
				b.SetChara(a, ButtonChara.Mode.Journal);
				b.item.text1.text = "123456";
				b.item.text2.text = "contribution".lang();
			}
		};
		list.Clear();
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.faction == EClass.Home)
			{
				list.Add(chara);
			}
		}
		list.Refresh();
		for (int i = 0; i < list.buttons.Count; i++)
		{
			ButtonChara buttonChara = list.buttons[i].component as ButtonChara;
			buttonChara.item.text3.text = "rank".lang((i + 1).ToString() ?? "");
			buttonChara.item.image1.SetActive(i < 3);
			if (i < 3)
			{
				buttonChara.item.image1.sprite = spriteTrophies[i];
			}
		}
		comingSoon.SetActive(id != "contribution");
		wet.SetActive(id == "wettunic");
		list.SetActive(id == "contribution");
		this.RebuildLayout(recursive: true);
	}
}
