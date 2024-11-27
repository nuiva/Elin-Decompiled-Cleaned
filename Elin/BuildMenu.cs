using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BuildMenu : EMono
{
	public BuildMenu.Mode mode
	{
		get
		{
			return EMono.scene.actionMode.buildMenuMode;
		}
	}

	public static void Toggle()
	{
		EMono.player.hotbars.ResetHotbar(3);
		EMono.player.hotbars.bars[3].dirty = true;
		EMono.player.hotbars.ResetHotbar(4);
		EMono.player.hotbars.bars[4].dirty = true;
		if (EMono.debug.godBuild || (EMono.Branch != null && EMono.Branch.elements.Has(4006)))
		{
			ActionMode.Inspect.Activate(true, false);
			return;
		}
		if (ActionMode.LastBuildMode != null)
		{
			ActionMode.LastBuildMode.Activate(true, false);
			return;
		}
		ActionMode.Terrain.Activate(true, false);
	}

	public static void Activate()
	{
		if (!BuildMenu.Instance)
		{
			BuildMenu.Instance = Util.Instantiate<BuildMenu>("UI/BuildMenu/BuildMenu", EMono.ui);
			BuildMenu.Instance.Init();
		}
		if (EMono.debug.godBuild)
		{
			EMono._map.RevealAll(true);
		}
		RecipeManager.BuildList();
		BuildMenu.Instance.OnActivate();
		EMono.screen.focusPos = null;
	}

	public void OnActivate()
	{
		if (EMono.debug.godBuild && EMono.debug.GetDebugContainer() == null)
		{
			EMono.debug.EnableDebugResource();
		}
		EMono.ui.HideFloats();
		this.SetActive(this.mode != BuildMenu.Mode.None);
		this.transMain.SetActive(this.mode == BuildMenu.Mode.Build && (EMono.debug.godBuild || EMono._zone.elements.Has(4005)));
		BuildMenu.Mode mode = this.mode;
		if (mode != BuildMenu.Mode.Build)
		{
			if (mode == BuildMenu.Mode.Area)
			{
				this.RefreshCategoryArea();
				this.Refresh();
			}
		}
		else
		{
			string cat = EMono.player.pref.lastBuildCategory.IsEmpty("wall");
			this.SelectCategory(cat);
			this.Refresh();
			UIScrollView componentInChildren = this.cg.GetComponentInChildren<UIScrollView>();
			if (componentInChildren != null)
			{
				componentInChildren.RebuildLayout(true);
			}
			this.buttonUndo.onClick.RemoveAllListeners();
			this.buttonUndo.onClick.AddListener(delegate()
			{
				EMono._map.tasks.undo.Perform();
			});
			this.buttonUndo.SetTooltip("note", delegate(UITooltip tp)
			{
				EMono._map.tasks.undo.WriteNote(tp.note);
			}, true);
		}
		if (EMono.debug.enable)
		{
			EMono._map.ResetEditorPos();
		}
		EMono.screen.RefreshAll();
	}

	public static void Deactivate()
	{
		if (!BuildMenu.Instance)
		{
			return;
		}
		UnityEngine.Object.DestroyImmediate(BuildMenu.Instance.gameObject);
		if (EMono.game.altUI)
		{
			EMono._zone.RefreshBGM();
		}
		EMono.ui.ShowFloats();
		EMono.screen.RefreshAll();
	}

	public void Init()
	{
		this.info1.Init();
		this.info2.Init();
		this.info2.transform.position = this.info1.transform.position;
		this.orgPos = this.info1.Rect().anchoredPosition;
		this.orgPosOption = this.transOption.anchoredPosition;
		for (int i = 0; i < this.categories.Length; i++)
		{
			if (!this.categories[i].IsEmpty())
			{
				this.catDic.Add(this.categories[i], i);
			}
		}
		UIButton t = this.gridTab.CreateMold(null);
		for (int j = 0; j < this.categories.Length - 1; j++)
		{
			int _i = j;
			UIButton uibutton = Util.Instantiate<UIButton>(t, this.gridTab);
			uibutton.onClick.AddListener(delegate()
			{
				this.ClearSearch(false);
				this.SelectCategory(this.categories[_i]);
			});
			uibutton.mainText.SetText(("cat_" + this.categories[j]).lang());
			this.tabs[this.categories[j]] = uibutton;
			if (this.categories[j].IsEmpty())
			{
				uibutton.mainText.SetActive(false);
				uibutton.interactable = false;
			}
		}
		this.groupTab.Init(-1, null, false);
		EMono.ui.hud.frame.SetActive(EMono.game.altUI);
		this.inputSearch.onValueChanged.AddListener(new UnityAction<string>(this.Search));
		this.inputSearch.onSubmit.AddListener(new UnityAction<string>(this.Search));
		if (EMono.game.altUI)
		{
			EMono.Sound.SwitchPlaylist(EMono.Sound.playlistBuild, true);
		}
	}

	public void Search(string s)
	{
		s = s.ToLower();
		this.buttonClearSearch.SetActive(this.inputSearch.text != "");
		if (s == this.lastSearch)
		{
			return;
		}
		this.timerSearch = this.intervalSearch;
		this.lastSearch = s;
	}

	private void _Search()
	{
		string s = this.lastSearch;
		RecipeManager.BuildList();
		HashSet<Recipe> newRecipes = new HashSet<Recipe>();
		if (!s.IsEmpty())
		{
			foreach (RecipeSource recipeSource in RecipeManager.list)
			{
				if (!recipeSource.noListing && !recipeSource.isBridgePillar && (EMono.debug.godBuild || recipeSource.alwaysKnown || EMono.player.recipes.knownRecipes.ContainsKey(recipeSource.id)) && (recipeSource.row.GetSearchName(false).Contains(s) || recipeSource.row.GetSearchName(true).Contains(s) || recipeSource.id.Contains(s) || recipeSource.row.category.Contains(s)))
				{
					for (int i = 0; i < ((recipeSource.row.skins == null) ? 1 : (recipeSource.row.skins.Length + 1)); i++)
					{
						Recipe recipe = Recipe.Create(recipeSource, -1, null);
						recipe.idSkin = i;
						if (recipe == null)
						{
							Debug.Log(recipeSource.Name + "/" + recipeSource.id);
						}
						else
						{
							newRecipes.Add(recipe);
						}
					}
				}
			}
			foreach (Thing thing in EMono._map.Stocked.Things)
			{
				if (EMono._map.Stocked.ShouldListAsResource(thing) && (thing.source.name.Contains(s) || thing.source.name_JP.Contains(s)))
				{
					Recipe recipe2 = Recipe.Create(thing);
					if (recipe2 == null)
					{
						Debug.Log(thing.Name + "/" + thing.id);
					}
					else
					{
						newRecipes.Add(recipe2);
					}
				}
			}
			EMono.pc.things.Foreach(delegate(Thing t)
			{
				if (!t.trait.CanBeDropped || t.trait.CanOnlyCarry)
				{
					return;
				}
				if (t.source.name.Contains(s) || t.source.name_JP.Contains(s))
				{
					Recipe recipe3 = Recipe.Create(t);
					if (recipe3 == null)
					{
						Debug.Log(t.Name + "/" + t.id);
						return;
					}
					newRecipes.Add(recipe3);
				}
			}, true);
		}
		if (!newRecipes.SetEquals(this.searchRecipes))
		{
			this.searchRecipes = newRecipes;
			this.RefreshCategory(this.currentCat);
		}
		this.lastSearch = s;
	}

	public void ClearSearch()
	{
		this.ClearSearch(true);
	}

	public void ClearSearch(bool refresh)
	{
		SE.Click();
		this.inputSearch.text = "";
		this.timerSearch = 0f;
		this.lastSearch = "";
		if (refresh)
		{
			this.RefreshCategory(this.currentCat);
		}
	}

	public static void Show()
	{
		if (BuildMenu.Instance)
		{
			BuildMenu.Instance.SetActive(true);
		}
	}

	public static void Hide()
	{
		if (BuildMenu.Instance)
		{
			BuildMenu.Instance.SetActive(false);
		}
	}

	private void LateUpdate()
	{
		this.Refresh();
		if (this.timerSearch > 0f)
		{
			this.timerSearch -= Core.delta;
			if (this.timerSearch <= 0f)
			{
				this._Search();
			}
		}
	}

	public void Refresh()
	{
		ButtonGrid buttonGrid = InputModuleEX.GetComponentOf<ButtonGrid>();
		if (buttonGrid && buttonGrid.recipe == null && buttonGrid.area == null)
		{
			buttonGrid = null;
		}
		bool flag = EMono.scene.actionMode.ShouldHideBuildMenu || (EMono.scene.actionMode != ActionMode.Build && EMono.scene.actionMode != ActionMode.Inspect && EMono.scene.actionMode != ActionMode.EditArea);
		this.cg.SetActive(!flag, delegate(bool enabled)
		{
			if (enabled)
			{
				this.list.RefreshHighlight(false);
				this.groupTab.RefreshButtons();
			}
		});
		this.groupTab.SetActive(EMono.scene.actionMode != ActionMode.EditArea);
		this.transOption.anchoredPosition = (this.groupTab.gameObject.activeInHierarchy ? this.orgPosOption : this.posOption);
		if (this.info1.hideMode != flag || this.firstRefresh)
		{
			this.info1.hideMode = flag;
			if (this.moveInfo1)
			{
				this.info1.Rect().anchoredPosition = (flag ? this.posInfo1 : this.orgPos);
			}
		}
		if (buttonGrid && !flag)
		{
			this.info2.transform.position = buttonGrid.transform.position;
			this.info2.Rect().anchoredPosition = new Vector2(this.info2.Rect().anchoredPosition.x + this.posInfo2.x, this.posInfo2.y);
			if (buttonGrid.area != null)
			{
				this.info2.SetArea(buttonGrid.area);
			}
			else
			{
				this.info2.SetRecipe(buttonGrid.recipe);
			}
			this.info2.SetActive(true);
		}
		else
		{
			this.info2.SetActive(false);
		}
		this.firstRefresh = false;
	}

	public void SelectCategory(string cat)
	{
		if (!BuildMenu.dirtyCat && this.currentCat == cat)
		{
			return;
		}
		BuildMenu.dirtyCat = false;
		this.groupTab.Select(this.catDic[cat]);
		this.RefreshCategory(cat);
	}

	public void Select(AM_Picker.Result r)
	{
		EInput.Consume(false, 10);
		Debug.Log(EInput.skipFrame);
		this.ClearSearch(false);
		this.RefreshCategory(r.source.recipeCat);
		foreach (object obj in this.list.objects)
		{
			Recipe recipe = obj as Recipe;
			if (r.source == recipe.source && (r.thing == null || r.thing.idSkin == recipe.idSkin))
			{
				int index = this.list.GetIndex(recipe);
				if (index != -1)
				{
					this.list.dsv.scrollByItemIndex(index);
					this.list.Refresh();
					break;
				}
				break;
			}
		}
		foreach (ButtonGrid buttonGrid in this.list.GetComponentsInChildren<ButtonGrid>())
		{
			if (buttonGrid.recipe.source == r.source && (r.thing == null || r.thing.idSkin == buttonGrid.recipe.idSkin))
			{
				this.info1.lastMats[buttonGrid.recipe.id] = r.mat.id;
				buttonGrid.onClick.Invoke();
				return;
			}
		}
	}

	public void Unselect()
	{
		this.info1.SetRecipe(null);
		this.list.Select(null, false);
	}

	public void RefreshCategoryArea()
	{
		UIDynamicList uidynamicList = this.listArea;
		List<Area> list = new List<Area>();
		foreach (SourceArea.Row row in EMono.sources.areas.rows)
		{
			Area area = Area.Create(row.id);
			area.data.name = null;
			list.Add(area);
		}
		uidynamicList.Clear();
		object selected = null;
		uidynamicList.callbacks = new UIList.Callback<Area, ButtonGrid>
		{
			onClick = delegate(Area a, ButtonGrid b)
			{
				SE.Click();
				ActionMode.CreateArea.SetArea(a);
				if (EMono.scene.actionMode != ActionMode.CreateArea)
				{
					ActionMode.CreateArea.Activate(true, false);
				}
				EMono.ui.hud.hint.Refresh();
				this.info1.SetArea(a);
			},
			onRedraw = delegate(Area a, ButtonGrid b, int i)
			{
				b.SetRecipe(a);
				if (this.info1.area != null && a.source == this.info1.area.source)
				{
					selected = a;
				}
			}
		};
		foreach (Area area2 in list)
		{
			if (area2.isListable)
			{
				uidynamicList.Add(area2);
			}
		}
		uidynamicList.List();
	}

	public void RefreshCategory(string cat)
	{
		cat = cat.IsEmpty(this.categories[0]);
		EMono.player.pref.lastBuildCategory = cat;
		this.currentCat = cat;
		this.list.Clear();
		object selected = null;
		this.counts.Clear();
		Action<Thing> <>9__4;
		this.list.callbacks = new UIList.Callback<Recipe, ButtonGrid>
		{
			onClick = delegate(Recipe a, ButtonGrid b)
			{
				SE.Click();
				EMono.ui.hud.hint.Refresh();
				EMono.player.pref.lastBuildRecipe = a.source.id;
				if (a.UseStock && (a.ingredients[0].uid == 0 || a.ingredients[0].thing == null))
				{
					Debug.LogError("expection: invalid ingredient:" + a.ingredients[0].uid.ToString());
				}
				if (this.info1.recipe != a && !a.UseStock)
				{
					foreach (Recipe.Ingredient ingredient in a.ingredients)
					{
						ingredient.SetThing(null);
					}
				}
				if (a.UseStock && (a.ingredients[0].uid == 0 || a.ingredients[0].thing == null))
				{
					Debug.LogError("expection: invalid ingredient:" + a.ingredients[0].uid.ToString());
				}
				this.info1.SetRecipe(a);
				if (a.UseStock && (a.ingredients[0].uid == 0 || a.ingredients[0].thing == null))
				{
					Debug.LogError("expection: invalid ingredient:" + a.ingredients[0].uid.ToString());
				}
				a.OnChangeIngredient();
				ActionMode.Build.StartBuild(a, () => this.list.GetComp<ButtonGrid>(a));
				this.info1.SetRecipe(a);
				this.list.Select(a, false);
				if (a.UseStock && (a.ingredients[0].uid == 0 || a.ingredients[0].thing == null))
				{
					Debug.LogError("expection: invalid ingredient:" + a.ingredients[0].uid.ToString());
				}
			},
			onRedraw = delegate(Recipe a, ButtonGrid b, int i)
			{
				b.SetActive(a != null);
				if (a == null)
				{
					return;
				}
				b.SetRecipe(a);
				if (this.info1.recipe != null && a.id == this.info1.recipe.id)
				{
					selected = a;
				}
				a.BuildIngredientList();
			},
			onList = delegate(UIList.SortMode m)
			{
				foreach (string key in this.categories)
				{
					this.counts[key] = 0;
				}
				if (this.inputSearch.text != "")
				{
					foreach (Recipe recipe in this.searchRecipes)
					{
						if (recipe.UseStock)
						{
							Thing thing = recipe.ingredients[0].thing;
							if (thing == null || thing.isDestroyed || thing.ExistsOnMap || (thing.parentCard != null && thing.parentCard.c_lockLv > 0))
							{
								continue;
							}
						}
						this.list.Add(recipe);
					}
					return;
				}
				foreach (RecipeSource recipeSource in RecipeManager.list)
				{
					if (EMono.debug.godBuild || (!recipeSource.noListing && !recipeSource.row.tileType.EditorTile && EMono.player.recipes.IsKnown(recipeSource.id) && (recipeSource.row.factory.Length == 0 || !(recipeSource.row.factory[0] == "none"))))
					{
						Dictionary<string, int> dictionary = this.counts;
						string recipeCat = recipeSource.recipeCat;
						int i = dictionary[recipeCat];
						dictionary[recipeCat] = i + 1;
						if (!(recipeSource.recipeCat != cat))
						{
							for (int j = 0; j < ((recipeSource.row.skins == null) ? 0 : recipeSource.row.skins.Length) + 1; j++)
							{
								Recipe recipe2 = Recipe.Create(recipeSource, -1, null);
								recipe2.idSkin = j;
								this.list.Add(recipe2);
							}
						}
					}
				}
				if (!EMono.debug.hidePCItemsInBuild)
				{
					foreach (Thing thing2 in EMono._map.Stocked.Things)
					{
						if (EMono._map.Stocked.ShouldListAsResource(thing2))
						{
							Dictionary<string, int> dictionary2 = this.counts;
							string recipeCat = thing2.trait.RecipeCat;
							int i = dictionary2[recipeCat];
							dictionary2[recipeCat] = i + 1;
							if (thing2.trait.RecipeCat == cat)
							{
								Card rootCard = thing2.GetRootCard();
								if (rootCard != null && rootCard.c_lockLv == 0)
								{
									this.list.Add(thing2.trait.GetBuildModeRecipe());
								}
							}
						}
					}
					ThingContainer things = EMono.pc.things;
					Action<Thing> action;
					if ((action = <>9__4) == null)
					{
						action = (<>9__4 = delegate(Thing t)
						{
							if (t.trait.CanOnlyCarry || !t.trait.CanBeDropped || t.c_isImportant)
							{
								return;
							}
							Dictionary<string, int> dictionary3 = this.counts;
							string recipeCat2 = t.trait.RecipeCat;
							int num = dictionary3[recipeCat2];
							dictionary3[recipeCat2] = num + 1;
							if (t.trait.RecipeCat == cat && t.invY != 1)
							{
								this.list.Add(t.trait.GetBuildModeRecipe());
							}
						});
					}
					things.Foreach(action, true);
				}
				if (cat == "other")
				{
					this.list.objects.Sort((object a, object b) => string.Compare((a as Recipe).id, (b as Recipe).id));
				}
				else if (cat == "wall" || cat == "floor" || cat == "foundation" || cat == "obj")
				{
					this.list.objects.Sort((object a, object b) => (a as Recipe).renderRow.sort - (b as Recipe).renderRow.sort);
				}
				else
				{
					this.list.objects.Sort(delegate(object a, object b)
					{
						Recipe recipe3 = a as Recipe;
						Recipe recipe4 = b as Recipe;
						int num = recipe3.renderRow.sort - recipe4.renderRow.sort;
						if (num == 0)
						{
							RecipeCard recipeCard = recipe3 as RecipeCard;
							RecipeCard recipeCard2 = recipe4 as RecipeCard;
							if (recipeCard != null && recipeCard2 != null)
							{
								num = string.Compare(recipeCard.sourceCard._origin.IsEmpty(recipeCard.sourceCard.id), recipeCard2.sourceCard._origin.IsEmpty(recipeCard2.sourceCard.id));
							}
							else
							{
								num = string.Compare(recipe3.id, recipe4.id);
							}
							if (num == 0)
							{
								num = (recipe3.IngAsProduct ? recipe3.ingredients[0].uid : 0) - (recipe4.IngAsProduct ? recipe4.ingredients[0].uid : 0);
							}
						}
						return num;
					});
				}
				foreach (string text in this.categories)
				{
					if (!(text == "area") && !(text == ""))
					{
						if (this.counts[text] == 0)
						{
							this.tabs[text].subText.text = "";
						}
						else
						{
							this.tabs[text].subText.text = (this.counts[text].ToString() ?? "");
						}
					}
				}
			}
		};
		this.list.List();
		if (this.lastCat != this.currentCat)
		{
			this.list.Scroll(0);
		}
		this.groupTab.Select(this.catDic[cat]);
		if (selected != null)
		{
			this.list.Select(selected, false);
		}
		this.lastCat = this.currentCat;
		this.list.RebuildLayoutTo<BuildMenu>();
	}

	public void OnClickPicker()
	{
		ActionMode.Picker.Activate(true, false);
	}

	public GridLayoutGroup gridTab;

	public UISelectableGroup groupTab;

	public UIDynamicList list;

	public UIDynamicList listArea;

	public Dictionary<string, int> catDic = new Dictionary<string, int>();

	public UIRecipeInfo info1;

	public UIRecipeInfo info2;

	public RectTransform transOption;

	public RectTransform transMain;

	public Vector2 posInfo2;

	public Vector2 posInfo1;

	public Vector2 posOption;

	public CanvasGroup cg;

	public float hideSpeed;

	public float intervalSearch;

	public bool moveInfo1;

	public TerrainMenu terrainMenu;

	public InputField inputSearch;

	public string currentCat;

	public string lastCat;

	public UIButton buttonClearSearch;

	[NonSerialized]
	public string[] categories = new string[]
	{
		"wall",
		"floor",
		"foundation",
		"door",
		"furniture",
		"storage",
		"spot",
		"mount",
		"facility",
		"tool",
		"deco",
		"mech",
		"light",
		"junk",
		"ext",
		"goods",
		"food",
		"obj",
		"other",
		"area"
	};

	[NonSerialized]
	public ButtonRecipe lastButtonRecipe;

	public Dictionary<string, int> counts = new Dictionary<string, int>();

	public Dictionary<string, UIButton> tabs = new Dictionary<string, UIButton>();

	private Vector2 orgPos;

	private Vector2 orgPosOption;

	public static BuildMenu Instance;

	public UIButton buttonUndo;

	public static bool dirtyCat;

	private float timerSearch;

	public string lastSearch;

	public HashSet<Recipe> searchRecipes = new HashSet<Recipe>();

	private bool firstRefresh = true;

	public enum Mode
	{
		Build,
		None,
		Hide,
		Area,
		PartialMap
	}
}
