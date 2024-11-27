using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class LayerPeople : ELayer
{
	public override bool HeaderIsListOf(int id)
	{
		return true;
	}

	public override void OnInit()
	{
		this.showMode = ELayer.player.pref.modePoeple;
		if (this.multi.owners.Count == 0)
		{
			this.multi.AddOwner(0, new ListPeople
			{
				textTab = "residents",
				memberType = FactionMemberType.Default
			});
			this.multi.AddOwner(0, new ListPeople
			{
				textTab = "livestock",
				memberType = FactionMemberType.Livestock
			});
			this.multi.AddOwner(0, new ListPeople
			{
				textTab = "guests",
				memberType = FactionMemberType.Guest
			});
			this.langHint = "h_residents";
		}
		this.multi.Build(ELayer.player.pref.sortPeople);
		this.multi.owners[0].menu = new WindowMenu(this.layoutMenu);
	}

	public LayerPeople SetOnConfirm(Action _onConfirm)
	{
		this.onConfirm = _onConfirm;
		return this;
	}

	public void Confirm()
	{
		if (this.onConfirm != null)
		{
			this.onConfirm();
		}
		this.Close();
	}

	public override void OnKill()
	{
		ELayer.player.pref.sortPeople = this.multi.owners[0].list.sortMode;
		ELayer.player.pref.modePoeple = this.showMode;
		ELayer.scene.screenElin.focusOption = null;
	}

	public override void OnSwitchContent(Window window)
	{
		if (this.multi.Double)
		{
			this.multi.owners[window.windowIndex].OnSwitchContent();
			return;
		}
		this.multi.owners[window.idTab].OnSwitchContent();
	}

	public static LayerPeople Create(LayerPeople.Mode mode)
	{
		string path = "LayerPeople";
		if (mode == LayerPeople.Mode.Double)
		{
			path = "LayerPeople/LayerPeopleDouble";
		}
		return Layer.Create(path) as LayerPeople;
	}

	public static LayerPeople Create<T>(string langHint = null, Chara owner = null) where T : BaseListPeople
	{
		LayerPeople layerPeople = LayerPeople.Create(LayerPeople.Mode.Select);
		T t = Activator.CreateInstance<T>();
		t.owner = owner;
		layerPeople.multi.AddOwner(0, t);
		layerPeople.langHint = langHint;
		return layerPeople;
	}

	public static LayerPeople CreateReserve()
	{
		LayerPeople layerPeople = LayerPeople.Create(LayerPeople.Mode.Hire);
		layerPeople.multi.AddOwner(0, new ListPeopleCallReserve
		{
			textHeader = "actCallReserve"
		});
		layerPeople.langHint = "h_reserve";
		ELayer.ui.AddLayer(layerPeople);
		return layerPeople;
	}

	public static LayerPeople CreateGraze(AreaType areaType)
	{
		return LayerPeople.Create(LayerPeople.Mode.Double);
	}

	public static LayerPeople CreateSelectEmbarkMembers(List<Chara> settlers)
	{
		LayerPeople layerPeople = LayerPeople.Create(LayerPeople.Mode.Double);
		List<Chara> list = new List<Chara>();
		foreach (Chara chara in ELayer.game.lastActiveZone.map.charas)
		{
			if (chara.IsHomeMember())
			{
				list.Add(chara);
			}
		}
		layerPeople.multi.AddOwner(0, new ListPeopleEmbark
		{
			textHeader = "candidates",
			charas = list
		});
		layerPeople.multi.AddOwner(1, new ListPeopleEmbark
		{
			textHeader = "listEmbark",
			charas = settlers
		});
		return layerPeople;
	}

	public static LayerPeople Create(BaseListPeople list)
	{
		LayerPeople layerPeople = LayerPeople.Create(LayerPeople.Mode.Select);
		layerPeople.multi.AddOwner(0, list);
		return layerPeople;
	}

	public static LayerPeople CreateSelect(string langHeader, string langHint, Action<UIList> onList, Action<Chara> onClick, Func<Chara, string> _onShowSubText = null)
	{
		LayerPeople layerPeople = LayerPeople.Create(LayerPeople.Mode.Select);
		layerPeople.multi.AddOwner(0, new ListPeopleSelect
		{
			textHeader = langHeader,
			onList = onList,
			onClick = onClick,
			onShowSubText = _onShowSubText
		});
		layerPeople.langHint = langHint;
		ELayer.ui.AddLayer(layerPeople);
		return layerPeople;
	}

	public static Chara slaveToBuy;

	public LayerPeople.ShowMode showMode;

	public LayoutGroup layoutMenu;

	public Action onConfirm;

	public UIMultiList multi;

	public enum Mode
	{
		Default,
		Select,
		Double,
		Hire
	}

	public enum ShowMode
	{
		Job,
		Race,
		Work
	}
}
