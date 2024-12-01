using System;
using Newtonsoft.Json;
using UnityEngine;

public class HotItemLayer : HotItem
{
	public static UIHighlightObject highlightHelp;

	public static UIHighlightObject highlightAbility;

	public static UIHighlightObject highlightBackpack;

	[JsonProperty]
	public string id;

	public override string Name => id.lang();

	public override string pathSprite => "icon_" + id;

	public override void OnSetItem(UIButton b)
	{
		if (!EClass.debug.enable)
		{
			AddHighlight("LayerHelp", () => EClass.player.flags.helpHighlightDisabled, ref highlightHelp);
			AddHighlight("stash", () => EClass.player.flags.backpackHighlightDisabled, ref highlightBackpack);
			AddHighlight("LayerAbility", () => EClass.player.flags.abilityHighlightDisabled, ref highlightAbility);
		}
		void AddHighlight(string _id, Func<bool> funcDisable, ref UIHighlightObject highlight)
		{
			if (!(id != _id) && !funcDisable())
			{
				if ((bool)highlight)
				{
					UnityEngine.Object.DestroyImmediate(highlight.gameObject);
				}
				UIHighlightObject _highlight = (highlight = Util.Instantiate<UIHighlightObject>("UI/Util/Highlight" + _id, EClass.ui));
				highlight.ShouldDestroy = () => b == null || !EClass.core.IsGameStarted || funcDisable();
				highlight.OnUpdate = delegate
				{
					_highlight.trans.SetActive(b.gameObject.activeInHierarchy);
					_highlight.transform.position = b.transform.position;
					_highlight.cg.alpha = ((EClass.ui.layers.Count == 0) ? 1 : 0);
				};
			}
		}
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		switch (id)
		{
		case "stash":
			if (!EClass.player.flags.backpackHighlightDisabled)
			{
				EClass.player.flags.backpackHighlightDisabled = true;
			}
			if (!EClass.ui.IsInventoryOpen)
			{
				SE.PopInventory();
			}
			EClass.ui.ToggleInventory();
			break;
		case "LayerHelp":
			if (!EClass.player.flags.helpHighlightDisabled)
			{
				EClass.player.flags.helpHighlightDisabled = true;
				LayerHelp.Toggle("general", "1");
			}
			else
			{
				LayerHelp.Toggle(LayerHelp.lastIdFile.IsEmpty("general"), LayerHelp.lastIdTopic.IsEmpty("2"));
			}
			break;
		case "LayerJournal":
			EClass.ui.ToggleLayer<LayerJournal>();
			break;
		case "LayerAbility":
			if (!EClass.player.flags.abilityHighlightDisabled)
			{
				EClass.player.flags.abilityHighlightDisabled = true;
			}
			EClass.ui.ToggleAbility();
			break;
		case "LayerChara":
			EClass.ui.ToggleLayer<LayerChara>()?.SetChara(EClass.pc);
			break;
		case "LayerTactics":
			EClass.ui.ToggleLayer<LayerTactics>();
			break;
		case "LayerTravel":
			EClass.ui.ToggleLayer<LayerTravel>();
			break;
		default:
			EClass.ui.ToggleLayer(id);
			break;
		}
	}
}
