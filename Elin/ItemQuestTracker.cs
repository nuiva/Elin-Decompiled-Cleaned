using System;
using UnityEngine;

public class ItemQuestTracker : EMono
{
	public void Refresh()
	{
		if (!this.quest.track || !EMono.game.quests.list.Contains(this.quest))
		{
			this.Kill();
			return;
		}
		this.sb.Clear();
		if (this.quest.deadline > 0 && !this.quest.UseInstanceZone)
		{
			this.sb.Append("- " + "days1".lang() + this.quest.TextDeadline);
			this.sb.Append(Environment.NewLine);
		}
		string value = this.quest.GetTrackerText().TrimEnd(Environment.NewLine.ToCharArray());
		this.sb.Append(value);
		bool enable = false;
		if (this.quest is QuestDeliver)
		{
			QuestDeliver questDeliver = this.quest as QuestDeliver;
			if (!questDeliver.IsDeliver || EMono._zone == questDeliver.DestZone)
			{
				ItemQuestTracker.<>c__DisplayClass7_0 CS$<>8__locals1 = new ItemQuestTracker.<>c__DisplayClass7_0();
				ItemQuestTracker.<>c__DisplayClass7_0 CS$<>8__locals2 = CS$<>8__locals1;
				Map map = EMono._map;
				int uid;
				if (!questDeliver.IsDeliver)
				{
					Chara chara = questDeliver.person.chara;
					uid = ((chara != null) ? chara.uid : 0);
				}
				else
				{
					uid = questDeliver.uidTarget;
				}
				CS$<>8__locals2.tg = map.FindChara(uid);
				if (CS$<>8__locals1.tg != null)
				{
					enable = true;
					this.buttonGoto.SetOnClick(delegate
					{
						if (!EMono.pc.HasNoGoal)
						{
							SE.BeepSmall();
							return;
						}
						EMono.pc.SetAIImmediate(new AI_Goto(CS$<>8__locals1.tg, 1, false, false));
					});
				}
			}
		}
		this.buttonGoto.SetActive(enable);
		if (this.sb.Equals(this.lastSb))
		{
			return;
		}
		this.SetActive(true);
		this.textTitle.SetText(this.quest.GetTitle().TagColor(this.colorTitle));
		this.text.SetText(this.sb.ToString());
		this.lastSb.Set(this.sb);
		this.RebuildLayout(false);
	}

	public void OnClickClose()
	{
		if (EMono.ui.GetLayer<LayerJournal>(false))
		{
			return;
		}
		this.quest.track = false;
		this.Kill();
		SE.Trash();
		if (this.quest is QuestTrackCraft)
		{
			EMono.game.quests.Remove(this.quest);
			if (LayerCraft.Instance)
			{
				LayerCraft.Instance.RefreshTrackButton();
			}
		}
	}

	public void Kill()
	{
		if (!WidgetQuestTracker.Instance)
		{
			return;
		}
		WidgetQuestTracker.Instance.items.Remove(this);
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	private FastString sb = new FastString(32);

	private FastString lastSb = new FastString(32);

	public Color colorTitle;

	public Quest quest;

	public UIText text;

	public UIText textTitle;

	public UIButton buttonGoto;
}
