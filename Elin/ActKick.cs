using System;

public class ActKick : Act
{
	public override CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.Kick;
		}
	}

	public override bool CanPressRepeat
	{
		get
		{
			return true;
		}
	}

	public override bool CanPerform()
	{
		return Act.TP.Distance(Act.CC.pos) <= 1 && Act.TC != null && Act.TC.isChara;
	}

	public override bool Perform()
	{
		Act.CC.Kick(Act.TC.Chara, false, true);
		return true;
	}
}
