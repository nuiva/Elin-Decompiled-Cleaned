using System.Collections.Generic;

public class TraitElevator : TraitNewZone
{
	public override ZoneTransition.EnterState enterState => ZoneTransition.EnterState.Elevator;

	public override string langOnUse => "actUse";

	public override bool IsTeleport => true;

	public override bool OnlyInTheSameTopZone => true;

	public override bool CanBeHeld => true;

	public override void TrySetAct(ActPlan p)
	{
		base.TrySetAct(p);
		if (p.input != ActInput.AllAction || EClass._zone.isExternalZone || (!EClass.debug.enable && owner.isNPCProperty))
		{
			return;
		}
		List<Zone> list = new List<Zone>();
		Zone topZone = EClass._zone.GetTopZone();
		if (topZone != EClass._zone)
		{
			list.Add(topZone);
		}
		foreach (Spatial child in topZone.children)
		{
			if (child != EClass._zone && !child.isExternalZone)
			{
				list.Add(child as Zone);
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		p.TrySetAct("actSetElevatorLevel", delegate
		{
			EClass.ui.AddLayer<LayerList>().SetList(list, (Zone z) => z.NameWithLevel, delegate(int a, string s)
			{
				base.zone = list[a];
			});
			return false;
		}, owner);
	}
}
