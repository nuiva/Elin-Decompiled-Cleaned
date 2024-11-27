using System;

public class LayerHelp : ELayer
{
	public override bool blockWidgetClick
	{
		get
		{
			return false;
		}
	}

	public override void OnAfterInit()
	{
		UIBook.Mode mode = this.book.mode;
		if (mode == UIBook.Mode.Announce)
		{
			this.book.Show("version" + (ELayer.core.version.demo ? "_demo" : ""), null, null, null);
			return;
		}
		if (mode != UIBook.Mode.About)
		{
			return;
		}
		this.book.Show("about_EA", null, null, null);
	}

	public static void Toggle(string idFile, string idTopic = null)
	{
		LayerHelp layerHelp = ELayer.ui.ToggleLayer<LayerHelp>(null);
		if (layerHelp != null)
		{
			layerHelp.book.Show(idFile, idTopic, null, null);
		}
	}

	public override void OnRightClick()
	{
		if (this.book.inputSearch && this.book.transSearchResult.gameObject.activeSelf)
		{
			this.book.showSearchResult = false;
			return;
		}
		base.OnRightClick();
	}

	private void OnDestroy()
	{
		if (this.book.mode == UIBook.Mode.Help)
		{
			LayerHelp.lastIdFile = this.book.idFile;
			LayerHelp.lastIdTopic = this.book.idTopic;
		}
	}

	public static string lastIdFile;

	public static string lastIdTopic;

	public UIBook book;
}
