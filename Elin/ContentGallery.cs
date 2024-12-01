using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ContentGallery : EContent
{
	public class Page : UIBook.Page
	{
		public class Item
		{
			public int id;

			public Sprite sprite;
		}

		public List<string> ids = new List<string>();

		public override void BuildNote(UINote n, string idTopic)
		{
			foreach (string id in ids)
			{
				UIItem uIItem = n.AddItem("ItemGallery");
				int num = id.ToInt();
				Sprite sprite = Resources.Load<Sprite>("Media/Gallery/" + CoreRef.GetArtDir(num) + "/" + EClass.core.refs.dictSketches[num]);
				uIItem.image1.sprite = sprite;
				uIItem.text1.text = "#" + id;
				uIItem.button1.SetOnClick(delegate
				{
					SE.Play("click_recipe");
					EClass.ui.AddLayer<LayerImage>().SetImage(sprite);
				});
			}
		}
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

	public override void OnSwitchContent(int idTab)
	{
		if (!first)
		{
			return;
		}
		if (EClass.debug.allArt)
		{
			EClass.player.sketches.Clear();
			foreach (int key in EClass.core.refs.dictSketches.Keys)
			{
				EClass.player.sketches.Add(key);
			}
		}
		Refresh();
		first = false;
	}

	public void Refresh()
	{
		book.pages.Clear();
		GridLayoutGroup[] array = grids;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].cellSize = gridSize[listMode ? 1 : 0];
		}
		Page page = new Page();
		List<int> list = EClass.player.sketches.ToList();
		list.Sort((int a, int b) => a - b);
		foreach (int item in list)
		{
			page.ids.Add(item.ToString() ?? "");
			if (page.ids.Count >= (listMode ? 8 : 2))
			{
				book.AddPage(page);
				page = new Page();
			}
		}
		if (page.ids.Count > 0)
		{
			book.AddPage(page);
		}
		book.currentPage = lastPage;
		book.Show();
		textCollected.SetText("sketch_collected".lang((list.Count * 100 / EClass.core.refs.dictSketches.Count()).ToString() ?? ""));
	}

	public void OnClickHelp()
	{
		LayerHelp.Toggle("other", "gallery");
	}

	public void ToggleMode()
	{
		listMode = !listMode;
		lastPage = (listMode ? (book.currentPage / 4) : (book.currentPage * 4));
		SE.Tab();
		Refresh();
	}

	private void OnDestroy()
	{
		lastPage = book.currentPage;
	}
}
