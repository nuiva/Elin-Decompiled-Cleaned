using System;

public class LayerGlobalMap : ELayer
{
	public override void OnInit()
	{
		this.RefreshSummary();
	}

	public void RefreshSummary()
	{
		this.textName.SetText(string.Concat(new string[]
		{
			ELayer.world.Name,
			Environment.NewLine,
			"<size=18> (",
			ELayer.pc.currentZone.Region.Name,
			")</size>"
		}));
		this.textExplore.SetText("0%");
		this.textTerritory.SetText(ELayer.Home.CountTerritories().ToString() ?? "");
		this.textRestore.SetText("1 (0/20)");
		this.textWealth.SetText(ELayer.Home.CountWealth().ToString() ?? "");
		this.textPopu.SetText(ELayer.Home.CountMembers().ToString() ?? "");
		this.textProgress.SetText("_progress".lang(ELayer.player.stats.days.ToString() ?? "", ELayer.player.stats.sieges.ToString() ?? "", null, null, null));
	}

	public UIText textName;

	public UIText textExplore;

	public UIText textTerritory;

	public UIText textPopu;

	public UIText textWealth;

	public UIText textRestore;

	public UIText textProgress;
}
