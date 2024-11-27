using System;
using Newtonsoft.Json;

public class HotItemLayer : HotItem
{
	public override string Name
	{
		get
		{
			return this.id.lang();
		}
	}

	public override string pathSprite
	{
		get
		{
			return "icon_" + this.id;
		}
	}

	public override void OnSetItem(UIButton b)
	{
		HotItemLayer.<>c__DisplayClass8_0 CS$<>8__locals1 = new HotItemLayer.<>c__DisplayClass8_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.b = b;
		if (!EClass.debug.enable)
		{
			CS$<>8__locals1.<OnSetItem>g__AddHighlight|0("LayerHelp", () => EClass.player.flags.helpHighlightDisabled, ref HotItemLayer.highlightHelp);
			CS$<>8__locals1.<OnSetItem>g__AddHighlight|0("stash", () => EClass.player.flags.backpackHighlightDisabled, ref HotItemLayer.highlightBackpack);
			CS$<>8__locals1.<OnSetItem>g__AddHighlight|0("LayerAbility", () => EClass.player.flags.abilityHighlightDisabled, ref HotItemLayer.highlightAbility);
		}
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		string text = this.id;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 2200499586U)
		{
			if (num != 459378836U)
			{
				if (num != 1457414945U)
				{
					if (num == 2200499586U)
					{
						if (text == "stash")
						{
							if (!EClass.player.flags.backpackHighlightDisabled)
							{
								EClass.player.flags.backpackHighlightDisabled = true;
							}
							if (!EClass.ui.IsInventoryOpen)
							{
								SE.PopInventory();
							}
							EClass.ui.ToggleInventory(false);
							return;
						}
					}
				}
				else if (text == "LayerChara")
				{
					LayerChara layerChara = EClass.ui.ToggleLayer<LayerChara>(null);
					if (layerChara == null)
					{
						return;
					}
					layerChara.SetChara(EClass.pc);
					return;
				}
			}
			else if (text == "LayerAbility")
			{
				if (!EClass.player.flags.abilityHighlightDisabled)
				{
					EClass.player.flags.abilityHighlightDisabled = true;
				}
				EClass.ui.ToggleAbility(false);
				return;
			}
		}
		else if (num <= 2922513849U)
		{
			if (num != 2853713114U)
			{
				if (num == 2922513849U)
				{
					if (text == "LayerHelp")
					{
						if (!EClass.player.flags.helpHighlightDisabled)
						{
							EClass.player.flags.helpHighlightDisabled = true;
							LayerHelp.Toggle("general", "1");
							return;
						}
						LayerHelp.Toggle(LayerHelp.lastIdFile.IsEmpty("general"), LayerHelp.lastIdTopic.IsEmpty("2"));
						return;
					}
				}
			}
			else if (text == "LayerTravel")
			{
				EClass.ui.ToggleLayer<LayerTravel>(null);
				return;
			}
		}
		else if (num != 3310650625U)
		{
			if (num == 4163083791U)
			{
				if (text == "LayerTactics")
				{
					EClass.ui.ToggleLayer<LayerTactics>(null);
					return;
				}
			}
		}
		else if (text == "LayerJournal")
		{
			EClass.ui.ToggleLayer<LayerJournal>(null);
			return;
		}
		EClass.ui.ToggleLayer(this.id);
	}

	public static UIHighlightObject highlightHelp;

	public static UIHighlightObject highlightAbility;

	public static UIHighlightObject highlightBackpack;

	[JsonProperty]
	public string id;
}
