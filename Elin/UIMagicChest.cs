using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIMagicChest : EMono
{
	public Card container
	{
		get
		{
			return this.uiInventory.owner.Container;
		}
	}

	public int GridSize
	{
		get
		{
			return this.container.things.GridSize;
		}
	}

	public void Init()
	{
		UIButton t = this.layoutPage.CreateMold(null);
		this.moldCat = this.layoutCat.CreateMold(null);
		for (int i = 0; i < 9; i++)
		{
			UIButton b = Util.Instantiate<UIButton>(t, this.layoutPage);
			b.mainText.text = ((i + 1).ToString() ?? "");
			this.buttonsPage.Add(b);
			int _i = i;
			b.SetOnClick(delegate
			{
				this.page = _i;
				this.groupPage.Select(b, true);
				SE.Tab();
				this.Redraw();
			});
		}
		this.groupPage.Init(0, null, false);
		this.inputSearch.onValueChanged.AddListener(new UnityAction<string>(this.Search));
		this.inputSearch.onSubmit.AddListener(new UnityAction<string>(this.Search));
		this.RefreshBottom();
		this.itemNum.RebuildLayout(false);
		this.itemElec.RebuildLayout(false);
		this.layoutBottom.RebuildLayout(false);
	}

	public void OnAfterRedraw()
	{
		this.RefreshBottom();
		for (int i = 0; i < this.buttonsPage.Count; i++)
		{
			this.buttonsPage[i].interactable = (i <= this.pageMax);
		}
		this.groupPage.selected = null;
		this.groupPage.Select(this.page);
	}

	public void RefreshCats()
	{
		foreach (string text in this.cats)
		{
			if (!this.catButton.ContainsKey(text))
			{
				UIButton uibutton = Util.Instantiate<UIButton>(this.moldCat, this.layoutCat);
				this.catButton[text] = uibutton;
				string _c = text;
				uibutton.SetOnClick(delegate
				{
					SE.Tab();
					if (this.idCat == _c)
					{
						this.idCat = "";
					}
					else
					{
						this.idCat = _c;
					}
					this.Redraw();
				});
			}
		}
		foreach (KeyValuePair<string, UIButton> keyValuePair in this.catButton)
		{
			bool flag = this.cats.Contains(keyValuePair.Key);
			UIButton value = keyValuePair.Value;
			value.SetActive(flag);
			if (flag)
			{
				value.mainText.text = EMono.sources.categories.map[keyValuePair.Key].GetName() + " (" + this.catCount[keyValuePair.Key].ToString() + ")";
				value.image.color = ((keyValuePair.Key == this.idCat) ? this.colorCatSelected : this.colorCat);
			}
		}
	}

	public void RefreshBottom()
	{
		this.itemNum.mainText.text = this.container.things.Count.ToString() + " / " + this.container.things.MaxCapacity.ToString();
		this.itemElec.mainText.SetText(Mathf.Abs(this.container.trait.Electricity).ToString() + " " + "mw".lang(), (this.container.trait.Electricity == 0 || this.container.isOn) ? FontColor.Good : FontColor.Bad);
	}

	private void LateUpdate()
	{
		if (this.timerSearch > 0f)
		{
			this.timerSearch -= Core.delta;
			if (this.timerSearch <= 0f)
			{
				this.Search(this.inputSearch.text);
			}
		}
		if (EInput.wheel != 0)
		{
			SE.Tab();
			this.page -= EInput.wheel;
			if (this.page < 0)
			{
				this.page = this.pageMax;
			}
			if (this.page > this.pageMax)
			{
				this.page = 0;
			}
			this.Redraw();
		}
	}

	public void Search(string s)
	{
		s = s.ToLower();
		if (s.IsEmpty())
		{
			s = "";
		}
		this.buttonClearSearch.SetActive(this.inputSearch.text != "");
		if (s == this.lastSearch)
		{
			return;
		}
		if (this.firstSearch)
		{
			this.firstSearch = false;
			foreach (Thing thing in this.container.things)
			{
				thing.tempName = thing.GetName(NameStyle.Full, 1).ToLower();
			}
		}
		this.timerSearch = this.intervalSearch;
		this.lastSearch = s;
		this.Redraw();
	}

	public void ClearSearch()
	{
		this.inputSearch.text = "";
		this.timerSearch = 0f;
		this.lastSearch = "";
		this.Redraw();
	}

	public void Redraw()
	{
		this.uiInventory.list.Redraw();
	}

	public LayoutGroup layoutPage;

	public LayoutGroup layoutCat;

	public LayoutGroup layoutBottom;

	public UIButton itemNum;

	public UIButton itemElec;

	public UIButton buttonClearSearch;

	public UIButton moldCat;

	public UISelectableGroup groupPage;

	public List<UIButton> buttonsPage;

	public UIInventory uiInventory;

	public InputField inputSearch;

	public float intervalSearch;

	public int page;

	public int pageMax;

	public List<Thing> filteredList = new List<Thing>();

	public Color colorCat;

	public Color colorCatSelected;

	public HashSet<string> cats = new HashSet<string>();

	public Dictionary<string, UIButton> catButton = new Dictionary<string, UIButton>();

	public Dictionary<string, int> catCount = new Dictionary<string, int>();

	public string idCat = "";

	private bool firstSearch = true;

	private float timerSearch;

	public string lastSearch = "";

	public HashSet<Recipe> searchRecipes = new HashSet<Recipe>();
}
