using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ContentGallery : EContent
{
	public override void OnSwitchContent(int idTab)
	{
		if (!this.first)
		{
			return;
		}
		if (EClass.debug.allArt)
		{
			EClass.player.sketches.Clear();
			foreach (int item in EClass.core.refs.dictSketches.Keys)
			{
				EClass.player.sketches.Add(item);
			}
		}
		this.Refresh();
		this.first = false;
	}

	public void Refresh()
	{
		this.book.pages.Clear();
		GridLayoutGroup[] array = this.grids;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].cellSize = this.gridSize[ContentGallery.listMode ? 1 : 0];
		}
		ContentGallery.Page page = new ContentGallery.Page();
		List<int> list = EClass.player.sketches.ToList<int>();
		list.Sort((int a, int b) => a - b);
		foreach (int num in list)
		{
			page.ids.Add(num.ToString() ?? "");
			if (page.ids.Count >= (ContentGallery.listMode ? 8 : 2))
			{
				this.book.AddPage(page);
				page = new ContentGallery.Page();
			}
		}
		if (page.ids.Count > 0)
		{
			this.book.AddPage(page);
		}
		this.book.currentPage = ContentGallery.lastPage;
		this.book.Show();
		this.textCollected.SetText("sketch_collected".lang((list.Count * 100 / EClass.core.refs.dictSketches.Count<KeyValuePair<int, string>>()).ToString() ?? "", null, null, null, null));
	}

	public void OnClickHelp()
	{
		LayerHelp.Toggle("other", "gallery");
	}

	public void ToggleMode()
	{
		ContentGallery.listMode = !ContentGallery.listMode;
		ContentGallery.lastPage = (ContentGallery.listMode ? (this.book.currentPage / 4) : (this.book.currentPage * 4));
		SE.Tab();
		this.Refresh();
	}

	private void OnDestroy()
	{
		ContentGallery.lastPage = this.book.currentPage;
	}

	public static int lastPage;

	public static bool listMode;

	public Transform transBig;

	public Image imageBig;

	public UIBook book;

	public UIText textCollected;

	public GridLayoutGroup[] grids;

	public Vector2[] gridSize;

	private bool first = true;

	public class Page : UIBook.Page
	{
		public override void BuildNote(UINote n, string idTopic)
		{
			foreach (string text in this.ids)
			{
				UIItem uiitem = n.AddItem("ItemGallery");
				int num = text.ToInt();
				Sprite sprite = Resources.Load<Sprite>("Media/Gallery/" + CoreRef.GetArtDir(num) + "/" + EClass.core.refs.dictSketches[num]);
				uiitem.image1.sprite = sprite;
				uiitem.text1.text = "#" + text;
				uiitem.button1.SetOnClick(delegate
				{
					SE.Play("click_recipe");
					EClass.ui.AddLayer<LayerImage>().SetImage(sprite);
				});
			}
		}

		public List<string> ids = new List<string>();

		public class Item
		{
			public int id;

			public Sprite sprite;
		}
	}
}
