using System;
using System.Collections.Generic;

public class ItemCulture : UIItem
{
	public void Refresh()
	{
		FactionBranch branch = Core.Instance.game.player.chara.currentZone.branch;
		ElementContainerZone elements = branch.elements;
		Element e = elements.GetOrCreateElement(this.id);
		this.button1.SetTooltip(delegate(UITooltip t)
		{
			e.WriteNote(t.note, null, null);
		}, true);
		Action <>9__2;
		this.button1.onClick.SetListener(delegate
		{
			UIContextMenu uicontextMenu = Core.Instance.ui.CreateContextMenuInteraction();
			string idLang = "detail";
			Action action;
			if ((action = <>9__2) == null)
			{
				action = (<>9__2 = delegate()
				{
					Core.Instance.ui.AddLayer<LayerInfo>().Set(e, false);
				});
			}
			uicontextMenu.AddButton(idLang, action, true);
			uicontextMenu.Show();
		});
		this.text1.SetText(e.Name);
		int num = 0;
		foreach (KeyValuePair<int, Element> keyValuePair in elements.dict)
		{
			if (keyValuePair.Value.source.aliasParent == this.id)
			{
				num++;
			}
		}
		this.textSkills.SetText(num.ToString() ?? "");
		this.textLv.SetText(num.ToString() ?? "");
		this.textHearth.SetText("_hearth".lang(branch.GetHearthIncome(e.source.alias).ToString("F1") ?? "", null, null, null, null));
	}

	public string id;

	public UIText textLv;

	public UIText textSkills;

	public UIText textInvest;

	public UIText textHearth;
}
