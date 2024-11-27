using System;
using System.Collections.Generic;
using UnityEngine;

public class ContentCodex : EContent
{
	public override void OnSwitchContent(int idTab)
	{
		if (this.list.isBuilt)
		{
			if (this.currentCodex != null)
			{
				this.list.Select(this.currentCodex, false);
				this.RefreshInfo();
			}
			return;
		}
		this.RefreshList();
		this.buttonToggleCollect.SetToggle(EClass.game.config.autoCollectCard, delegate(bool a)
		{
			EClass.game.config.autoCollectCard = a;
		});
	}

	public void RefreshList()
	{
		this.currentCodex = null;
		this.list.Clear();
		BaseList baseList = this.list;
		UIList.Callback<CodexCreature, ButtonGrid> callback = new UIList.Callback<CodexCreature, ButtonGrid>();
		callback.onClick = delegate(CodexCreature a, ButtonGrid b)
		{
			this.currentCodex = a;
			this.RefreshInfo();
			this.list.Select(a, false);
		};
		callback.onRedraw = delegate(CodexCreature a, ButtonGrid b, int i)
		{
			b.refObj = a.source;
			b.mainText.SetText(a.Name);
			b.subText.text = (a.numCard.ToString() ?? "");
			b.subText2.SetText(a.source._id.ToString() + ".");
			if (EClass.core.uiScale < 1.2f)
			{
				b.icon.transform.localScale = new Vector3(1f / EClass.core.uiScale, 1f / EClass.core.uiScale, 1f);
			}
			a.SetImage(b.icon, true);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			List<CodexCreature> list = new List<CodexCreature>();
			foreach (KeyValuePair<string, CodexCreature> keyValuePair in EClass.player.codex.creatures)
			{
				if (keyValuePair.Value.numCard > 0 && EClass.sources.charas.map.ContainsKey(keyValuePair.Value.id))
				{
					list.Add(keyValuePair.Value);
				}
			}
			list.Sort((CodexCreature a, CodexCreature b) => a.source._id - b.source._id);
			foreach (CodexCreature o in list)
			{
				this.list.Add(o);
			}
		};
		baseList.callbacks = callback;
		this.list.List();
		bool flag = this.list.objects.Count > 0;
		if (flag)
		{
			this.list.dsv.scrollByItemIndex(0);
			this.list.Select(this.list.objects[0], true);
			this.currentCodex = (this.list.objects[0] as CodexCreature);
		}
		this.transInfo.SetActive(flag);
		this.buttonTake.SetActive(flag);
	}

	public void RefreshInfo()
	{
		CodexCreature codexCreature = this.currentCodex;
		if (codexCreature == null)
		{
			return;
		}
		UINote uinote = this.note;
		SourceChara.Row source = codexCreature.source;
		uinote.Clear();
		UIItem uiitem = uinote.AddHeader("HeaderCodex", codexCreature.Name.Replace("『", Environment.NewLine + "『"), null);
		codexCreature.SetImage(uiitem.image1, true);
		uiitem.text2.text = string.Concat(new string[]
		{
			"Lv.",
			source.LV.ToString(),
			Environment.NewLine,
			EClass.sources.races.map[source.race].GetName().ToTitleCase(true),
			Environment.NewLine,
			EClass.sources.jobs.map[source.job].GetName().ToTitleCase(true)
		});
		uiitem.image1.rectTransform.pivot = this.pivot;
		uiitem.image1.rectTransform.localPosition = this.localPos;
		uiitem.image1.rectTransform.localScale = new Vector3(-uiitem.image1.transform.localScale.x, uiitem.image1.transform.localScale.y, 1f);
		uinote.Build();
		this.textOwn.text = (codexCreature.numCard.ToString() ?? "");
		this.textValue.text = (codexCreature.source.value.ToString() ?? "");
		this.textKills.text = (codexCreature.kills.ToString() ?? "");
		this.textWeakspot.SetText((codexCreature.weakspot == 0) ? "undiscovered".lang() : "discovered".lang(codexCreature.weakspot.ToString() ?? "", null, null, null, null), (codexCreature.weakspot == 0) ? FontColor.Default : FontColor.Good);
		this.textSpawns.text = (codexCreature.spawns.ToString() ?? "");
		this.textBonus.text = codexCreature.GetTextBonus();
		this.currentCodex = codexCreature;
	}

	public void OnClickAddCards()
	{
		List<Thing> list = EClass.pc.things.List((Thing c) => c.id == "figure3", false);
		int num = 0;
		if (list.Count > 0)
		{
			foreach (Thing thing in list)
			{
				EClass.player.codex.AddCard(thing.c_idRefCard, thing.Num);
				num += thing.Num;
				thing.Destroy();
				this.RefreshList();
			}
			SE.Play("use_card");
			Msg.Say("addedCards", num, num.ToString() ?? "", null);
		}
		else
		{
			SE.BeepSmall();
			Msg.Say("noCard");
		}
		this.RefreshInfo();
	}

	public static void Collect(Thing t)
	{
		EClass.player.codex.AddCard(t.c_idRefCard, t.Num);
		SE.Play("use_card");
		Msg.Say("addedCards", t.Num, t.Num.ToString() ?? "", null);
		t.Destroy();
	}

	public void OnClickGetCard()
	{
		CodexCreature codexCreature = this.currentCodex;
		int numCard = codexCreature.numCard;
		codexCreature.numCard = numCard - 1;
		Thing thing = ThingGen.Create("figure3", -1, -1);
		thing.MakeFigureFrom(this.currentCodex.id);
		EClass.pc.Pick(thing, true, true);
		if (this.currentCodex.numCard == 0)
		{
			this.RefreshList();
			return;
		}
		this.list.Redraw();
		this.list.Select(this.currentCodex, false);
		this.RefreshInfo();
	}

	public UIDynamicList list;

	public UIText textName;

	public UIText textOwn;

	public UIText textValue;

	public UIText textKills;

	public UIText textWeakspot;

	public UIText textSpawns;

	public UIText textBonus;

	public UINote note;

	public CodexCreature currentCodex;

	public Transform transInfo;

	public UIButton buttonTake;

	public UIButton buttonAddCards;

	public UIButton buttonToggleCollect;

	public Vector2 pivot;

	public Vector2 localPos;
}
