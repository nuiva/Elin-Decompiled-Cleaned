using System;

public class ContentHallOfFame : EContent
{
	private void OnEnable()
	{
		base.InvokeRepeating("Refresh", 1f, 1f);
	}

	private void OnDisable()
	{
		base.CancelInvoke("Refresh");
	}

	public override void OnSwitchContent(int idTab)
	{
		this.textKilled.text = (EClass.player.stats.kills.ToString() ?? "");
		this.textMined.text = (EClass.player.stats.digs.ToString() ?? "");
		this.textTax.text = "u2_money".lang(EClass.player.stats.taxBillsPaid.ToString() ?? "", null, null, null, null);
		this.Refresh();
	}

	public void Refresh()
	{
		double num = EClass.player.stats.timeElapsed % 60.0;
		double num2 = EClass.player.stats.timeElapsed / 60.0 % 60.0;
		double num3 = EClass.player.stats.timeElapsed / 3600.0;
		this.textElapsedTime.text = Lang.Parse("elapsedTime", ((int)num3).ToString() ?? "", ((int)num2).ToString() ?? "", ((int)num).ToString() ?? "", null, null);
		this.textGamneTime.text = "tGameTime".lang(EClass.player.stats.days.ToString() ?? "", EClass.player.stats.turns.ToString() ?? "", null, null, null);
	}

	public UIText textElapsedTime;

	public UIText textGamneTime;

	public UIText textKilled;

	public UIText textMined;

	public UIText textTax;
}
