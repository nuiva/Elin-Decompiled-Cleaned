using System;
using System.Runtime.CompilerServices;

public class ItemStatistics : EMono
{
	public void Refresh(FactionBranch.Statistics stat)
	{
		FactionBranch.Statistics lastStatistics = EMono.Branch.lastStatistics;
		this.textVisitor.text = stat.visitor.ToString() + " (" + lastStatistics.visitor.ToString() + ")";
		this.textInn.text = ItemStatistics.<Refresh>g__F|6_0(stat.inn, lastStatistics.inn);
		this.textTourism.text = ItemStatistics.<Refresh>g__F|6_0(stat.tourism, lastStatistics.tourism);
		this.textShip.text = ItemStatistics.<Refresh>g__F|6_0(stat.ship, lastStatistics.ship);
		this.textShop.text = ItemStatistics.<Refresh>g__F|6_0(stat.shop, lastStatistics.shop);
		this.textTax.text = ItemStatistics.<Refresh>g__F|6_0(stat.tax, lastStatistics.tax);
	}

	[CompilerGenerated]
	internal static string <Refresh>g__F|6_0(int a, int b)
	{
		return a.ToFormat() + " (" + b.ToFormat() + ")";
	}

	public UIText textVisitor;

	public UIText textInn;

	public UIText textTourism;

	public UIText textShip;

	public UIText textShop;

	public UIText textTax;
}
