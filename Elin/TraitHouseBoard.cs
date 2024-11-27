using System;
using Newtonsoft.Json;
using UnityEngine;

public class TraitHouseBoard : TraitBoard
{
	public TraitHouseBoard.Data data
	{
		get
		{
			return this.owner.GetObj<TraitHouseBoard.Data>(1);
		}
		set
		{
			this.owner.SetObj(1, value);
		}
	}

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

	public bool CanBeUsed
	{
		get
		{
			return this.owner.IsInstalled && this.owner.pos.cell.room != null && this.data != null;
		}
	}

	public void ApplyData()
	{
		if (this.owner.IsInstalled && this.owner.pos.cell.room != null)
		{
			this.owner.pos.cell.room.lot.RefreshData(this.data);
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass.debug.enable && !EClass._zone.IsPCFaction)
		{
			return;
		}
		Room room = this.owner.pos.cell.room;
		if (room == null)
		{
			return;
		}
		if (room.lot.board == this)
		{
			p.TrySetAct("actLotBGM", delegate()
			{
				EClass.ui.AddLayer<LayerEditPlaylist>().Activate(this);
				return false;
			}, this.owner, null, 1, false, true, false);
			p.TrySetAct("actChangeRoof", delegate()
			{
				EClass.ui.AddLayer<LayerEditHouse>().SetBoard(this);
				return false;
			}, this.owner, null, 1, false, true, false);
		}
	}

	public override void OnChangePlaceState(PlaceState state)
	{
		string[] array = new string[6];
		array[0] = "Lot OnChangePlaceState ";
		int num = 1;
		Card owner = this.owner;
		array[num] = ((owner != null) ? owner.ToString() : null);
		array[2] = ": ";
		array[3] = this.owner.placeState.ToString();
		array[4] = "/";
		int num2 = 5;
		Point pos = this.owner.pos;
		array[num2] = ((pos != null) ? pos.ToString() : null);
		Debug.Log(string.Concat(array));
		EClass._map.rooms.dirtyLots = true;
	}

	public class Data
	{
		[JsonProperty]
		public int idRoofStyle;

		[JsonProperty]
		public int height;

		[JsonProperty]
		public int idBGM;

		[JsonProperty]
		public int heightFix;

		[JsonProperty]
		public int idRoofTile;

		[JsonProperty]
		public int idBlock = 30;

		[JsonProperty]
		public int idRamp;

		[JsonProperty]
		public int idDeco;

		[JsonProperty]
		public int idDeco2;

		[JsonProperty]
		public int decoFix;

		[JsonProperty]
		public int decoFix2;

		[JsonProperty]
		public int colRoof;

		[JsonProperty]
		public int colBlock;

		[JsonProperty]
		public int colDeco;

		[JsonProperty]
		public int colDeco2;

		[JsonProperty]
		public bool reverse;

		[JsonProperty]
		public bool snow;

		[JsonProperty]
		public bool altRoof;
	}
}
