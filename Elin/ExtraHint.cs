public class ExtraHint : EMono
{
	public UINote note;

	public ActionMode mode => EMono.scene.actionMode;

	public void OnChangeActionMode()
	{
		this.SetActive(mode.ShowExtraHint);
		if (mode.ShowExtraHint)
		{
			Refresh();
		}
	}

	public void Refresh()
	{
		note.Clear();
		mode.OnShowExtraHint(note);
		note.Build();
	}
}
