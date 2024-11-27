using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownGrid : EMono
{
	private void Awake()
	{
		this.rectDrop.SetActive(false);
	}

	private void Update()
	{
		this.rectDrop.SetActive(DropdownGrid.IsActive);
	}

	public void Build(List<GridItem> items)
	{
		this.list.Clear();
		BaseList baseList = this.list;
		UIList.Callback<GridItem, ButtonGrid> callback = new UIList.Callback<GridItem, ButtonGrid>();
		callback.onInstantiate = delegate(GridItem a, ButtonGrid b)
		{
			a.SetButton(b);
		};
		callback.onClick = delegate(GridItem a, ButtonGrid b)
		{
			a.OnClick(b);
		};
		callback.onRedraw = delegate(GridItem a, ButtonGrid b, int i)
		{
			a.SetButton(b);
		};
		baseList.callbacks = callback;
		for (int j = 0; j < this.slots; j++)
		{
			if (j < items.Count)
			{
				this.list.Add(items[j]);
			}
			else
			{
				this.list.Add(new GridItem());
			}
		}
		this.list.Refresh(false);
	}

	public List<Thing> ListIngredients(Recipe.Ingredient ingredient, StockSearchMode searchMode)
	{
		return EMono._map.Stocked.ListThingStack(ingredient, searchMode).list;
	}

	public void BuildIngredients(Recipe _recipe, Image _icon, Action _onValueChange, StockSearchMode _searchMode)
	{
		this.recipe = _recipe;
		this.icon = _icon;
		this.list.Clear();
		this.onValueChange = _onValueChange;
		this.searchMode = _searchMode;
		if (this.recipe == null)
		{
			return;
		}
		this.list.callbacks = new UIList.Callback<Recipe.Ingredient, ButtonGrid>
		{
			onInstantiate = delegate(Recipe.Ingredient ingredient, ButtonGrid b)
			{
				if (ingredient.id.IsEmpty())
				{
					b.SetIngredient(this.recipe, ingredient);
					return;
				}
				List<Thing> things = this.ListIngredients(ingredient, this.searchMode);
				if (things.Count == 0)
				{
					if (EMono.debug.godBuild && ingredient.thing == null)
					{
						Thing thing = ThingGen.Create(ingredient.IdThing, this.recipe.DefaultMaterial.alias).SetNum(99);
						things.Add(thing);
						ingredient.SetThing(thing);
					}
					else
					{
						ingredient.SetThing(null);
						things.Insert(0, null);
					}
				}
				else if (!ingredient.optional)
				{
					if (ingredient.thing == null)
					{
						int num = this.lastMats.TryGetValue(this.recipe.id, -1);
						if (num != -1)
						{
							foreach (Thing thing2 in things)
							{
								if (thing2.material.id == num && thing2.Num >= ingredient.req)
								{
									ingredient.SetThing(thing2);
									break;
								}
							}
						}
						if (ingredient.thing == null)
						{
							SourceMaterial.Row defaultMaterial = this.recipe.DefaultMaterial;
							foreach (Thing thing3 in things)
							{
								if (thing3.material.id == defaultMaterial.id && thing3.Num >= ingredient.req)
								{
									ingredient.SetThing(thing3);
									break;
								}
							}
							if (EMono.debug.godBuild && ingredient.thing == null)
							{
								Thing thing4 = (ingredient.useCat ? ThingGen.CreateFromCategory(ingredient.id, -1) : ThingGen.Create(ingredient.id, defaultMaterial.alias)).SetNum(99);
								things.Add(thing4);
								ingredient.SetThing(thing4);
							}
						}
						if (ingredient.thing == null)
						{
							ingredient.SetThing(things[0]);
						}
					}
					else
					{
						bool flag = true;
						foreach (Thing thing5 in things)
						{
							if (ingredient.thing == thing5)
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							ingredient.SetThing(things[0]);
						}
					}
				}
				b.SetIngredient(this.recipe, ingredient);
				b.onClick.RemoveAllListeners();
				b.onClick.AddListener(delegate()
				{
					things = this.ListIngredients(ingredient, this.searchMode);
					if (ingredient.optional)
					{
						if (things.Count == 0 || things[0] == null)
						{
							SE.Beep();
							return;
						}
					}
					else if (ingredient.thing == null)
					{
						SE.Beep();
						return;
					}
					if (this.rectDrop)
					{
						this.Activate(ingredient, things);
					}
				});
			},
			onRedraw = delegate(Recipe.Ingredient a, ButtonGrid b, int i)
			{
				b.SetIngredient(this.recipe, a);
			}
		};
		for (int j = 0; j < this.slots; j++)
		{
			if (j < this.recipe.ingredients.Count)
			{
				this.list.Add(this.recipe.ingredients[j]);
			}
			else
			{
				this.list.Add(new Recipe.Ingredient
				{
					id = ""
				});
			}
		}
		this.list.Refresh(false);
	}

	public void Redraw()
	{
		this.list.Redraw();
	}

	public void Activate(Recipe.Ingredient ingredient, List<Thing> things)
	{
		DropdownGrid.IsActive = true;
		DropdownGrid.Instance = this;
		this.rectDrop.SetActive(true);
		this.rectDrop.SetParent(EMono.ui.transform);
		this.rectDrop.sizeDelta = new Vector2(0f, 0f);
		this.rectDrop.anchoredPosition = Vector3.zero;
		this.listDrop.callbacks = new UIList.Callback<Thing, ButtonGrid>
		{
			onClick = delegate(Thing a, ButtonGrid b)
			{
				SE.Resource();
				ingredient.SetThing(a);
				this.OnChangeIngredient();
				if (this.onValueChange != null)
				{
					this.onValueChange();
				}
				this.Deactivate();
				if (LayerCraft.Instance)
				{
					LayerCraft.Instance.OnChangeIngredient();
				}
			},
			onInstantiate = delegate(Thing a, ButtonGrid b)
			{
				b.SetIngredient(this.recipe, a);
				b.SetOnClick(delegate
				{
				});
				b.onRightClick = delegate()
				{
				};
			}
		};
		this.listDrop.Clear();
		foreach (Thing o in things)
		{
			this.listDrop.Add(o);
		}
		this.listDrop.Refresh(false);
		this.rectDropContent.position = this.list.transform.position + this.fixPos;
		if (this.listDrop.items.Count == 0)
		{
			SE.Beep();
			this.Deactivate();
			return;
		}
		if (this.soundPop)
		{
			this.soundPop.Play();
		}
	}

	public void OnChangeIngredient()
	{
		if (this.recipe == null)
		{
			return;
		}
		this.recipe.OnChangeIngredient();
		if (!this.rectDrop)
		{
			return;
		}
		if (this.icon)
		{
			this.recipe.renderRow.SetImage(this.icon, null, this.recipe.renderRow.GetColorInt(this.recipe.GetColorMaterial()), true, 0, this.recipe.idSkin);
		}
		this.list.Redraw();
	}

	public void Deactivate()
	{
		DropdownGrid.IsActive = false;
		if (this.rectDrop != null)
		{
			this.rectDrop.SetActive(false);
		}
	}

	public void OnDestroy()
	{
		DropdownGrid.IsActive = false;
		UnityEngine.Object.DestroyImmediate(this.rectDrop.gameObject);
	}

	public void TrySelect(int a)
	{
		ButtonGrid componentOf = InputModuleEX.GetComponentOf<ButtonGrid>();
		if (!componentOf || componentOf.ing == null)
		{
			return;
		}
		Recipe.Ingredient ing = componentOf.ing;
		List<Thing> list = EMono._map.Stocked.ListThingStack(ing, this.searchMode).list;
		int num = list.IndexOf(ing.thing);
		if (num == -1)
		{
			return;
		}
		SE.Resource();
		num += a;
		if (num < 0)
		{
			num = 0;
		}
		if (num >= list.Count)
		{
			num = list.Count - 1;
		}
		ing.SetThing(list[num]);
		this.OnChangeIngredient();
		TooltipManager.Instance.ResetTooltips();
		componentOf.ShowTooltip();
		if (this.recipeInfo)
		{
			this.recipeInfo.RefreshImages();
		}
		if (LayerCraft.Instance)
		{
			LayerCraft.Instance.RefreshProduct();
		}
	}

	public void Next()
	{
		this.TrySelect(1);
	}

	public void Prev()
	{
		this.TrySelect(-1);
	}

	public static bool IsActive;

	public static DropdownGrid Instance;

	public UIList list;

	public UIList listDrop;

	public RectTransform rectDrop;

	public RectTransform rectDropContent;

	public RectTransform pivot;

	public Recipe recipe;

	public Image icon;

	public SoundData soundPop;

	public Action onValueChange;

	public int slots = 4;

	public Vector3 fixPos;

	public StockSearchMode searchMode;

	public UIRecipeInfo recipeInfo;

	[NonSerialized]
	public Dictionary<string, int> lastMats = new Dictionary<string, int>();
}
