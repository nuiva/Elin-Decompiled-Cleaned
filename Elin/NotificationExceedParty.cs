using System;

public class NotificationExceedParty : BaseNotification
{
	public override bool Visible
	{
		get
		{
			return EClass.pc.party.members.Count - 1 > EClass.player.MaxAlly;
		}
	}

	public override Action<UITooltip> onShowTooltip
	{
		get
		{
			return delegate(UITooltip a)
			{
				a.note.Clear();
				a.note.AddHeader("exceedParty", null);
				a.note.AddText("exceedParty_tip".lang((EClass.pc.party.members.Count - 1).ToString() ?? "", EClass.player.MaxAlly.ToString() ?? "", null, null, null), FontColor.DontChange);
				a.note.Build();
			};
		}
	}

	public override void OnRefresh()
	{
		this.text = "exceedParty".lang();
	}
}
