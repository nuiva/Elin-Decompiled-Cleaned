using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIFactionInfo : EMono
{
	public EloMap elomap
	{
		get
		{
			return EMono.scene.elomapActor.elomap;
		}
	}

	public EloMapActor actor
	{
		get
		{
			return EMono.scene.elomapActor;
		}
	}

	private void Awake()
	{
		if (this.imageFull)
		{
			this.imageFull.SetAlpha(0f);
		}
	}

	public void SetFaction(Faction f)
	{
		this.note.Clear();
		this.note.AddHeader("HeaderNoteFaction", f.Name, null);
		this.note.AddTopic("relation", f.relation.GetTextRelation() ?? "");
		this.note.AddTopic("type", f.TextType ?? "");
		this.note.Build();
		if (!this.imageFull)
		{
			return;
		}
		this.transImage.SetPositionX(this.pivot.position.x);
		Sprite sprite = Resources.Load<Sprite>("Media/Graphics/Image/Faction/" + f.source.id + "_avatar");
		TweenUtil.KillTween(ref this.tween, false);
		if (sprite)
		{
			if (sprite != this.imageFull.sprite)
			{
				this.imageFull.SetAlpha(0f);
			}
			this.imageFull.sprite = sprite;
			this.imageFull.SetNativeSize();
			this.tween = this.imageIn.Play(this.imageFull.transform, null, -1f, 0f);
			return;
		}
		this.tween = this.imageOut.Play(this.imageFull.transform, null, -1f, 0f);
	}

	public void SetReligion(Religion f)
	{
		this.note.Clear();
		UIItem c = this.note.AddHeader("HeaderFaction", "", f.GetSprite());
		c.Find("Name", true).SetText(f.Name);
		c.Find("Topic1", true).SetTopic("relationGod", f.relation.ToString() ?? "");
		c.Find("Topic2", true).SetTopic("type", f.source.GetText("textType", false) ?? "");
		c.Find("Topic3", true).SetTopic("avatar", f.source.textAvatar ?? "");
		string lang = "GSSleep";
		if (f.id == "harmony")
		{
			lang = "GSWithdraw";
		}
		if (f.id == "oblivion")
		{
			lang = "GSStray";
		}
		c.Find("Topic4", true).SetTopic("status", lang);
		this.note.AddText(f.source.GetDetail(), FontColor.DontChange).Hyphenate();
		this.note.Build();
		if (!this.imageFull)
		{
			return;
		}
		this.transImage.SetPositionX(this.pivot.position.x);
		Sprite sprite = Resources.Load<Sprite>("Media/Graphics/Image/Faction/" + f.source.id + "_avatar");
		TweenUtil.KillTween(ref this.tween, false);
		if (sprite)
		{
			if (sprite != this.imageFull.sprite)
			{
				this.imageFull.SetAlpha(0f);
			}
			this.imageFull.sprite = sprite;
			this.imageFull.SetNativeSize();
			this.tween = this.imageIn.Play(this.imageFull.transform, null, -1f, 0f);
			return;
		}
		this.tween = this.imageOut.Play(this.imageFull.transform, null, -1f, 0f);
	}

	public void SetZone(Zone _zone)
	{
		this.zone = _zone;
		FactionBranch branch = this.zone.branch;
		this.gx = this.zone.x;
		this.gy = this.zone.y;
		this.note.Clear();
		this.note.AddHeader("HeaderNoteFaction", this.zone.Name, null);
		this.note.AddTopic("mainFaction", this.zone.mainFaction.Name ?? "");
		this.note.AddTopic("branchLv", branch.TextLv);
		this.note.AddTopic("wealth", branch.resources.worth.value.ToFormat() ?? "");
		this.note.AddTopic("ranking", EMono.game.spatials.ranks.GetRankText(this.zone) ?? "");
		this.note.AddTopic("rank_income", "rank_income2".lang(EMono.game.spatials.ranks.GetIncome(this.zone).ToFormat(), null, null, null, null));
		this.note.Space(0, 1);
		this.note.AddHeaderTopic("landfeat".lang(), null);
		List<Element> list = _zone.ListLandFeats();
		for (int i = 0; i < list.Count; i++)
		{
			this.note.AddText(list[i].Name + (((i == 1 && branch.lv < 4) || (i == 2 && branch.lv < 7)) ? "landfeat_locked".lang() : ""), FontColor.DontChange);
		}
		this.note.Space(0, 1);
		this.note.AddHeaderTopic("listRoamers".lang(), null);
		int num = 0;
		foreach (Chara chara in EMono.game.cards.globalCharas.Values)
		{
			if (chara.homeBranch == branch)
			{
				this.note.AddText(chara.Name, FontColor.DontChange);
				num++;
				if (num > 5)
				{
					break;
				}
			}
		}
		if (num == 0)
		{
			this.note.AddText("????????", FontColor.DontChange);
		}
		this.note.Space(0, 1);
		this.note.AddHeaderTopic("listPolicies".lang(), null);
		foreach (Element element in branch.elements.dict.Values)
		{
			if (element.source.category == "policy")
			{
				this.note.AddText(element.Name, FontColor.DontChange);
			}
		}
		this.note.Build();
	}

	public void Clear()
	{
		this.note.Clear();
		this.note.Build();
	}

	public UINote note;

	public Image imageFull;

	public Anime imageIn;

	public Anime imageOut;

	public Tween tween;

	public Transform transImage;

	public Transform pivot;

	public UIButton buttonBuy;

	public UIButton buttonVisit;

	public UIButton buttonExplore;

	public int gx;

	public int gy;

	public Zone zone;

	public LayoutGroup layoutButtons;
}
