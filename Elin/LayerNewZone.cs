using System;
using System.Collections.Generic;

public class LayerNewZone : ELayer
{
	public override void OnInit()
	{
		this.buttonSelectMember.SetActive(ELayer._zone.IsPlayerFaction);
	}

	public override void OnKill()
	{
	}

	public void OnClickExit()
	{
		this.Close();
		ELayer.player.MoveZone(ELayer.pc.currentZone);
		ActionMode.EloMap.Activate(true, false);
	}

	public void OnClickSelectMembers()
	{
		this.settlers.Clear();
		ELayer.ui.AddLayer(LayerPeople.CreateSelectEmbarkMembers(this.settlers).SetOnConfirm(new Action(this.Embark)));
	}

	public void Embark()
	{
		ELayer.game.Save(false, null, false);
		this.Close();
		ActionMode.Sim.Activate(true, false);
		ELayer.screen.Focus(this.settlers[0]);
		Dialog d = Dialog.CreateNarration("embark", "embark");
		d.list.AddButton(null, Lang.Get("ok"), delegate
		{
			Chara chara = CharaGen.Create("chicken", -1);
			chara.SetFaction(ELayer.Home);
			ELayer._zone.AddCard(chara, this.settlers[0].pos);
			ELayer.Branch.Recruit(chara);
			d.Close();
		}, null);
		ELayer.ui.AddLayer(d);
		ELayer._zone.RefreshBGM();
		ELayer.Sound.PlayBGM("jingle_embark");
	}

	public UIMapSelector selector;

	public UIButton buttonSelectMember;

	public List<Chara> settlers = new List<Chara>();
}
