using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerCraftFloat : ELayer
{
	public override void OnInit()
	{
		LayerCraftFloat.Instance = this;
	}

	public override void OnSwitchContent(Window window)
	{
		if (!ELayer.core.IsGameStarted)
		{
			return;
		}
		ELayer.game.updater.recipe.Build(ELayer.pc.pos, RecipeUpdater.Mode.Immediate);
	}

	public void Update()
	{
		if (EInput.middleMouse.down)
		{
			ButtonGrid componentOf = InputModuleEX.GetComponentOf<ButtonGrid>();
			if (componentOf && componentOf.transform.IsChildOf(base.transform))
			{
				Recipe recipe = componentOf.recipe;
				if (recipe != null && this.windows[0].idTab == 0)
				{
					this.ShowContextMenu(recipe);
				}
			}
		}
		if (Input.GetMouseButton(0))
		{
			ButtonGrid componentOf2 = InputModuleEX.GetComponentOf<ButtonGrid>();
			if (componentOf2 && componentOf2.transform.IsChildOf(base.transform) && ELayer.player.waitingInput && ELayer.pc.HasNoGoal && componentOf2.recipe.id == this.idLastRecipe)
			{
				this.list.callbacks.OnClick(componentOf2.recipe, componentOf2);
				return;
			}
		}
		else
		{
			this.idLastRecipe = null;
		}
	}

	public void OnCompleteCraft()
	{
	}

	public void RefreshCraft()
	{
		this.list.callbacks = new UIList.Callback<Recipe, ButtonGrid>
		{
			onClick = delegate(Recipe a, ButtonGrid b)
			{
				ELayer.ui.RemoveLayers(false);
				ELayer.game.updater.recipe.Build(ELayer.pc.pos, RecipeUpdater.Mode.Validate);
				if (!RecipeUpdater.recipes.Contains(a))
				{
					SE.BeepSmall();
					ELayer.game.updater.recipe.Build(ELayer.pc.pos, RecipeUpdater.Mode.Immediate);
					return;
				}
				a.OnChangeIngredient();
				this.recipe = a;
				this.idLastRecipe = this.recipe.id;
				if (!ELayer.player.waitingInput)
				{
					SE.BeepSmall();
					return;
				}
				TaskCraft taskCraft = new TaskCraft
				{
					recipe = this.recipe,
					num = 1,
					repeat = false,
					floatMode = true
				};
				taskCraft.ResetReq();
				if (!taskCraft.IsIngredientsValid(false, taskCraft.num))
				{
					SE.Beep();
					return;
				}
				ELayer.pc.SetAI(taskCraft);
				ActionMode.Adv.SetTurbo(-1);
			},
			onRedraw = delegate(Recipe a, ButtonGrid b, int i)
			{
				b.SetCraftRecipe(a, ButtonGrid.Mode.RecipeGrid, true);
				if (!ELayer.player.recipes.hoveredRecipes.Contains(a.id))
				{
					b.Attach("recipe_new", false);
				}
				b.onRightClick = delegate()
				{
					this.ShowContextMenu(a);
				};
			},
			onList = delegate(UIList.SortMode m)
			{
				foreach (Recipe o in RecipeUpdater.recipes)
				{
					this.list.Add(o);
				}
			}
		};
		this.list.List();
		this.RefreshSize();
		this.list.dsv.OnResize();
	}

	public void ShowContextMenu(Recipe a)
	{
		UIContextMenu m = ELayer.ui.CreateContextMenuInteraction();
		Point point = ELayer.pc.pos.Copy();
		using (List<Recipe.Ingredient>.Enumerator enumerator = a.ingredients.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Recipe.Ingredient ing = enumerator.Current;
				UIContextMenu uicontextMenu = (a.ingredients.Count == 1) ? m : m.AddChild(ing.GetName() + "x" + ing.req.ToString());
				List<Thing> list = new List<Thing>();
				foreach (Thing thing in ELayer.pc.things)
				{
					thing.GetIngredients(ing, list);
				}
				for (int i = point.x - 1; i < point.x + 2; i++)
				{
					if (i >= 0 && i < ELayer._map.Size)
					{
						for (int j = point.z - 1; j < point.z + 2; j++)
						{
							if (j >= 0 && j < ELayer._map.Size)
							{
								Cell cell = ELayer._map.cells[i, j];
								if (cell.detail != null && cell.detail.things.Count != 0)
								{
									foreach (Thing thing2 in cell.detail.things)
									{
										thing2.GetIngredients(ing, list);
									}
								}
							}
						}
					}
				}
				using (List<Thing>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Thing t = enumerator2.Current;
						UIButton uibutton = Util.Instantiate<UIButton>(this.moldButtonContext, null);
						uicontextMenu.AddGameObject<UIButton>(uibutton);
						t.SetImage(uibutton.icon);
						Text mainText = uibutton.mainText;
						string name = t.Name;
						string str3;
						if (t.parent != ELayer.pc)
						{
							string str = "(";
							string str2;
							if (t.parent != ELayer._zone)
							{
								Card card = t.parent as Card;
								str2 = ((card != null) ? card.Name : null);
							}
							else
							{
								str2 = "ground".lang();
							}
							str3 = str + str2 + ")";
						}
						else
						{
							str3 = "";
						}
						mainText.text = name + str3;
						uibutton.onClick.AddListener(delegate()
						{
							ing.thing = t;
							SE.ClickOk();
							this.list.List();
							m.Hide();
						});
					}
				}
			}
		}
		m.Show();
	}

	public void RefreshDisassemble()
	{
		BaseList baseList = this.list;
		UIList.Callback<Thing, ButtonGrid> callback = new UIList.Callback<Thing, ButtonGrid>();
		callback.onClick = delegate(Thing a, ButtonGrid b)
		{
			if (!ELayer.player.waitingInput)
			{
				SE.BeepSmall();
				return;
			}
			a.Disassemble();
			ELayer.player.EndTurn(true);
		};
		callback.onRedraw = delegate(Thing a, ButtonGrid b, int i)
		{
			b.SetCard(a, ButtonGrid.Mode.RecipeGrid, delegate(UINote n)
			{
				RecipeSource recipeSource = RecipeManager.Get(a.id);
				if (recipeSource != null)
				{
					n.Space(0, 1);
					Recipe.Create(recipeSource, -1, null).WriteReqSkill(n);
				}
			});
			b.mainText.SetActive(false);
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (Thing o in RecipeUpdater.listD)
			{
				this.list.Add(o);
			}
		};
		baseList.callbacks = callback;
		this.list.List();
		this.RefreshSize();
		this.list.dsv.OnResize();
	}

	public void RefreshSize()
	{
		this.list.dsv.GetComponent<UIScrollView>().enabled = (this.list.ItemCount > 0);
		RectTransform rectTransform = this.windows[0].Rect();
		float x = rectTransform.sizeDelta.x;
		float num = this.list.Rect().sizeDelta.y;
		if (num > (float)this.maxSize)
		{
			num = (float)this.maxSize;
		}
		rectTransform.sizeDelta = new Vector2(x, num + (float)((this.list.ItemCount > 0) ? this.paddingY : this.paddingY2));
		rectTransform.RebuildLayout(false);
	}

	public static LayerCraftFloat Instance;

	public UIDynamicList list;

	public Recipe recipe;

	public UIButton moldButtonContext;

	public int maxSize;

	public int paddingY;

	public int paddingY2;

	public string idLastRecipe;
}
