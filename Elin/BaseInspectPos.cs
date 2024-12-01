using System;
using UnityEngine;

public class BaseInspectPos : IInspect
{
	public Point pos = new Point();

	public Point InspectPoint => pos;

	public Vector3 InspectPosition => pos.Position();

	public virtual bool CanInspect => pos.HasBlock;

	public virtual string InspectName => "BaseInspectPos";

	public void OnInspect()
	{
	}

	public virtual void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
	}
}
