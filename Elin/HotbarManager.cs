using System;
using Newtonsoft.Json;

public class HotbarManager : EClass
{
	public void ResetHotbar(int id)
	{
		Hotbar hotbar = this.bars[id];
		WidgetHotbar widgetHotbar = ((hotbar != null) ? hotbar.actor : null) ?? null;
		Hotbar hotbar2 = null;
		switch (id)
		{
		case 0:
			hotbar2 = this.SetHobar(0, 4, null);
			break;
		case 1:
			hotbar2 = this.SetHobar(1, 4, null);
			hotbar2.SetItem(new HotItemContext
			{
				id = "system"
			}, 0, -1, false);
			hotbar2.SetItem(new HotItemToggleLog(), 1, -1, false);
			hotbar2.SetItem(new HotItemContext
			{
				id = "mapTool"
			}, 2, -1, false);
			hotbar2.SetItem(new HotItemToggleWidget(), 3, -1, false);
			break;
		case 2:
			hotbar2 = this.SetHobar(2, 7, null);
			hotbar2.SetItem(new HotItemLayer
			{
				id = "stash"
			}, -1, -1, false);
			hotbar2.SetItem(new HotItemLayer
			{
				id = "LayerAbility"
			}, -1, -1, false);
			hotbar2.SetItem(new HotItemLayer
			{
				id = "LayerChara"
			}, -1, -1, false);
			hotbar2.SetItem(new HotItemLayer
			{
				id = "LayerJournal"
			}, -1, -1, false);
			if (EClass.core.IsGameStarted && EClass._zone.CanEnterBuildModeAnywhere)
			{
				hotbar2.SetItem(new HotItemActionMode
				{
					id = "Inspect"
				}, -1, -1, false);
			}
			if (EClass.core.IsGameStarted && EClass._zone.IsRegion)
			{
				hotbar2.SetItem(new HotItemLayer
				{
					id = "LayerTravel"
				}, -1, -1, false);
			}
			break;
		case 3:
		{
			hotbar2 = this.SetHobar(3, 12, null);
			if (!EClass.core.IsGameStarted)
			{
				return;
			}
			bool flag = EClass.debug.godBuild || EClass.debug.ignoreBuildRule;
			if (flag || EClass.Branch != null)
			{
				if (flag || EClass.Branch.elements.Has(4006))
				{
					hotbar2.SetItem(new HotItemActionMode
					{
						id = "Inspect"
					}, -1, -1, false).always = true;
				}
				if (flag || EClass.Branch.elements.Has(4000))
				{
					hotbar2.SetItem(new HotItemActionMode
					{
						id = "Cut"
					}, -1, -1, false);
				}
				if (flag || EClass.Branch.elements.Has(4001))
				{
					hotbar2.SetItem(new HotItemActionMode
					{
						id = "Mine"
					}, -1, -1, false);
				}
				if (flag || EClass.Branch.elements.Has(4001))
				{
					hotbar2.SetItem(new HotItemActionMode
					{
						id = "Dig"
					}, -1, -1, false);
				}
				if (flag || EClass.Branch.elements.Has(4002))
				{
					hotbar2.SetItem(new HotItemActionMode
					{
						id = "Terrain"
					}, -1, -1, false);
				}
				if (flag)
				{
					hotbar2.SetItem(new HotItemActionMode
					{
						id = "Copy"
					}, -1, -1, false);
				}
				if (flag)
				{
					hotbar2.SetItem(new HotItemActionMode
					{
						id = "Populate"
					}, -1, -1, false);
				}
				if (flag || EClass.Branch.elements.Has(4004))
				{
					hotbar2.SetItem(new HotItemActionMode
					{
						id = "Deconstruct"
					}, -1, -1, false);
				}
				hotbar2.SetItem(new HotItemActionMode
				{
					id = "ExitBuild"
				}, -1, -1, false);
			}
			break;
		}
		case 4:
			hotbar2 = this.SetHobar(4, 9, null);
			if (!EClass.core.IsGameStarted)
			{
				return;
			}
			if (EClass.debug.godBuild || EClass.Branch != null)
			{
				hotbar2.SetItem(new HotItemActionMode
				{
					id = "Cinema"
				}, -1, -1, false);
				hotbar2.SetItem(new HotItemActionMode
				{
					id = "FlagCell"
				}, -1, -1, false);
				if (EClass.debug.godBuild || EClass.Branch.elements.Has(4006))
				{
					hotbar2.SetItem(new HotItemActionMode
					{
						id = "Visibility"
					}, -1, -1, false);
				}
				if (EClass.debug.godBuild)
				{
					hotbar2.SetItem(new HotItemActionMode
					{
						id = "EditMarker"
					}, -1, -1, false);
				}
				if (EClass.debug.godBuild || EClass.Branch.elements.Has(4005))
				{
					hotbar2.SetItem(new HotItemActionMode
					{
						id = "Picker"
					}, -1, -1, false);
				}
				if (EClass.debug.godBuild)
				{
					hotbar2.SetItem(new HotItemContext
					{
						id = "mapTool",
						autoExpand = false
					}, -1, -1, false);
				}
			}
			break;
		case 5:
			hotbar2 = this.SetHobar(5, 9, null);
			hotbar2.SetItem(new HotItemLayer
			{
				id = "LayerHelp"
			}, 0, -1, false);
			hotbar2.SetItem(new HotItemWidget
			{
				id = "Codex"
			}, 1, -1, false);
			hotbar2.SetItem(new HotItemWidget
			{
				id = "Memo"
			}, 2, -1, false);
			hotbar2.SetItem(new HotItemWidget
			{
				id = "QuestTracker"
			}, 3, -1, false);
			hotbar2.SetItem(new HotItemActionSleep(), 4, -1, false);
			break;
		case 6:
			hotbar2 = this.SetHobar(6, 9, null);
			break;
		case 7:
			hotbar2 = this.SetHobar(7, 7, null);
			hotbar2.SetItem(new HotItemToggle
			{
				type = HotItemToggle.Type.SnapFreepos
			}, -1, -1, false);
			hotbar2.SetItem(new HotItemToggle
			{
				type = HotItemToggle.Type.ToggleRoof
			}, -1, -1, false);
			hotbar2.SetItem(new HotItemToggle
			{
				type = HotItemToggle.Type.ToggleSlope
			}, -1, -1, false);
			hotbar2.SetItem(new HotItemToggle
			{
				type = HotItemToggle.Type.ToggleBuildLight
			}, -1, -1, false);
			break;
		}
		hotbar2.actor = widgetHotbar;
		if (widgetHotbar)
		{
			widgetHotbar.Rebuild();
		}
	}

	public Hotbar SetHobar(int id, int slots, WidgetHotbar actor = null)
	{
		Hotbar hotbar = new Hotbar();
		hotbar.itemsPerPage = slots;
		this.bars[id] = hotbar;
		hotbar.AddPage();
		hotbar.AddPage();
		hotbar.id = id;
		return hotbar;
	}

	public void OnCreateGame()
	{
		this.ResetHotbar(0);
		this.ResetHotbar(1);
		this.ResetHotbar(2);
		this.ResetHotbar(3);
		this.ResetHotbar(4);
		this.ResetHotbar(5);
		this.ResetHotbar(6);
		this.ResetHotbar(7);
	}

	[JsonProperty]
	public Hotbar[] bars = new Hotbar[8];
}
