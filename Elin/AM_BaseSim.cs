public class AM_BaseSim : AM_BaseGameMode
{
	public override AreaHighlightMode AreaHihlight => AreaHighlightMode.Sim;

	public override bool ShouldPauseGame => false;
}
