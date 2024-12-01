public class ActChat : Act
{
	public override bool ResetAxis => true;

	public override int PerformDistance => 3;

	public override CursorInfo CursorIcon => CursorSystem.IconChat;

	public override bool Perform()
	{
		Act.TC.Chara?.ShowDialog();
		return false;
	}
}
