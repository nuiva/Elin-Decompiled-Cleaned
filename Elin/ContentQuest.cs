using System;
using UnityEngine;
using UnityEngine.UI;

public class ContentQuest : EContent
{
	public override void OnSwitchContent(int idTab)
	{
		this.list.sortMode = UIList.SortMode.ByValue;
		BaseList baseList = this.list;
		UIList.Callback<Quest, ItemGeneral> callback = new UIList.Callback<Quest, ItemGeneral>();
		callback.onInstantiate = delegate(Quest a, ItemGeneral b)
		{
			b.SetSound(null);
			string text = a.GetTitle();
			if (EClass.debug.showExtra)
			{
				text = text + "(" + a.phase.ToString() + ")";
			}
			b.button1.mainText.SetText(a.TitlePrefix + text);
			b.SetSubText(a.TextDeadline, 260, FontColor.Default, TextAnchor.MiddleRight);
			b.button2.SetActive(EClass.debug.enable && a.CanAutoAdvance);
			if (EClass.debug.enable && a.CanAutoAdvance)
			{
				b.button2.SetOnClick(delegate
				{
					if (a.task != null)
					{
						a.CompleteTask();
					}
					else if (a is QuestSequence)
					{
						a.NextPhase();
					}
					else
					{
						a.Complete();
					}
					this.list.List(false);
				});
			}
			b.button3.SetOnClick(delegate
			{
				a.track = !a.track;
				if (!WidgetQuestTracker.Instance)
				{
					EClass.player.questTracker = true;
					EClass.ui.widgets.ActivateWidget("QuestTracker");
					WidgetHotbar.RefreshButtons();
				}
				WidgetQuestTracker.Instance.Refresh();
				this.list.Select(a, false);
				this.SelectQuest(a);
				this.RefreshTrackButtons();
			});
		};
		callback.onClick = delegate(Quest a, ItemGeneral b)
		{
			this.SelectQuest(a);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (Quest o in EClass.game.quests.list)
			{
				this.list.Add(o);
			}
		};
		callback.onSort = ((Quest a, UIList.SortMode m) => -EClass.sources.quests.rows.IndexOf(a.source));
		baseList.callbacks = callback;
		this.list.List(false);
		this.SelectQuest(this.list.items[0] as Quest);
		this.RefreshTrackButtons();
	}

	public void RefreshTrackButtons()
	{
		foreach (UIList.ButtonPair buttonPair in this.list.buttons)
		{
			Quest quest = buttonPair.obj as Quest;
			(buttonPair.component as ItemGeneral).button3.icon.SetActive(quest.track);
		}
	}

	public void SelectQuest(Quest q)
	{
		this.buttonAbandon.SetActive(q.CanAbandon);
		Action <>9__1;
		this.buttonAbandon.SetOnClick(delegate
		{
			string langDetail = "dialog_abandonQuest";
			Action actionYes;
			if ((actionYes = <>9__1) == null)
			{
				actionYes = (<>9__1 = delegate()
				{
					Msg.Say("questAbandon", q.GetTitle(), null, null, null);
					q.Fail();
					this.OnSwitchContent(0);
				});
			}
			Dialog.YesNo(langDetail, actionYes, null, "yes", "no");
		});
		this.textClient.text = q.person.NameBraced;
		this.textReward.SetText(q.GetRewardText());
		string text = q.GetDetail(true);
		if (EClass.debug.showExtra && q.person != null)
		{
			text += Environment.NewLine;
			string[] array = new string[7];
			array[0] = text;
			array[1] = q.person.id;
			array[2] = "/";
			array[3] = q.person.name;
			array[4] = "/";
			int num = 5;
			Chara chara = q.person.chara;
			array[num] = ((chara != null) ? chara.ToString() : null);
			array[6] = Environment.NewLine;
			text = string.Concat(array);
			QuestDeliver questDeliver = q as QuestDeliver;
			if (questDeliver != null)
			{
				string str = text;
				string str2 = questDeliver.uidDest.ToString();
				string str3 = "/";
				Chara destChara = questDeliver.DestChara;
				text = str + str2 + str3 + ((destChara != null) ? destChara.ToString() : null);
			}
		}
		this.textDetail.SetText(text);
		this.textHours.text = q.TextDeadline;
		Text text2 = this.textZone;
		Zone clientZone = q.ClientZone;
		text2.text = (((clientZone != null) ? clientZone.Name : null) ?? "-");
		this.portrait.SetPerson(q.person);
		this.RebuildLayout(true);
	}

	public UIList list;

	public UIText textClient;

	public UIText textTitle;

	public UIText textDetail;

	public UIText textHours;

	public UIText textNote;

	public UIText textReward;

	public UIText textZone;

	public Portrait portrait;

	public UIButton buttonAbandon;
}
