using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LayerEditPlaylist : ELayer
{
	public Dictionary<int, BGMData> bgms
	{
		get
		{
			return ELayer.core.refs.dictBGM;
		}
	}

	public override void OnInit()
	{
	}

	public void Activate(TraitJukeBox _box)
	{
		this.jukebox = _box;
		this.single = true;
		this.Activate(LayerEditPlaylist.Mode.LotBGM);
	}

	public void Activate(TraitHouseBoard _board)
	{
		this.board = _board;
		this.single = true;
		this.Activate(LayerEditPlaylist.Mode.LotBGM);
	}

	public void Activate(LayerEditPlaylist.Mode _mode = LayerEditPlaylist.Mode.Playlist)
	{
		this.mode = _mode;
		Msg.Nerun("nPlaylist", "UN_nerun_smile3");
		if (this.mode == LayerEditPlaylist.Mode.Playlist)
		{
			if (this.dayNight)
			{
				this.windows[0].AddBottomButton("togglePlaylist", new UnityAction(this.TogglePL), false);
			}
			foreach (int key in ELayer._map._plDay)
			{
				if (this.bgms.ContainsKey(key))
				{
					this.itemsDay.Add(this.bgms[key]);
				}
			}
			using (List<int>.Enumerator enumerator = ELayer._map._plNight.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int key2 = enumerator.Current;
					if (this.bgms.ContainsKey(key2))
					{
						this.itemsNight.Add(this.bgms[key2]);
					}
				}
				goto IL_12C;
			}
		}
		this.windows[1].SetActive(false);
		this.windows[0].SetPosition();
		this.multi.Double = false;
		IL_12C:
		this.Refresh();
		foreach (ListOwner listOwner in this.multi.owners)
		{
			listOwner.OnSwitchContent();
		}
	}

	public void Refresh()
	{
		List<BGMData> list = (this.mode == LayerEditPlaylist.Mode.LotBGM) ? new List<BGMData>() : (this.day ? this.itemsDay : this.itemsNight);
		List<BGMData> list2 = new List<BGMData>();
		foreach (BGMData bgmdata in this.bgms.Values)
		{
			if (ELayer.debug.allBGM || ELayer.player.knownBGMs.Contains(bgmdata.id))
			{
				if (this.mode == LayerEditPlaylist.Mode.LotBGM)
				{
					list.Add(bgmdata);
				}
				else
				{
					bool flag = bgmdata.id != 0;
					using (List<BGMData>.Enumerator enumerator2 = list.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.id == bgmdata.id)
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						list2.Add(bgmdata);
					}
				}
			}
		}
		this.multi.Clear();
		if (this.mode == LayerEditPlaylist.Mode.LotBGM)
		{
			this.multi.AddOwner(0, new LayerEditPlaylist.ListBGM
			{
				items = list,
				idTitle = "wBGMLot",
				single = true
			});
		}
		else
		{
			this.multi.AddOwner(0, new LayerEditPlaylist.ListBGM
			{
				items = list,
				idTitle = "wPlaylist"
			});
			this.multi.AddOwner(1, new LayerEditPlaylist.ListBGM
			{
				items = list2,
				idTitle = "wPlaylist2"
			});
		}
		this.multi.Build(UIList.SortMode.ByNone);
	}

	public void TogglePL()
	{
		this.day = !this.day;
		this.Refresh();
		this.multi.Refresh();
		SE.Click();
	}

	public override void OnKill()
	{
		if (this.mode == LayerEditPlaylist.Mode.LotBGM)
		{
			ELayer.Sound.StopBGM(0f, false);
			ELayer._zone.RefreshBGM();
			return;
		}
		ELayer._map._plDay.Clear();
		ELayer._map._plNight.Clear();
		foreach (BGMData bgmdata in this.itemsDay)
		{
			ELayer._map._plDay.Add(bgmdata.id);
		}
		foreach (BGMData bgmdata2 in this.itemsNight)
		{
			ELayer._map._plNight.Add(bgmdata2.id);
		}
		UnityEngine.Object.DestroyImmediate(ELayer._map.plDay);
		ELayer._map.plDay = null;
		ELayer._zone.RefreshPlaylist();
		if (!this.keepPlaying)
		{
			ELayer.Sound.StopBGM(0f, false);
			ELayer._zone.RefreshBGM();
		}
	}

	public override void OnSwitchContent(Window window)
	{
		if (this.multi.owners.Count > 0)
		{
			this.multi.owners[window.windowIndex].OnSwitchContent();
		}
	}

	public List<BGMData> itemsDay = new List<BGMData>();

	public List<BGMData> itemsNight = new List<BGMData>();

	public UIMultiList multi;

	public bool day;

	public bool keepPlaying;

	public bool single;

	public bool dayNight;

	public LayerEditPlaylist.Mode mode;

	public TraitHouseBoard board;

	public TraitJukeBox jukebox;

	public enum Mode
	{
		Playlist,
		LotBGM
	}

	public class ListBGM : ListOwner<BGMData, ItemGeneral>
	{
		public new LayerEditPlaylist layer
		{
			get
			{
				return this.layer as LayerEditPlaylist;
			}
		}

		public override string IdTitle
		{
			get
			{
				return this.idTitle;
			}
		}

		public override void List()
		{
			this.list.callbacks = new UIList.Callback<BGMData, ItemGeneral>
			{
				onInstantiate = delegate(BGMData a, ItemGeneral b)
				{
					b.SetMainText(((a.id > 1) ? "♪ " : "") + a._name.IsEmpty(a.name).ToTitleCase(false), null, true);
					b.button1.keyText.text = (a.id.ToString() ?? "");
					if (a.id != 0 && this.layer.jukebox == null)
					{
						b.AddSubButton(EClass.core.refs.icons.resume, delegate
						{
							EClass.Sound.PlayBGM(a, 0f, 0f);
						}, null, null);
					}
					if (this.main && this.layer.mode == LayerEditPlaylist.Mode.Playlist)
					{
						b.AddSubButton(EClass.core.refs.icons.down, delegate
						{
							BGMData select = this.items.Move(a, 1);
							this.list.OnMove(a, select);
						}, null, null);
						b.AddSubButton(EClass.core.refs.icons.up, delegate
						{
							BGMData select = this.items.Move(a, -1);
							this.list.OnMove(a, select);
						}, null, null);
					}
					b.Build();
					if (!this.single)
					{
						b.button1.soundClick = null;
					}
				},
				onClick = delegate(BGMData c, ItemGeneral i)
				{
					if (this.layer.mode == LayerEditPlaylist.Mode.Playlist)
					{
						this.items.Remove(c);
						(this.other as LayerEditPlaylist.ListBGM).items.Add(c);
						base.MoveToOther(c);
						return;
					}
					if (this.layer.board != null)
					{
						this.layer.board.data.idBGM = ((this.layer.board.data.idBGM == c.id) ? 0 : c.id);
						this.layer.board.ApplyData();
						this.layer.Close();
						return;
					}
					this.layer.jukebox.OnSetBGM(c);
					this.layer.Close();
				},
				onList = delegate(UIList.SortMode m)
				{
					foreach (BGMData o in this.items)
					{
						this.list.Add(o);
					}
				}
			};
			this.list.List(false);
		}

		public List<BGMData> items;

		public string idTitle;

		public bool single;
	}
}
