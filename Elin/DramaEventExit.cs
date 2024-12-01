public class DramaEventExit : DramaEvent
{
	public override bool Play()
	{
		sequence.Exit();
		return false;
	}
}
