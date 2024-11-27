using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LayerCraft : LayerBaseCraft
{
	public override void RefreshCurrentGrid()
	{
		this.RefreshRecipe(true);
	}

	public override void ClearButtons()
	{
		this.RefreshRecipe(true);
	}

	public override string GetTextHeader(Window window)
	{
		return null;
	}

	public override void OnAfterInit()
	{
		this.info1.searchMode = StockSearchMode.AroundPC;
		this.info1.ddList.pivot.SetActive(true);
		LayerCraft.Instance = this;
	}

	public override void OnKill()
	{
		if (this.workbenchCrafted)
		{
			Tutorial.Play("middle_click");
		}
		if (this.pickaxeCrafted)
		{
			Tutorial.Play("hardness");
		}
		string key = (this.factory == null) ? "hand" : this.factory.id;
		ELayer.player.lastRecipes[key] = this.recipe.id;
	}

	public void SetFactory(Thing t)
	{
		this.factory = t;
		this.RefreshCategory("all", true);
		bool flag = t != null && t.trait.IsRequireFuel;
		this.transFuel.SetActive(flag);
		if (flag)
		{
			Action <>9__2;
			this.buttonRefuel.SetOnClick(delegate
			{
				foreach (Window window in this.windows)
				{
					window.SetInteractable(false, 0f);
				}
				Layer layer = LayerDragGrid.Create(new InvOwnerRefuel(t, null, CurrencyType.None), false);
				Action onKill;
				if ((onKill = <>9__2) == null)
				{
					onKill = (<>9__2 = delegate()
					{
						if (!this.isDestroyed)
						{
							this.OnEndCraft();
						}
					});
				}
				layer.SetOnKill(onKill);
			});
			this.buttonAutoRefuel.SetOnClick(delegate
			{
				SE.Click();
				t.autoRefuel = !t.autoRefuel;
				this.RefreshProduct();
			});
		}
	}

	public void OnClickCraft()
	{
		Dictionary<Thing, int> dictionary = new Dictionary<Thing, int>();
		foreach (Recipe.Ingredient ingredient in this.recipe.ingredients)
		{
			if (ingredient.thing != null)
			{
				if (!dictionary.ContainsKey(ingredient.thing))
				{
					dictionary.Add(ingredient.thing, 0);
				}
				Dictionary<Thing, int> dictionary2 = dictionary;
				Thing thing = ingredient.thing;
				dictionary2[thing] += ingredient.req * this.inputNum.Num;
			}
		}
		foreach (KeyValuePair<Thing, int> keyValuePair in dictionary)
		{
			if (keyValuePair.Key.Num < keyValuePair.Value)
			{
				SE.Beep();
				Msg.Say("craftDupError");
				return;
			}
		}
		if (this.inputNum.Num == 0)
		{
			SE.Beep();
			return;
		}
		Thing thing2 = this.factory;
		TraitCrafter traitCrafter = ((thing2 != null) ? thing2.trait : null) as TraitCrafter;
		if (traitCrafter == null)
		{
			traitCrafter = Trait.SelfFactory;
			traitCrafter.owner = ELayer.pc;
		}
		(traitCrafter as TraitFactory).recipe = this.recipe;
		string id = this.recipe.id;
		if (!(id == "workbench"))
		{
			if (id == "axe" || id == "hammer" || id == "pickaxe")
			{
				this.pickaxeCrafted = true;
			}
		}
		else
		{
			this.workbenchCrafted = true;
		}
		ELayer.pc.SetAI(new AI_UseCrafter
		{
			crafter = traitCrafter,
			layer = this,
			recipe = this.recipe,
			num = this.inputNum.Num
		});
		ActionMode.Adv.SetTurbo(-1);
		base.gameObject.SetActive(false);
	}

	public override List<Thing> GetTargets()
	{
		List<Thing> list = new List<Thing>();
		foreach (Recipe.Ingredient ingredient in this.recipe.ingredients)
		{
			if (!ingredient.optional || (ingredient.thing != null && !ingredient.thing.isDestroyed))
			{
				list.Add(ingredient.thing);
			}
		}
		return list;
	}

	public override int GetReqIngredient(int index)
	{
		return this.recipe.ingredients[index].req * this.inputNum.Num;
	}

	public override void OnEndCraft()
	{
		this.OnCompleteCraft();
	}

	public void OnCompleteCraft()
	{
		base.CancelInvoke("WaitUntilIdle");
		EInput.haltInput = true;
		TweenUtil.Tween(this.waitComplete, null, delegate()
		{
			EInput.haltInput = false;
			base.gameObject.SetActive(true);
			foreach (Window window in this.windows)
			{
				window.SetInteractable(true, 0.5f);
			}
			this.list.Redraw();
			this.RefreshRecipe(true);
			this.windowList.groupTab.RefreshButtons();
			this.list.Select<Recipe>((Recipe r) => this.recipe.id == r.id, false);
		});
	}

	public void WaitUntilIdle()
	{
		if (!ActionMode.Adv.IsActive || ELayer.pc.HasNoGoal)
		{
			base.CancelInvoke();
			this.Close();
		}
	}

	private void Update()
	{
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (Input.GetMouseButton(0))
		{
			this.wcount = 2;
		}
		if (axis != 0f)
		{
			this.wcount = 2;
			return;
		}
		if (this.wcount > 0)
		{
			this.wcount--;
			return;
		}
		this.RefreshInfo();
	}

	public void RefreshInfo()
	{
		ButtonGrid buttonGrid = InputModuleEX.GetComponentOf<ButtonGrid>();
		if (buttonGrid && buttonGrid.recipe == null)
		{
			buttonGrid = null;
		}
		if (buttonGrid == this.lastB)
		{
			return;
		}
		this.lastB = buttonGrid;
	}

	public void RefreshCategory(string cat, bool first = false)
	{
		Dictionary<string, int> cats = new Dictionary<string, int>();
		RecipeManager.BuildList();
		BaseList baseList = this.list;
		UIList.Callback<Recipe, ButtonGrid> callback = new UIList.Callback<Recipe, ButtonGrid>();
		callback.onClick = delegate(Recipe a, ButtonGrid b)
		{
			this.recipe = a;
			SE.Play("click_recipe");
			this.RefreshRecipe(true);
			ELayer.player.recipes.hoveredRecipes.Add(a.id);
			b.Dettach("recipe_new");
			this.list.Select(a, false);
		};
		callback.onRedraw = delegate(Recipe a, ButtonGrid b, int i)
		{
			if (a.ingredients.Count == 0)
			{
				a.BuildIngredientList();
			}
			b.SetCraftRecipe(a, ButtonGrid.Mode.Recipe, false);
			if (!ELayer.player.recipes.hoveredRecipes.Contains(a.id))
			{
				b.Attach("recipe_new", false);
			}
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			this.newRecipes.Clear();
			foreach (RecipeSource recipeSource in ELayer.player.recipes.ListSources(this.factory, this.newRecipes))
			{
				if (cat == "all" || recipeSource.row.Category.IsChildOf(cat))
				{
					Recipe o2 = Recipe.Create(recipeSource, -1, null);
					this.list.Add(o2);
				}
				SourceCategory.Row row = recipeSource.row.Category.GetSecondRoot();
				if (row.id != "lightsource" && row.IsChildOf("armor"))
				{
					row = ELayer.sources.categories.map["armor"];
				}
				Dictionary<string, int> cats;
				if (!cats.ContainsKey(row.id))
				{
					cats.Add(row.id, 1);
				}
				else
				{
					cats = cats;
					string id = row.id;
					int num = cats[id];
					cats[id] = num + 1;
				}
			}
			this.list.objects.Sort((object a, object b) => (b as Recipe).GetSortVal() - (a as Recipe).GetSortVal());
			int count = this.newRecipes.Count;
		};
		callback.onSort = ((Recipe a, UIList.SortMode m) => a.GetSortVal());
		baseList.callbacks = callback;
		this.list.sortMode = UIList.SortMode.ByLevel;
		this.list.List();
		if (cats.Count > 1 && !this.tabBuilt)
		{
			this.windowList.AddTab("all", null, delegate
			{
				this.RefreshCategory("all", false);
			}, null, null);
			using (Dictionary<string, int>.Enumerator enumerator = cats.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, int> c = enumerator.Current;
					this.windowList.AddTab(ELayer.sources.categories.map[c.Key].GetName() + "(" + c.Value.ToString() + ")", null, delegate
					{
						this.RefreshCategory(c.Key, false);
					}, null, null);
				}
			}
			this.tabBuilt = true;
			this.windowList.BuildTabs(0);
		}
		this.list.Scroll(0);
		if (this.list.rows.Count > 0)
		{
			object o = this.list.rows[0].objects[0];
			if (first)
			{
				string key = (this.factory == null) ? "hand" : this.factory.id;
				if (ELayer.player.lastRecipes.ContainsKey(key))
				{
					string b2 = ELayer.player.lastRecipes[key];
					foreach (object obj in this.list.objects)
					{
						if ((obj as Recipe).id == b2)
						{
							o = obj;
							break;
						}
					}
				}
				this.list.Scroll(o);
				if (!this.list.Select(o, true))
				{
					this.list.Scroll(0);
					this.list.Select(this.list.rows[0].objects[0], false);
					return;
				}
			}
			else
			{
				this.list.Select(o, true);
			}
		}
	}

	public void OnChangeIngredient()
	{
		this.RefreshRecipe(true);
	}

	public void RefreshRecipe(bool first)
	{
		foreach (Recipe.Ingredient ingredient in this.recipe.ingredients)
		{
			if (this.info1.recipe != this.recipe || (!ingredient.optional && (ingredient.thing == null || ingredient.thing.isDestroyed)))
			{
				ingredient.SetThing(null);
			}
		}
		if (first)
		{
			this.toggleCraftTo.SetToggle(false, null);
			this.toggleRepeat.SetToggle(false, null);
			this.inputNum.Num = 1;
			this.info1.recipe = null;
		}
		this.info1.factory = this.factory;
		this.info1.SetRecipe(this.recipe);
		this.recipe.OnChangeIngredient();
		this.RefreshProduct();
		this.RefreshTrackButton();
	}

	public void RefreshTrackButton()
	{
		QuestTrackCraft quest = null;
		foreach (Quest quest2 in ELayer.game.quests.list)
		{
			if (quest2 is QuestTrackCraft)
			{
				quest = (quest2 as QuestTrackCraft);
				break;
			}
		}
		this.buttonTrack.SetOnClick(delegate
		{
			if (quest != null && quest.idRecipe == this.recipe.id)
			{
				ELayer.game.quests.Remove(quest);
			}
			else
			{
				if (quest != null)
				{
					ELayer.game.quests.Remove(quest);
				}
				QuestTrackCraft questTrackCraft = Quest.Create("track_craft", null, null) as QuestTrackCraft;
				questTrackCraft.SetRecipe(this.recipe);
				ELayer.game.quests.Start(questTrackCraft);
			}
			if (!WidgetQuestTracker.Instance)
			{
				ELayer.player.questTracker = true;
				ELayer.ui.widgets.ActivateWidget("QuestTracker");
				WidgetHotbar.RefreshButtons();
			}
			WidgetQuestTracker.Instance.Refresh();
			this.RefreshTrackButton();
		});
		this.buttonTrack.icon.SetActive(quest != null && quest.idRecipe == this.recipe.id);
	}

	public void RefreshInputNum()
	{
		this.inputNum.SetMinMax(1, this.recipe.GetMaxCount());
		this.inputNum.onValueChanged = delegate(int n)
		{
			this.RefreshProduct();
		};
		this.inputNum.Validate();
	}

	public void RefreshProduct()
	{
		this.RefreshInputNum();
		ELayer.screen.tileSelector.summary.countValid = this.inputNum.Num;
		ELayer.screen.tileSelector.summary.factory = this.factory;
		this.info1.RefreshBalance();
		if (this.factory != null)
		{
			this.textFuel.text = "craftFuel".lang(((int)((float)this.factory.c_charges / (float)this.factory.trait.MaxFuel * 100f)).ToString() ?? "", null, null, null, null);
			this.buttonAutoRefuel.mainText.text = (this.factory.autoRefuel ? "On" : "Off");
			this.buttonAutoRefuel.icon.SetAlpha(this.factory.autoRefuel ? 1f : 0.4f);
		}
		this.RefreshQuality();
		List<Thing> list = new List<Thing>();
		foreach (Recipe.Ingredient ingredient in this.recipe.ingredients)
		{
			if (ingredient.thing != null)
			{
				list.Add(ingredient.thing);
			}
		}
		Thing thing = this.recipe.Craft(BlessedState.Normal, false, list, true);
		thing.SetNum(this.inputNum.Num);
		if (thing.sockets != null)
		{
			thing.sockets.Clear();
		}
		if (thing.IsEquipmentOrRanged)
		{
			foreach (Element element in thing.elements.dict.Values.ToList<Element>())
			{
				if (!element.IsTrait)
				{
					thing.elements.Remove(element.id);
				}
			}
		}
		this.info1.buttonProduct.SetCard(thing, ButtonGrid.Mode.Default, null);
		thing.WriteNote(this.info1.note, null, IInspect.NoteMode.Product, this.recipe);
		this.product = thing;
	}

	public bool IsQualityMet()
	{
		return true;
	}

	public void RefreshQuality()
	{
	}

	public void OnClickExamine()
	{
		ELayer.ui.AddLayer<LayerInfo>().Set(this.product, false);
	}

	public static LayerCraft Instance;

	public UIDynamicList list;

	public UIRecipeInfo info1;

	public UIRecipeInfo info2;

	public Vector3 posInfo;

	public Thing factory;

	public Thing product;

	public Recipe recipe;

	public UIButton toggleRepeat;

	public UIButton toggleCraftTo;

	public UIButton buttonTrack;

	public UIInputText inputNum;

	public UIInputText moldInputNum;

	public List<RecipeSource> newRecipes = new List<RecipeSource>();

	public Window windowList;

	public bool showInfo;

	private bool pickaxeCrafted;

	private bool workbenchCrafted;

	public UIButton buttonRefuel;

	public UIButton buttonAutoRefuel;

	public Transform transFuel;

	public UIText textFuel;

	public UIText textQualityInvalid;

	public UIText textQualityValid;

	public Image imageQualityReq;

	public Image imageQualityCurrent;

	public float waitComplete;

	public float qualityBarWidth;

	private int wcount;

	public ButtonGrid lastB;

	private bool tabBuilt;
}
