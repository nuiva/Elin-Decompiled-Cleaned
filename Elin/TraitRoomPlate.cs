using System;
using System.Collections.Generic;

public class TraitRoomPlate : TraitBoard
{
	public override bool IsHomeItem
	{
		get
		{
			return true;
		}
	}

	public override bool CanBeMasked
	{
		get
		{
			return true;
		}
	}

	public override bool ShouldTryRefreshRoom
	{
		get
		{
			return true;
		}
	}

	public override bool MaskOnBuild
	{
		get
		{
			return true;
		}
	}

	public override bool ShowContextOnPick
	{
		get
		{
			return true;
		}
	}

	public AreaData areaData
	{
		get
		{
			return this.owner.GetObj<AreaData>(3) ?? this.owner.SetObj<AreaData>(3, new AreaData());
		}
		set
		{
			this.owner.SetObj(3, value);
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass.debug.enable && !EClass._zone.IsPCFaction)
		{
			return;
		}
		if (!this.owner.IsInstalled || this.owner.pos.cell.room == null)
		{
			return;
		}
		using (List<BaseArea.Interaction>.Enumerator enumerator = this.owner.pos.cell.room.ListInteractions().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				BaseArea.Interaction a = enumerator.Current;
				p.TrySetAct(a.text, delegate()
				{
					a.action();
					EClass._map.rooms.RefreshAll();
					return false;
				}, this.owner, null, 1, false, true, false);
			}
		}
	}
}
