using System;
using UnityEngine;

public interface IInspect
{
	bool CanInspect { get; }

	string InspectName { get; }

	Point InspectPoint { get; }

	Vector3 InspectPosition { get; }

	void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null);

	void OnInspect();

	public enum NoteMode
	{
		Default,
		Recipe,
		Product,
		Info
	}
}
