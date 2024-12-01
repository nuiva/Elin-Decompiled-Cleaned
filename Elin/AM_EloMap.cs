public class AM_EloMap : AM_BaseSim
{
	public override bool ShouldPauseGame => true;

	public EloMapActor actor => EClass.scene.elomapActor;

	public override void OnActivate()
	{
		ActionMode.DefaultMode = this;
	}
}
