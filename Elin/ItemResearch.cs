using UnityEngine.UI;

public class ItemResearch : UIItem
{
	public Image imageExp;

	public Image goFocus;

	public ResearchPlan plan;

	public void SetPlan(ResearchPlan p, UIList list, FactionBranch branch, LayerTech layer)
	{
		bool isFinished = !branch.researches.plans.Contains(p);
		plan = p;
		button1.mainText.text = p.Name;
		goFocus.SetActive(branch.researches.focused == p);
		string text = p.source.tech.ToString("#,0").TagColorGoodBad(() => branch.researches.CanCompletePlan(p));
		button1.subText.text = text;
		button1.subText.SetActive(!isFinished);
		imageExp.fillAmount = (float)branch.resources.knowledge.value / (float)p.source.tech;
		button1.tooltip.onShowTooltip = delegate(UITooltip tp)
		{
			p.WriteNote(tp.note);
		};
		button1.onClick.AddListener(delegate
		{
			if (!isFinished)
			{
				if (!branch.researches.CanCompletePlan(p))
				{
					SE.Beep();
				}
				else
				{
					UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction().SetHighlightTarget(button1);
					uIContextMenu.AddButton("buy", delegate
					{
						branch.resources.knowledge.Mod(-p.source.tech);
						branch.researches.CompletePlan(p);
						layer.RefreshTech();
					});
					uIContextMenu.Show();
				}
			}
		});
	}
}
