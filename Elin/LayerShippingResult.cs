using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LayerShippingResult : ELayer
{
	private new void Awake()
	{
		Rand.SetSeed(ELayer.game.seed + ELayer.player.stats.days);
		int num = ELayer.rnd(this.spriteBG.Length);
		if (num == 1 && ELayer.game.cards.globalCharas.Find("corgon") == null)
		{
			num = 0;
		}
		if (num == 2 && ELayer.game.cards.globalCharas.Find("loytel") == null)
		{
			num = 0;
		}
		this.imageBG.sprite = this.spriteBG[num];
		Rand.SetSeed(-1);
		base.Awake();
	}

	public void Show()
	{
		this.Show(ELayer.player.shippingResults.LastItem<ShippingResult>() ?? new ShippingResult());
		SE.Play("money_shipping");
		base.InvokeRepeating("UpdateRepeating", 0f, this.duration / (float)this.maxUpdate);
	}

	public void UpdateRepeating()
	{
		if (!this.showEffect)
		{
			return;
		}
		this.Refresh();
	}

	public void Refresh()
	{
		if (this.update == 0)
		{
			this.lastBonus = 0;
		}
		if (!this.showEffect)
		{
			if (this.update != this.maxUpdate)
			{
				foreach (DOTweenAnimation dotweenAnimation in base.GetComponentsInChildren<DOTweenAnimation>())
				{
					if (dotweenAnimation.tween != null)
					{
						dotweenAnimation.tween.Kill(true);
					}
				}
			}
			this.update = this.maxUpdate;
		}
		FactionBranch branch = (ELayer.game.spatials.Find(this.result.uidZone) ?? ELayer.pc.homeZone).branch;
		float num = (float)this.update / (float)this.maxUpdate;
		int num2 = this.result.GetIncome();
		num2 = (int)((float)num2 * num);
		int a = this.result.total + num2;
		this.textIncome.text = Lang._currency(num2, true, 14);
		this.textIncomeTotal.text = Lang._currency(a, true, 14);
		int shippingBonus = ELayer.player.stats.GetShippingBonus(this.result.total);
		int shippingBonus2 = ELayer.player.stats.GetShippingBonus(a);
		int lastShippingExp = ELayer.player.stats.lastShippingExp;
		int lastShippingExpMax = ELayer.player.stats.lastShippingExpMax;
		int hearthLv = this.result.hearthLv;
		int num3 = (int)((float)this.result.hearthExpGained * num);
		int nextExp = branch.GetNextExp(hearthLv);
		int num4 = this.result.hearthExp + num3;
		this.gaugeGold.rectTransform.sizeDelta = new Vector2((float)Mathf.Min(300, 300 * lastShippingExp / lastShippingExpMax), 50f);
		this.gaugeHearth.rectTransform.sizeDelta = new Vector2((float)Mathf.Min(300, 300 * num4 / nextExp), 50f);
		this.gaugeDebt.rectTransform.sizeDelta = new Vector2(Mathf.Min(300f, 300f * ((float)this.result.debt / 20000000f)), 50f);
		this.textGold.text = "shipping_nextGold".lang(lastShippingExp.ToFormat(), lastShippingExpMax.ToFormat(), null, null, null);
		int num5 = shippingBonus2 - shippingBonus;
		this.textGold2.text = ((num5 == 0) ? "" : ("+" + num5.ToString()));
		this.textHearth.text = "shipping_hearth".lang(hearthLv.ToString() ?? "", (100f * (float)num4 / (float)nextExp).ToString("F1"), null, null, null);
		float num6 = 100f * (float)num3 / (float)nextExp;
		this.textHearth2.text = ((num6 == 0f) ? "" : ("+" + num6.ToString("F1") + "%"));
		this.textDebt.text = (ELayer.game.quests.IsStarted<QuestDebt>() ? (Lang._currency(this.result.debt, true, 14) ?? "") : "???");
		this.gaugeDebt.SetActive(ELayer.game.quests.IsStarted<QuestDebt>());
		if (shippingBonus2 - shippingBonus != this.lastBonus)
		{
			this.lastBonus = shippingBonus2 - shippingBonus;
			if (this.showEffect)
			{
				SE.Play("ore_drop");
			}
		}
		this.update++;
		if (this.showEffect && this.update > this.maxUpdate)
		{
			SE.Play("regi");
			this.showEffect = false;
		}
	}

	public void Show(ShippingResult _result)
	{
		this.result = _result;
		Zone zone = ELayer.game.spatials.Find(this.result.uidZone) ?? ELayer.pc.homeZone;
		FactionBranch branch = zone.branch;
		BaseList baseList = this.list;
		UIList.Callback<ShippingResult.Item, UIItem> callback = new UIList.Callback<ShippingResult.Item, UIItem>();
		callback.onInstantiate = delegate(ShippingResult.Item a, UIItem b)
		{
			b.text1.SetText(a.text);
			b.text2.SetText(a.income.ToFormat());
		};
		baseList.callbacks = callback;
		this.list.Clear();
		foreach (ShippingResult.Item o in this.result.items)
		{
			this.list.Add(o);
		}
		this.list.Refresh(false);
		Date date = Date.ToDate(this.result.rawDate);
		this.textTitle.text = string.Concat(new string[]
		{
			date.month.ToString(),
			"/",
			date.day.ToString(),
			" ",
			date.year.ToString()
		});
		this.textHome.text = "shipping_home".lang(zone.Name, null, null, null, null);
		this.Refresh();
		int count = ELayer.player.shippingResults.Count;
		int index = ELayer.player.shippingResults.IndexOf(this.result);
		this.buttonNext.SetActive(index != -1 && count > index + 1);
		this.buttonPrev.SetActive(index > 0);
		this.buttonNext.SetOnClick(delegate
		{
			this.showEffect = false;
			this.Show(ELayer.player.shippingResults[index + 1]);
		});
		this.buttonPrev.SetOnClick(delegate
		{
			this.showEffect = false;
			this.Show(ELayer.player.shippingResults[index - 1]);
		});
		this.RebuildLayout(true);
	}

	public UIList list;

	public UIButton buttonNext;

	public UIButton buttonPrev;

	public Image gaugeGold;

	public Image gaugeHearth;

	public Image gaugeDebt;

	public Image imageBG;

	public UIText textGold;

	public UIText textGold2;

	public UIText textHearth;

	public UIText textHearth2;

	public UIText textHome;

	public UIText textIncome;

	public UIText textIncomeTotal;

	public UIText textTitle;

	public UIText textDebt;

	public bool showEffect = true;

	public float duration;

	public int update;

	public int maxUpdate;

	public ShippingResult result;

	public Sprite[] spriteBG;

	private int lastBonus;
}
