using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGrid : UIButton, IMouseHint, IPrefImage
{
	public ColorProfile Colors
	{
		get
		{
			return Core.Instance.Colors;
		}
	}

	public Card Card
	{
		get
		{
			if (this.card == null || this.card.isDestroyed)
			{
				return null;
			}
			return this.card;
		}
	}

	public virtual float extFixY
	{
		get
		{
			return 0f;
		}
	}

	public override string ToString()
	{
		Card card = this.card;
		if (card == null)
		{
			return null;
		}
		return card.ToString();
	}

	public T Attach<T>(string id, bool rightAttach = true) where T : Component
	{
		return this.Attach(id, rightAttach).GetComponent<T>();
	}

	public Transform Attach(string id, bool rightAttach = true)
	{
		Transform transform = PoolManager.Spawn<Transform>("attach_" + id, "UI/Element/Grid/Attach/" + id, base.transform);
		this.listAttach.Add(transform);
		if (rightAttach)
		{
			RectTransform rectTransform = transform.Rect();
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, (float)(-8 - 8 * this.rightAttachCount));
			transform.tag = "RightAttach";
			this.rightAttachCount++;
		}
		transform.name = id;
		return transform;
	}

	public void Dettach(string id)
	{
		this.listAttach.ForeachReverse(delegate(Transform t)
		{
			if (t.name == id)
			{
				if (t.tag.Contains("RightAttach"))
				{
					this.rightAttachCount--;
				}
				this.listAttach.Remove(t);
				PoolManager.Despawn(t);
			}
		});
	}

	public void Reset()
	{
		base.image.color = Color.white;
		if (this.listAttach.Count > 0)
		{
			foreach (Transform c in this.listAttach)
			{
				PoolManager.Despawn(c);
			}
			this.listAttach.Clear();
		}
		if (this.icon)
		{
			this.icon.enabled = true;
		}
		this.rightAttachCount = 0;
	}

	protected override void OnDestroy()
	{
		if (this.listAttach.Count > 0)
		{
			this.Reset();
		}
		this.rightAttachCount = 0;
	}

	public void SetBodySlot(BodySlot b, InvOwner owner, bool showIndex = false)
	{
		this.SetCardGrid(b.thing, owner);
		this.imageCheck.sprite = SpriteSheet.Get("Media/Graphics/Icon/Element/", "eq_" + b.element.alias);
		this.imageCheck.SetNativeSize();
		this.imageCheck.SetAlpha((b.thing == null) ? 1f : 0.4f);
		if (this.subText)
		{
			if (showIndex)
			{
				owner.owner.Chara.body.SetBodyIndexText(b, this.subText);
				return;
			}
			this.subText.SetActive(false);
		}
	}

	public void Redraw()
	{
		this.SetCardGrid(this.card, this.invOwner);
	}

	public void SetCardGrid(Card c, InvOwner owner = null)
	{
		if (owner != null)
		{
			this.invOwner = owner;
		}
		if (c != null && !c.isDestroyed)
		{
			base.interactable = true;
		}
		this.SetCard(c, ButtonGrid.Mode.Grid, delegate(UINote n)
		{
			InvOwner invOwner = this.invOwner;
			if (invOwner == null)
			{
				return;
			}
			invOwner.OnWriteNote(this, n);
		});
		this.onRightClick = delegate()
		{
			if (!EClass.ui.AllowInventoryInteractions)
			{
				return;
			}
			if (this.invOwner == null)
			{
				return;
			}
			InvOwner.clickTimer = 0f;
			this.invOwner.OnRightClick(this);
			UIButton.onPressed = delegate()
			{
				this.invOwner.OnRightPressed(this);
			};
			EInput.Consume(false, 1);
			UIButton.TryShowTip(null, true, true);
		};
		this.SetOnClick(delegate
		{
			if (!EClass.ui.AllowInventoryInteractions)
			{
				return;
			}
			InvOwner invOwner = this.invOwner;
			if (invOwner != null)
			{
				invOwner.OnClick(this);
			}
			EInput.Consume(false, 1);
			UIButton.TryShowTip(null, true, true);
		});
	}

	public void SetCard(Card c, ButtonGrid.Mode mode = ButtonGrid.Mode.Default, Action<UINote> onWriteNote = null)
	{
		ButtonGrid.<>c__DisplayClass25_0 CS$<>8__locals1 = new ButtonGrid.<>c__DisplayClass25_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.c = c;
		CS$<>8__locals1.onWriteNote = onWriteNote;
		this.Reset();
		CS$<>8__locals1.refs = EClass.core.refs.buttonAssets;
		this.card = CS$<>8__locals1.c;
		ButtonGrid.<>c__DisplayClass25_0 CS$<>8__locals2 = CS$<>8__locals1;
		InvOwner invOwner = this.invOwner;
		CS$<>8__locals2.showNew = (invOwner != null && invOwner.ShowNew);
		if (CS$<>8__locals1.c != null && !CS$<>8__locals1.c.isDestroyed)
		{
			base.SetTooltip("note", delegate(UITooltip t)
			{
				if (CS$<>8__locals1.c.isNew)
				{
					CS$<>8__locals1.c.isNew = false;
					base.<SetCard>g__RefreshBG|0();
				}
				CS$<>8__locals1.c.WriteNote(t.note, CS$<>8__locals1.onWriteNote, IInspect.NoteMode.Default, null);
				if (CS$<>8__locals1.<>4__this.ing != null)
				{
					t.note.Space(0, 1);
					t.note.AddText("mark_ing".lang() + (CS$<>8__locals1.<>4__this.ing.optional ? "opIngredient" : "reqIng").lang(CS$<>8__locals1.<>4__this.ing.GetName(), null, null, null, null), FontColor.DontChange);
				}
			}, true);
			CS$<>8__locals1.c.SetImage(this.icon);
			if (CS$<>8__locals1.c == EClass.pc.held && CS$<>8__locals1.c.invY != 1)
			{
				this.Attach("held", true);
			}
			if (CS$<>8__locals1.c.trait is TraitAbility && (CS$<>8__locals1.c.trait as TraitAbility).CanUse(EClass.pc))
			{
				this.Attach<Image>("target_self", true).sprite = (CS$<>8__locals1.c.trait as TraitAbility).act.TargetType.IconType;
			}
			if (EClass.core.config.test.showRefIcon && CS$<>8__locals1.c.IsIdentified)
			{
				SourceElement.Row refElement = CS$<>8__locals1.c.trait.GetRefElement();
				if (refElement != null)
				{
					if (refElement.GetSprite())
					{
						Image image = this.Attach<Image>("reficon", false);
						refElement.SetImage(image);
					}
				}
				else
				{
					Sprite refSprite = CS$<>8__locals1.c.trait.GetRefSprite();
					if (refSprite)
					{
						Image image2 = this.Attach<Image>("reficon", false);
						image2.sprite = refSprite;
						image2.color = Color.white;
					}
				}
			}
			if (CS$<>8__locals1.c.qualityTier > 0)
			{
				this.Attach<Image>("quality", false).sprite = EClass.core.refs.icons.quality[Mathf.Clamp(CS$<>8__locals1.c.qualityTier - 1, 0, EClass.core.refs.icons.quality.Count - 1)];
			}
			if (CS$<>8__locals1.c.c_equippedSlot != 0 && this.invOwner != null && this.invOwner.owner.isChara && !this.invOwner.owner.IsPC)
			{
				this.Attach("equip", false);
			}
			if (CS$<>8__locals1.c.isNPCProperty || CS$<>8__locals1.c.isGifted)
			{
				this.Attach("npcProperty", true);
			}
			if (mode != ButtonGrid.Mode.Search)
			{
				CS$<>8__locals1.c.trait.OnSetCardGrid(this);
			}
			switch (mode)
			{
			case ButtonGrid.Mode.Default:
			{
				Color c2 = this.Colors.Skin.textUnidentified;
				if (CS$<>8__locals1.c.IsIdentified)
				{
					switch (CS$<>8__locals1.c.blessedState)
					{
					case BlessedState.Doomed:
						c2 = this.Colors.Skin.textDoomed;
						goto IL_544;
					case BlessedState.Cursed:
						c2 = this.Colors.Skin.textCursed;
						goto IL_544;
					case BlessedState.Blessed:
						c2 = this.Colors.Skin.textBlessed;
						goto IL_544;
					}
					c2 = this.Colors.Skin.textIdentified;
				}
				IL_544:
				this.mainText.SetText(CS$<>8__locals1.c.Name, c2);
				if (this.subText)
				{
					this.subText.SetText(Lang._weight(CS$<>8__locals1.c.ChildrenAndSelfWeight, true, 0));
				}
				break;
			}
			case ButtonGrid.Mode.Grid:
			case ButtonGrid.Mode.Search:
				CS$<>8__locals1.<SetCard>g__RefreshBG|0();
				if (CS$<>8__locals1.c.IsContainer && CS$<>8__locals1.c.c_indexContainerIcon != 0)
				{
					this.Attach("icon_container", false).GetComponent<Image>().sprite = EClass.core.refs.spritesContainerIcon[CS$<>8__locals1.c.c_indexContainerIcon];
				}
				if (CS$<>8__locals1.c.IsIdentified)
				{
					BlessedState blessedState = CS$<>8__locals1.c.blessedState;
					if (blessedState != BlessedState.Normal)
					{
						this.Attach("status_" + blessedState.ToString(), true);
					}
				}
				else
				{
					this.Attach("status_unidentified", true);
				}
				if (CS$<>8__locals1.c.IsDecayed)
				{
					this.Attach("rotten", true);
				}
				else if (CS$<>8__locals1.c.IsRotting)
				{
					this.Attach("rotting", true);
				}
				CS$<>8__locals1.c.trait.SetMainText(this.mainText, false);
				if (mode == ButtonGrid.Mode.Search)
				{
					Card rootCard = CS$<>8__locals1.c.GetRootCard();
					if (rootCard != EClass.pc)
					{
						this.Attach((rootCard == CS$<>8__locals1.c) ? "searched" : "searched_container", false);
					}
				}
				break;
			case ButtonGrid.Mode.Ingredient:
				if (CS$<>8__locals1.c.IsIdentified)
				{
					BlessedState blessedState2 = CS$<>8__locals1.c.blessedState;
					if (blessedState2 != BlessedState.Normal)
					{
						this.Attach("status_" + blessedState2.ToString(), true);
					}
				}
				else
				{
					this.Attach("status_unidentified", true);
				}
				if (CS$<>8__locals1.c.IsDecayed)
				{
					this.Attach("rotten", true);
				}
				else if (CS$<>8__locals1.c.IsRotting)
				{
					this.Attach("rotting", true);
				}
				break;
			}
			if (CS$<>8__locals1.c.c_isImportant)
			{
				this.Attach("important", true);
			}
			return;
		}
		if (mode != ButtonGrid.Mode.Ingredient)
		{
			CS$<>8__locals1.<SetCard>g__RefreshBG|0();
			this.icon.sprite = EClass.core.refs.icons.trans;
			this.mainText.text = "";
			if (this.subText)
			{
				this.subText.text = "";
			}
		}
		else
		{
			this.icon.sprite = Core.Instance.refs.spriteNoIng;
		}
		this.icon.SetNativeSize();
		if (this.invOwner == null || !this.invOwner.AlwaysShowTooltip || CS$<>8__locals1.onWriteNote == null)
		{
			base.SetTooltip(null, false);
			return;
		}
		base.SetTooltip("note", delegate(UITooltip t)
		{
			CS$<>8__locals1.onWriteNote(t.note);
		}, true);
	}

	public void SetArea(Area a)
	{
		this.Reset();
		this.icon.sprite = SpriteSheet.Get("icon_area");
		this.icon.SetNativeSize();
		this.mainText.SetActive(false);
	}

	public void SetItem(GridItem i)
	{
		this.Reset();
		this.mainText.SetActive(true);
		i.SetButton(this);
	}

	public void SetObject(object o)
	{
		if (o is Area)
		{
			this.SetArea(o as Area);
			return;
		}
		if (o is Card)
		{
			this.SetCard(o as Card, ButtonGrid.Mode.Default, null);
		}
	}

	public void SetDummy()
	{
		this.Reset();
		this.icon.enabled = false;
		this.mainText.SetActive(false);
		base.onClick.RemoveAllListeners();
		base.onClick.AddListener(delegate()
		{
			SE.BeepSmall();
		});
		this.tooltip.enable = false;
		base.image.color = new Color(1f, 1f, 1f, 0.3f);
	}

	public virtual void SetDragParent(IDragParent p)
	{
	}

	public void SetIngredient(Recipe r, Recipe.Ingredient _ing)
	{
		if (_ing.id.IsEmpty())
		{
			this.SetDummy();
			return;
		}
		this.mainText.SetActive(true);
		base.image.color = Color.white;
		this.ing = _ing;
		if (this.ing.thing != null && (this.ing.thing.isDestroyed || this.ing.thing.Num == 0))
		{
			this.ing.thing = null;
		}
		Thing thing = this.ing.thing;
		this.SetCard(thing, ButtonGrid.Mode.Ingredient, null);
		if (r != null && r.source.colorIng != 0 && r.ingredients != null && r.ingredients.Count > 0 && r.ingredients.IndexOf(_ing) == r.source.colorIng)
		{
			this.Attach("ing_color", false);
		}
		if (this.ing.optional)
		{
			UIButton componentInChildren = this.Attach("ing_option", false).GetComponentInChildren<UIButton>(true);
			componentInChildren.SetActive(this.ing.thing != null);
			componentInChildren.SetOnClick(delegate
			{
				SE.Trash();
				this.ing.thing = null;
				this.SetIngredient(r, _ing);
				r.OnChangeIngredient();
				if (LayerCraft.Instance)
				{
					LayerCraft.Instance.OnChangeIngredient();
				}
			});
		}
		HitSummary summary = Core.Instance.screen.tileSelector.summary;
		int num = (thing == null) ? 0 : thing.Num;
		int num2 = summary.countValid;
		if (num2 == 0)
		{
			num2 = 1;
		}
		num2 *= this.ing.req;
		if (this.ing.optional && num == 0)
		{
			num2 = 0;
		}
		bool flag = num < num2 && !this.ing.optional;
		string text = (num.ToString() ?? "").TagColor(this.Colors.colorIngPredict) + (" -" + num2.ToString()).TagColor(flag ? this.Colors.colorIngCost : this.Colors.colorIngReq);
		this.mainText.text = text;
		if (thing == null)
		{
			base.SetTooltip("note", delegate(UITooltip tt)
			{
				tt.note.Clear();
				tt.note.Space(16, 1);
				tt.note.AddText("mark_ing".lang() + (this.ing.optional ? "opIngredient" : "noIngredient").lang(_ing.GetName(), null, null, null, null), this.ing.optional ? FontColor.Good : FontColor.Bad);
				tt.note.Build();
			}, true);
			EClass.sources.cards.map[this.ing.IdThing].SetImage(this.icon, null, 0, true, 0, 0);
			this.icon.SetAlpha(0.5f);
			return;
		}
		if (thing.GetRootCard() != EClass.pc)
		{
			this.Attach("pcInv", false);
		}
	}

	public void SetIngredient(Recipe r, Thing t)
	{
		this.SetCard(t, ButtonGrid.Mode.Ingredient, null);
		int num = t.Num;
		this.mainText.text = (num.ToString() ?? "");
		this.mainText.SetActive(true);
		if (t.GetRootCard() != EClass.pc)
		{
			this.Attach("pcInv", false);
		}
	}

	public void SetRecipeVariation(UIRecipeInfo.RecipeVariation a)
	{
		this.Reset();
		Recipe recipe = a.recipe;
		if (recipe.UseStock && !recipe.VirtualBlock && recipe.ingredients[0].thing != null)
		{
			recipe.ingredients[0].thing.SetImage(this.icon, a.dir, recipe.ingredients[0].thing.idSkin);
		}
		else
		{
			recipe.renderRow.SetImage(this.icon, null, recipe.renderRow.GetColorInt(recipe.GetColorMaterial()), true, a.dir, recipe.idSkin);
		}
		this.mainText.SetActive(false);
	}

	public void SetRecipe()
	{
		this.SetRecipe(this.recipe);
	}

	public void SetRecipe(Area a)
	{
		this.Reset();
		this.area = a;
		this.icon.sprite = Core.Instance.refs.spriteArea;
		this.icon.SetNativeSize();
		this.mainText.SetActive(false);
	}

	public void SetRecipe(Recipe r)
	{
		this.Reset();
		this.recipe = r;
		if (r.UseStock)
		{
			this.recipe.ingredients[0].RefreshThing();
			Thing thing = this.recipe.ingredients[0].thing;
			this.mainText.SetText(thing.Num.ToString() ?? "", SkinManager.CurrentColors.buttonGrid);
			if (r.VirtualBlock)
			{
				r.renderRow.SetImage(this.icon, null, r.GetDefaultColor(), true, 0, thing.idSkin);
			}
			else
			{
				thing.SetImage(this.icon);
			}
		}
		else
		{
			r.renderRow.SetImage(this.icon, null, r.GetDefaultColor(), true, 0, r.idSkin);
		}
		this.mainText.SetActive(r.UseStock);
		if (r.UseStock)
		{
			Thing thing2 = r.ingredients[0].thing;
			if (((thing2 != null) ? thing2.GetRootCard() : null) != EClass.pc)
			{
				this.Attach("pcInv", false);
			}
		}
	}

	public void RefreshCraftable()
	{
		ThingStack thingStack = EClass.pc.things.GetThingStack(this.recipe.GetIdThing(), this.recipe.GetRefVal());
		bool flag = this.recipe.IsCraftable();
		this.mainText.SetText(this.recipe.Name, flag ? FontColor.Good : FontColor.Bad);
		this.subText.text = (thingStack.count.ToString() ?? "");
		this.recipe.SetTextDifficulty(this.subText2);
	}

	public void SetCraftRecipe(Recipe r, ButtonGrid.Mode mode, bool tooltip = false)
	{
		this.Reset();
		this.recipe = r;
		if (mode == ButtonGrid.Mode.Recipe)
		{
			this.RefreshCraftable();
		}
		else
		{
			this.mainText.SetActive(false);
		}
		RecipeCard rCard = r as RecipeCard;
		if (rCard != null)
		{
			r.SetImage(this.icon);
		}
		else
		{
			r.SetImage(this.icon);
		}
		if (tooltip)
		{
			Action<UINote> <>9__1;
			base.SetTooltip("note", delegate(UITooltip t)
			{
				if (r.ingredients.Count == 0)
				{
					r.BuildIngredientList();
				}
				EClass.player.recipes.hoveredRecipes.Add(r.id);
				this.Dettach("recipe_new");
				if (rCard != null)
				{
					if (mode == ButtonGrid.Mode.RecipeGrid && rCard._mold == null)
					{
						CardBlueprint.SetNormalRarity(false);
						rCard._mold = ThingGen.Create(r.id, -1, -1);
					}
					Card mold = rCard.Mold;
					UINote note = t.note;
					Action<UINote> onWriteNote;
					if ((onWriteNote = <>9__1) == null)
					{
						onWriteNote = (<>9__1 = delegate(UINote n)
						{
							n.Space(0, 1);
							this.WriteReqMat(n, r);
						});
					}
					mold.WriteNote(note, onWriteNote, IInspect.NoteMode.Product, r);
					return;
				}
				UINote note2 = t.note;
				note2.Clear();
				note2.AddHeaderCard(r.GetName(), this.icon.sprite);
				if (!r.GetDetail().IsEmpty())
				{
					note2.AddText(r.GetDetail(), FontColor.DontChange);
				}
				this.WriteReqMat(note2, r);
				t.note.Build();
			}, true);
		}
	}

	public void WriteReqMat(UINote n, Recipe r)
	{
		r.WriteReqFactory(n, true);
		r.WriteReqSkill(n);
		n.AddHeaderTopic("reqMat", null);
		foreach (Recipe.Ingredient ingredient in r.ingredients)
		{
			int count = EClass._map.Stocked.ListThingStack(ingredient, StockSearchMode.AroundPC).count;
			Color c = (count >= ingredient.req) ? SkinManager.CurrentColors.textGood : SkinManager.CurrentColors.textBad;
			string text = string.Concat(new string[]
			{
				"<color=",
				c.ToHex(),
				">",
				ingredient.GetName(),
				" x ",
				ingredient.req.ToString(),
				"  (",
				count.ToString(),
				")</color>"
			});
			if (ingredient.optional)
			{
				text = "opIngredient".lang(text, null, null, null, null);
			}
			n.AddText(text, FontColor.DontChange).text1.SetSize(-1);
		}
	}

	public bool ShowMouseHintLeft()
	{
		return this.invOwner != null && this.card != null && (this.invOwner.CanShiftClick(this, false) || this.invOwner.CanCtrlClick(this) || this.invOwner.CanAltClick(this));
	}

	public string GetTextMouseHintLeft()
	{
		if (this.invOwner.CanShiftClick(this, false))
		{
			return this.invOwner.GetTextShiftClick(this);
		}
		if (this.invOwner.CanCtrlClick(this))
		{
			return this.invOwner.GetTextCtrlClick(this);
		}
		if (!this.invOwner.CanAltClick(this))
		{
			return "";
		}
		return this.invOwner.GetTextAltClick(this);
	}

	public bool ShowMouseHintRight()
	{
		return this.invOwner != null && this.card != null && this.invOwner.AllowAutouse;
	}

	public string GetTextMouseHintRight()
	{
		return this.invOwner.GetAutoUseLang(this) ?? "";
	}

	public RenderRow GetRenderRow()
	{
		if (this.refObj is RenderRow)
		{
			return (RenderRow)this.refObj;
		}
		if (this.recipe != null)
		{
			return this.recipe.renderRow;
		}
		if (this.card == null)
		{
			return null;
		}
		return this.card.sourceCard;
	}

	public void OnRefreshPref()
	{
		RenderRow renderRow = this.refObj as RenderRow;
		if (renderRow != null)
		{
			renderRow.SetImage(this.icon, null, 0, false, 0, 0);
		}
		if (this.recipe != null)
		{
			this.SetRecipe(this.recipe);
			return;
		}
		if (this.card != null)
		{
			this.SetCard(this.card, ButtonGrid.Mode.Default, null);
		}
	}

	public override bool CanMiddleClick()
	{
		return this.invOwner != null && this.card != null && EClass.ui.AllowInventoryInteractions;
	}

	public override void OnMiddleClick(bool forceClick)
	{
		Thing t = this.card.Thing;
		InvOwner o = this.invOwner;
		if ((forceClick || EInput.middleMouse.clicked) && o.AllowContext)
		{
			o.ShowContextMenu(this);
		}
		EInput.middleMouse.pressedLongAction = delegate()
		{
			if (EClass.ui.AllowInventoryInteractions && EClass.core.config.game.holdMiddleButtonToHold && o.TryHold(t))
			{
				if (!(t.trait is TraitAbility) && t.trait.IsTool)
				{
					HotItemHeld.disableTool = true;
				}
				if (EClass.ui.contextMenu.isActive)
				{
					EClass.ui.contextMenu.currentMenu.Hide();
				}
				EInput.Consume(false, 1);
			}
		};
	}

	public Area area;

	public Recipe recipe;

	public Recipe.Ingredient ing;

	public Card card;

	public int index;

	public InvOwner invOwner;

	public bool keepBg;

	[NonSerialized]
	public List<Transform> listAttach = new List<Transform>();

	private int rightAttachCount;

	public enum Mode
	{
		Default,
		Grid,
		Ingredient,
		Recipe,
		RecipeGrid,
		Search
	}
}
