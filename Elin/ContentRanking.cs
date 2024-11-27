using System;
using UnityEngine;

public class ContentRanking : EContent
{
	public override void OnSwitchContent(int idTab)
	{
		this.SwitchRanking("contribution");
	}

	public void SwitchRanking(string id)
	{
		this.textTitle.text = Lang.Get("rank_" + id);
		this.textFactionName.text = EClass.Home.name;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(id);
		if (num <= 2148445359U)
		{
			if (num <= 1206285387U)
			{
				if (num != 1028682697U)
				{
					if (num == 1206285387U)
					{
						if (!(id == "improve"))
						{
						}
					}
				}
				else if (!(id == "food"))
				{
				}
			}
			else if (num != 1536741984U)
			{
				if (num == 2148445359U)
				{
					if (!(id == "contribution"))
					{
					}
				}
			}
			else if (!(id == "work"))
			{
			}
		}
		else if (num <= 2659088997U)
		{
			if (num != 2631713119U)
			{
				if (num == 2659088997U)
				{
					if (!(id == "disease"))
					{
					}
				}
			}
			else
			{
				id == "lostparts";
			}
		}
		else if (num != 3184533872U)
		{
			if (num == 4166973234U)
			{
				if (!(id == "wettunic"))
				{
				}
			}
		}
		else if (!(id == "troublemaker"))
		{
		}
		BaseList baseList = this.list;
		UIList.Callback<Chara, ButtonChara> callback = new UIList.Callback<Chara, ButtonChara>();
		callback.onInstantiate = delegate(Chara a, ButtonChara b)
		{
			b.SetChara(a, ButtonChara.Mode.Journal);
			b.item.text1.text = "123456";
			b.item.text2.text = "contribution".lang();
		};
		baseList.callbacks = callback;
		this.list.Clear();
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.faction == EClass.Home)
			{
				this.list.Add(chara);
			}
		}
		this.list.Refresh(false);
		for (int i = 0; i < this.list.buttons.Count; i++)
		{
			ButtonChara buttonChara = this.list.buttons[i].component as ButtonChara;
			buttonChara.item.text3.text = "rank".lang((i + 1).ToString() ?? "", null, null, null, null);
			buttonChara.item.image1.SetActive(i < 3);
			if (i < 3)
			{
				buttonChara.item.image1.sprite = this.spriteTrophies[i];
			}
		}
		this.comingSoon.SetActive(id != "contribution");
		this.wet.SetActive(id == "wettunic");
		this.list.SetActive(id == "contribution");
		this.RebuildLayout(true);
	}

	public UIList list;

	public UIText textTitle;

	public UIText textFactionName;

	public Sprite[] spriteTrophies;

	public GameObject comingSoon;

	public GameObject wet;
}
