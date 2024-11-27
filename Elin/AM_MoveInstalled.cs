using System;
using System.Collections.Generic;
using UnityEngine;

public class AM_MoveInstalled : AM_Designation<TaskMoveInstalled>
{
	public override bool IsRoofEditMode(Card c = null)
	{
		return ActionMode.Build._IsRoofEditMode(this.target);
	}

	public override int hitW
	{
		get
		{
			if (this.target != null)
			{
				return this.moldCard.W;
			}
			return 1;
		}
	}

	public override int hitH
	{
		get
		{
			if (this.target != null)
			{
				return this.moldCard.H;
			}
			return 1;
		}
	}

	public override int CostMoney
	{
		get
		{
			if (this.target != null)
			{
				return 0;
			}
			return 0;
		}
	}

	public override BaseTileMap.CardIconMode cardIconMode
	{
		get
		{
			return BaseTileMap.CardIconMode.Inspect;
		}
	}

	public override HitResult HitResultOnDesignation(Point p)
	{
		return HitResult.Invalid;
	}

	public override bool AllowMiddleClickFunc
	{
		get
		{
			return this.target == null;
		}
	}

	public bool FreePos
	{
		get
		{
			return EClass.game.config.FreePos;
		}
	}

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			return BaseTileSelector.SelectType.Single;
		}
	}

	public override bool ShouldHideBuildMenu
	{
		get
		{
			return this.target != null;
		}
	}

	public override bool CanInstaComplete(TaskMoveInstalled t)
	{
		return EClass.player.instaComplete;
	}

	public override MeshPass GetGuidePass(Point point)
	{
		return EClass.screen.guide.passGuideBlock;
	}

	public void Activate(Thing t)
	{
		base.Activate(true, false);
		this.SetTarget(t);
		this.onetime = true;
		EClass.ui.hud.hint.UpdateText();
	}

	public override void OnActivate()
	{
		this.onetime = false;
		this.list = base.Designations.moveInstalled;
		this.target = (this.moldCard = null);
		base.OnActivate();
	}

	public override void OnDeactivate()
	{
		this.target = (this.moldCard = (this.mold.target = null));
	}

	public override void OnCreateMold(bool processing)
	{
		this.mold.target = this.target;
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (EClass.debug.enable)
		{
			int hotkey = EInput.hotkey;
			if (hotkey != 0)
			{
				if (hotkey == 1)
				{
					EClass._map.AddDecal(point.x, point.z, 2, 1, true);
				}
			}
			else
			{
				EClass._map.SetDecal(point.x, point.z, 0, 1, true);
			}
		}
		if (this.target == null)
		{
			if (EClass.scene.mouseTarget.CanCycle())
			{
				return HitResult.Warning;
			}
			if (this.GetTarget(point) != null)
			{
				return HitResult.Valid;
			}
			return HitResult.Default;
		}
		else
		{
			if (this.target.isChara && this.CheckEnemyNearBy(this.target, point, false))
			{
				return HitResult.Invalid;
			}
			this.moldCard.ignoreStackHeight = Input.GetKey(KeyCode.LeftControl);
			if (!EClass.debug.ignoreBuildRule)
			{
				if (this.target.isChara && (point.IsBlocked || point.HasChara))
				{
					return HitResult.Invalid;
				}
				if (!EClass._map.bounds.Contains(point))
				{
					return HitResult.Invalid;
				}
			}
			return base.HitTest(point, start);
		}
	}

	public void SetTarget(Card _target)
	{
		foreach (TaskMoveInstalled taskMoveInstalled in EClass._map.tasks.designations.moveInstalled.items.ToArray())
		{
			if (taskMoveInstalled.target == _target)
			{
				taskMoveInstalled.Destroy();
			}
		}
		this.mold.target = _target;
		this.target = _target;
		if (this.target.isThing)
		{
			this.moldCard = this.target.Duplicate(1);
			this.moldCard.placeState = PlaceState.installed;
		}
		else
		{
			this.moldCard = CharaGen.Create(_target.id, -1);
			this.moldCard.bio.SetGender(_target.bio.gender);
			this.moldCard.idSkin = this.target.idSkin;
		}
		EClass.ui.hud.hint.UpdateText();
	}

	public Card GetTarget(Point point)
	{
		return EClass.scene.mouseTarget.card;
	}

	public bool CanPutAway()
	{
		return EClass.debug.ignoreBuildRule || (this.target != null && !this.target.isChara && !this.target.trait.CanOnlyCarry);
	}

	public bool TryPutAway()
	{
		if (!this.CanPutAway())
		{
			return false;
		}
		if (!EClass._map.PutAway(this.target))
		{
			SE.Beep();
			return false;
		}
		SE.Click();
		this.target = null;
		EClass.ui.hud.hint.UpdateText();
		return true;
	}

	public bool CheckEnemyNearBy(Card t, Point p, bool msg = true)
	{
		if (t.isChara && !EClass.debug.ignoreBuildRule)
		{
			bool flag = true;
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.hostility <= Hostility.Enemy && chara.Dist(p) <= 6)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				if (msg)
				{
					SE.BeepSmall();
					EClass.ui.Say("enemyInMap".langGame(), null);
				}
				return true;
			}
		}
		return false;
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		if (this.target == null)
		{
			Card t = this.GetTarget(point);
			if (this.CheckEnemyNearBy(t, t.pos, true))
			{
				return;
			}
			if (t.trait.ShowContextOnPick)
			{
				ActPlan actPlan = new ActPlan
				{
					pos = point.Copy(),
					ignoreAdddCondition = true,
					input = ActInput.AllAction,
					altAction = true
				};
				t.trait.TrySetAct(actPlan);
				if (actPlan.list.Count > 0)
				{
					UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
					uicontextMenu.AddButton("move", delegate()
					{
						this.SetTarget(t);
						SE.Click();
					}, true);
					using (List<ActPlan.Item>.Enumerator enumerator = actPlan.list.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ActPlan.Item i = enumerator.Current;
							uicontextMenu.AddButton(i.GetTextContext(false), delegate()
							{
								i.act.Perform();
							}, true);
						}
					}
					uicontextMenu.Show();
					return;
				}
			}
			this.SetTarget(t);
			SE.Click();
			return;
		}
		else
		{
			if (this.CheckEnemyNearBy(this.target, point, true))
			{
				return;
			}
			this.mold.dir = this.moldCard.dir;
			this.mold.altitude = this.moldCard.altitude;
			if (this.target.isChara)
			{
				Point orgPos = this.target.Chara.orgPos;
				if (orgPos != null)
				{
					orgPos.Set(point);
				}
			}
			base.OnProcessTiles(point, dir);
			this.target.ignoreStackHeight = this.moldCard.ignoreStackHeight;
			this.target.freePos = this.moldCard.freePos;
			this.target.fx = (this.FreePos ? this.moldCard.fx : 0f);
			this.target.fy = (this.FreePos ? this.moldCard.fy : 0f);
			if (this.target.isChara && EClass.debug)
			{
				foreach (Thing thing in point.Things)
				{
					TraitShackle traitShackle = thing.trait as TraitShackle;
					if (traitShackle != null)
					{
						traitShackle.Restrain(this.target.Chara, false);
					}
				}
			}
			if (this.target.isThing)
			{
				this.target.isRoofItem = this.IsRoofEditMode(null);
				if (this.target.isRoofItem)
				{
					this.target.SetPlaceState(PlaceState.roaming, false);
				}
			}
			SE.Click();
			if (this.target.renderer.hasActor)
			{
				this.target.renderer.RefreshSprite();
			}
			this.target = null;
			return;
		}
	}

	public unsafe override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (this.target == null)
		{
			Card card = this.GetTarget(point);
			if (card != null && card.isThing)
			{
				card.Thing.RenderMarker(point, true, result, true, -1, true);
			}
			base.OnRenderTile(point, (result == HitResult.Valid) ? (EClass.scene.mouseTarget.CanCycle() ? HitResult.Warning : HitResult.Default) : result, dir);
			return;
		}
		if (this.moldCard == null)
		{
			base.OnRenderTile(point, result, dir);
			return;
		}
		this.moldCard.SetFreePos(point);
		if (result != HitResult.Valid && result != HitResult.Warning)
		{
			base.OnRenderTile(point, result, dir);
		}
		else
		{
			int desiredDir = this.target.TileType.GetDesiredDir(point, this.moldCard.dir);
			if (desiredDir != -1)
			{
				dir = (this.moldCard.dir = desiredDir);
			}
			bool flag = !base.tileSelector.multisize || (base.tileSelector.firstInMulti && base.Summary.count == base.Summary.countValid);
			this.moldCard.RenderMarker(point, false, result, flag, -1, false);
			if (flag)
			{
				this.target.trait.OnRenderTile(point, result, dir);
			}
		}
		EClass.screen.guide.DrawLine(*this.target.pos.PositionCenter(), *point.PositionCenter());
	}

	public override void RotateUnderMouse()
	{
		if (this.target != null)
		{
			SE.Rotate();
			this.moldCard.Rotate(false);
			return;
		}
		base.RotateUnderMouse();
	}

	public override void InputWheel(int wheel)
	{
		if (EInput.isAltDown || EInput.isCtrlDown)
		{
			return;
		}
		if (this.target != null && this.target.TileType.MaxAltitude > 0)
		{
			this.moldCard.ChangeAltitude(wheel);
			return;
		}
		if (EClass.scene.mouseTarget.CanCycle())
		{
			EClass.scene.mouseTarget.CycleTarget(wheel);
			return;
		}
		base.InputWheel(wheel);
	}

	public override void OnCancel()
	{
		if (this.target != null)
		{
			this.target = null;
			if (this.onetime)
			{
				base.OnCancel();
			}
			return;
		}
		base.OnCancel();
	}

	public override void OnFinishProcessTiles()
	{
		if (this.onetime)
		{
			base.Deactivate();
		}
	}

	public Card target;

	public Card moldCard;

	public bool onetime;
}
