public class TraitDoor : Trait
{
	public int count;

	public override bool CanBeOnlyBuiltInHome => true;

	public override bool CanBuildInTown => false;

	public override bool CanBeDisassembled => !owner.IsInstalled;

	public override TileMode tileMode => TileMode.Door;

	public override bool HaveUpdate => true;

	public override bool IsOpenSight => IsOpen();

	public override bool IsDoor => true;

	public override bool ShouldRefreshTile => true;

	public virtual string idSound => "door1";

	public override void Update()
	{
		TryAutoClose();
	}

	public void ForceClose()
	{
		if (IsOpen())
		{
			ToggleDoor();
		}
	}

	public void TryOpen(Chara c)
	{
		if (IsOpen())
		{
			return;
		}
		ToggleDoor();
		c.Say("openDoor", c, owner);
		if (owner.Cell.Front.FirstThing != null)
		{
			foreach (Thing thing in owner.Cell.Front.Things)
			{
				thing.trait.OnOpenDoor(c);
			}
		}
		if (owner.Cell.Right.FirstThing == null)
		{
			return;
		}
		foreach (Thing thing2 in owner.Cell.Right.Things)
		{
			thing2.trait.OnOpenDoor(c);
		}
	}

	public void TryAutoClose()
	{
		count++;
		if (count > 5 && CanClose() && IsOpen())
		{
			ToggleDoor(sound: false);
		}
	}

	public virtual bool CanClose()
	{
		int num = 0;
		foreach (Thing thing in owner.pos.Things)
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
		return !owner.pos.HasChara;
	}

	public virtual bool IsOpen()
	{
		int dir = owner.dir;
		Cell cell = owner.pos.cell;
		if (!cell.Right.HasFullBlockOrWallOrFence && (dir == 0 || dir == 2) && cell.Front.HasFullBlockOrWallOrFence)
		{
			return true;
		}
		if (!cell.Front.HasFullBlockOrWallOrFence && (dir == 1 || dir == 3) && cell.Right.HasFullBlockOrWallOrFence)
		{
			return true;
		}
		return false;
	}

	public bool IsValid()
	{
		_ = owner.dir;
		Cell cell = owner.pos.cell;
		if (!cell.HasBlock)
		{
			return false;
		}
		bool hasFullBlockOrWallOrFence = cell.Left.HasFullBlockOrWallOrFence;
		bool hasFullBlockOrWallOrFence2 = cell.Right.HasFullBlockOrWallOrFence;
		bool hasFullBlockOrWallOrFence3 = cell.Front.HasFullBlockOrWallOrFence;
		bool hasFullBlockOrWallOrFence4 = cell.Back.HasFullBlockOrWallOrFence;
		if ((hasFullBlockOrWallOrFence ? 1 : 0) + (hasFullBlockOrWallOrFence2 ? 1 : 0) + (hasFullBlockOrWallOrFence3 ? 1 : 0) + (hasFullBlockOrWallOrFence4 ? 1 : 0) >= 3)
		{
			return false;
		}
		if (hasFullBlockOrWallOrFence && hasFullBlockOrWallOrFence2)
		{
			return true;
		}
		if (hasFullBlockOrWallOrFence3 && hasFullBlockOrWallOrFence4)
		{
			return true;
		}
		return false;
	}

	public virtual void ToggleDoor(bool sound = true, bool refresh = true)
	{
		if (sound)
		{
			owner.PlaySound(idSound);
		}
		RotateDoor();
		count = 0;
		if (refresh)
		{
			EClass._map.RefreshSingleTile(owner.pos.x, owner.pos.z);
			EClass._map.RefreshFOV(owner.pos.x, owner.pos.z);
		}
	}

	public void RotateDoor()
	{
		if (owner.dir == 0)
		{
			owner.dir = 1;
		}
		else if (owner.dir == 1)
		{
			owner.dir = 0;
		}
		else if (owner.dir == 2)
		{
			owner.dir = 3;
		}
		else
		{
			owner.dir = 2;
		}
		owner.renderer.RefreshSprite();
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!owner.IsInstalled)
		{
			return;
		}
		if (owner.c_lockLv > 0)
		{
			p.TrySetAct(new AI_OpenLock
			{
				target = owner.Thing
			}, owner);
		}
		else if (!IsOpen())
		{
			p.TrySetAct("actOpen", delegate
			{
				EClass.pc.Say("openDoor", EClass.pc, owner);
				ToggleDoor();
				return true;
			}, owner, CursorSystem.Door);
		}
		else if (CanClose() && p.altAction)
		{
			p.TrySetAct("actClose", delegate
			{
				EClass.pc.Say("close", EClass.pc, owner);
				ToggleDoor();
				return true;
			}, owner, CursorSystem.Door);
		}
	}
}
