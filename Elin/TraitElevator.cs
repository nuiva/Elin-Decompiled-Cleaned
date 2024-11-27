using System;
using System.Collections.Generic;

public class TraitElevator : TraitNewZone
{
	public override ZoneTransition.EnterState enterState
	{
		get
		{
			return ZoneTransition.EnterState.Elevator;
		}
	}

	public override string langOnUse
	{
		get
		{
			return "actUse";
		}
	}

	public override bool IsTeleport
	{
		get
		{
			return true;
		}
	}

	public override bool OnlyInTheSameTopZone
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

	public override void TrySetAct(ActPlan p)
	{
		base.TrySetAct(p);
		if (p.input == ActInput.AllAction && !EClass._zone.isExternalZone && (EClass.debug.enable || !this.owner.isNPCProperty))
		{
			List<Zone> list = new List<Zone>();
			Zone topZone = EClass._zone.GetTopZone();
			if (topZone != EClass._zone)
			{
				list.Add(topZone);
			}
			foreach (Spatial spatial in topZone.children)
			{
				if (spatial != EClass._zone && !spatial.isExternalZone)
				{
					list.Add(spatial as Zone);
				}
			}
			if (list.Count > 0)
			{
				Action<int, string> <>9__2;
				p.TrySetAct("actSetElevatorLevel", delegate()
				{
					LayerList layerList = EClass.ui.AddLayer<LayerList>();
					ICollection<Zone> list = list;
					Func<Zone, string> getString = (Zone z) => z.NameWithLevel;
					Action<int, string> onSelect;
					if ((onSelect = <>9__2) == null)
					{
						onSelect = (<>9__2 = delegate(int a, string s)
						{
							this.zone = list[a];
						});
					}
					layerList.SetList<Zone>(list, getString, onSelect, true);
					return false;
				}, this.owner, null, 1, false, true, false);
			}
		}
	}
}
