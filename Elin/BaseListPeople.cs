using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseListPeople : ListOwner<Chara, ItemGeneral>
{
	public new LayerPeople layer
	{
		get
		{
			return this.layer as LayerPeople;
		}
	}

	public override string IdHeaderRow
	{
		get
		{
			return "HeaderRowPeople";
		}
	}

	public virtual bool ShowCharaSheet
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShowShowMode
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShowGoto
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShowHome
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsDisabled(Chara c)
	{
		return !c.isDead && c.memberType != FactionMemberType.Guest && !c.IsInHomeZone();
	}

	public override void List()
	{
		BaseList list = this.list;
		UIList.Callback<Chara, ItemGeneral> callback = new UIList.Callback<Chara, ItemGeneral>();
		callback.onInstantiate = delegate(Chara a, ItemGeneral b)
		{
			b.SetChara(a);
			this.OnInstantiate(a, b);
			b.Build();
		};
		callback.onClick = delegate(Chara c, ItemGeneral i)
		{
			this.OnClick(c, i);
		};
		callback.onSort = delegate(Chara a, UIList.SortMode m)
		{
			a.SetSortVal(m, CurrencyType.Money);
			return -a.sortVal;
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			this.OnList();
		};
		callback.onRefresh = null;
		list.callbacks = callback;
		this.list.List(false);
	}

	public override void OnInstantiate(Chara a, ItemGeneral b)
	{
		BaseListPeople.<>c__DisplayClass16_0 CS$<>8__locals1 = new BaseListPeople.<>c__DisplayClass16_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.a = a;
		this.SetSubText(CS$<>8__locals1.a, b);
		if (this.ShowHome)
		{
			BaseArea roomWork = null;
			bool flag = false;
			foreach (Hobby hobby in CS$<>8__locals1.a.ListWorks(true))
			{
				AIWork ai = hobby.GetAI(CS$<>8__locals1.a);
				flag = ai.SetDestination();
				if (flag)
				{
					if (ai.destArea != null)
					{
						roomWork = ai.destArea;
						break;
					}
					if (ai.destThing != null)
					{
						roomWork = ai.destThing.pos.cell.room;
						break;
					}
					break;
				}
			}
			b.AddSubButton(EClass.core.refs.icons.work, delegate
			{
				if (roomWork == null)
				{
					SE.BeepSmall();
					return;
				}
				EClass.pc.SetAI(new AI_Goto(roomWork.GetRandomFreePos(), 1, false, false));
				CS$<>8__locals1.<>4__this.layer.Close();
			}, null, delegate(UITooltip t)
			{
				BaseListPeople.<>c__DisplayClass16_2 CS$<>8__locals3;
				CS$<>8__locals3.t = t;
				CS$<>8__locals3.t.note.Clear();
				BaseArea roomWork;
				CS$<>8__locals3.t.note.AddHeader("infoWork".lang((roomWork != null) ? roomWork.Name : "none".lang(), null, null, null, null), null);
				foreach (Hobby h in CS$<>8__locals1.a.ListWorks(true))
				{
					CS$<>8__locals1.<OnInstantiate>g__AddText|5(h, "work", ref CS$<>8__locals3);
				}
				foreach (Hobby h2 in CS$<>8__locals1.a.ListHobbies(true))
				{
					CS$<>8__locals1.<OnInstantiate>g__AddText|5(h2, "hobby", ref CS$<>8__locals3);
				}
				CS$<>8__locals3.t.note.Build();
				roomWork = roomWork;
			}).icon.SetAlpha(flag ? 1f : 0.4f);
			Room room = CS$<>8__locals1.a.FindRoom();
			TraitBed bed = CS$<>8__locals1.a.FindBed();
			if (CS$<>8__locals1.a.memberType == FactionMemberType.Default)
			{
				b.AddSubButton(EClass.core.refs.icons.home, delegate
				{
					if (room == null)
					{
						SE.BeepSmall();
						return;
					}
					EClass.pc.SetAI(new AI_Goto(room.GetRandomFreePos(), 1, false, false));
					CS$<>8__locals1.<>4__this.layer.Close();
				}, null, delegate(UITooltip t)
				{
					t.note.Clear();
					t.note.AddHeader("infoHome".lang((room != null) ? room.Name : "none".lang(), null, null, null, null), null);
					t.note.AddTopic("TopicLeft", "infoBed".lang(), (bed != null) ? bed.Name.ToTitleCase(false) : "none".lang());
					t.note.Build();
					if (room != null)
					{
						EClass.core.actionsNextFrame.Add(delegate
						{
						});
					}
				}).icon.SetAlpha((room != null) ? 1f : 0.4f);
			}
		}
		if (this.ShowCharaSheet && EClass.debug.showExtra)
		{
			b.AddSubButton(EClass.core.refs.icons.inspect, delegate
			{
				SE.Play("pop_paper");
				LayerChara layerChara = EClass.ui.AddLayerDontCloseOthers<LayerChara>();
				layerChara.windows[0].SetRect(EClass.core.refs.rects.center, false);
				layerChara.SetChara(CS$<>8__locals1.a);
			}, "charaInfo", null);
		}
		if (this.IsDisabled(CS$<>8__locals1.a) || CS$<>8__locals1.a.currentZone != EClass._zone)
		{
			b.gameObject.AddComponent<CanvasGroup>().alpha = 0.6f;
		}
	}

	public virtual void SetSubText(Chara a, ItemGeneral b)
	{
		switch (this.layer.showMode)
		{
		case LayerPeople.ShowMode.Job:
			b.SetSubText(a.job.GetName().ToTitleCase(true), 300, FontColor.Default, TextAnchor.MiddleLeft);
			return;
		case LayerPeople.ShowMode.Race:
			b.SetSubText(a.race.GetName().ToTitleCase(true), 300, FontColor.Default, TextAnchor.MiddleLeft);
			return;
		case LayerPeople.ShowMode.Work:
		{
			string lang = a.GetTextWork(true) + "," + a.GetTextHobby(true);
			b.SetSubText(lang, 300, FontColor.Default, TextAnchor.MiddleLeft);
			return;
		}
		default:
			return;
		}
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		if (!c.IsAliveInCurrentZone)
		{
			if (c.currentZone != EClass._zone)
			{
				Msg.Say("isIn", c, (c.currentZone == null) ? "???" : c.currentZone.name, null, null);
			}
			SE.BeepSmall();
			return;
		}
		UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction().SetHighlightTarget(i);
		if (c.IsGuest())
		{
			uicontextMenu.AddButton("findMember", delegate()
			{
				EClass.pc.SetAI(new AI_Goto(c, 1, false, false));
				this.layer.Close();
			}, true);
		}
		else if (c.IsHomeMember())
		{
			if (!c.IsPC)
			{
				uicontextMenu.AddButton("findMember", delegate()
				{
					EClass.pc.SetAI(new AI_Goto(c, 1, false, false));
					this.layer.Close();
				}, true);
			}
			Action<bool, string> <>9__3;
			uicontextMenu.AddButton("changeName", delegate()
			{
				string langDetail = "dialogChangeName";
				string text2 = c.c_altName.IsEmpty(c.NameSimple);
				Action<bool, string> onClose;
				if ((onClose = <>9__3) == null)
				{
					onClose = (<>9__3 = delegate(bool cancel, string text)
					{
						if (!cancel)
						{
							if (text == "*r")
							{
								text = NameGen.getRandomName();
							}
							c.c_altName = text;
							this.layer.OnSwitchContent(this.layer.windows[0]);
						}
					});
				}
				Dialog.InputName(langDetail, text2, onClose, Dialog.InputType.Default);
			}, true);
			if (c != EClass.pc)
			{
				if (c.sourceCard.idActor.IsEmpty() && c.host == null)
				{
					if (c.pccData == null)
					{
						uicontextMenu.AddButton("togglePCC", delegate()
						{
							bool isSynced = c.isSynced;
							c.pccData = PCCData.Create(c.IDPCCBodySet);
							c.pccData.Randomize(c.IDPCCBodySet, null, true);
							if (isSynced)
							{
								c.renderer.OnLeaveScreen();
							}
							EClass.scene.syncList.Remove(c.renderer);
							c._CreateRenderer();
							if (isSynced)
							{
								EClass.scene.syncList.Add(c.renderer);
								c.renderer.OnEnterScreen();
							}
							this.list.Refresh(false);
							SE.Click();
						}, true);
					}
					else
					{
						uicontextMenu.AddButton("editPCC", delegate()
						{
							EClass.ui.AddLayer<LayerEditPCC>().Activate(c, UIPCC.Mode.Full, null, null);
						}, true);
						uicontextMenu.AddButton("togglePCC", delegate()
						{
							bool isSynced = c.isSynced;
							if (isSynced)
							{
								c.renderer.OnLeaveScreen();
							}
							EClass.scene.syncList.Remove(c.renderer);
							c.pccData = null;
							c._CreateRenderer();
							if (isSynced)
							{
								EClass.scene.syncList.Add(c.renderer);
								c.renderer.OnEnterScreen();
							}
							this.list.Refresh(false);
						}, true);
					}
				}
				uicontextMenu.AddButton("makeMaid", delegate()
				{
					if (EClass.Branch.uidMaid == c.uid)
					{
						EClass.Branch.uidMaid = 0;
					}
					else
					{
						EClass.Branch.uidMaid = c.uid;
					}
					this.list.Refresh(false);
					SE.Click();
				}, true);
				int @int = c.GetInt(36, null);
				bool isLivestockTimerOn = this.memberType == FactionMemberType.Default && !EClass.world.date.IsExpired(@int);
				int remainingHours = EClass.world.date.GetRemainingHours(@int);
				if (!c.IsPCParty)
				{
					uicontextMenu.AddButton((c.memberType == FactionMemberType.Livestock) ? "daMakeResident" : (isLivestockTimerOn ? "daMakeLivestock2".lang(Date.GetText(remainingHours), null, null, null, null) : "daMakeLivestock"), delegate()
					{
						if (isLivestockTimerOn)
						{
							SE.Beep();
							return;
						}
						if (c.memberType == FactionMemberType.Livestock)
						{
							c.SetInt(36, EClass.world.date.GetRaw(0) + 14400);
						}
						EClass.Branch.ChangeMemberType(c, (c.memberType == FactionMemberType.Livestock) ? FactionMemberType.Default : FactionMemberType.Livestock);
						this.List();
						SE.Click();
					}, true);
					uicontextMenu.AddButton("addToReserve", delegate()
					{
						if (EClass.Home.listReserve.Count >= EClass.Home.maxReserve)
						{
							SE.Beep();
							Msg.Say("reserveLimit");
							return;
						}
						SE.MoveZone();
						EClass.Home.AddReserve(c);
						this.list.List(false);
					}, true);
				}
			}
		}
		uicontextMenu.Show();
	}

	public override void OnList()
	{
		if (this.memberType == FactionMemberType.Guest)
		{
			using (List<Chara>.Enumerator enumerator = EClass._map.charas.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Chara chara = enumerator.Current;
					if (chara.IsGuest())
					{
						this.list.Add(chara);
					}
				}
				return;
			}
		}
		foreach (Chara chara2 in EClass.Branch.members)
		{
			if (chara2.memberType == this.memberType && !chara2.isSummon)
			{
				this.list.Add(chara2);
			}
		}
	}

	public override void OnRefreshMenu()
	{
		WindowMenu menuLeft = this.window.menuLeft;
		menuLeft.Clear();
		if (!this.main)
		{
			return;
		}
		menuLeft.AddButton2Line("sort", () => this.list.sortMode.ToString(), delegate(UIButton b)
		{
			this.list.NextSort();
		}, null, "2line");
		if (this.ShowShowMode)
		{
			Lang.GetList("info_people");
			menuLeft.AddButton2Line("info", () => "show" + this.layer.showMode.ToString(), delegate(UIButton b)
			{
				this.layer.showMode = this.layer.showMode.NextEnum<LayerPeople.ShowMode>();
				this.List();
			}, null, "2line");
		}
	}

	public Chara owner;

	public FactionMemberType memberType;
}
