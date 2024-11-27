using System;

public class TraitTent : TraitNewZone
{
	public override bool CanBeDropped
	{
		get
		{
			return !(EClass._zone is Zone_Tent);
		}
	}

	public override bool CanBuildInTown
	{
		get
		{
			return true;
		}
	}

	public override bool CreateExternalZone
	{
		get
		{
			return true;
		}
	}

	public override bool CanBeHeld
	{
		get
		{
			return true;
		}
	}

	public override int UseDist
	{
		get
		{
			return 1;
		}
	}

	public override void OnChangePlaceState(PlaceState state)
	{
		if (base.zone == null)
		{
			return;
		}
		if (state == PlaceState.installed)
		{
			if (!EClass._zone.children.Contains(base.zone))
			{
				EClass._zone.AddChild(base.zone);
				return;
			}
		}
		else
		{
			EClass._zone.RemoveChild(base.zone);
			this.owner.ChangeWeight(this.owner.Thing.source.weight + base.zone.GetInt(1, null) * 150 / 100);
		}
	}
}
