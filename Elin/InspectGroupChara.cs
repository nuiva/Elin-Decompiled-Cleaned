using System;

public class InspectGroupChara : InspectGroup<Chara>
{
	public override string MultiName => "Chara";

	public override void OnSetActions()
	{
		Chara first = base.FirstTarget;
		string text = "charaInfo".lang();
		if (!first.IsHomeMember())
		{
			text = text + "(" + "unidentified".lang() + ")";
		}
		if (first.IsHomeMember())
		{
			Add("tTalk".lang(), "", (Action)delegate
			{
				first.ShowDialog();
			}, sound: false, 0, auto: false);
			return;
		}
		Add(text, "", (Action)delegate
		{
			if (!first.IsHomeMember())
			{
				SE.Beep();
			}
		}, sound: false, 0, auto: false);
	}
}
