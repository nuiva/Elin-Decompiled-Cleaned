using System;

public class Queue
{
	public bool CanCancel
	{
		get
		{
			return !this.removed && this.interaction.IsRunning;
		}
	}

	public AIAct interaction;

	public UIButton button;

	public bool removed;

	public bool pinned;
}
