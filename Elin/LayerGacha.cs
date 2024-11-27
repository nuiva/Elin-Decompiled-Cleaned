using System;
using DG.Tweening;
using UnityEngine;

public class LayerGacha : ELayer
{
	public override void OnAfterInit()
	{
		Msg.TalkHomeMemeber("layerGacha");
		if (this.alt)
		{
			ActionMode.NoMap.Activate(true, false);
		}
		this.RefreshCoin();
		base.InvokeRepeating("RandomAnime", 2f, 2f);
	}

	private void RandomAnime()
	{
		this.randomAnimes.RandomItem<DOTweenAnimation>().DORestart();
	}

	public void OnClickGoldGacha(int num)
	{
		this.PlayGacha(true, num);
	}

	public void OnClickSilverGacha(int num)
	{
		this.PlayGacha(false, num);
	}

	public void PlayGacha(bool gold, int num)
	{
		Thing thing = null;
		if (ELayer.player.dailyGacha && !gold && num == 1)
		{
			ELayer.player.dailyGacha = false;
		}
		else
		{
			thing = ELayer.pc.things.Find("gacha_coin", gold ? "gold" : "silver");
			if (thing == null || thing.Num < num)
			{
				SE.Beep();
				return;
			}
			thing.ModNum(-num, true);
		}
		this.RefreshCoin();
		Dialog d = Layer.Create("DialogGacha") as Dialog;
		d.windows[0].setting.textCaption = "confirm".lang();
		d.note.AddPrefab("Media/Graphics/Image/IllustGacha");
		d.note.Space(0, 1);
		bool lose = true;
		for (int i = 0; i < num; i++)
		{
			int num2 = 1;
			if (ELayer.rnd(6) == 0)
			{
				num2 = 2;
			}
			if (ELayer.rnd(30) == 0)
			{
				num2 = 3;
			}
			if (ELayer.rnd(100) == 0)
			{
				num2 = 4;
			}
			if (ELayer.rnd(1000) == 0)
			{
				num2 = 5;
			}
			if (gold)
			{
				num2++;
			}
			if (i == 9)
			{
				if (gold)
				{
					if (num2 < 4)
					{
						num2 = 4;
					}
				}
				else if (num2 < 3)
				{
					num2 = 3;
				}
			}
			if (num2 > 5)
			{
				num2 = 5;
			}
			Hoard.Item item = ELayer.player.hoard.AddRandom(num2, true);
			int rarity = item.Source.rarity;
			if (rarity == 4 || rarity == 5)
			{
				lose = false;
			}
			d.note.AddText(null, item.Name(1), ELayer.Colors.GetRarityColor(rarity, false));
		}
		d.note.Space(0, 1);
		if (thing == null)
		{
			d.note.AddText("dailyGacha".lang() + "  1 > 0", FontColor.DontChange);
		}
		else
		{
			d.note.AddText(string.Concat(new string[]
			{
				thing.NameSimple,
				"  ",
				(thing.Num + num).ToString(),
				" > ",
				thing.Num.ToString()
			}), FontColor.DontChange);
		}
		d.note.Build();
		d.windows[0].Find("GachaAnime", false).SetActive(true);
		d.list.AddButton(null, Lang.Get("ok"), delegate
		{
			d.Close();
		}, null);
		ELayer.ui.AddLayer(d);
		ELayer.Sound.Play("gacha");
		TweenUtil.Delay(0.5f, delegate
		{
			ELayer.Sound.Play(lose ? "gacha_lose" : "gacha_win");
		});
		d.SetOnKill(delegate
		{
			if (!lose)
			{
				Msg.TalkHomeMemeber("ding_other");
			}
		});
	}

	public void RefreshCoin()
	{
		this.goFree.SetActive(ELayer.player.dailyGacha);
		this.listCoin.Clear();
		BaseList baseList = this.listCoin;
		UIList.Callback<Thing, ButtonGrid> callback = new UIList.Callback<Thing, ButtonGrid>();
		callback.onInstantiate = delegate(Thing a, ButtonGrid b)
		{
			b.SetCard(a, ButtonGrid.Mode.Default, null);
		};
		callback.onClick = delegate(Thing a, ButtonGrid b)
		{
			ELayer.ui.AddLayer<LayerInfo>().Set(a, false);
		};
		baseList.callbacks = callback;
		this.listCoin.Refresh(false);
	}

	public void OnClickBuyCoin()
	{
		SE.Beep();
	}

	public override void OnKill()
	{
		if (this.alt)
		{
			ActionMode.DefaultMode.Activate(true, false);
		}
	}

	public static GameObject slot;

	public GameObject goFree;

	public Color colorC;

	public Color colorR;

	public Color colorSR;

	public Color colorSSR;

	public Color colorLE;

	public DOTweenAnimation[] randomAnimes;

	public bool alt;

	public UIList listCoin;
}
