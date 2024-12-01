using Newtonsoft.Json;

public class HotItemWidget : HotItem
{
	[JsonProperty]
	public string id;

	public override string Name => ("Widget" + id).lang();

	public override string pathSprite => "icon_" + id + ((id == "QuestTracker" && !EClass.player.questTracker) ? "_inactive" : "");

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		if (id == "QuestTracker")
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
		}
		else
		{
			EClass.ui.widgets.Toggle(id)?.SoundActivate();
		}
	}
}
