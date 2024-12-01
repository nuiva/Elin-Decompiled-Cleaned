public class ActKick : Act
{
	public override CursorInfo CursorIcon => CursorSystem.Kick;

	public override bool CanPressRepeat => true;

	public override bool CanPerform()
	{
		if (Act.TP.Distance(Act.CC.pos) <= 1 && Act.TC != null)
		{
			return Act.TC.isChara;
		}
		return false;
	}

	public override bool Perform()
	{
		Act.CC.Kick(Act.TC.Chara);
		return true;
	}
}
