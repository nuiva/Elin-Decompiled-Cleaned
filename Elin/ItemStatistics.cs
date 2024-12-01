public class ItemStatistics : EMono
{
	public UIText textVisitor;

	public UIText textInn;

	public UIText textTourism;

	public UIText textShip;

	public UIText textShop;

	public UIText textTax;

	public void Refresh(FactionBranch.Statistics stat)
	{
		FactionBranch.Statistics lastStatistics = EMono.Branch.lastStatistics;
		textVisitor.text = stat.visitor + " (" + lastStatistics.visitor + ")";
		textInn.text = F(stat.inn, lastStatistics.inn);
		textTourism.text = F(stat.tourism, lastStatistics.tourism);
		textShip.text = F(stat.ship, lastStatistics.ship);
		textShop.text = F(stat.shop, lastStatistics.shop);
		textTax.text = F(stat.tax, lastStatistics.tax);
		static string F(int a, int b)
		{
			return a.ToFormat() + " (" + b.ToFormat() + ")";
		}
	}
}
