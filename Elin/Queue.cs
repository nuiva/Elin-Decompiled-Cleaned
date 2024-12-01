public class Queue
{
	public AIAct interaction;

	public UIButton button;

	public bool removed;

	public bool pinned;

	public bool CanCancel
	{
		get
		{
			if (!removed)
			{
				return interaction.IsRunning;
			}
			return false;
		}
	}
}
