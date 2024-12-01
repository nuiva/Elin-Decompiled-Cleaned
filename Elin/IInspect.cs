using System;
using UnityEngine;

public interface IInspect
{
	public enum NoteMode
	{
		Default,
		Recipe,
		Product,
		Info
	}

	bool CanInspect { get; }

	string InspectName { get; }

	Point InspectPoint { get; }

	Vector3 InspectPosition { get; }

	void WriteNote(UINote n, Action<UINote> onWriteNote = null, NoteMode mode = NoteMode.Default, Recipe recipe = null);

	void OnInspect();
}
