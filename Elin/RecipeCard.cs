using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class RecipeCard : Recipe
{
	public CardRow sourceCard
	{
		get
		{
			CardRow result;
			if ((result = this._sourceCard) == null)
			{
				result = (this._sourceCard = EClass.sources.cards.map.TryGetValue(base.source.row.idString, null));
			}
			return result;
		}
	}

	public override RenderRow renderRow
	{
		get
		{
			return this.sourceCard;
		}
	}

	public override TileType tileType
	{
		get
		{
			return this.Mold.trait.tileType;
		}
	}

	public override IRenderer GetRenderer()
	{
		return this.Mold.renderer;
	}

	public string idCard
	{
		get
		{
			return this.sourceCard.id;
		}
	}

	public override string GetDetail()
	{
		return this.sourceCard.GetDetail();
	}

	public override string GetIdThing()
	{
		return this.id;
	}

	public override bool CanRotate
	{
		get
		{
			return this.tileType.CanRotate(true);
		}
	}

	public override bool IsThing
	{
		get
		{
			return true;
		}
	}

	public override Card Mold
	{
		get
		{
			Card result;
			if ((result = this._mold) == null)
			{
				result = (this._mold = this.CreateMold());
			}
			return result;
		}
	}

	public bool isDish
	{
		get
		{
			return this.sourceCard._origin == "dish";
		}
	}

	public override void OnRenderMarker(Point point, bool active, HitResult result, bool main, int dir, int bridgeHeight)
	{
		this.Mold.ignoreStackHeight = Input.GetKey(KeyCode.LeftControl);
		this.Mold.SetFreePos(point);
		this.freePos = this.Mold.freePos;
		this.fx = this.Mold.fx;
		this.fy = this.Mold.fy;
		this.Mold.RenderMarker(point, active, result, main, dir, false);
		if (!point.cell.skipRender)
		{
			this.Mold.trait.OnRenderTile(point, result, dir);
		}
	}

	public override void BuildIngredientList()
	{
		if (this.ingredients.Count != 0)
		{
			return;
		}
		base.BuildIngredientList();
		if (!EClass.core.IsGameStarted || EClass.core.game.isLoading)
		{
			return;
		}
		if (this.isDish && EClass.pc.Evalue(1650) >= 2)
		{
			bool flag = this.ingredients.Count < 3;
			using (List<Recipe.Ingredient>.Enumerator enumerator = this.ingredients.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.id == "seasoning")
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				this.ingredients.Add(new Recipe.Ingredient
				{
					id = "seasoning",
					optional = true,
					req = 1
				});
			}
		}
	}

	public override void OnChangeIngredient()
	{
		if (this.UseStock)
		{
			return;
		}
		this.Mold.ChangeMaterial(this.GetMainMaterial());
		if (base.source.colorIng != 0)
		{
			this.Mold.Dye(this.GetColorMaterial());
		}
	}

	public override void OnSelected()
	{
		this.CreateMold();
	}

	public Card CreateMold()
	{
		if (this._mold != null)
		{
			this._mold.Destroy();
		}
		if (this.UseStock)
		{
			this.ingredients[0].RefreshThing();
		}
		if (this.UseStock && this.ingredients[0].thing != null)
		{
			this._mold = this.ingredients[0].thing.Duplicate(1);
		}
		else
		{
			if (this.sourceCard.isChara)
			{
				this._mold = CharaGen.Create(this.idCard, -1);
			}
			else
			{
				this._mold = ThingGen.Create(this.idCard, -1, -1);
			}
			this.OnChangeIngredient();
		}
		this._mold.dir = this._dir;
		this._mold.altitude = ActionMode.Build.altitude;
		this._mold.idSkin = this.idSkin;
		return this._mold;
	}

	public override Thing Craft(BlessedState blessed, bool sound = false, List<Thing> ings = null, bool model = false)
	{
		string text = this.idCard;
		int idMat = this.GetMainMaterial().id;
		Element reqSkill = base.source.GetReqSkill();
		int num = reqSkill.Value - EClass.pc.Evalue(reqSkill.id);
		int qualityBonus = base.GetQualityBonus();
		int num2 = this.renderRow.LV + qualityBonus;
		bool flag = qualityBonus < 0;
		CardRow cardRow = EClass.sources.cards.map.TryGetValue(text, null);
		bool flag2 = false;
		if (ings != null)
		{
			foreach (Thing thing in ings)
			{
				if (thing != null && thing.IsDecayed)
				{
					flag2 = true;
				}
			}
		}
		if (EClass.debug.godCraft || model)
		{
			flag = false;
		}
		else if (cardRow != null && cardRow.tag.Contains("no_fail"))
		{
			flag = false;
		}
		else if (this.isDish && !EClass.scene.actionMode.IsBuildMode)
		{
			if (num > 0 && EClass.rnd(num * 10) > EClass.rnd(100))
			{
				text = this.GetIdFailDish();
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		if (!model)
		{
			string id = this.id;
			if (!(id == "weapon_stone") && !(id == "weapon_wood"))
			{
				if (id == "weapon_anvil")
				{
					Rand.SetSeed(-1);
					text = new string[]
					{
						"dagger",
						"sword",
						"axe_hand",
						"blunt_club",
						"spear",
						"staff_long"
					}.RandomItem<string>();
					idMat = ings[1].material.id;
				}
			}
			else
			{
				Rand.SetSeed(-1);
				text = new string[]
				{
					"dagger",
					"sword",
					"axe_hand",
					"blunt_club",
					"spear",
					"staff_long"
				}.RandomItem<string>();
			}
		}
		if (!this.isDish && num2 < 1)
		{
			num2 = 1;
		}
		if (EClass.sources.cards.map[text].tag.Contains("noQuality"))
		{
			num2 = -1;
		}
		if (EClass.sources.cards.map[text].tag.Contains("noMaterialChange"))
		{
			idMat = -1;
		}
		CardBlueprint.Set(new CardBlueprint
		{
			qualityBonus = qualityBonus,
			rarity = (flag ? Rarity.Crude : Rarity.Normal)
		});
		Thing thing2 = ThingGen.Create(text, idMat, num2);
		if (thing2.trait.CraftNum > 1)
		{
			thing2.SetNum(thing2.trait.CraftNum);
		}
		thing2.idSkin = this.idSkin;
		thing2.Identify(false, IDTSource.Identify);
		thing2.isCrafted = true;
		if (base.source.colorIng != 0)
		{
			thing2.Dye(this.GetColorMaterial());
		}
		if (thing2.IsContainer)
		{
			thing2.RemoveThings();
			thing2.c_lockLv = 0;
		}
		thing2.SetBlessedState(blessed);
		if (this.isDish)
		{
			if (!flag)
			{
				this.MakeDish(thing2);
			}
		}
		else
		{
			this.MixIngredients(thing2);
		}
		if (this.isDish && flag2)
		{
			thing2.decay = thing2.MaxDecay + 1;
		}
		thing2.trait.OnCrafted(this);
		if (thing2.IsAmmo && num < 0)
		{
			thing2.SetEncLv(-num / 10);
		}
		if (model)
		{
			thing2.SetNum(1);
			return thing2;
		}
		if (EClass.pc.held == null || !thing2.TryStackTo(EClass.pc.held.Thing))
		{
			EClass.pc.HoldCard(thing2, -1);
		}
		if (sound)
		{
			thing2.PlaySoundDrop(false);
		}
		Msg.Say("crafted", thing2, null, null, null);
		if (thing2.Num > EClass.rnd(1000) || EClass.debug.enable)
		{
			EClass.player.recipes.ComeUpWithRandomRecipe(thing2.category.id, 0);
		}
		if (this.isDish && (EClass.debug.enable || (EClass.player.flags.canComupWithFoodRecipe && EClass.rnd(30) == 0)))
		{
			EClass.player.recipes.ComeUpWithRandomRecipe(thing2.category.id, 0);
			EClass.player.flags.canComupWithFoodRecipe = false;
		}
		return thing2;
	}

	public void MakeDish(Thing t)
	{
		Rand.SetSeed(EClass.pc.turn);
		List<Thing> list = new List<Thing>();
		foreach (Recipe.Ingredient ingredient in this.ingredients)
		{
			list.Add(ingredient.thing);
		}
		CraftUtil.MakeDish(t, list, base.GetQualityBonus(), EClass.pc);
		Rand.SetSeed(-1);
	}

	public void MixIngredients(Thing t)
	{
		Rand.SetSeed(EClass.pc.turn);
		List<Thing> list = new List<Thing>();
		foreach (Recipe.Ingredient ingredient in this.ingredients)
		{
			list.Add(ingredient.thing);
		}
		CraftUtil.MixIngredients(t, list, CraftUtil.MixType.General, base.GetQualityBonus(), null);
		Rand.SetSeed(-1);
	}

	public string GetIdFailDish()
	{
		string category = this.sourceCard.category;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(category);
		if (num <= 2191978763U)
		{
			if (num <= 1612508523U)
			{
				if (num != 228605988U)
				{
					if (num == 1612508523U)
					{
						if (category == "meal_bread")
						{
							return "fail_dough_bread";
						}
					}
				}
				else if (category == "meal_meat")
				{
					return "fail_meat";
				}
			}
			else if (num != 1937649754U)
			{
				if (num != 2035834996U)
				{
					if (num == 2191978763U)
					{
						if (category == "meal_cake")
						{
							return "fail_dough_cake";
						}
					}
				}
				else if (category == "meal_noodle")
				{
					return "fail_noodle";
				}
			}
			else if (category == "meal_vegi")
			{
				return "fail_vegi";
			}
		}
		else if (num <= 2808738325U)
		{
			if (num != 2235373338U)
			{
				if (num == 2808738325U)
				{
					if (category == "meal_fish")
					{
						return "fail_fish";
					}
				}
			}
			else if (category == "meal_rice")
			{
				return "fail_rice";
			}
		}
		else if (num != 3150879124U)
		{
			if (num != 3348186596U)
			{
				if (num == 4237839943U)
				{
					if (category == "meal_fruit")
					{
						return "fail_fruit";
					}
				}
			}
			else if (category == "meal_egg")
			{
				return "fail_egg";
			}
		}
		else if (category == "meal_soup")
		{
			return "fail_drink";
		}
		return "fail_dish";
	}

	public override void Build(TaskBuild task)
	{
		Card card;
		if (task.target != null)
		{
			card = task.target;
		}
		else if (this.IngAsProduct)
		{
			if (task.resources.Count == 0)
			{
				Thing thing = this.ingredients[0].RefreshThing();
				if (thing == null)
				{
					SE.Beep();
					return;
				}
				Thing item = thing.Split(1);
				task.resources.Add(item);
				if (this.ingredients[0].thing.isDestroyed || this.ingredients[0].thing.ExistsOnMap)
				{
					this.ingredients[0].thing = null;
				}
			}
			card = task.resources[0];
		}
		else
		{
			if (this.sourceCard.isChara)
			{
				card = CharaGen.Create(this.idCard, Mathf.Max(EClass._zone.DangerLv, EClass.pc.LV));
			}
			else
			{
				card = ThingGen.Create(this.idCard, -1, Mathf.Max(EClass._zone.DangerLv, EClass.pc.LV));
			}
			if (!card.isChara)
			{
				if (!card.IsUnique)
				{
					card.ChangeMaterial(this.GetMainMaterial());
				}
				if (base.source.colorIng != 0)
				{
					card.Dye(this.GetColorMaterial());
				}
				if (card.IsContainer)
				{
					card.RemoveThings();
				}
			}
		}
		this.Build(task.owner, card, task.pos, this.ingredients[0].mat, task.dir, task.altitude, task.bridgeHeight);
		card.renderer.PlayAnime(AnimeID.Place, default(Vector3), false);
	}

	public override void Build(Chara chara, Card t, Point pos, int mat, int dir, int altitude, int bridgeHeight)
	{
		if (mat == -1)
		{
			mat = 2;
		}
		EClass.pc.PlaySound(EClass.sources.materials.rows[mat].GetSoundImpact(null), 1f, true);
		t.SetDir(dir);
		t.idSkin = this.idSkin;
		EClass._zone.AddCard(t, pos);
		if (t.trait is TraitHouseBoard && ActionMode.Build.houseBoard != null)
		{
			(t.trait as TraitHouseBoard).data = IO.DeepCopy<TraitHouseBoard.Data>(ActionMode.Build.houseBoard.data);
		}
		t.SetPlaceState(PlaceState.installed, true);
		t.altitude = altitude;
		t.isPlayerCreation = true;
		if (EClass._zone.idCurrentSubset != null)
		{
			t.isSubsetCard = true;
		}
		t.ignoreStackHeight = Input.GetKey(KeyCode.LeftControl);
		if (ActionMode.Build.IsActive && t.trait.MaskOnBuild && EClass.debug.enable)
		{
			t.isMasked = true;
		}
		if (EClass.scene.actionMode.IsRoofEditMode(t) && t.isThing)
		{
			t.isRoofItem = true;
			t.SetPlaceState(PlaceState.roaming, false);
		}
		t.ForeachPoint(delegate(Point p, bool main)
		{
			base.<Build>g__CheckBlock|0(p);
		});
		t.freePos = this.freePos;
		if (this.freePos)
		{
			t.fx = this.fx;
			t.fy = this.fy;
		}
		t.renderer.RefreshSprite();
	}

	public override void OnChangeAltitude(int a)
	{
		this.Mold.altitude = a;
	}

	public override void Rotate()
	{
		this.Mold.Rotate(false);
		this._dir = this.Mold.dir;
		if (BuildMenu.Instance)
		{
			BuildMenu.Instance.info1.OnRotate();
		}
	}

	public override void SetDir(int d)
	{
		this.Mold.dir = d;
		base.SetDir(d);
	}

	public override void WriteNote(UINote n)
	{
		n.Clear();
		if (!this.Mold.isChara)
		{
			this.Mold.elements.AddNote(n, null, null, ElementContainer.NoteMode.Default, false, null, null);
		}
		n.Build();
	}

	public override Recipe Duplicate()
	{
		RecipeCard recipeCard = IO.DeepCopy<RecipeCard>(this);
		recipeCard._mold = this._mold;
		return recipeCard;
	}

	public CardRow _sourceCard;

	[JsonProperty]
	public bool freePos;

	[JsonProperty]
	public float fx;

	[JsonProperty]
	public float fy;

	public Card _mold;
}
