using System;
using UnityEngine;

public class BaseInspectPos : IInspect
{
	public void OnInspect()
	{
	}

	public Point InspectPoint
	{
		get
		{
			return this.pos;
		}
	}

	public unsafe Vector3 InspectPosition
	{
		get
		{
			return *this.pos.Position();
		}
	}

	public virtual bool CanInspect
	{
		get
		{
			return this.pos.HasBlock;
		}
	}

	public virtual string InspectName
	{
		get
		{
			return "BaseInspectPos";
		}
	}

	public virtual void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
	}

	public Point pos = new Point();
}
