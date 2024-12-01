using System.Collections.Generic;
using Newtonsoft.Json;

public class HotItemLayout : HotAction
{
	public class Layout
	{
		[JsonProperty]
		public Dictionary<string, Window.SaveData> dataWindow = new Dictionary<string, Window.SaveData>();
	}

	[JsonProperty]
	public Layout layout;

	public override string Id => "WindowLayout";

	public HotItemLayout Save()
	{
		layout = new Layout();
		layout.dataWindow = Window.dictData;
		SE.Equip();
		return this;
	}

	public override void OnShowContextMenu(UIContextMenu m)
	{
		base.OnShowContextMenu(m);
		m.AddButton("updateLayout", delegate
		{
			Save();
		});
	}

	public override void Perform()
	{
		bool isInventoryOpen = EClass.ui.IsInventoryOpen;
		bool isAbilityOpen = EClass.ui.IsAbilityOpen;
		Window.dictData = (EClass.player.dataWindow = layout.dataWindow);
		EClass.ui.CloseLayers();
		EClass.ui.layerFloat.CloseLayers();
		if (isInventoryOpen)
		{
			EClass.ui.ToggleInventory();
		}
		if (isAbilityOpen)
		{
			EClass.ui.ToggleAbility();
		}
		SE.Equip();
	}
}
