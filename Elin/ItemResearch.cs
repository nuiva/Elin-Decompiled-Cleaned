using System;
using UnityEngine.UI;

public class ItemResearch : UIItem
{
	public void SetPlan(ResearchPlan p, UIList list, FactionBranch branch, LayerTech layer)
	{
		bool isFinished = !branch.researches.plans.Contains(p);
		this.plan = p;
		this.button1.mainText.text = p.Name;
		this.goFocus.SetActive(branch.researches.focused == p);
		string text = p.source.tech.ToString("#,0").TagColorGoodBad(() => branch.researches.CanCompletePlan(p), false);
		this.button1.subText.text = text;
		this.button1.subText.SetActive(!isFinished);
		this.imageExp.fillAmount = (float)branch.resources.knowledge.value / (float)p.source.tech;
		this.button1.tooltip.onShowTooltip = delegate(UITooltip tp)
		{
			p.WriteNote(tp.note);
		};
		Action <>9__3;
		this.button1.onClick.AddListener(delegate()
		{
			if (isFinished)
			{
				return;
			}
			if (!branch.researches.CanCompletePlan(p))
			{
				SE.Beep();
				return;
			}
			UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction().SetHighlightTarget(this.button1);
			string idLang = "buy";
			Action action;
			if ((action = <>9__3) == null)
			{
				action = (<>9__3 = delegate()
				{
					branch.resources.knowledge.Mod(-p.source.tech, true);
					branch.researches.CompletePlan(p);
					layer.RefreshTech();
				});
			}
			uicontextMenu.AddButton(idLang, action, true);
			uicontextMenu.Show();
		});
	}

	public Image imageExp;

	public Image goFocus;

	public ResearchPlan plan;
}
