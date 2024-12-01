public class ContentHallOfFame : EContent
{
	public UIText textElapsedTime;

	public UIText textGamneTime;

	public UIText textKilled;

	public UIText textMined;

	public UIText textTax;

	private void OnEnable()
	{
		InvokeRepeating("Refresh", 1f, 1f);
	}

	private void OnDisable()
	{
		CancelInvoke("Refresh");
	}

	public override void OnSwitchContent(int idTab)
	{
		textKilled.text = EClass.player.stats.kills.ToString() ?? "";
		textMined.text = EClass.player.stats.digs.ToString() ?? "";
		textTax.text = "u2_money".lang(EClass.player.stats.taxBillsPaid.ToString() ?? "");
		Refresh();
	}

	public void Refresh()
	{
		double num = EClass.player.stats.timeElapsed % 60.0;
		double num2 = EClass.player.stats.timeElapsed / 60.0 % 60.0;
		double num3 = EClass.player.stats.timeElapsed / 3600.0;
		textElapsedTime.text = Lang.Parse("elapsedTime", ((int)num3).ToString() ?? "", ((int)num2).ToString() ?? "", ((int)num).ToString() ?? "");
		textGamneTime.text = "tGameTime".lang(EClass.player.stats.days.ToString() ?? "", EClass.player.stats.turns.ToString() ?? "");
	}
}
