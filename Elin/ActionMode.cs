using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionMode : EClass
{
	public static AM_Adv AdvOrRegion
	{
		get
		{
			if (!EClass._zone.IsRegion)
			{
				return ActionMode.Adv;
			}
			return ActionMode.Region;
		}
	}

	public static bool IsAdv
	{
		get
		{
			return ActionMode.DefaultMode is AM_Adv;
		}
	}

	public static void OnGameInstantiated()
	{
		ActionMode.LastBuildMode = null;
		ActionMode.Sim = new AM_Sim();
		ActionMode.View = new AM_ViewZone();
		ActionMode.Adv = new AM_Adv();
		ActionMode.AdvTarget = new AM_ADV_Target();
		ActionMode.Region = new AM_Region();
		ActionMode.NoMap = new AM_NoMap();
		ActionMode.MiniGame = new AM_MiniGame();
		ActionMode.EloMap = new AM_EloMap();
		ActionMode.NewZone = new AM_NewZone();
		ActionMode.Select = new AM_Select();
		ActionMode.Inspect = new AM_Inspect();
		ActionMode.Bird = new AM_Bird();
		ActionMode.DefaultMode = ActionMode.Adv;
		ActionMode.Mine = new AM_Mine();
		ActionMode.Dig = new AM_Dig();
		ActionMode.Harvest = new AM_Harvest();
		ActionMode.Cut = new AM_Cut();
		ActionMode.Copy = new AM_Copy();
		ActionMode.Blueprint = new AM_Blueprint();
		ActionMode.Build = new AM_Build();
		ActionMode.Picker = new AM_Picker();
		ActionMode.StateEditor = new AM_StateEditor();
		ActionMode.CreateArea = new AM_CreateArea();
		ActionMode.EditArea = new AM_EditArea();
		ActionMode.ExpandArea = new AM_ExpandArea();
		ActionMode.RemoveDesignation = new AM_RemoveDesignation();
		ActionMode.Deconstruct = new AM_Deconstruct();
		ActionMode.ViewMap = new AM_ViewMap();
		ActionMode.Terrain = new AM_Terrain();
		ActionMode.Populate = new AM_Populate();
		ActionMode.EditMarker = new AM_EditMarker();
		ActionMode.Visibility = new AM_Visibility();
		ActionMode.Cinema = new AM_Cinema();
		ActionMode.FlagCell = new AM_FlagCell();
		ActionMode.Paint = new AM_Paint();
	}

	public virtual float gameSpeed
	{
		get
		{
			return 1f;
		}
	}

	public bool IsActive
	{
		get
		{
			return EClass.scene.actionMode == this;
		}
	}

	public virtual BaseTileSelector.HitType hitType
	{
		get
		{
			return BaseTileSelector.HitType.Floor;
		}
	}

	public virtual BaseTileSelector.SelectType selectType
	{
		get
		{
			return BaseTileSelector.SelectType.Multiple;
		}
	}

	public virtual BaseTileSelector.BoxType boxType
	{
		get
		{
			return BaseTileSelector.BoxType.Box;
		}
	}

	public virtual bool ContinuousClick
	{
		get
		{
			return false;
		}
	}

	public virtual int hitW
	{
		get
		{
			return 1;
		}
	}

	public virtual int hitH
	{
		get
		{
			return 1;
		}
	}

	public HitSummary Summary
	{
		get
		{
			return this.tileSelector.summary;
		}
	}

	public bool Multisize
	{
		get
		{
			return this.hitW != 1 || this.hitW != 1;
		}
	}

	public virtual string id
	{
		get
		{
			return base.GetType().Name.Split('_', StringSplitOptions.None)[1];
		}
	}

	public virtual CursorInfo DefaultCursor
	{
		get
		{
			return null;
		}
	}

	public virtual string idHelpTopic
	{
		get
		{
			return "4";
		}
	}

	public virtual string idSound
	{
		get
		{
			return null;
		}
	}

	public virtual bool enableMouseInfo
	{
		get
		{
			return false;
		}
	}

	public virtual bool hideBalloon
	{
		get
		{
			return false;
		}
	}

	public virtual string textHintTitle
	{
		get
		{
			return null;
		}
	}

	public virtual bool AllowAutoClick
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShowActionHint
	{
		get
		{
			return true;
		}
	}

	public virtual bool ShowMouseoverTarget
	{
		get
		{
			return true;
		}
	}

	public virtual AreaHighlightMode AreaHihlight
	{
		get
		{
			return AreaHighlightMode.None;
		}
	}

	public virtual bool CanSelectTile
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanTargetOutsideBounds
	{
		get
		{
			return true;
		}
	}

	public virtual bool ShouldPauseGame
	{
		get
		{
			return true;
		}
	}

	public virtual bool FixFocus
	{
		get
		{
			return false;
		}
	}

	public virtual bool HideSubWidgets
	{
		get
		{
			return true;
		}
	}

	public virtual bool IsBuildMode
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShowBuildWidgets
	{
		get
		{
			return this.IsBuildMode;
		}
	}

	public virtual BuildMenu.Mode buildMenuMode
	{
		get
		{
			if (!this.IsBuildMode)
			{
				return BuildMenu.Mode.None;
			}
			return BuildMenu.Mode.Hide;
		}
	}

	public virtual bool ShouldHideBuildMenu
	{
		get
		{
			return this.IsBuildMode && this.tileSelector.start != null;
		}
	}

	public virtual bool CanTargetFog
	{
		get
		{
			return false;
		}
	}

	public virtual int CostMoney
	{
		get
		{
			return 0;
		}
	}

	public virtual bool AllowBuildModeShortcuts
	{
		get
		{
			return this.IsBuildMode;
		}
	}

	public virtual bool AllowMiddleClickFunc
	{
		get
		{
			return false;
		}
	}

	public virtual bool AllowHotbar
	{
		get
		{
			return true;
		}
	}

	public virtual bool AllowGeneralInput
	{
		get
		{
			return true;
		}
	}

	public virtual bool ShowMaskedThings
	{
		get
		{
			return false;
		}
	}

	public virtual int SelectorHeight
	{
		get
		{
			return -1;
		}
	}

	public virtual bool AllowWheelZoom
	{
		get
		{
			return true;
		}
	}

	public virtual int TopHeight(Point p)
	{
		return -1;
	}

	public virtual float TargetZoom
	{
		get
		{
			return 1f;
		}
	}

	public virtual BaseTileMap.CardIconMode cardIconMode
	{
		get
		{
			return BaseTileMap.CardIconMode.None;
		}
	}

	public virtual BaseGameScreen TargetGameScreen
	{
		get
		{
			return null;
		}
	}

	public virtual bool IsNoMap
	{
		get
		{
			return false;
		}
	}

	public virtual void SEExecuteSummary()
	{
		SE.Play("plan");
	}

	public virtual bool UseSubMenu
	{
		get
		{
			return false;
		}
	}

	public virtual bool UseSubMenuSlider
	{
		get
		{
			return false;
		}
	}

	public virtual bool SubMenuAsGroup
	{
		get
		{
			return false;
		}
	}

	public virtual bool HighlightWall(Point p)
	{
		return false;
	}

	public virtual int SubMenuModeIndex
	{
		get
		{
			return 0;
		}
	}

	public virtual void OnClickSubMenu(int a)
	{
	}

	public virtual string OnSetSubMenuButton(int a, UIButton b)
	{
		return null;
	}

	public virtual bool IsRoofEditMode(Card c = null)
	{
		return false;
	}

	public virtual bool IsFillMode()
	{
		return false;
	}

	public virtual int GetDefaultTile(Point p)
	{
		return 0;
	}

	public virtual bool ShowExtraHint
	{
		get
		{
			return Lang.GetList("exhint_" + base.GetType().Name) != null;
		}
	}

	public virtual void OnShowExtraHint(UINote n)
	{
		string[] list = Lang.GetList("exhint_" + base.GetType().Name);
		if (list == null)
		{
			return;
		}
		foreach (string text in list)
		{
			n.AddText("NoteText_extrahint", text, Color.white);
		}
	}

	public BaseTileSelector tileSelector
	{
		get
		{
			return EClass.screen.tileSelector;
		}
	}

	public void Activate(bool toggle = true, bool forceActivate = false)
	{
		if (this.TargetGameScreen != null)
		{
			this.TargetGameScreen.Activate();
		}
		if (EClass.scene.actionMode == this && !forceActivate)
		{
			if (toggle && EClass.scene.actionMode != ActionMode.DefaultMode)
			{
				ActionMode.DefaultMode.Activate(true, false);
			}
			return;
		}
		if (ActionMode.ignoreSound)
		{
			ActionMode.ignoreSound = false;
		}
		else
		{
			EClass.Sound.Play(this.idSound);
		}
		EInput.Consume(0);
		ActionMode actionMode = EClass.scene.actionMode;
		EClass.scene.actionMode = this;
		if (actionMode != null)
		{
			if (actionMode.IsBuildMode && !(actionMode is AM_ViewMap))
			{
				ActionMode.LastBuildMode = actionMode;
			}
			actionMode.ClearSimpleTexts();
			actionMode.OnDeactivate();
			if (EClass.core.IsGameStarted)
			{
				foreach (Card card in EClass._map.things.Concat(EClass._map.charas))
				{
					card.renderer.DespawnSimpleText();
				}
			}
			AM_NoMap noMap = ActionMode.NoMap;
			EClass.ui.RemoveLayers(false);
		}
		if (this.IsBuildMode)
		{
			BuildMenu.Activate();
			BuildMenu.Instance.terrainMenu.Show(this);
		}
		else
		{
			BuildMenu.Deactivate();
		}
		if (!(this is AM_Adv))
		{
			EClass.ui.hud.transRight.SetActive(false);
		}
		EClass.ui.hud.frame.SetActive(this.IsBuildMode && EClass.game.altUI);
		if (this.hideBalloon)
		{
			EClass.ui.ShowBalloon(false);
		}
		else
		{
			EClass.ui.ShowBalloon(!EClass.scene.hideBalloon);
		}
		this.OnActivate();
		this.RefreshTexts();
		this.ShowLayer();
		EClass.ui.widgets.OnChangeActionMode();
		EClass.ui.extraHint.OnChangeActionMode();
		CursorSystem.leftIcon = null;
		EClass.scene.UpdateCursor();
	}

	public virtual void OnActivate()
	{
	}

	public unsafe void RefreshTexts()
	{
		if (!EClass.core.IsGameStarted)
		{
			return;
		}
		this.ClearSimpleTexts();
		if (EClass._zone.IsRegion)
		{
			foreach (Spatial spatial in EClass._zone.children)
			{
				if (!spatial.destryoed)
				{
					Sprite sprite = null;
					if (spatial.isRandomSite && !spatial.isConquered && spatial.visitCount > 0)
					{
						sprite = EClass.core.refs.tcs.spriteVisited;
					}
					else if (spatial.isConquered)
					{
						sprite = EClass.core.refs.tcs.spriteConquer;
					}
					else if (!spatial.IsPlayerFaction && spatial is Zone_Field && spatial.isDeathLocation)
					{
						sprite = EClass.core.refs.tcs.spriteDeath;
					}
					if (sprite)
					{
						TCSimpleText tcsimpleText = TCSimpleText.SpawnIcon(sprite);
						ActionMode.simpleTexts.Add(tcsimpleText);
						Cell cell = spatial.RegionPos.cell;
						tcsimpleText.transform.position = *cell.GetPoint().PositionTopdown() + EClass.setting.render.tc.simpleTextPos;
					}
				}
			}
		}
		if (!this.IsBuildMode || !this.ShowMaskedThings)
		{
			return;
		}
		foreach (Card card in EClass._map.things.Concat(EClass._map.charas))
		{
			string simpleText = this.GetSimpleText(card);
			if (!simpleText.IsEmpty())
			{
				TCSimpleText tcsimpleText2 = TCSimpleText.Spawn();
				Popper.SetText(tcsimpleText2.tm, simpleText);
				ActionMode.simpleTexts.Add(tcsimpleText2);
				tcsimpleText2.transform.position = *card.pos.Position() + EClass.setting.render.tc.simpleTextPos;
			}
		}
		if (Application.isEditor)
		{
			foreach (KeyValuePair<int, int> keyValuePair in EClass._map.backerObjs)
			{
				SourceBacker.Row row = EClass.sources.backers.map[keyValuePair.Value];
				string text = "Backer:" + row.id.ToString() + "/" + row.Name;
				if (row.isStatic != 0)
				{
					text = "★" + text;
				}
				TCSimpleText tcsimpleText3 = TCSimpleText.Spawn();
				Popper.SetText(tcsimpleText3.tm, text);
				ActionMode.simpleTexts.Add(tcsimpleText3);
				Cell cell2 = EClass._map.GetCell(keyValuePair.Key);
				tcsimpleText3.transform.position = *cell2.GetPoint().Position() + EClass.setting.render.tc.simpleTextPos;
			}
			foreach (Card card2 in EClass._map.Cards)
			{
				if (card2.isBackerContent)
				{
					SourceBacker.Row row2 = EClass.sources.backers.map.TryGetValue(card2.c_idBacker, null);
					if (row2 != null)
					{
						string text2 = "Backer:" + row2.id.ToString() + "/" + row2.Name;
						if (row2.isStatic != 0)
						{
							text2 = "★" + text2;
						}
						TCSimpleText tcsimpleText4 = TCSimpleText.Spawn();
						Popper.SetText(tcsimpleText4.tm, text2);
						ActionMode.simpleTexts.Add(tcsimpleText4);
						tcsimpleText4.transform.position = card2.renderer.position + EClass.setting.render.tc.simpleTextPos;
					}
				}
			}
		}
	}

	public virtual string GetSimpleText(Card c)
	{
		if (c.trait is TraitRoomPlate && c.Cell.room != null)
		{
			return "#" + c.Cell.room.data.group.ToString();
		}
		return null;
	}

	public void ClearSimpleTexts()
	{
		foreach (TCSimpleText tcsimpleText in ActionMode.simpleTexts)
		{
			if (tcsimpleText != null)
			{
				PoolManager.Despawn(tcsimpleText);
			}
		}
		ActionMode.simpleTexts.Clear();
	}

	public void Deactivate()
	{
		ActionMode.DefaultMode.Activate(true, false);
	}

	public virtual void OnDeactivate()
	{
	}

	public virtual void OnCancel()
	{
		if (this.IsBuildMode && !EClass.core.config.input.rightClickExitBuildMode && !Input.GetKey(KeyCode.Escape))
		{
			return;
		}
		this.Deactivate();
	}

	public virtual void OnBeforeUpdate()
	{
	}

	public virtual void OnAfterUpdate()
	{
	}

	public virtual void OnUpdateCursor()
	{
		CursorSystem.leftIcon = null;
		CursorSystem.SetCursor(null, 0);
	}

	public void OnRotate()
	{
		this.RotateUnderMouse();
	}

	public void SetCursorOnMap(CursorInfo cursor)
	{
		CursorSystem.SetCursor(EClass.ui.isPointerOverUI ? null : cursor, 0);
	}

	public void UpdateInput()
	{
		ActionMode.mpos = Input.mousePosition;
		if (!this.IsNoMap)
		{
			EClass.scene.mouseTarget.Update(Scene.HitPoint);
			if (WidgetMouseover.Instance)
			{
				WidgetMouseover.Instance.Refresh();
			}
		}
		if (LayerAbility.hotElement != null)
		{
			ActionMode.hotElementTimer += Core.delta;
			ButtonAbility hotElement = LayerAbility.hotElement;
			if (EInput.leftMouse.down)
			{
				if (EClass.core.config.game.doubleClickToHold && ActionMode.hotElementTimer < 0.35f)
				{
					EClass.player.SetCurrentHotItem(new HotItemAct(hotElement.source));
					SE.SelectHotitem();
				}
				else
				{
					ButtonGrid componentOf = InputModuleEX.GetComponentOf<ButtonGrid>();
					if (componentOf && componentOf.invOwner != null && componentOf.invOwner.owner == EClass.pc && (componentOf.card == null || componentOf.card.trait is TraitAbility) && !(componentOf.invOwner is InvOwnerEquip))
					{
						if (componentOf.card != null)
						{
							componentOf.card.Destroy();
						}
						SE.Equip();
						CardBlueprint.SetNormalRarity(false);
						Thing thing = ThingGen.Create("ability", -1, -1);
						int num = (componentOf.invOwner is InvOwnerHotbar) ? 1 : 0;
						componentOf.invOwner.Container.AddThing(thing, false, componentOf.index, num);
						thing.c_idAbility = hotElement.source.alias;
						thing.invY = num;
						thing.invX = componentOf.index;
						WidgetCurrentTool.dirty = true;
					}
					else
					{
						SE.BeepSmall();
					}
				}
				LayerAbility.ClearHotElement();
			}
			if (EInput.rightMouse.down)
			{
				LayerAbility.ClearHotElement();
			}
		}
		else
		{
			ActionMode.hotElementTimer = 0f;
		}
		ActionMode.focusTimer += Core.delta;
		if (EClass.ui.isPointerOverUI && (EInput.leftMouse.down || EInput.rightMouse.down || (EClass.core.config.ui.autoFocusWindow && !Input.GetMouseButton(0) && ActionMode.focusTimer > 0.2f)))
		{
			LayerInventory componentOf2 = InputModuleEX.GetComponentOf<LayerInventory>();
			if (componentOf2 != null && EClass.ui.layerFloat.layers.Contains(componentOf2))
			{
				if (EInput.rightMouse.down)
				{
					if (!componentOf2.mainInv && !InputModuleEX.GetComponentOf<UIButton>() && !componentOf2.windows[0].saveData.noRightClickClose)
					{
						componentOf2.Close();
					}
				}
				else
				{
					componentOf2.transform.SetAsLastSibling();
				}
			}
			LayerAbility componentOf3 = InputModuleEX.GetComponentOf<LayerAbility>();
			if (componentOf3 != null && EClass.ui.layerFloat.layers.Contains(componentOf3))
			{
				componentOf3.transform.SetAsLastSibling();
			}
			ActionMode.focusTimer = 0f;
		}
		if ((EInput.isShiftDown && Input.GetMouseButton(0)) || (EInput.rightMouse.dragging2 && !Input.GetMouseButtonDown(1) && Input.GetMouseButton(1)))
		{
			bool rightMouse = !Input.GetMouseButtonDown(1) && Input.GetMouseButton(1);
			ButtonGrid componentOf4 = InputModuleEX.GetComponentOf<ButtonGrid>();
			if (componentOf4 != null && componentOf4.invOwner != null && componentOf4.card != null && componentOf4.invOwner.CanShiftClick(componentOf4, rightMouse) && EClass.ui.AllowInventoryInteractions)
			{
				componentOf4.invOwner.OnShiftClick(componentOf4, rightMouse);
			}
		}
		if (EInput.rightMouse.down)
		{
			if (EClass.ui.contextMenu.isActive)
			{
				EClass.ui.contextMenu.currentMenu.Hide();
				EInput.rightMouse.Consume();
				Layer.rightClicked = false;
			}
			if (DropdownGrid.IsActive && !EClass.ui.GetLayer<LayerInfo>(false))
			{
				DropdownGrid.Instance.Deactivate();
				EInput.rightMouse.Consume();
				Layer.rightClicked = false;
			}
		}
		if (EInput.middleMouse.down && this.TryShowWidgetMenu())
		{
			EInput.Consume(false, 1);
		}
		CoreConfig.InputSetting input = EClass.core.config.input;
		bool flag = (EInput.mouse3.clicked && input.mouse3Click == CoreConfig.GameFunc.AllAction) || (EInput.mouse4.clicked && input.mouse4Click == CoreConfig.GameFunc.AllAction) || (EInput.mouse3.pressedLong && input.mouse3PressLong == CoreConfig.GameFunc.AllAction) || (EInput.mouse4.pressedLong && input.mouse4PressLong == CoreConfig.GameFunc.AllAction);
		if (flag || EInput.middleMouse.down || EInput.middleMouse.clicked || EInput.middleMouse.pressedLong)
		{
			UIButton componentOf5 = InputModuleEX.GetComponentOf<UIButton>();
			if (componentOf5 && componentOf5.CanMiddleClick() && (flag || EInput.middleMouse.clicked || EInput.middleMouse.pressedLong))
			{
				componentOf5.OnMiddleClick(flag);
			}
		}
		if (EClass.ui.contextMenu.currentMenu)
		{
			UIContextMenu.timeSinceClosed = 0f;
		}
		else
		{
			UIContextMenu.timeSinceClosed += Time.deltaTime;
		}
		if (EClass.ui.currentDrag != null)
		{
			EClass.ui.OnDrag();
			if (EInput.leftMouse.down || EInput.rightMouse.down)
			{
				if (Input.GetMouseButton(0) && EInput.rightMouse.down)
				{
					EClass.ui.OnDragSpecial();
				}
				else
				{
					EClass.ui.EndDrag(EInput.rightMouse.down);
				}
			}
		}
		ButtonState middleMouse = EInput.middleMouse;
		if (middleMouse.pressedLongAction != null)
		{
			if (middleMouse.pressedLong)
			{
				middleMouse.pressedLongAction();
				middleMouse.pressedLongAction = null;
			}
			else if (!EInput.middleMouse.pressing)
			{
				middleMouse.pressedLongAction = null;
			}
		}
		EClass.ui.UpdateInput();
		if (EInput.action == EAction.Mute)
		{
			EClass.scene.ToggleMuteBGM();
		}
		if (this.AllowGeneralInput && !EClass.ui.IsDragging)
		{
			if (Input.GetKeyDown(KeyCode.Tab) && !EClass.debug.debugInput && !EClass.ui.BlockInput && !EInput.waitReleaseAnyKey)
			{
				if (!EClass.ui.IsInventoryOpen)
				{
					SE.PopInventory();
				}
				EClass.ui.ToggleInventory(false);
			}
			if (!EClass.ui.BlockInput && !this.IsBuildMode && !EInput.waitReleaseAnyKey)
			{
				switch (EInput.action)
				{
				case EAction.MenuInventory:
					EClass.ui.ToggleInventory(false);
					break;
				case EAction.MenuChara:
				{
					LayerChara layerChara = EClass.ui.ToggleLayer<LayerChara>(null);
					if (layerChara != null)
					{
						layerChara.SetChara(EClass.pc);
					}
					break;
				}
				case EAction.MenuJournal:
					EClass.ui.ToggleLayer<LayerJournal>(null);
					break;
				case EAction.MenuAbility:
					if (!EClass.debug.enable)
					{
						EClass.ui.ToggleAbility(false);
					}
					break;
				}
			}
			if (!this.FixFocus)
			{
				this.InputMovement();
			}
			if (!EClass.ui.canvas.enabled && EInput.IsAnyKeyDown(true))
			{
				EClass.ui.canvas.enabled = true;
				EInput.Consume(false, 1);
				return;
			}
		}
		if (EInput.action == EAction.Examine && !this.IsBuildMode)
		{
			ButtonGrid componentOf6 = InputModuleEX.GetComponentOf<ButtonGrid>();
			Card card = null;
			if (componentOf6 && componentOf6.card != null)
			{
				card = componentOf6.card;
			}
			if (card == null)
			{
				UIItem componentOf7 = InputModuleEX.GetComponentOf<UIItem>();
				if (componentOf7 && componentOf7.refObj is Thing)
				{
					card = (Thing)componentOf7.refObj;
				}
			}
			if (card == null)
			{
				LayerCraft layerCraft = EClass.ui.GetLayer<LayerCraft>(false);
				if (layerCraft)
				{
					card = layerCraft.product;
				}
			}
			if (card != null)
			{
				if (EClass.ui.GetLayer<LayerInfo>(false))
				{
					EClass.ui.RemoveLayer<LayerInfo>();
				}
				EClass.ui.AddLayer<LayerInfo>().Set(card, true);
			}
		}
		if (EInput.buttonCtrl.clicked && !LayerDrama.Instance && EClass.scene.actionMode is AM_Adv && WidgetCurrentTool.Instance)
		{
			WidgetCurrentTool.Instance.SwitchPage();
		}
		if (EClass.ui.BlockActions)
		{
			return;
		}
		if (this.AllowBuildModeShortcuts && Input.GetKeyDown(KeyCode.Q) && ActionMode.Picker.CanActivate)
		{
			ActionMode.Picker.Activate(true, false);
		}
		if (EClass.debug.godBuild && EInput.isCtrlDown && EInput.wheel != 0 && Scene.HitPoint.IsValid && HotItemHeld.taskBuild == null)
		{
			Point hitPoint = Scene.HitPoint;
			bool flag2 = EInput.wheel > 0;
			if (EClass.scene.mouseTarget.card != null)
			{
				Card card2 = EClass.scene.mouseTarget.card;
				if (card2.isThing && card2.IsInstalled)
				{
					SourceMaterial.Row currentMat = card2.isDyed ? card2.DyeMat : card2.material;
					List<SourceMaterial.Row> source = (from m in EClass.sources.materials.rows
					where m.thing == currentMat.thing
					select m).ToList<SourceMaterial.Row>();
					if (EInput.isShiftDown)
					{
						source = EClass.sources.materials.rows;
					}
					SourceMaterial.Row row = flag2 ? source.NextItem(currentMat) : source.PrevItem(currentMat);
					if (EInput.isAltDown)
					{
						row = ActionMode.lastEditorMat;
					}
					if (card2.isDyed)
					{
						card2.Dye(row);
					}
					else
					{
						card2.ChangeMaterial(row);
					}
					ActionMode.lastEditorMat = row;
					Msg.Say(row.GetName() + "(" + row.alias + ")");
				}
			}
			else if (hitPoint.HasObj)
			{
				SourceMaterial.Row matObj = hitPoint.cell.matObj;
				List<SourceMaterial.Row> rows = EClass.sources.materials.rows;
				SourceMaterial.Row row2 = flag2 ? rows.NextItem(matObj) : rows.PrevItem(matObj);
				if (EInput.isAltDown)
				{
					row2 = ActionMode.lastEditorMat;
				}
				hitPoint.cell.objMat = (byte)row2.id;
				ActionMode.lastEditorMat = row2;
				Msg.Say(row2.GetName());
			}
			EInput.Consume(false, 1);
		}
		if (this.AllowHotbar && !this.IsBuildMode && EClass.debug.debugHotkeys == CoreDebug.DebugHotkey.None)
		{
			if (EInput.hotkey != -1)
			{
				WidgetCurrentTool.Instance.Select(EInput.hotkey, true);
				ActionMode.AdvOrRegion.UpdatePlans();
			}
			else if (!EClass.debug.enable && EInput.functionkey != -1)
			{
				WidgetHotbar hotbarExtra = WidgetHotbar.HotbarExtra;
				if (hotbarExtra)
				{
					hotbarExtra.TryUse(EInput.functionkey);
				}
			}
		}
		this.OnUpdateInput();
		if (EClass.debug.enable)
		{
			EClass.core.debug.UpdateInput();
		}
		ActionMode.textTimer += Core.delta;
		if (ActionMode.textTimer > 0.1f)
		{
			ActionMode.textTimer = 0f;
			this.RefreshTexts();
		}
		if (this.IsBuildMode && this.AllowBuildModeShortcuts)
		{
			if (EInput.middleMouse.clicked)
			{
				this.DoFunc(input.b_middleClick);
			}
			else if (EInput.middleMouse.pressedLong)
			{
				this.DoFunc(input.b_middlePressLong);
				EInput.middleMouse.Consume();
			}
			if (EInput.mouse3.clicked)
			{
				this.DoFunc(input.b_mouse3Click);
			}
			else if (EInput.mouse3.pressedLong)
			{
				this.DoFunc(input.b_mouse3PressLong);
				EInput.mouse3.Consume();
			}
			if (EInput.mouse4.clicked)
			{
				this.DoFunc(input.b_mouse4Click);
				return;
			}
			if (EInput.mouse4.pressedLong)
			{
				this.DoFunc(input.b_mouse4PressLong);
				EInput.mouse4.Consume();
			}
		}
	}

	public void InputMovement()
	{
		float num = 0f;
		float num2 = 0f;
		Vector2 vector = EInput.axis;
		bool flag = !EClass.core.IsGameStarted;
		if ((!this.FixFocus && !EInput.rightMouse.pressing) || EClass.scene.actionMode.IsBuildMode)
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				vector *= 3f;
			}
			if (EClass.core.config.camera.edgeScroll)
			{
				Vector2 zero = Vector2.zero;
				if (ActionMode.mpos.x < 16f)
				{
					zero.x -= 1f;
				}
				if (ActionMode.mpos.x > (float)(Screen.width - 16))
				{
					zero.x += 1f;
				}
				if (ActionMode.mpos.y < 8f)
				{
					zero.y -= 1f;
				}
				if (ActionMode.mpos.y > (float)(Screen.height - 8))
				{
					zero.y += 1f;
				}
				if (zero != Vector2.zero)
				{
					vector += zero * (EClass.core.config.camera.sensEdge * 2f);
				}
			}
			EInput.hasAxisMoved = (vector != Vector2.zero);
			ButtonState buttonScroll = EInput.buttonScroll;
			if (ActionMode.Adv.IsActive)
			{
				bool zoomOut = ActionMode.Adv.zoomOut;
			}
			if (EInput.buttonScroll != null && EInput.buttonScroll.pressing && (flag || !EClass.ui.BlockActions))
			{
				num = EInput.dragAmount.x * (0.1f + EClass.core.config.camera.sensDrag);
				num2 = EInput.dragAmount.y * (0.1f + EClass.core.config.camera.sensDrag);
				if (!EClass.core.config.camera.invertX)
				{
					num *= -1f;
				}
				if (!EClass.core.config.camera.invertY)
				{
					num2 *= -1f;
				}
				if (EInput.hasMouseMoved)
				{
					EInput.hasAxisMoved = true;
				}
			}
			if (EInput.hasAxisMoved)
			{
				EClass.screen.zoomPos = Vector3.zero;
			}
			if (EInput.wheel != 0 && !InputModuleEX.GetComponentOf<UIScrollView>())
			{
				this.InputWheel(EInput.wheel);
			}
		}
		float momentum = EClass.core.config.camera.momentum;
		if (momentum > 0f && EInput.axis == Vector2.zero)
		{
			ActionMode.smoothX = Mathf.Lerp(ActionMode.smoothX, num, Time.smoothDeltaTime * momentum);
			ActionMode.smoothY = Mathf.Lerp(ActionMode.smoothY, num2, Time.smoothDeltaTime * momentum);
		}
		else
		{
			ActionMode.smoothX = num;
			ActionMode.smoothY = num2;
		}
		EClass.screen.ScrollMouse(ActionMode.smoothX * 0.1f, ActionMode.smoothY * 0.1f);
		if (EInput.hasAxisMoved)
		{
			this.OnScroll();
			EClass.screen.ScrollAxis(vector, false);
		}
	}

	public virtual void InputWheel(int wheel)
	{
		if (this.AllowWheelZoom)
		{
			EClass.screen.ModTargetZoomIndex(-wheel);
		}
	}

	public virtual void OnUpdateInput()
	{
	}

	public void DoFunc(CoreConfig.GameFuncBuild func)
	{
		switch (func)
		{
		case CoreConfig.GameFuncBuild.ExitBuildMode:
			ActionMode.DefaultMode.Activate(true, false);
			break;
		case CoreConfig.GameFuncBuild.Rotate:
			this.OnRotate();
			break;
		case CoreConfig.GameFuncBuild.ToggleFreepos:
			EClass.scene.ToggleFreePos();
			break;
		case CoreConfig.GameFuncBuild.SnapFreepos:
			EClass.scene.ToggleSnapFreePos();
			break;
		case CoreConfig.GameFuncBuild.ToggleRoof:
			EClass.scene.ToggleShowRoof();
			break;
		case CoreConfig.GameFuncBuild.ToggleSlope:
			EClass.scene.ToggleSlope();
			break;
		case CoreConfig.GameFuncBuild.ToggleWall:
			EClass.scene.ToggleShowWall();
			break;
		case CoreConfig.GameFuncBuild.TogglePicker:
			if (!ActionMode.Picker.CanActivate)
			{
				ActionMode.Picker.Activate(true, false);
			}
			break;
		case CoreConfig.GameFuncBuild.ToggleBuildLight:
			EClass.scene.ToggleLight();
			break;
		}
		WidgetHotbar.RefreshHighlights();
	}

	public void DoFunc(CoreConfig.GameFunc func)
	{
		Chara targetChara = EClass.scene.mouseTarget.TargetChara;
		switch (func)
		{
		case CoreConfig.GameFunc.ToggleZoom:
			ActionMode.Adv.ToggleZoom();
			break;
		case CoreConfig.GameFunc.ShowInv:
			EClass.ui.ToggleInventory(true);
			break;
		case CoreConfig.GameFunc.ShowChara:
		{
			LayerChara layerChara = EClass.ui.ToggleLayer<LayerChara>(null);
			if (layerChara)
			{
				layerChara.SetChara(EClass.pc);
				layerChara.windows[0].SetRect(EClass.core.refs.rects.center, false);
				layerChara.Delay(0.05f);
			}
			break;
		}
		case CoreConfig.GameFunc.ShowAbility:
			EClass.ui.ToggleAbility(true);
			break;
		case CoreConfig.GameFunc.ToggleBuild:
			if (!EClass._zone.CanEnterBuildModeAnywhere)
			{
				SE.Beep();
				Msg.Say("invalidAction");
				return;
			}
			ActionMode.Inspect.Activate(true, false);
			break;
		case CoreConfig.GameFunc.ShowJournal:
		{
			LayerJournal layerJournal = EClass.ui.ToggleLayer<LayerJournal>(null);
			if (layerJournal)
			{
				layerJournal.windows[0].SetRect(EClass.core.refs.rects.center, false);
				layerJournal.Delay(0.05f);
			}
			break;
		}
		case CoreConfig.GameFunc.EmuShift:
		case CoreConfig.GameFunc.EmuAlt:
			return;
		case CoreConfig.GameFunc.AllAction:
			if (!EClass.ui.isPointerOverUI)
			{
				ActionMode.Adv.ShowAllAction();
			}
			return;
		case CoreConfig.GameFunc.ToggleNoRoof:
			EClass.game.config.noRoof = !EClass.game.config.noRoof;
			break;
		case CoreConfig.GameFunc.OpenAllyInv:
			if (!EClass.pc.HasNoGoal || (targetChara != null && !targetChara.IsPCFaction))
			{
				return;
			}
			if (EClass.scene.mouseTarget.pos.Equals(EClass.pc.pos))
			{
				if (!EClass.ui.IsInventoryOpen)
				{
					SE.PopInventory();
				}
				EClass.ui.ToggleInventory(false);
			}
			else if (targetChara != null && EClass.pc.Dist(targetChara) <= 1)
			{
				LayerInventory.CreateContainer(targetChara);
			}
			break;
		case CoreConfig.GameFunc.Talk:
			if (!EClass.pc.HasNoGoal || targetChara == null || targetChara.hostility <= Hostility.Enemy || targetChara.IsPC || EClass.pc.Dist(targetChara) > 2)
			{
				return;
			}
			ACT.Chat.Perform(EClass.pc, targetChara, null);
			break;
		case CoreConfig.GameFunc.EmptyHand:
			EInput.action = EAction.EmptyHand;
			return;
		case CoreConfig.GameFunc.Fire:
			if (EInput.keyFire.Update(true))
			{
				EInput.action = EAction.Fire;
				return;
			}
			break;
		case CoreConfig.GameFunc.SwitchHotbar:
			WidgetCurrentTool.Instance.SwitchPage();
			break;
		default:
			return;
		}
		EInput.Consume(false, 1);
	}

	public virtual void OnScroll()
	{
	}

	public void TryRightClickCloseWidget()
	{
		Widget componentOf = InputModuleEX.GetComponentOf<Widget>();
		if (componentOf && componentOf.RightClickToClose)
		{
			componentOf.Close();
		}
	}

	public bool TryShowWidgetMenu()
	{
		Widget componentOf = InputModuleEX.GetComponentOf<Widget>();
		if (!componentOf || !componentOf.CanShowContextMenu())
		{
			return false;
		}
		if (componentOf.RightClickToClose)
		{
			componentOf.Close();
			return true;
		}
		componentOf.ShowContextMenu();
		return true;
	}

	public HitResult _HitTest(Point point, Point start)
	{
		if (!EClass.debug.ignoreBuildRule && !this.CanTargetOutsideBounds && (!point.IsValid || !point.IsInBoundsPlus))
		{
			return HitResult.NoTarget;
		}
		if (!point.IsValid)
		{
			return HitResult.NoTarget;
		}
		if (!this.CanTargetFog && !point.cell.isSeen)
		{
			return HitResult.Default;
		}
		return this.HitTest(point, start);
	}

	public virtual HitResult HitTest(Point point, Point start)
	{
		return HitResult.Default;
	}

	public virtual void OnSelectStart(Point point)
	{
	}

	public virtual void OnSelectEnd(bool cancel)
	{
	}

	public virtual MeshPass GetGuidePass(Point point)
	{
		if ((!point.HasNonWallBlock && !point.HasBlockRecipe && point.IsSeen) || (point.sourceBlock.tileType.Invisible && !this.IsBuildMode))
		{
			return EClass.screen.guide.passGuideFloor;
		}
		return EClass.screen.guide.passGuideBlock;
	}

	public unsafe virtual void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (result == HitResult.NoTarget || !point.IsValid || (!point.IsSeen && !this.CanTargetFog))
		{
			return;
		}
		MeshPass guidePass = this.GetGuidePass(point);
		int num = (int)result;
		if (this.tileSelector.start != null && result == HitResult.Default)
		{
			num = 2;
		}
		Vector3 vector = *point.Position();
		if (num == 0)
		{
			num = this.GetDefaultTile(point);
		}
		guidePass.Add(vector.x, vector.y, vector.z - 0.01f, (float)num, 0.3f);
		if (!this.IsRoofEditMode(null) && point.HasWallOrFence && !point.cell.hasDoor && this.HighlightWall(point))
		{
			EClass.screen.guide.DrawWall(point, EClass.Colors.blockColors.Passive, true, 0f);
		}
		if (this.IsBuildMode && EClass.screen.tileSelector.start == null && point.Installed != null && ActionMode.Inspect.IsActive)
		{
			point.Installed.trait.OnRenderTile(point, result, dir);
		}
	}

	public unsafe void OnRenderTileFloor(Point point, HitResult result)
	{
		if (result == HitResult.NoTarget || !point.IsValid || (!point.IsSeen && !this.CanTargetFog))
		{
			return;
		}
		MeshPass guidePass = this.GetGuidePass(point);
		int num = (int)result;
		if (this.tileSelector.start != null && result == HitResult.Default)
		{
			num = 2;
		}
		Vector3 vector = *point.Position((int)point.cell.height);
		if (num == 0)
		{
			num = this.GetDefaultTile(point);
		}
		guidePass.Add(vector.x, vector.y, vector.z - 0.01f, (float)num, 0.3f);
	}

	public virtual bool CanProcessTiles()
	{
		return !this.Multisize || this.Summary.countValid == this.hitW * this.hitH;
	}

	public virtual void OnBeforeProcessTiles()
	{
	}

	public virtual void OnProcessTiles(Point point, int dir)
	{
	}

	public virtual void OnAfterProcessTiles(Point start, Point end)
	{
	}

	public virtual void OnFinishProcessTiles()
	{
	}

	public virtual void OnRefreshSummary(Point point, HitResult result, HitSummary summary)
	{
		if (result == HitResult.Valid || result == HitResult.Warning)
		{
			summary.money += this.CostMoney;
			summary.countValid++;
		}
	}

	public void ShowLayer()
	{
		this.layer = this.OnShowLayer();
		if (this.layer != null)
		{
			CursorSystem.SetCursor(null, 0);
		}
		else
		{
			CursorSystem.SetCursor(this.DefaultCursor, 0);
		}
		if (EClass.core.screen.tileSelector)
		{
			EClass.core.screen.tileSelector.OnChangeActionMode();
		}
		EClass.ui.hud.hint.Refresh();
	}

	public virtual Layer OnShowLayer()
	{
		return null;
	}

	public void HideLayer()
	{
		EClass.ui.RemoveLayer(this.layer);
		CursorSystem.SetCursor(CursorSystem.Select, 0);
		EClass.ui.hud.hint.UpdateText();
		this.OnHideLayer();
	}

	public virtual void OnHideLayer()
	{
	}

	public virtual string GetHintText()
	{
		string text = this.textHintTitle.IsEmpty(Lang.Get(this.id)) + "   - ";
		BaseTileSelector tileSelector = EClass.screen.tileSelector;
		ActionMode actionMode = EClass.scene.actionMode;
		if (this.layer)
		{
			if (actionMode == ActionMode.Build)
			{
				text += "hintBuild".lang();
			}
			if (actionMode == ActionMode.CreateArea)
			{
				text += "hintCreateArea".lang();
			}
		}
		else if (actionMode == ActionMode.Inspect && ActionMode.Inspect.target != null)
		{
			text = "hintMoveTarget".lang();
		}
		else if (this.selectType == BaseTileSelector.SelectType.Single)
		{
			text += "hintSelectSingle".lang();
		}
		else if (tileSelector.start == null)
		{
			text += "hintSelectStart".lang();
		}
		else
		{
			text += "hintSelectEnd".lang();
		}
		if (actionMode == ActionMode.Build && ActionMode.Build.recipe != null && tileSelector.start == null)
		{
			if (ActionMode.Build.recipe.CanRotate)
			{
				text = text + "  " + "hintRotate".lang();
			}
			if (ActionMode.Build.MaxAltitude > 0)
			{
				text = text + "  " + "hintAltitude2".lang();
			}
		}
		return text;
	}

	public virtual void RotateUnderMouse()
	{
		SE.Rotate();
		Point hitPoint = Scene.HitPoint;
		if (!hitPoint.IsValid)
		{
			return;
		}
		hitPoint.cell.RotateAll();
		if (EClass.debug.enable)
		{
			GrowSystem growth = hitPoint.growth;
			if (growth != null)
			{
				growth.Grow(100);
			}
			Thing installed = hitPoint.Installed;
			if (((installed != null) ? installed.trait : null) is TraitSeed)
			{
				(hitPoint.Installed.trait as TraitSeed).TrySprout(true, false, null);
				return;
			}
		}
	}

	public virtual ref string SetMouseInfo(ref string s)
	{
		return ref s;
	}

	public void TogglePause()
	{
		if (ActionMode.IsAdv)
		{
			EClass.core.config.game.autopause = !EClass.core.config.game.autopause;
			if (EClass.core.config.game.autopause)
			{
				SE.Play("tick0");
				return;
			}
			SE.Play("tick1");
			return;
		}
		else
		{
			if (EClass.game.gameSpeedIndex != 0)
			{
				this.Pause(true);
				return;
			}
			this.UnPause(true);
			return;
		}
	}

	public void Pause(bool sound = false)
	{
		this.ChangeGameSpeed(1, sound);
	}

	public void UnPause(bool sound = false)
	{
		this.ChangeGameSpeed(EClass.game.lastGameSpeedIndex, sound);
	}

	public void ChangeGameSpeed(int a, bool sound = false)
	{
		if (EClass.game.gameSpeedIndex == a)
		{
			return;
		}
		if (sound)
		{
			SE.Play("tick" + a.ToString());
		}
		EClass.game.gameSpeedIndex = a;
		if (a != 0)
		{
			EClass.game.lastGameSpeedIndex = a;
		}
		WidgetHotbar.RefreshHighlights();
	}

	public static ActionMode DefaultMode;

	public static AM_Title Title = new AM_Title();

	public static AM_Sim Sim;

	public static AM_ViewZone View;

	public static AM_Adv Adv;

	public static AM_Region Region;

	public static AM_ADV_Target AdvTarget;

	public static AM_EloMap EloMap;

	public static AM_Inspect Inspect;

	public static AM_NoMap NoMap;

	public static AM_MiniGame MiniGame;

	public static AM_NewZone NewZone;

	public static AM_Bird Bird;

	public static AM_Mine Mine;

	public static AM_Dig Dig;

	public static AM_Harvest Harvest;

	public static AM_Cut Cut;

	public static AM_StateEditor StateEditor;

	public static AM_Picker Picker;

	public static AM_Copy Copy;

	public static AM_Blueprint Blueprint;

	public static AM_Build Build;

	public static AM_CreateArea CreateArea;

	public static AM_EditArea EditArea;

	public static AM_ExpandArea ExpandArea;

	public static AM_Deconstruct Deconstruct;

	public static AM_Select Select;

	public static AM_RemoveDesignation RemoveDesignation;

	public static AM_ViewMap ViewMap;

	public static AM_Terrain Terrain;

	public static AM_Populate Populate;

	public static AM_EditMarker EditMarker;

	public static AM_Visibility Visibility;

	public static AM_Cinema Cinema;

	public static AM_Paint Paint;

	public static AM_FlagCell FlagCell;

	public static ActionMode LastBuildMode;

	public static SourceMaterial.Row lastEditorMat;

	public static bool ignoreSound;

	private static float smoothX = 0f;

	private static float smoothY = 0f;

	private static float textTimer = 0f;

	protected static Vector3 mpos;

	public static float[] GameSpeeds = new float[]
	{
		0f,
		1f,
		2f,
		5f
	};

	public static List<TCSimpleText> simpleTexts = new List<TCSimpleText>();

	public Layer layer;

	public int brushRadius = 4;

	private static float focusTimer;

	public static float hotElementTimer;
}
