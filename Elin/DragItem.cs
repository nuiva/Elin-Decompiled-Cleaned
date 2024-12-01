using UnityEngine.UI;

public class DragItem : EClass
{
	public virtual UIButton Button => null;

	public virtual Image GetDragImage()
	{
		return Button.icon;
	}

	public virtual void OnStartDrag()
	{
	}

	public virtual void OnEndDrag()
	{
	}

	public virtual bool OnDrag(bool execute, bool cancel = false)
	{
		return true;
	}

	public virtual bool OnDragSpecial()
	{
		return false;
	}
}
