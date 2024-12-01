using System.Collections.Generic;

public class TraitDeedRelocate : TraitScroll
{
	public override bool CanBeDestroyed => false;

	public override void OnRead(Chara c)
	{
		if (!EClass._zone.IsRegion)
		{
			Msg.Say("globalmap_item");
			return;
		}
		EloMap.Cell cell = EClass.scene.elomap.GetCell(EClass.pc.pos);
		if (cell == null || (cell.zone != null && (!(cell.zone is Zone_Field) || cell.zone.IsPCFaction)))
		{
			Msg.Say("cannot_use_here");
			return;
		}
		List<FactionBranch> children = EClass.pc.faction.GetChildren();
		EClass.ui.AddLayer<LayerList>().SetNoSound().SetList2(children, (FactionBranch b) => b.owner.NameWithLevel, delegate(FactionBranch a, ItemGeneral s)
		{
			Zone z = a.owner;
			Dialog.YesNo("dialog_relocateLand", delegate
			{
				if (cell.zone != null)
				{
					cell.zone.Destroy();
				}
				EClass.game.Save();
				EClass.Sound.Play("jingle_embark");
				EClass.pc.PlaySound("build");
				owner.ModNum(-1);
				EClass.scene.elomap.SetZone(z.x, z.y, null);
				Point point = EClass.pc.pos.ToRegionPos();
				z.x = point.x;
				z.y = point.z;
				EClass.scene.elomap.SetZone(z.x, z.y, z, updateMesh: true);
				Msg.Say("base_relocate", z.Name);
			});
		}, null)
			.TryShowHint("h_relocate");
	}
}
