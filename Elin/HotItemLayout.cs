using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class HotItemLayout : HotAction
{
	public override string Id
	{
		get
		{
			return "WindowLayout";
		}
	}

	public HotItemLayout Save()
	{
		this.layout = new HotItemLayout.Layout();
		this.layout.dataWindow = Window.dictData;
		SE.Equip();
		return this;
	}

	public override void OnShowContextMenu(UIContextMenu m)
	{
		base.OnShowContextMenu(m);
		m.AddButton("updateLayout", delegate()
		{
			this.Save();
		}, true);
	}

	public override void Perform()
	{
		bool isInventoryOpen = EClass.ui.IsInventoryOpen;
		bool isAbilityOpen = EClass.ui.IsAbilityOpen;
		Window.dictData = (EClass.player.dataWindow = this.layout.dataWindow);
		EClass.ui.CloseLayers();
		EClass.ui.layerFloat.CloseLayers();
		if (isInventoryOpen)
		{
			EClass.ui.ToggleInventory(false);
		}
		if (isAbilityOpen)
		{
			EClass.ui.ToggleAbility(false);
		}
		SE.Equip();
	}

	[JsonProperty]
	public HotItemLayout.Layout layout;

	public class Layout
	{
		[JsonProperty]
		public Dictionary<string, Window.SaveData> dataWindow = new Dictionary<string, Window.SaveData>();
	}
}
