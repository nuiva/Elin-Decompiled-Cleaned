using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class WidgetQuestTracker : Widget
{
	private void OnEnable()
	{
		base.InvokeRepeating("Refresh", 0.5f, 0.5f);
	}

	private void OnDisable()
	{
		base.CancelInvoke();
	}

	public override void OnActivate()
	{
		WidgetQuestTracker.Instance = this;
		this.Refresh();
	}

	public void Refresh()
	{
		if (EMono.game.isLoading || !EMono._zone.isStarted || LayerDrama.Instance)
		{
			return;
		}
		foreach (Quest quest in EMono.game.quests.list)
		{
			if (quest.track)
			{
				ItemQuestTracker itemQuestTracker = null;
				foreach (ItemQuestTracker itemQuestTracker2 in this.items)
				{
					if (itemQuestTracker2.quest == quest)
					{
						itemQuestTracker = itemQuestTracker2;
						break;
					}
				}
				if (!(itemQuestTracker != null))
				{
					itemQuestTracker = Util.Instantiate<ItemQuestTracker>(this.mold, this.layout);
					itemQuestTracker.SetActive(false);
					itemQuestTracker.quest = quest;
					this.items.Add(itemQuestTracker);
				}
			}
		}
		this.items.ForeachReverse(delegate(ItemQuestTracker i)
		{
			i.Refresh();
		});
		this.RebuildLayout(false);
		if (this.items.Count == 0)
		{
			EMono.ui.widgets.DeactivateWidget(this);
			return;
		}
		EMono.player.questTracker = true;
	}

	public static void Show()
	{
		if (!EMono.ui.widgets.GetWidget("QuestTracker"))
		{
			EMono.ui.widgets.Toggle("QuestTracker");
		}
	}

	public static bool TryShow()
	{
		bool flag = false;
		using (List<Quest>.Enumerator enumerator = EMono.game.quests.list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.track)
				{
					flag = true;
				}
			}
		}
		if (flag || EMono.ui.widgets.GetWidget("QuestTracker"))
		{
			Widget widget = EMono.ui.widgets.Toggle("QuestTracker");
			if (widget != null)
			{
				widget.SoundActivate();
			}
			return false;
		}
		if (!EMono.ui.GetLayer<LayerJournal>(false))
		{
			EMono.ui.ToggleLayer<LayerJournal>(null).SwitchContent(0, 0);
		}
		return true;
	}

	public static WidgetQuestTracker Instance;

	public List<ItemQuestTracker> items;

	public ItemQuestTracker mold;

	public LayoutGroup layout;
}
