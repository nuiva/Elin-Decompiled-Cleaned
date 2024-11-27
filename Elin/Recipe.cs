using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class Recipe : EClass
{
	public static Recipe GetOrCreate(string id)
	{
		Recipe recipe = Recipe.recipeCache.TryGetValue(id, null);
		if (recipe == null)
		{
			RecipeSource recipeSource = RecipeManager.dict.TryGetValue(id, null);
			if (recipeSource != null)
			{
				recipe = Recipe.Create(recipeSource, -1, null);
				Recipe.recipeCache.Add(id, recipe);
			}
		}
		return recipe;
	}

	public static Recipe Create(RecipeSource _source, int idMat = -1, Thing ing = null)
	{
		string type = _source.type;
		Recipe recipe = _source.isBridgePillar ? new RecipeBridgePillar() : (type.IsEmpty() ? new RecipeCard() : ((type == "Custom") ? new RecipeCustom() : new Recipe()));
		recipe.id = _source.id;
		recipe._source = _source;
		recipe.idMat = idMat;
		if (ing != null)
		{
			recipe.BuildIngredientList(ing);
			if (ing.trait is TraitTile)
			{
				recipe.VirtualBlock = true;
			}
		}
		return recipe;
	}

	public static Recipe Create(Thing t)
	{
		RecipeSource recipeSource = RecipeManager.dict.TryGetValue(t.id, null);
		if (recipeSource == null)
		{
			return null;
		}
		RecipeCard recipeCard = new RecipeCard
		{
			id = recipeSource.id,
			_source = recipeSource,
			IngAsProduct = true,
			idSkin = t.idSkin
		};
		recipeCard.BuildIngredientList(t);
		if (t.uid == 0 || recipeCard.ingredients[0].uid == 0 || recipeCard.ingredients[0].thing == null)
		{
			Debug.LogError("expection: invalid ingredient " + ((t != null) ? t.ToString() : null));
		}
		return recipeCard;
	}

	public static void DropIngredients(string id, string idMatMain, int num = 1)
	{
		RecipeManager.BuildList();
		Recipe orCreate = Recipe.GetOrCreate(EClass.sources.cards.map[id].RecipeID);
		orCreate.BuildIngredientList();
		Msg.Say("dropReward");
		foreach (Recipe.Ingredient ingredient in orCreate.ingredients)
		{
			Thing thing = ThingGen.Create(ingredient.id, -1, -1).SetNum(ingredient.req);
			if (ingredient == orCreate.ingredients[0])
			{
				thing.ChangeMaterial(idMatMain);
				thing.SetNum(num);
			}
			EClass.player.DropReward(thing, true);
		}
	}

	public RecipeSource source
	{
		get
		{
			RecipeSource result;
			if ((result = this._source) == null)
			{
				result = (this._source = RecipeManager.Get(this.id));
			}
			return result;
		}
	}

	public SourceMaterial.Row DefaultMaterial
	{
		get
		{
			return this.renderRow.DefaultMaterial;
		}
	}

	public virtual TileRow tileRow
	{
		get
		{
			return this.source.row as TileRow;
		}
	}

	public virtual RenderRow renderRow
	{
		get
		{
			return this.source.row;
		}
	}

	public string Name
	{
		get
		{
			return this.GetName();
		}
	}

	public virtual TileType tileType
	{
		get
		{
			if (!(this.source.type == "Bridge") || this.source.row.tileType is TileTypeBridge)
			{
				return this.renderRow.tileType;
			}
			return TileType.Bridge;
		}
	}

	public virtual bool IsFloorOrBridge
	{
		get
		{
			return this.tileType.IsFloorOrBridge;
		}
	}

	public int MaxAltitude
	{
		get
		{
			if (this.IsThing || this.IsBridge || this.tileType.blockRenderMode == BlockRenderMode.Pillar)
			{
				return this.tileType.MaxAltitude;
			}
			return 0;
		}
	}

	public int RecipeLv
	{
		get
		{
			return EClass.player.recipes.knownRecipes.TryGetValue(this.id, 0);
		}
	}

	public bool IsFloor
	{
		get
		{
			return this.tileType.IsFloor;
		}
	}

	public bool IsObj
	{
		get
		{
			return this.tileRow is SourceObj.Row;
		}
	}

	public virtual bool IsBlock
	{
		get
		{
			return !this.IsFloorOrBridge;
		}
	}

	public bool IsWallOrFence
	{
		get
		{
			return this.tileType.IsWallOrFence;
		}
	}

	public bool IsBridge
	{
		get
		{
			return this.source.type == "Bridge";
		}
	}

	public virtual bool IsThing
	{
		get
		{
			return false;
		}
	}

	public virtual Card Mold
	{
		get
		{
			return null;
		}
	}

	public bool IsStaticLV()
	{
		string idFactory = this.source.idFactory;
		return idFactory == "factory_floor" || idFactory == "factory_block" || idFactory == "factory_wall" || idFactory == "factory_platform" || this.renderRow.Category.IsChildOf("block");
	}

	public int GetQualityBonus()
	{
		if (this.IsStaticLV())
		{
			return 0;
		}
		Element reqSkill = this.source.GetReqSkill();
		int num = reqSkill.Value - EClass.pc.Evalue(reqSkill.id);
		int num2 = 0;
		if (num <= 0)
		{
			num2 += EClass.curve(-num, 10, 20, 80) / 10 * 10 + 10;
			if (this.RecipeLv > 0)
			{
				num2 += (int)Mathf.Sqrt((float)(this.RecipeLv - 1)) * 10;
			}
			return num2;
		}
		if (num < 5)
		{
			return 0;
		}
		return -(num - 4) * 10;
	}

	public virtual int W
	{
		get
		{
			if (this._dir % 2 != 0)
			{
				return this.renderRow.H;
			}
			return this.renderRow.W;
		}
	}

	public virtual int H
	{
		get
		{
			if (this._dir % 2 != 0)
			{
				return this.renderRow.W;
			}
			return this.renderRow.H;
		}
	}

	public virtual bool RequireIngredients
	{
		get
		{
			return true;
		}
	}

	public virtual int GetRefVal()
	{
		if (this.source.row is SourceBlock.Row)
		{
			return (this.source.row as SourceBlock.Row).id;
		}
		if (this.source.row is SourceFloor.Row)
		{
			return (this.source.row as SourceFloor.Row).id;
		}
		if (this.source.row is SourceObj.Row)
		{
			return (this.source.row as SourceObj.Row).id;
		}
		return -1;
	}

	public virtual string GetIdThing()
	{
		if (this.source.row is SourceBlock.Row)
		{
			return (this.source.row as SourceBlock.Row).idThing;
		}
		if (this.source.row is SourceFloor.Row)
		{
			if (!this.IsBridge)
			{
				return "floor";
			}
			return "platform";
		}
		else
		{
			if (this.source.row is SourceObj.Row)
			{
				return "obj";
			}
			return null;
		}
	}

	public bool MultiSize
	{
		get
		{
			return this.W != 1 || this.H != 1;
		}
	}

	public virtual bool ShouldShowHighlight(HitResult r)
	{
		return r != HitResult.Valid;
	}

	public BaseTileSelector.BoxType GetBoxType()
	{
		if (this.tileType.BoxType != BaseTileSelector.BoxType.Fence)
		{
			return BaseTileSelector.BoxType.Box;
		}
		return BaseTileSelector.BoxType.Fence;
	}

	public BaseTileSelector tileSelector
	{
		get
		{
			return EClass.screen.tileSelector;
		}
	}

	public int GetDefaultColor()
	{
		if (this.UseStock)
		{
			Thing thing = this.ingredients[0].thing;
			return this.renderRow.GetColorInt(thing.isDyed ? thing.DyeMat : thing.material);
		}
		return this.renderRow.GetColorInt(this.DefaultMaterial);
	}

	public virtual SourceMaterial.Row GetMainMaterial()
	{
		int num = -1;
		if (this.ingredients.Count > 0)
		{
			if (this.ingredients[0].thing != null)
			{
				num = this.ingredients[0].thing.material.id;
			}
			else
			{
				num = this.ingredients[0].mat;
			}
		}
		if (num == -1)
		{
			num = 3;
		}
		return EClass.sources.materials.rows[num];
	}

	public virtual SourceMaterial.Row GetColorMaterial()
	{
		if (this.idMat != -1)
		{
			return EClass.sources.materials.rows[this.idMat];
		}
		if (!this.UseStock)
		{
			int num = (this.ingredients.Count > 0) ? this.ingredients[this.source.colorIng].mat : 3;
			if (num == -1)
			{
				num = 3;
			}
			return EClass.sources.materials.rows[num];
		}
		this.ingredients[0].RefreshThing();
		Thing thing = this.ingredients[0].thing;
		if (thing == null)
		{
			return EClass.sources.materials.rows[3];
		}
		if (!thing.isDyed)
		{
			return thing.material;
		}
		return thing.DyeMat;
	}

	public virtual int CostMoney
	{
		get
		{
			return 0;
		}
	}

	public virtual void BuildIngredientList()
	{
		if (this.UseStock || !this.RequireIngredients)
		{
			return;
		}
		this.ingredients = this.source.GetIngredients();
	}

	public void BuildIngredientList(Thing t)
	{
		this.UseStock = true;
		Recipe.Ingredient item = new Recipe.Ingredient
		{
			id = t.id,
			tag = null,
			req = 1,
			uid = t.uid,
			thing = t,
			mat = t.material.id
		};
		this.ingredients.Add(item);
	}

	public virtual void OnChangeIngredient()
	{
	}

	public virtual bool CanRotate
	{
		get
		{
			return this.tileType.CanRotate(true) && this.tileRow._tiles.Length > 1;
		}
	}

	public virtual Thing Craft(BlessedState blessed, bool sound = false, List<Thing> ings = null, TraitCrafter crafter = null, bool model = false)
	{
		string type = this.source.type;
		Thing thing;
		if (!(type == "Block"))
		{
			if (!(type == "Obj"))
			{
				thing = ThingGen.CreateFloor(this.tileRow.id, this.GetMainMaterial().id, this.source.isBridge);
			}
			else
			{
				thing = ThingGen.CreateObj(this.tileRow.id, this.GetMainMaterial().id);
			}
		}
		else
		{
			thing = ThingGen.CreateBlock(this.tileRow.id, this.GetMainMaterial().id);
		}
		if (thing == null)
		{
			return null;
		}
		thing.trait.OnCrafted(this);
		thing.SetBlessedState(blessed);
		if (model)
		{
			return thing;
		}
		Msg.Say("crafted", thing, null, null, null);
		thing = (EClass.pc.AddCard(thing) as Thing);
		if (thing.GetRootCard() == EClass.pc)
		{
			EClass.pc.HoldCard(thing, -1);
		}
		return thing;
	}

	public bool HasSameTile(Point point, int dir, int altitude, int bridgeHeight)
	{
		if (this.IsObj)
		{
			if (point.growth != null && point.sourceObj == this.tileRow && point.cell.matObj == this.GetColorMaterial())
			{
				return true;
			}
			if (dir == point.cell.objDir && point.cell.matObj == this.GetColorMaterial() && point.sourceObj == this.tileRow)
			{
				return true;
			}
		}
		else if (this.IsBridge)
		{
			if (dir == point.cell.floorDir && point.matBridge == this.GetColorMaterial() && point.sourceBridge == this.tileRow && Mathf.Clamp(bridgeHeight + altitude, 0, 255) == (int)point.cell.bridgeHeight)
			{
				return true;
			}
		}
		else if (this.IsFloorOrBridge)
		{
			if (dir == point.cell.floorDir && point.matFloor == this.GetColorMaterial() && point.sourceFloor == this.tileRow)
			{
				return true;
			}
		}
		else if (this.IsWallOrFence)
		{
			if (point.matBlock == this.GetColorMaterial() && point.sourceBlock == this.tileRow && dir == point.cell.blockDir)
			{
				return true;
			}
		}
		else if (this.IsBlock && point.matBlock == this.GetColorMaterial() && point.sourceBlock == this.tileRow)
		{
			return !point.sourceBlock.tileType.AltitudeAsDir || dir == point.cell.objDir;
		}
		return false;
	}

	public virtual void Build(TaskBuild task)
	{
		this.Build(task.owner, null, task.pos, (this.ingredients.Count > 0) ? this.ingredients[0].mat : task.recipe.idMat, task.dir, task.altitude, task.bridgeHeight);
	}

	public virtual void Build(Chara chara, Card t, Point pos, int mat, int dir, int altitude, int bridgeHeight)
	{
		if (mat == -1)
		{
			mat = 2;
		}
		EClass.pc.PlaySound(EClass.sources.materials.rows[mat].GetSoundImpact(null), 1f, true);
		pos.cell.isModified = true;
		string type = this.source.type;
		if (!(type == "Block"))
		{
			if (type == "Floor")
			{
				if (pos.sourceObj.tileType.RemoveOnFloorChange && (!BuildMenu.Instance || !EClass.debug.ignoreBuildRule))
				{
					EClass._map.SetObj(pos.x, pos.z, 0, 1, 0);
				}
				EClass._map.SetFloor(pos.x, pos.z, mat, this.tileRow.id, dir);
				return;
			}
			if (type == "Bridge")
			{
				if (pos.sourceObj.tileType.RemoveOnFloorChange)
				{
					EClass._map.SetObj(pos.x, pos.z, 0, 1, 0);
				}
				EClass._map.SetBridge(pos.x, pos.z, Mathf.Clamp(bridgeHeight + altitude, 0, 255), mat, this.tileRow.id, dir);
				return;
			}
			if (!(type == "Obj"))
			{
				return;
			}
			EClass._map.SetObj(pos.x, pos.z, mat, this.tileRow.id, 1, dir);
			if (this.tileType.ChangeBlockDir)
			{
				EClass._map.SetBlockDir(pos.x, pos.z, dir);
			}
			if (pos.growth != null)
			{
				pos.growth.SetDefaultStage();
			}
			return;
		}
		else
		{
			int ramp = this.tileRow.id;
			if (ramp == 3)
			{
				ramp = EClass.sources.materials.rows[mat].ramp;
			}
			if (EClass.scene.actionMode.IsRoofEditMode(null))
			{
				EClass._map.SetRoofBlock(pos.x, pos.z, mat, ramp, dir, altitude);
				return;
			}
			if (pos.HasObj && !pos.sourceObj.tileType.CanBuiltOnBlock)
			{
				EClass._map.MineObjSound(pos);
				EClass._map.SetObj(pos.x, pos.z, 0, 1, 0);
			}
			EClass._map.SetBlock(pos.x, pos.z, mat, ramp, dir);
			if (this.tileType.IsBlockPass && pos.HasChara)
			{
				foreach (Chara t2 in pos.ListCharas())
				{
					chara.Kick(t2, false, true);
				}
			}
			if (this.tileType.AltitudeAsDir)
			{
				pos.cell.objDir = dir;
			}
			foreach (Card card in pos.ListCards(false))
			{
				if (card.isThing && card.placeState == PlaceState.roaming)
				{
					card._Move(card.pos.GetNearestPoint(false, true, true, false), Card.MoveType.Walk);
				}
			}
			return;
		}
	}

	public unsafe virtual void OnRenderMarker(Point point, bool active, HitResult result, bool main, int dir, int bridgeHeight)
	{
		if (dir != -1)
		{
			this._dir = dir;
		}
		RenderParam renderParam = this.tileRow.GetRenderParam(this.DefaultMaterial, this._dir, point, bridgeHeight);
		renderParam.matColor = (float)(active ? EClass.Colors.blockColors.Active : EClass.Colors.blockColors.Inactive);
		point.ApplyAnime(renderParam);
		if (this.tileType.IsWater)
		{
			if (EClass.screen.guide.isActive)
			{
				EClass.screen.guide.passGuideFloor.Add(point, (float)result, 0.3f);
			}
			return;
		}
		if (this.tileType.IsWallOrFence && point.HasNonWallBlock)
		{
			Vector3 vector = *point.Position();
			if (EClass.screen.guide.isActive)
			{
				EClass.screen.guide.passGuideBlock.Add(ref vector, 5f, 0f);
			}
		}
		if (this.renderRow.renderData.pass == EClass.screen.tileMap.passBlock || this.renderRow.renderData.pass == EClass.screen.tileMap.passBlockEx || this.renderRow.renderData.pass == EClass.screen.tileMap.passLiquid)
		{
			if (bridgeHeight > 0 && !point.cell.skipRender && EClass.screen.guide.isActive)
			{
				EClass.screen.guide.passGuideFloor.Add(point, (float)result, 0.3f);
			}
			if (!this.tileType.IsWallOrFence && (point.HasChara || (point.HasObj && point.cell.blocked)))
			{
				Vector3 vector2 = *point.Position();
				if (EClass.screen.guide.isActive)
				{
					EClass.screen.guide.passGuideBlock.Add(ref vector2, 5f, 0f);
				}
				return;
			}
			if (EClass.scene.actionMode.IsRoofEditMode(null))
			{
				EClass.screen.tileMap.SetRoofHeight(renderParam, point.cell, point.x, point.z, 0, bridgeHeight, this.tileType.IsWallOrFence ? this._dir : -1, true);
			}
			renderParam.y = renderParam.y - EClass.screen.tileMap.rendererBlockMarker.offset.y + this.renderRow.renderData.offset.y;
			renderParam.z += EClass.setting.render.tileMarkerZ;
			int num = (this.tileType.blockRenderMode == BlockRenderMode.Pillar) ? (this._dir + 1 + ((this._dir >= 7) ? this._dir : 0)) : 1;
			if (num == 1)
			{
				EClass.screen.tileMap.rendererBlockMarker.Draw(renderParam);
				return;
			}
			EClass.screen.tileMap.rendererBlockMarker.DrawRepeat(renderParam, num, this.tileType.RepeatSize, false);
			return;
		}
		else
		{
			if (this.renderRow.renderData.pass == EClass.screen.tileMap.passFloor || this.renderRow.renderData.pass == EClass.screen.tileMap.passFloorEx || this.renderRow.renderData.pass == EClass.screen.tileMap.passFloorWater)
			{
				if (point.HasObj)
				{
					point.Position();
				}
				renderParam.z += ((point.cell.liquidLv > 0) ? -0.01f : EClass.setting.render.tileMarkerZFloor);
				EClass.screen.tileMap.rendererFloorMarker.Draw(renderParam);
				return;
			}
			renderParam.z += EClass.setting.render.tileMarkerZ;
			this.renderRow.renderData.Draw(renderParam);
			return;
		}
	}

	public string GetName()
	{
		if (this.UseStock && this.ingredients[0].thing == null)
		{
			return "";
		}
		string text = this.UseStock ? this.ingredients[0].thing.Name : this.source.Name;
		if (text == "")
		{
			text = ("card_" + this.source.id).lang();
		}
		if (text == "*r")
		{
			text = this.source.row.GetText("aka", false);
		}
		if (text == "*r")
		{
			text = "(" + this.source.row.GetField("id") + ")";
		}
		return text.ToTitleCase(false);
	}

	public virtual string GetDetail()
	{
		return this.renderRow.GetText("detail", false).IsEmpty(this.source.GetDetail());
	}

	public virtual void WriteNote(UINote n)
	{
		n.Clear();
		n.Build();
	}

	public void WriteReqFactory(UINote n, bool hasFactory = true)
	{
		if (this.source.NeedFactory)
		{
			n.AddHeaderTopic("reqFactory".lang(this.source.NameFactory.TagColor(hasFactory ? FontColor.Default : FontColor.Bad, null), null, null, null, null), null);
			n.Space(8, 1);
		}
	}

	public void WriteReqSkill(UINote n)
	{
		n.AddHeaderTopic("reqSkill", null);
		Element reqSkill = this.source.GetReqSkill();
		int value = EClass.pc.elements.GetOrCreateElement(reqSkill).Value;
		Color textColor = EClass.Colors.Skin.GetTextColor((value >= reqSkill.Value) ? FontColor.Good : FontColor.Warning);
		n.AddText(null, string.Concat(new string[]
		{
			reqSkill.Name,
			" ",
			reqSkill.Value.ToString(),
			" (",
			value.ToString(),
			")"
		}), textColor).text1.SetSize(-1);
		n.Space(8, 1);
	}

	public virtual IRenderer GetRenderer()
	{
		return this.tileRow.renderData;
	}

	public virtual void OnSelected()
	{
	}

	public virtual void OnChangeAltitude(int a)
	{
	}

	public virtual void Rotate()
	{
		if (EInput.isShiftDown || Input.GetMouseButton(1))
		{
			this._dir--;
		}
		else
		{
			this._dir++;
		}
		if (this.tileType.AltitudeAsDir)
		{
			if (this._dir >= this.tileType.MaxAltitude)
			{
				this._dir = 0;
			}
			if (this._dir < 0)
			{
				this._dir = this.tileType.MaxAltitude - 1;
			}
		}
		else
		{
			int num = this.tileRow._tiles.Length;
			if (this.tileRow.tileType == TileType.Door)
			{
				num = 2;
			}
			if (this._dir < 0)
			{
				this._dir = num - 1;
			}
			if (this._dir >= num)
			{
				this._dir = 0;
			}
		}
		if (BuildMenu.Instance)
		{
			BuildMenu.Instance.info1.OnRotate();
		}
	}

	public virtual void SetDir(int d)
	{
		this._dir = d;
		if (BuildMenu.Instance)
		{
			BuildMenu.Instance.info1.OnRotate();
		}
	}

	public virtual Recipe Duplicate()
	{
		return IO.DeepCopy<Recipe>(this);
	}

	public virtual void SetImage(Image icon)
	{
		this.renderRow.SetImage(icon, null, this.renderRow.GetColorInt((this.ingredients != null && this.ingredients.Count > 0 && this.ingredients[0].thing != null) ? this.ingredients[0].thing.material : this.DefaultMaterial), true, 0, this.idSkin);
	}

	public bool IsCraftable()
	{
		foreach (Recipe.Ingredient ingredient in this.ingredients)
		{
			if (!ingredient.optional && EClass._map.Stocked.ListThingStack(ingredient, StockSearchMode.AroundPC).max < ingredient.req)
			{
				return false;
			}
		}
		return true;
	}

	public int GetMaxCount()
	{
		int num = 999;
		for (int i = 0; i < this.ingredients.Count; i++)
		{
			Recipe.Ingredient ingredient = this.ingredients[i];
			Thing thing = ingredient.thing;
			int num2 = 0;
			if (!ingredient.optional || thing != null)
			{
				if (thing != null && !thing.isDestroyed)
				{
					num2 = thing.Num / ingredient.req;
				}
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	public void SetTextDifficulty(UIText text)
	{
		Element reqSkill = this.source.GetReqSkill();
		int value = EClass.pc.elements.GetOrCreateElement(reqSkill).Value;
		string text2 = reqSkill.Name + " " + reqSkill.Value.ToString();
		text.SetText(text2);
	}

	public int GetSortVal()
	{
		Element reqSkill = this.source.GetReqSkill();
		return reqSkill.source.id * 10000 - reqSkill.Value;
	}

	public static Dictionary<string, Recipe> recipeCache = new Dictionary<string, Recipe>();

	[JsonProperty]
	public string id;

	[JsonProperty]
	public List<Recipe.Ingredient> ingredients = new List<Recipe.Ingredient>();

	[JsonProperty]
	public bool UseStock;

	[JsonProperty]
	public bool IngAsProduct;

	[JsonProperty]
	public bool VirtualBlock;

	public int _dir;

	public int sync;

	public int idMat = -1;

	public int idSkin;

	public Recipe.State state;

	public RecipeSource _source;

	public enum State
	{
		Valid,
		Invalid
	}

	public class Ingredient : EClass
	{
		public bool IsThingSpecified
		{
			get
			{
				return this.uid != 0;
			}
		}

		public string IdThing
		{
			get
			{
				if (!this.useCat)
				{
					return this.id;
				}
				return EClass.sources.categories.map[this.id].GetIdThing();
			}
		}

		public Thing RefreshThing()
		{
			if (this.thing == null)
			{
				this.thing = EClass.pc.things.Find(this.uid);
				if (this.thing == null)
				{
					this.thing = EClass._map.Stocked.Find(this.uid);
					if (this.thing == null)
					{
						this.thing = EClass.pc.things.Find(this.id, this.mat, this.refVal);
						if (this.thing == null)
						{
							this.thing = EClass._map.Stocked.Find(this.id, this.mat, this.refVal, false);
						}
					}
				}
			}
			return this.thing;
		}

		public bool CanSetThing(Thing t)
		{
			return !(t.id != this.id) && t.Num >= this.req;
		}

		public void SetThing(Thing t = null)
		{
			this.uid = ((t != null) ? t.uid : 0);
			this.thing = t;
			this.mat = ((t != null) ? t.material.id : -1);
		}

		public string GetName()
		{
			if (this.useCat)
			{
				string text = Recipe.Ingredient.<GetName>g__CatName|18_0(this.id);
				if (this.idOther.Count > 0)
				{
					foreach (string text2 in this.idOther)
					{
						text = text + ", " + Recipe.Ingredient.<GetName>g__CatName|18_0(text2);
					}
				}
				return "ingCat".lang(text, null, null, null, null);
			}
			string str = EClass.sources.cards.map[this.id].GetName().IsEmpty(("card_" + this.id).lang());
			string text3 = this.tag.IsEmpty() ? "" : ("(" + ("tag_" + this.tag).lang() + ")");
			if (this.idOther.Count > 0)
			{
				foreach (string key in this.idOther)
				{
					text3 = text3 + ", " + EClass.sources.cards.map[key].GetName();
				}
			}
			return str + text3;
		}

		[CompilerGenerated]
		internal static string <GetName>g__CatName|18_0(string id)
		{
			return EClass.sources.categories.map[id].GetName();
		}

		[JsonProperty]
		public int uid;

		[JsonProperty]
		public int req;

		[JsonProperty]
		public int mat = -1;

		[JsonProperty]
		public int refVal = -1;

		[JsonProperty]
		public string id;

		[JsonProperty]
		public string tag;

		[JsonProperty]
		public List<string> idOther = new List<string>();

		public Thing thing;

		public bool optional;

		public bool dye;

		public bool useCat;
	}
}
