public class MeetingFarris : Meeting
{
	public override string IdChara => "farris";

	public override bool IsGlobalChara => true;

	public override void PlayDrama()
	{
		LayerDrama.ActivateMain("farris", "1-1", chara);
	}
}
