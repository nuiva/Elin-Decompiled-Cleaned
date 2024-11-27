using System;
using System.Collections.Generic;
using UnityEngine;

public class PointTarget : EClass
{
	public Chara TargetChara
	{
		get
		{
			if (this.card != null)
			{
				return this.card.Chara;
			}
			return null;
		}
	}

	public void Update(Point _pos)
	{
		this.pos.Set(_pos);
		this.isValid = this.pos.IsValid;
		this.cards.Clear();
		if (!this.isValid || this.pos.IsHidden || EClass.ui.BlockMouseOverUpdate || (this.mouse && (EClass.ui.isPointerOverUI || !EClass.scene.actionMode.ShowMouseoverTarget)))
		{
			this.Clear();
			return;
		}
		this.card = null;
		this.area = null;
		this.task = null;
		this.block = (BlockInfo._CanInspect(this.pos) ? BlockInfo.GetTemp(this.pos) : null);
		if (this.pos.sourceBlock.tileType.Invisible && !EClass.scene.actionMode.IsBuildMode)
		{
			this.block = null;
		}
		this.obj = (ObjInfo._CanInspect(this.pos) ? ObjInfo.GetTemp(this.pos) : null);
		if (this.obj != null)
		{
			this.target = this.obj;
		}
		else if (this.block != null)
		{
			this.target = this.block;
		}
		else
		{
			this.target = null;
		}
		this.drawHighlight = (this.target != null);
		CellDetail detail = this.pos.detail;
		if (detail != null)
		{
			this.area = detail.area;
			this.task = detail.designation;
			Thing thing = null;
			Chara chara = null;
			foreach (Chara chara2 in detail.charas)
			{
				if (!this.ShouldIgnore(chara2))
				{
					this.cards.Add(chara2);
					if (chara == null || chara2.hostility < chara.hostility)
					{
						chara = chara2;
					}
				}
			}
			foreach (Thing thing2 in detail.things)
			{
				if (!this.ShouldIgnore(thing2))
				{
					this.cards.Add(thing2);
					thing = thing2;
				}
			}
			if (chara != null)
			{
				this.target = (this.card = chara);
				this.drawHighlight = true;
			}
			else if (thing != null)
			{
				this.target = (this.card = thing);
				this.drawHighlight = true;
			}
			else if (this.task != null)
			{
				this.target = this.task;
				this.drawHighlight = true;
			}
			else if (this.area != null && EClass.scene.actionMode.AreaHihlight != AreaHighlightMode.None)
			{
				this.target = this.area;
			}
			if (this.cards.Count > 0 && EClass.scene.actionMode.IsBuildMode)
			{
				this.target = (this.card = this.cards[Mathf.Abs(this.index) % this.cards.Count]);
				this.drawHighlight = true;
			}
		}
		if ((this.target == null || this.target is Chara) && EClass._zone.IsRegion)
		{
			Zone zone = EClass.scene.elomap.GetZone(this.pos);
			if (zone != null)
			{
				this.target = zone;
			}
		}
		this.hasInteraction = (this.target != null);
		this.CheckLastTarget();
	}

	public bool ShouldIgnore(Card c)
	{
		if (c.isChara)
		{
			if (c.Chara.host != null)
			{
				return true;
			}
			if (EClass.scene.actionMode.IsBuildMode)
			{
				if (!EClass.debug.ignoreBuildRule && (c.IsPC || !c.IsPCFaction))
				{
					return true;
				}
			}
			else
			{
				if ((!EClass.pc.hasTelepathy || !c.Chara.race.visibleWithTelepathy) && c.isHidden && !EClass.pc.canSeeInvisible)
				{
					return true;
				}
				if (c.IsPC)
				{
					return true;
				}
				if (!c.isSynced || !EClass.player.CanSee(c.Chara))
				{
					return true;
				}
			}
			return false;
		}
		else
		{
			if (c.trait.IsGround && !this.pos.cell.IsFloorWater && c.Cell.IsFloorWater)
			{
				return true;
			}
			if (EClass.scene.actionMode.IsBuildMode)
			{
				if (EClass.scene.actionMode.IsRoofEditMode(null))
				{
					if (!c.isRoofItem)
					{
						return true;
					}
				}
				else if (c.isRoofItem)
				{
					return true;
				}
			}
			else if (c.isHidden || c.isRoofItem || c.isMasked)
			{
				return true;
			}
			return false;
		}
	}

	public void CycleTarget(int a)
	{
		SE.Rotate();
		this.index += a;
		this.Update(Scene.HitPoint);
		if (WidgetMouseover.Instance)
		{
			WidgetMouseover.Instance.Refresh();
		}
	}

	public bool CanCycle()
	{
		return this.cards.Count >= 2;
	}

	public void Clear()
	{
		this.card = null;
		this.area = null;
		this.target = null;
		this.hasInteraction = false;
		this.CheckLastTarget();
	}

	public void CheckLastTarget()
	{
		this.hasTargetChanged = (this.target != this.lastTarget || !this.pos.Equals(this.lastPos));
		this.hasValidTarget = (this.target != null);
		this.lastTarget = this.target;
		this.lastPos.Set(this.pos);
	}

	public Card card;

	public Area area;

	public ObjInfo obj;

	public BlockInfo block;

	public TaskPoint task;

	public IInspect target;

	public IInspect lastTarget;

	public bool hasTargetChanged;

	public bool hasValidTarget;

	public bool drawHighlight;

	public bool mouse = true;

	public bool hasInteraction;

	public bool isValid;

	public Point pos = new Point();

	public Point lastPos = new Point();

	public int index;

	public List<Card> cards = new List<Card>();
}
