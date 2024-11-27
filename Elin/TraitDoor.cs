using System;

public class TraitDoor : Trait
{
	public override bool CanBeOnlyBuiltInHome
	{
		get
		{
			return true;
		}
	}

	public override bool CanBuildInTown
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeDisassembled
	{
		get
		{
			return !this.owner.IsInstalled;
		}
	}

	public override Trait.TileMode tileMode
	{
		get
		{
			return Trait.TileMode.Door;
		}
	}

	public override bool HaveUpdate
	{
		get
		{
			return true;
		}
	}

	public override bool IsOpenSight
	{
		get
		{
			return this.IsOpen();
		}
	}

	public override bool IsDoor
	{
		get
		{
			return true;
		}
	}

	public override bool ShouldRefreshTile
	{
		get
		{
			return true;
		}
	}

	public virtual string idSound
	{
		get
		{
			return "door1";
		}
	}

	public override void Update()
	{
		this.TryAutoClose();
	}

	public void ForceClose()
	{
		if (this.IsOpen())
		{
			this.ToggleDoor(true, true);
		}
	}

	public void TryOpen(Chara c)
	{
		if (this.IsOpen())
		{
			return;
		}
		this.ToggleDoor(true, true);
		c.Say("openDoor", c, this.owner, null, null);
		if (this.owner.Cell.Front.FirstThing != null)
		{
			foreach (Thing thing in this.owner.Cell.Front.Things)
			{
				thing.trait.OnOpenDoor(c);
			}
		}
		if (this.owner.Cell.Right.FirstThing != null)
		{
			foreach (Thing thing2 in this.owner.Cell.Right.Things)
			{
				thing2.trait.OnOpenDoor(c);
			}
		}
	}

	public void TryAutoClose()
	{
		this.count++;
		if (this.count > 5 && this.CanClose() && this.IsOpen())
		{
			this.ToggleDoor(false, true);
		}
	}

	public virtual bool CanClose()
	{
		int num = 0;
		foreach (Thing thing in this.owner.pos.Things)
		{
			if (!thing.isRoofItem && !thing.isHidden && thing.TileType != TileType.Illumination)
			{
				num++;
				if (num > 1)
				{
					return false;
				}
			}
		}
		return !this.owner.pos.HasChara;
	}

	public virtual bool IsOpen()
	{
		int dir = this.owner.dir;
		Cell cell = this.owner.pos.cell;
		return (!cell.Right.HasFullBlockOrWallOrFence && (dir == 0 || dir == 2) && cell.Front.HasFullBlockOrWallOrFence) || (!cell.Front.HasFullBlockOrWallOrFence && (dir == 1 || dir == 3) && cell.Right.HasFullBlockOrWallOrFence);
	}

	public bool IsValid()
	{
		int dir = this.owner.dir;
		Cell cell = this.owner.pos.cell;
		if (!cell.HasBlock)
		{
			return false;
		}
		bool hasFullBlockOrWallOrFence = cell.Left.HasFullBlockOrWallOrFence;
		bool hasFullBlockOrWallOrFence2 = cell.Right.HasFullBlockOrWallOrFence;
		bool hasFullBlockOrWallOrFence3 = cell.Front.HasFullBlockOrWallOrFence;
		bool hasFullBlockOrWallOrFence4 = cell.Back.HasFullBlockOrWallOrFence;
		return (hasFullBlockOrWallOrFence ? 1 : 0) + (hasFullBlockOrWallOrFence2 ? 1 : 0) + (hasFullBlockOrWallOrFence3 ? 1 : 0) + (hasFullBlockOrWallOrFence4 ? 1 : 0) < 3 && ((hasFullBlockOrWallOrFence && hasFullBlockOrWallOrFence2) || (hasFullBlockOrWallOrFence3 && hasFullBlockOrWallOrFence4));
	}

	public virtual void ToggleDoor(bool sound = true, bool refresh = true)
	{
		if (sound)
		{
			this.owner.PlaySound(this.idSound, 1f, true);
		}
		this.RotateDoor();
		this.count = 0;
		if (refresh)
		{
			EClass._map.RefreshSingleTile(this.owner.pos.x, this.owner.pos.z);
			EClass._map.RefreshFOV(this.owner.pos.x, this.owner.pos.z, 6, false);
		}
	}

	public void RotateDoor()
	{
		if (this.owner.dir == 0)
		{
			this.owner.dir = 1;
		}
		else if (this.owner.dir == 1)
		{
			this.owner.dir = 0;
		}
		else if (this.owner.dir == 2)
		{
			this.owner.dir = 3;
		}
		else
		{
			this.owner.dir = 2;
		}
		this.owner.renderer.RefreshSprite();
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!this.owner.IsInstalled)
		{
			return;
		}
		if (this.owner.c_lockLv > 0)
		{
			p.TrySetAct(new AI_OpenLock
			{
				target = this.owner.Thing
			}, this.owner);
			return;
		}
		if (!this.IsOpen())
		{
			p.TrySetAct("actOpen", delegate()
			{
				EClass.pc.Say("openDoor", EClass.pc, this.owner, null, null);
				this.ToggleDoor(true, true);
				return true;
			}, this.owner, CursorSystem.Door, 1, false, true, false);
			return;
		}
		if (this.CanClose() && p.altAction)
		{
			p.TrySetAct("actClose", delegate()
			{
				EClass.pc.Say("close", EClass.pc, this.owner, null, null);
				this.ToggleDoor(true, true);
				return true;
			}, this.owner, CursorSystem.Door, 1, false, true, false);
		}
	}

	public int count;
}
