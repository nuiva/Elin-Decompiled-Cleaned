using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIRecipeInfo : EMono
{
	public HitSummary summary
	{
		get
		{
			return EMono.screen.tileSelector.summary;
		}
	}

	public void Init()
	{
		DropdownRecipe.colorCost = this.colorCost;
		DropdownRecipe.colorPredict = this.colorPredict;
		this.SetRecipe(null);
	}

	public void SetArea(Area a)
	{
		if (a == null)
		{
			this.Clear();
			return;
		}
		if (this.main)
		{
			this.goInfo.SetActive(true);
			this.goIngredients.SetActive(false);
		}
		if (this.area == a)
		{
			return;
		}
		this.area = a;
		this.recipe = null;
		this.textName.text = a.Name;
		this.textDetail.SetActive(true);
		this.textDetail.SetText(a.source.GetDetail());
		this.note.Clear();
		this.note.Build();
		if (!this.main)
		{
			this.listIngredients.SetActive(false);
		}
		this.RefreshList();
		if (this.imageArea)
		{
			this.imageArea.SetActive(true);
		}
		if (this.listVariations)
		{
			this.listVariations.SetActive(false);
		}
		this.SetActive(true);
		this.RebuildLayout(true);
	}

	public void SetRecipe(Recipe r)
	{
		if (this.imageArea)
		{
			this.imageArea.SetActive(false);
		}
		this.area = null;
		if (r == null)
		{
			this.textDetail.SetActive(false);
			this.Clear();
			return;
		}
		if (this.main)
		{
			this.goInfo.SetActive(true);
			this.goIngredients.SetActive(r.ingredients.Count > 0 && !r.UseStock);
		}
		if (this.recipe == r)
		{
			return;
		}
		this.recipe = r;
		this.Refresh();
	}

	public void Clear()
	{
		this.recipe = null;
		this.area = null;
		if (this.main)
		{
			this.goIngredients.SetActive(false);
			this.goInfo.SetActive(false);
			if (this.listVariations)
			{
				this.listVariations.SetActive(false);
			}
		}
		if (this.textName.text != "")
		{
			this.textName.text = (this.textDetail.text = "");
			this.RefreshList();
			this.RefreshBalance();
			if (this.imageArea)
			{
				this.imageArea.SetActive(false);
			}
			this.note.Clear();
			this.note.Build();
			this.RebuildLayout(true);
		}
	}

	public void RefreshBalance()
	{
		if (!this.main)
		{
			return;
		}
		string text = Lang._Number(EMono.pc.GetCurrency("money"));
		if (this.summary.money != 0)
		{
			text = string.Concat(new string[]
			{
				text,
				"<color=",
				this.colorCost.ToHex(),
				">  -",
				this.summary.money.ToString(),
				"</color>"
			});
		}
		this.textCost.SetText(text);
		foreach (DropdownRecipe dropdownRecipe in this.dds)
		{
			dropdownRecipe.RefreshLabel();
		}
		if (this.ddList)
		{
			this.ddList.Redraw();
		}
		if (this.textCostSP)
		{
			int countValid = this.summary.countValid;
			int num = (countValid == 0) ? -1 : (this.recipe.source.GetSPCost(this.summary.factory) * countValid);
			text = ((num == -1) ? "-" : (num.ToString() ?? ""));
			DOTweenAnimation component = this.textCostSP.GetComponent<DOTweenAnimation>();
			Transform transform = this.textCostSP.Find("nerun");
			bool flag = num >= EMono.pc.stamina.value;
			if (component && !flag)
			{
				component.DOPause();
			}
			this.textCostSP.SetText(text, (num == -1) ? FontColor.Bad : ((num < EMono.pc.stamina.value) ? FontColor.Good : FontColor.Warning));
			if (component && flag)
			{
				component.tween.Restart(true, -1f);
			}
			if (transform)
			{
				transform.SetActive(flag);
			}
		}
	}

	public void RefreshImages()
	{
		Recipe recipe = this.recipe;
		if (this.listVariations)
		{
			this.listVariations.Redraw();
		}
		if (this.imageIcon)
		{
			if (recipe.UseStock && !recipe.VirtualBlock)
			{
				recipe.ingredients[0].thing.SetImage(this.imageIcon, recipe._dir, recipe.ingredients[0].thing.idSkin);
				return;
			}
			recipe.renderRow.SetImage(this.imageIcon, null, recipe.renderRow.GetColorInt(recipe.GetColorMaterial()), true, recipe._dir, recipe.idSkin);
		}
	}

	public void Refresh()
	{
		if (this.recipe == null)
		{
			return;
		}
		if (this.recipe.UseStock || this.recipe.IngAsProduct)
		{
			Thing thing = this.recipe.ingredients[0].thing;
			if (thing == null || thing.isDestroyed || thing.ExistsOnMap)
			{
				return;
			}
		}
		Recipe r = this.recipe;
		r.OnSelected();
		this.RefreshList();
		this.RefreshBalance();
		if (r.UseStock && r.ingredients[0].thing == null)
		{
			return;
		}
		this.textName.text = r.GetName();
		string text = "";
		int[] tiles = this.recipe.renderRow.tiles;
		if (EMono.debug.showExtra)
		{
			text = string.Concat(new string[]
			{
				text,
				"(",
				this.recipe.id,
				"  ",
				this.recipe.renderRow.idRenderData,
				"/",
				((tiles.Length != 0) ? tiles[0] : -1).ToString(),
				")",
				Environment.NewLine
			});
		}
		this.textDetail.SetActive(true);
		if (this.CraftMode)
		{
			if (this.main)
			{
				EMono.pc.things.GetThingStack(r.GetIdThing(), r.GetRefVal());
				int num = EMono.player.recipes.knownRecipes.TryGetValue(r.id, 0);
				this.textInfo.SetText("recipe_lv".lang(num.ToString() ?? "", null, null, null, null));
				text += r.GetDetail();
			}
			else
			{
				text += r.GetDetail();
				r.WriteNote(this.note);
			}
		}
		else
		{
			string detail = r.GetDetail();
			if (!detail.IsEmpty())
			{
				text = text + detail + "\n\n";
			}
			text = string.Concat(new string[]
			{
				text,
				"( ",
				r.tileType.LangPlaceType.lang(),
				" ) ",
				(r.tileType.LangPlaceType + "_hint").lang()
			});
			if (r.UseStock && r.ingredients[0].thing != null)
			{
				r.ingredients[0].thing.WriteNote(this.note, null, IInspect.NoteMode.Recipe, null);
			}
			else
			{
				r.WriteNote(this.note);
			}
			if (!r.UseStock && r.source.NeedFactory)
			{
				PropSet propSet = EMono._map.Installed.cardMap.TryGetValue(r.source.idFactory, null);
				if (propSet == null || propSet.Count == 0)
				{
					this.note.Space(8, 1);
					this.note.AddText("noFactory".lang(r.source.NameFactory, null, null, null, null), FontColor.Bad);
					this.note.Build();
				}
			}
		}
		this.textDetail.SetText(text);
		if (this.goMoney)
		{
			this.goMoney.transform.SetAsLastSibling();
		}
		if (this.main)
		{
			if (this.listVariations)
			{
				UIList uilist = this.listVariations;
				bool flag = r.CanRotate && r.renderRow != null;
				int[] array = flag ? r.renderRow._tiles : null;
				if (flag && array.Length <= 1)
				{
					flag = false;
				}
				uilist.SetActive(flag);
				if (flag)
				{
					uilist.Clear();
					BaseList baseList = uilist;
					UIList.Callback<UIRecipeInfo.RecipeVariation, ButtonGrid> callback = new UIList.Callback<UIRecipeInfo.RecipeVariation, ButtonGrid>();
					callback.onClick = delegate(UIRecipeInfo.RecipeVariation a, ButtonGrid b)
					{
						r.SetDir(a.dir);
					};
					callback.onInstantiate = delegate(UIRecipeInfo.RecipeVariation a, ButtonGrid b)
					{
						b.SetRecipeVariation(a);
					};
					callback.onRedraw = delegate(UIRecipeInfo.RecipeVariation a, ButtonGrid b, int i)
					{
						b.SetRecipeVariation(a);
					};
					baseList.callbacks = callback;
					for (int j = 0; j < array.Length; j++)
					{
						uilist.Add(new UIRecipeInfo.RecipeVariation
						{
							recipe = r,
							dir = j
						});
					}
					uilist.Refresh(false);
					uilist.group.Init(r._dir, null, false);
				}
			}
			if (this.goMoney)
			{
				this.goMoney.SetActive(!this.CraftMode);
			}
		}
		else
		{
			this.listIngredients.SetActive(!this.recipe.UseStock || this.recipe.VirtualBlock);
			if (this.goMoney)
			{
				this.goMoney.SetActive(r.CostMoney != 0);
			}
		}
		this.RefreshImages();
		this.SetActive(true);
		this.RebuildLayout(true);
	}

	public void OnRotate()
	{
		if (this.listVariations.gameObject.activeSelf)
		{
			this.listVariations.group.Select(this.recipe._dir);
		}
		this.RefreshImages();
	}

	public void RefreshQuality()
	{
		if (!this.textQuality)
		{
			return;
		}
		this.recipe.SetTextDifficulty(this.textQuality);
	}

	public void RefreshList()
	{
		if (this.textReqSkill)
		{
			string str = "";
			FontColor c = FontColor.Good;
			if (this.recipe != null)
			{
				Element reqSkill = this.recipe.source.GetReqSkill();
				int value = EMono.pc.elements.GetOrCreateElement(reqSkill).Value;
				reqSkill.Name + " " + reqSkill.Value.ToString();
				str = "reqSkill2".lang(reqSkill.Name, reqSkill.Value.ToString() ?? "", value.ToString() ?? "", null, null);
				if (value < reqSkill.Value)
				{
					c = FontColor.Warning;
				}
			}
			this.textReqSkill.SetText(str.IsEmpty("noReqSkill".lang()), c);
		}
		if (this.ddFactory)
		{
			List<GridItem> list = new List<GridItem>();
			if (this.recipe.source.NeedFactory)
			{
				GridItemCardSource gridItemCardSource = new GridItemCardSource
				{
					source = EMono.sources.cards.map[this.recipe.source.idFactory]
				};
				if (this.factory != null && this.recipe.source.idFactory == this.factory.id)
				{
					gridItemCardSource.thing = this.factory;
				}
				list.Add(gridItemCardSource);
			}
			this.ddFactory.Build(list);
		}
		this.RefreshQuality();
		if (this.ddList)
		{
			this.ddList.SetActive(this.recipe != null && !this.recipe.UseStock);
			if (this.recipe == null || this.recipe.UseStock)
			{
				return;
			}
			this.ddList.recipeInfo = this;
			this.ddList.lastMats = this.lastMats;
			this.ddList.BuildIngredients(this.recipe, this.imageIcon, new Action(this.RefreshQuality), this.searchMode);
			return;
		}
		else
		{
			Recipe r = this.recipe;
			UIList uilist = this.listIngredients;
			uilist.Clear();
			this.dds.Clear();
			if (r == null || r.UseStock)
			{
				return;
			}
			BaseList baseList = uilist;
			UIList.Callback<Recipe.Ingredient, UIItem> callback = new UIList.Callback<Recipe.Ingredient, UIItem>();
			callback.onClick = delegate(Recipe.Ingredient a, UIItem b)
			{
			};
			callback.onInstantiate = delegate(Recipe.Ingredient ingredient, UIItem b)
			{
				DropdownRecipe dropdownRecipe = b.dd as DropdownRecipe;
				dropdownRecipe.recipe = r;
				this.dds.Add(dropdownRecipe);
				ThingStack thingStack = EMono._map.Stocked.ListThingStack(ingredient, this.searchMode);
				int num = thingStack.count;
				Color color = (num >= ingredient.req) ? SkinManager.CurrentColors.textGood : SkinManager.CurrentColors.textBad;
				List<string> list2 = new List<string>();
				list2.Add(ingredient.id);
				dropdownRecipe.SetList<string>(0, list2, (string n, int i) => string.Concat(new string[]
				{
					"<color=",
					color.ToHex(),
					">",
					ingredient.GetName(),
					" x ",
					ingredient.req.ToString(),
					"  (",
					num.ToString(),
					")</color>"
				}), delegate(int i, string v)
				{
				}, true);
				dropdownRecipe.textLabel.text = dropdownRecipe.orgLabel.text;
				if (this.textCost)
				{
					this.textCost.text = (r.CostMoney.ToString() ?? "");
				}
			};
			baseList.callbacks = callback;
			foreach (Recipe.Ingredient o in r.ingredients)
			{
				uilist.Add(o);
			}
			uilist.Refresh(false);
			return;
		}
	}

	public UIText textName;

	public UIText textDetail;

	public UIText textCost;

	public UIText textInfo;

	public UIText textQuality;

	public UIText textReqSkill;

	public UIText textCostSP;

	public Thing factory;

	public Recipe recipe;

	public Area area;

	public UIList listIngredients;

	public UIList listVariations;

	public Image imageArea;

	public Image imageIcon;

	public UINote note;

	public Color colorCost;

	public Color colorPredict;

	public GameObject goMoney;

	public GameObject goIngredients;

	public GameObject goInfo;

	public bool main;

	public bool CraftMode;

	public StockSearchMode searchMode;

	public DropdownGrid ddList;

	public DropdownGrid ddFactory;

	public ButtonGrid buttonProduct;

	[NonSerialized]
	public Dictionary<string, int> lastMats = new Dictionary<string, int>();

	[NonSerialized]
	public bool hideMode = true;

	[NonSerialized]
	public List<DropdownRecipe> dds = new List<DropdownRecipe>();

	public class RecipeVariation
	{
		public Recipe recipe;

		public int dir;
	}
}
