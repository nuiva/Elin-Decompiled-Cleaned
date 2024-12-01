using DG.Tweening;
using UnityEngine;

public class LayerGacha : ELayer
{
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

	public override void OnAfterInit()
	{
		Msg.TalkHomeMemeber("layerGacha");
		if (alt)
		{
			ActionMode.NoMap.Activate();
		}
		RefreshCoin();
		InvokeRepeating("RandomAnime", 2f, 2f);
	}

	private void RandomAnime()
	{
		randomAnimes.RandomItem().DORestart();
	}

	public void OnClickGoldGacha(int num)
	{
		PlayGacha(gold: true, num);
	}

	public void OnClickSilverGacha(int num)
	{
		PlayGacha(gold: false, num);
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
			thing.ModNum(-num);
		}
		RefreshCoin();
		Dialog d = Layer.Create("DialogGacha") as Dialog;
		d.windows[0].setting.textCaption = "confirm".lang();
		d.note.AddPrefab("Media/Graphics/Image/IllustGacha");
		d.note.Space();
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
			Hoard.Item item = ELayer.player.hoard.AddRandom(num2);
			int rarity = item.Source.rarity;
			if (rarity == 4 || rarity == 5)
			{
				lose = false;
			}
			d.note.AddText(null, item.Name(1), ELayer.Colors.GetRarityColor(rarity));
		}
		d.note.Space();
		if (thing == null)
		{
			d.note.AddText("dailyGacha".lang() + "  1 > 0");
		}
		else
		{
			d.note.AddText(thing.NameSimple + "  " + (thing.Num + num) + " > " + thing.Num);
		}
		d.note.Build();
		d.windows[0].Find<Transform>("GachaAnime").SetActive(enable: true);
		d.list.AddButton(null, Lang.Get("ok"), delegate
		{
			d.Close();
		});
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
		goFree.SetActive(ELayer.player.dailyGacha);
		listCoin.Clear();
		listCoin.callbacks = new UIList.Callback<Thing, ButtonGrid>
		{
			onInstantiate = delegate(Thing a, ButtonGrid b)
			{
				b.SetCard(a);
			},
			onClick = delegate(Thing a, ButtonGrid b)
			{
				ELayer.ui.AddLayer<LayerInfo>().Set(a);
			}
		};
		listCoin.Refresh();
	}

	public void OnClickBuyCoin()
	{
		SE.Beep();
	}

	public override void OnKill()
	{
		if (alt)
		{
			ActionMode.DefaultMode.Activate();
		}
	}
}
