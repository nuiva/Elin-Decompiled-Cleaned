using System;

public class LayerGlobalMap : ELayer
{
	public UIText textName;

	public UIText textExplore;

	public UIText textTerritory;

	public UIText textPopu;

	public UIText textWealth;

	public UIText textRestore;

	public UIText textProgress;

	public override void OnInit()
	{
		RefreshSummary();
	}

	public void RefreshSummary()
	{
		textName.SetText(ELayer.world.Name + Environment.NewLine + "<size=18> (" + ELayer.pc.currentZone.Region.Name + ")</size>");
		textExplore.SetText("0%");
		textTerritory.SetText(ELayer.Home.CountTerritories().ToString() ?? "");
		textRestore.SetText("1 (0/20)");
		textWealth.SetText(ELayer.Home.CountWealth().ToString() ?? "");
		textPopu.SetText(ELayer.Home.CountMembers().ToString() ?? "");
		textProgress.SetText("_progress".lang(ELayer.player.stats.days.ToString() ?? "", ELayer.player.stats.sieges.ToString() ?? ""));
	}
}
