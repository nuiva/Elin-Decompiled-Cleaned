using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WidgetCodex : Widget
{
	public virtual WidgetCodex.SearchType type
	{
		get
		{
			return WidgetCodex.SearchType.Codex;
		}
	}

	public override void OnActivate()
	{
		this.field.onValueChanged.AddListener(new UnityAction<string>(this.Search));
		this.field.onSubmit.AddListener(new UnityAction<string>(this.Search));
		this.lastSearch = null;
		this.Search("");
		if (EMono._zone.isStarted)
		{
			this.field.Select();
		}
	}

	public void Clear()
	{
		this.field.text = "";
	}

	public virtual void Search(string s)
	{
		s = s.ToLower();
		this.buttonClear.SetActive(this.field.text != "");
		if (s == this.lastSearch)
		{
			return;
		}
		RecipeManager.BuildList();
		HashSet<Recipe> hashSet = new HashSet<Recipe>();
		if (!s.IsEmpty())
		{
			using (List<RecipeSource>.Enumerator enumerator = RecipeManager.list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					RecipeSource recipeSource = enumerator.Current;
					if (!recipeSource.isChara && !recipeSource.noListing && !recipeSource.isBridgePillar && (recipeSource.row.GetSearchName(false).Contains(s) || recipeSource.row.GetSearchName(true).Contains(s)) && (EMono.player.recipes.knownRecipes.ContainsKey(recipeSource.id) || recipeSource.alwaysKnown))
					{
						hashSet.Add(Recipe.Create(recipeSource, -1, null));
					}
				}
				goto IL_16D;
			}
		}
		foreach (RecipeSource recipeSource2 in RecipeManager.list)
		{
			if (!recipeSource2.isChara && !recipeSource2.noListing && !recipeSource2.isBridgePillar && (EMono.player.recipes.knownRecipes.ContainsKey(recipeSource2.id) || recipeSource2.alwaysKnown))
			{
				hashSet.Add(Recipe.Create(recipeSource2, -1, null));
			}
		}
		IL_16D:
		if (!hashSet.SetEquals(this.recipes))
		{
			this.recipes = hashSet;
			this.RefreshList();
		}
		this.lastSearch = s;
	}

	public virtual void RefreshList()
	{
		BaseList baseList = this.list;
		UIList.Callback<Recipe, ButtonGrid> callback = new UIList.Callback<Recipe, ButtonGrid>();
		callback.onClick = delegate(Recipe a, ButtonGrid b)
		{
		};
		callback.onRedraw = delegate(Recipe a, ButtonGrid b, int i)
		{
			b.SetCraftRecipe(a, ButtonGrid.Mode.RecipeGrid, true);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (Recipe o in this.recipes)
			{
				this.list.Add(o);
			}
		};
		baseList.callbacks = callback;
		this.list.List();
		this.list.dsv.OnResize();
	}

	protected void Update()
	{
		this.count++;
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			base.Close();
			EInput.Consume(false, 5);
			return;
		}
		int functionkeyDown = EInput.GetFunctionkeyDown();
		if (this.count > 10 && functionkeyDown != -1)
		{
			WidgetHotbar hotbarExtra = WidgetHotbar.HotbarExtra;
			if (hotbarExtra)
			{
				HotItemWidget hotItemWidget = hotbarExtra.GetItem(functionkeyDown) as HotItemWidget;
				if (hotItemWidget != null && hotItemWidget.id == base.ID)
				{
					base.Close();
					EInput.Consume(false, 5);
					return;
				}
			}
		}
		if (this.CheckClose())
		{
			base.Close();
			EInput.Consume(false, 1);
		}
	}

	public virtual bool CheckClose()
	{
		return false;
	}

	public InputField field;

	public WordSearch search = new WordSearch();

	public UIDynamicList list;

	public HashSet<Recipe> recipes = new HashSet<Recipe>();

	public string lastSearch;

	public UIButton buttonClear;

	public Transform transResult;

	private int count;

	public enum SearchType
	{
		Codex,
		Search
	}
}
