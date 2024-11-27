using System;

public class ActChat : Act
{
	public override bool ResetAxis
	{
		get
		{
			return true;
		}
	}

	public override int PerformDistance
	{
		get
		{
			return 3;
		}
	}

	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.IconChat;
		}
	}

	public override bool Perform()
	{
		Chara chara = Act.TC.Chara;
		if (chara != null)
		{
			chara.ShowDialog();
		}
		return false;
	}
}
