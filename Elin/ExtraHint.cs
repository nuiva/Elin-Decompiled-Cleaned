using System;

public class ExtraHint : EMono
{
	public ActionMode mode
	{
		get
		{
			return EMono.scene.actionMode;
		}
	}

	public void OnChangeActionMode()
	{
		this.SetActive(this.mode.ShowExtraHint);
		if (this.mode.ShowExtraHint)
		{
			this.Refresh();
		}
	}

	public void Refresh()
	{
		this.note.Clear();
		this.mode.OnShowExtraHint(this.note);
		this.note.Build();
	}

	public UINote note;
}
