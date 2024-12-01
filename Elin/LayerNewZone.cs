using System.Collections.Generic;

public class LayerNewZone : ELayer
{
	public UIMapSelector selector;

	public UIButton buttonSelectMember;

	public List<Chara> settlers = new List<Chara>();

	public override void OnInit()
	{
		buttonSelectMember.SetActive(ELayer._zone.IsPlayerFaction);
	}

	public override void OnKill()
	{
	}

	public void OnClickExit()
	{
		Close();
		ELayer.player.MoveZone(ELayer.pc.currentZone);
		ActionMode.EloMap.Activate();
	}

	public void OnClickSelectMembers()
	{
		settlers.Clear();
		ELayer.ui.AddLayer(LayerPeople.CreateSelectEmbarkMembers(settlers).SetOnConfirm(Embark));
	}

	public void Embark()
	{
		ELayer.game.Save();
		Close();
		ActionMode.Sim.Activate();
		ELayer.screen.Focus(settlers[0]);
		Dialog d = Dialog.CreateNarration("embark", "embark");
		d.list.AddButton(null, Lang.Get("ok"), delegate
		{
			Chara chara = CharaGen.Create("chicken");
			chara.SetFaction(ELayer.Home);
			ELayer._zone.AddCard(chara, settlers[0].pos);
			ELayer.Branch.Recruit(chara);
			d.Close();
		});
		ELayer.ui.AddLayer(d);
		ELayer._zone.RefreshBGM();
		ELayer.Sound.PlayBGM("jingle_embark");
	}
}
