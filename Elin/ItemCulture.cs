using System.Collections.Generic;

public class ItemCulture : UIItem
{
	public string id;

	public UIText textLv;

	public UIText textSkills;

	public UIText textInvest;

	public UIText textHearth;

	public void Refresh()
	{
		FactionBranch branch = Core.Instance.game.player.chara.currentZone.branch;
		ElementContainerZone elements = branch.elements;
		Element e = elements.GetOrCreateElement(id);
		button1.SetTooltip(delegate(UITooltip t)
		{
			e.WriteNote(t.note);
		});
		button1.onClick.SetListener(delegate
		{
			UIContextMenu uIContextMenu = Core.Instance.ui.CreateContextMenuInteraction();
			uIContextMenu.AddButton("detail", delegate
			{
				Core.Instance.ui.AddLayer<LayerInfo>().Set(e);
			});
			uIContextMenu.Show();
		});
		text1.SetText(e.Name);
		int num = 0;
		foreach (KeyValuePair<int, Element> item in elements.dict)
		{
			if (item.Value.source.aliasParent == id)
			{
				num++;
			}
		}
		textSkills.SetText(num.ToString() ?? "");
		textLv.SetText(num.ToString() ?? "");
		textHearth.SetText("_hearth".lang(branch.GetHearthIncome(e.source.alias).ToString("F1") ?? ""));
	}
}
