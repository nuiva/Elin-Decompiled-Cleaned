using System;

public class InspectGroupChara : InspectGroup<Chara>
{
	public override string MultiName
	{
		get
		{
			return "Chara";
		}
	}

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
			base.Add("tTalk".lang(), "", delegate()
			{
				first.ShowDialog();
			}, false, 0, false);
			return;
		}
		base.Add(text, "", delegate()
		{
			if (!first.IsHomeMember())
			{
				SE.Beep();
				return;
			}
		}, false, 0, false);
	}
}
