using System;
using Newtonsoft.Json;

public class HotItemWidget : HotItem
{
	public override string Name
	{
		get
		{
			return ("Widget" + this.id).lang();
		}
	}

	public override string pathSprite
	{
		get
		{
			return "icon_" + this.id + ((this.id == "QuestTracker" && !EClass.player.questTracker) ? "_inactive" : "");
		}
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		if (this.id == "QuestTracker")
		{
			if (EClass.player.questTracker)
			{
				EClass.player.questTracker = false;
				SE.Tab();
				EClass.ui.widgets.DeactivateWidget("QuestTracker");
			}
			else
			{
				EClass.player.questTracker = true;
				WidgetQuestTracker.TryShow();
			}
			SE.Tab();
			b.RefreshItem();
			return;
		}
		Widget widget = EClass.ui.widgets.Toggle(this.id);
		if (widget == null)
		{
			return;
		}
		widget.SoundActivate();
	}

	[JsonProperty]
	public string id;
}
