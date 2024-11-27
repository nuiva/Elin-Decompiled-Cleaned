using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DramaActor : EMono
{
	public DialogDrama dialog
	{
		get
		{
			return this.sequence.dialog;
		}
	}

	public void Init(DramaSequence _sequence, string _id, Person _owner)
	{
		this.sequence = _sequence;
		this.id = _id;
		this.owner = _owner;
		base.gameObject.SetActive(false);
	}

	public void Talk(string text, List<DramaChoice> choices, bool center, bool unknown)
	{
		Chara chara = this.owner.chara;
		this.dialog.gameObject.SetActive(true);
		this.dialog.textName.text = (unknown ? "???" : this.owner.GetDramaTitle());
		if (this.dialog.portrait)
		{
			this.dialog.portrait.enableFull = this.sequence.fullPortrait;
			this.dialog.portrait.SetPerson(this.owner);
		}
		text = this.ConvertAdv(text);
		text = GameLang.ConvertDrama(text, this.owner.chara);
		if (!this.owner.HumanSpeak)
		{
			if (!text.StartsWith("("))
			{
				text = "(" + text + ")";
			}
			text = text.Replace("。)", ")");
		}
		if (chara != null)
		{
			string a = chara.id;
			if (!(a == "adv_gaki"))
			{
				if (a == "corgon")
				{
					BackerContent.GakiConvert(ref text, "mokyu");
				}
			}
			else
			{
				BackerContent.GakiConvert(ref text, "zako");
			}
		}
		if (this.dialog.goAffinity)
		{
			this.dialog.layoutInterest.SetActive(chara != null);
			this.dialog.textNoInterest.SetActive(chara == null);
			bool flag = chara != null && chara.trait.ShowAdvRank;
			if (this.dialog.transFav)
			{
				this.dialog.transFav.SetActive(chara != null && chara.knowFav);
			}
			if (this.dialog.transRank)
			{
				this.dialog.transRank.SetActive(flag);
				if (flag)
				{
					this.dialog.textRank.text = "factionRank2".lang() + " " + chara.trait.GetAdvRankText();
				}
			}
			if (chara != null)
			{
				this.dialog.textAffinity.text = ((chara.c_bossType != BossType.none) ? " - " : (chara.affinity.Name + "(" + chara._affinity.ToString() + ")"));
				this.dialog.layoutInterest.DestroyChildren(false, true);
				int num = chara.interest / 10 + 1;
				if (chara.interest <= 0)
				{
					num = 0;
				}
				if (num > 10)
				{
					num = 10;
				}
				for (int i = 0; i < num; i++)
				{
					Util.Instantiate<Transform>(this.dialog.moldInterest, this.dialog.layoutInterest);
				}
				this.dialog.layoutInterest.RebuildLayout(false);
				this.dialog.textNoInterest.SetActive(num <= 0);
				if (this.dialog.textFav && chara.knowFav)
				{
					this.dialog.textFav.text = chara.GetFavCat().GetName().ToTitleCase(false) + Environment.NewLine + chara.GetFavFood().GetName().ToTitleCase(false);
				}
			}
			else
			{
				this.dialog.textAffinity.text = "???";
				this.dialog.textNoInterest.text = "???";
			}
		}
		text = text.Replace("((", "(").Replace("))", ")").Replace("（（", "（").Replace("））", "）");
		if (this.dialog.glitch)
		{
			this.dialog.glitch.enabled = DramaActor.useGlitch;
			DramaActor.useGlitch = false;
		}
		this.dialog.SetText(text.ToTitleCase(false), center);
		this.dialog.ClearChoice();
		this.choiceIdx = 1;
		for (int j = 0; j < choices.Count; j++)
		{
			this.SetChoice(choices[j], j);
		}
		this.dialog.transChoices.RebuildLayout(true);
		this.dialog.iconNext.SetActive(this.choiceIdx == 1);
		UIButton.TryHihlight();
		EMono.core.actionsNextFrame.Add(new Action(UIButton.TryHihlight));
		EMono.core.actionsNextFrame.Add(delegate
		{
			EMono.core.actionsNextFrame.Add(new Action(UIButton.TryHihlight));
		});
	}

	private void SetChoice(DramaChoice item, int index)
	{
		item.index = index;
		if (!item.IF.IsEmpty() && !DramaOutcome.If(item, this.owner.chara))
		{
			return;
		}
		if (item.activeCondition != null && !item.activeCondition())
		{
			return;
		}
		DramaOutcome.idJump = null;
		string text = item.text.TrimEnd(Environment.NewLine.ToCharArray());
		text = this.ConvertAdv(text);
		text = GameLang.ConvertDrama(text, this.owner.chara);
		if (item.check != null && EMono.debug.logDice)
		{
			text = text + " (" + item.check.GetText(EMono.pc, this.owner.chara, true) + ")";
		}
		Check check = null;
		string idJump = item.idJump;
		string idAction = item.idAction;
		object[] PARAMS = null;
		int? affinity = null;
		if (!item.CHECK.IsEmpty())
		{
			string[] array = item.CHECK.Split('/', StringSplitOptions.None);
			string a = array[0];
			if (!(a == "check"))
			{
				if (!(a == "affinity"))
				{
				}
			}
			else
			{
				check = Check.Get(array[1], 1f);
			}
		}
		UIButton uibutton = this.dialog.AddChoice(item, text, delegate
		{
			this.gameObject.SetActive(false);
			DramaChoice.lastChoice = item;
			Check check = check;
			if (affinity != null)
			{
				bool flag = true;
				if (flag)
				{
					idJump = idJump.Split('/', StringSplitOptions.None)[0];
				}
				else
				{
					idJump = idJump.Split('/', StringSplitOptions.None)[1];
				}
				PARAMS = new object[]
				{
					flag
				};
			}
			if (!idAction.IsEmpty())
			{
				typeof(DramaOutcome).GetMethod(this.sequence.id + "_" + idAction).Invoke(this.sequence.manager.outcome, PARAMS);
				if (DramaOutcome.idJump != null)
				{
					this.sequence.Play(DramaOutcome.idJump);
					return;
				}
			}
			if (item.onClick != null)
			{
				item.onClick();
			}
			this.sequence.tempEvents.Clear();
			if (item.onJump != null)
			{
				item.onJump();
				return;
			}
			if (idJump.IsEmpty())
			{
				this.sequence.PlayNext();
				return;
			}
			this.sequence.Play(idJump);
		}, true);
		if (uibutton.subText)
		{
			uibutton.subText.SetText(this.choiceIdx.ToString() + ".");
		}
		if (!item.sound)
		{
			uibutton.soundClick = null;
		}
		if (item.onTooltip != null)
		{
			uibutton.SetTooltip(item.onTooltip, true);
		}
		if (item.forceHighlight)
		{
			uibutton.DoHighlightTransition(false);
			item.forceHighlight = false;
		}
		this.choiceIdx++;
	}

	public string ConvertAdv(string text)
	{
		StringBuilder stringBuilder = new StringBuilder(text);
		Religion currentReligion = LayerDrama.currentReligion;
		if (currentReligion != null)
		{
			stringBuilder.Replace("#god_name", currentReligion.Name);
			stringBuilder.Replace("#god_desc", currentReligion.source.GetDetail());
			stringBuilder.Replace("#god_benefit", currentReligion.GetTextBenefit());
			stringBuilder.Replace("#godtalk_worship", currentReligion.GetGodTalk("worship"));
		}
		return stringBuilder.ToString();
	}

	public static bool useGlitch;

	public string id;

	public Person owner;

	public DramaSequence sequence;

	private int choiceIdx;
}
