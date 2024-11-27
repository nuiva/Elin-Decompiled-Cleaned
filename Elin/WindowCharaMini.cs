using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WindowCharaMini : WindowController
{
	public void SetChara(Chara c)
	{
		this.chara = c;
		this.Refresh(0);
	}

	public override void OnSwitchContent(Window window)
	{
		if (this.chara == null)
		{
			return;
		}
		this.Refresh(window.idTab);
	}

	public void Refresh(int idTab)
	{
		WindowCharaMini.<>c__DisplayClass16_0 CS$<>8__locals1 = new WindowCharaMini.<>c__DisplayClass16_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.idTab = idTab;
		if (this.chara == null)
		{
			return;
		}
		this.contentList.DestroyChildren(false, true);
		this.note.Clear();
		this.note.SetActive(false);
		this.textName.SetText(this.chara.NameSimple);
		this.textInfo1.text = "_DV".lang(this.chara.DV.ToString() ?? "", this.chara.PV.ToString() ?? "", null, null, null);
		this.textInfo2.text = "_style".lang(Lang._weight(this.chara.body.GetWeight(true), true, 0) ?? "", this.chara.elements.GetOrCreateElement(this.chara.GetArmorSkill()).Name, ("style" + this.chara.body.GetAttackStyle().ToString()).lang(), null, null);
		if (CS$<>8__locals1.idTab == 0)
		{
			CS$<>8__locals1.<Refresh>g__Header|0("resistance", null);
			CS$<>8__locals1.<Refresh>g__ListResist|1();
			CS$<>8__locals1.list.callbacks = new UIList.Callback<Element, ButtonElement>
			{
				onInstantiate = delegate(Element a, ButtonElement b)
				{
					b.SetGrid(a, CS$<>8__locals1.<>4__this.chara);
				},
				onList = delegate(UIList.SortMode m)
				{
					IEnumerable<SourceElement.Row> rows = EClass.sources.elements.rows;
					Func<SourceElement.Row, bool> predicate;
					if ((predicate = CS$<>8__locals1.<>9__5) == null)
					{
						predicate = (CS$<>8__locals1.<>9__5 = ((SourceElement.Row a) => a.category == "resist" && ((!a.tag.Contains("hidden") && !a.tag.Contains("high")) || CS$<>8__locals1.<>4__this.chara.Evalue(a.id) != 0)));
					}
					foreach (SourceElement.Row row in rows.Where(predicate))
					{
						CS$<>8__locals1.list.Add(CS$<>8__locals1.<>4__this.chara.elements.GetOrCreateElement(row.id));
					}
				}
			};
			CS$<>8__locals1.list.List(false);
		}
		else if (CS$<>8__locals1.idTab == 1)
		{
			CS$<>8__locals1.<Refresh>g__Header|0("mutation", null);
			CS$<>8__locals1.<Refresh>g__ListFeat|2();
			BaseList list = CS$<>8__locals1.list;
			UIList.Callback<Feat, ButtonElement> callback = new UIList.Callback<Feat, ButtonElement>();
			callback.onClick = delegate(Feat a, ButtonElement b)
			{
				WidgetTracker.Toggle(a);
			};
			callback.onInstantiate = delegate(Feat a, ButtonElement b)
			{
				b.SetElement(a, CS$<>8__locals1.<>4__this.chara.elements, ButtonElement.Mode.FeatMini);
			};
			callback.onList = delegate(UIList.SortMode m)
			{
				foreach (Element o in CS$<>8__locals1.<>4__this.chara.elements.ListElements((Element a) => a.source.category == "mutation" && a.Value != 0, null))
				{
					CS$<>8__locals1.list.Add(o);
				}
			};
			list.callbacks = callback;
			CS$<>8__locals1.list.List(false);
			if (CS$<>8__locals1.list.items.Count == 0)
			{
				this._header.SetActive(false);
				CS$<>8__locals1.list.SetActive(false);
			}
			CS$<>8__locals1.<Refresh>g__Header|0("etherDisease", null);
			CS$<>8__locals1.<Refresh>g__ListFeat|2();
			BaseList list2 = CS$<>8__locals1.list;
			UIList.Callback<Feat, ButtonElement> callback2 = new UIList.Callback<Feat, ButtonElement>();
			callback2.onClick = delegate(Feat a, ButtonElement b)
			{
				WidgetTracker.Toggle(a);
			};
			callback2.onInstantiate = delegate(Feat a, ButtonElement b)
			{
				b.SetElement(a, CS$<>8__locals1.<>4__this.chara.elements, ButtonElement.Mode.FeatMini);
			};
			callback2.onList = delegate(UIList.SortMode m)
			{
				foreach (Element o in CS$<>8__locals1.<>4__this.chara.elements.ListElements((Element a) => a.source.category == "ether" && a.Value != 0, null))
				{
					CS$<>8__locals1.list.Add(o);
				}
			};
			list2.callbacks = callback2;
			CS$<>8__locals1.list.List(false);
			if (CS$<>8__locals1.list.items.Count == 0)
			{
				this._header.SetActive(false);
				CS$<>8__locals1.list.SetActive(false);
			}
			CS$<>8__locals1.<Refresh>g__Header|0("innateFeats", null);
			CS$<>8__locals1.<Refresh>g__ListFeat|2();
			BaseList list3 = CS$<>8__locals1.list;
			UIList.Callback<Feat, ButtonElement> callback3 = new UIList.Callback<Feat, ButtonElement>();
			callback3.onClick = delegate(Feat a, ButtonElement b)
			{
				WidgetTracker.Toggle(a);
			};
			callback3.onInstantiate = delegate(Feat a, ButtonElement b)
			{
				b.SetElement(a, CS$<>8__locals1.<>4__this.chara.elements, ButtonElement.Mode.FeatMini);
			};
			callback3.onList = delegate(UIList.SortMode m)
			{
				foreach (Element o in CS$<>8__locals1.<>4__this.chara.elements.ListElements((Element a) => a.source.category == "feat" && a.HasTag("innate") && a.Value != 0, null))
				{
					CS$<>8__locals1.list.Add(o);
				}
			};
			list3.callbacks = callback3;
			CS$<>8__locals1.list.List(false);
			CS$<>8__locals1.<Refresh>g__Header|0("feats", null);
			CS$<>8__locals1.<Refresh>g__ListFeat|2();
			BaseList list4 = CS$<>8__locals1.list;
			UIList.Callback<Feat, ButtonElement> callback4 = new UIList.Callback<Feat, ButtonElement>();
			callback4.onClick = delegate(Feat a, ButtonElement b)
			{
				WidgetTracker.Toggle(a);
			};
			callback4.onInstantiate = delegate(Feat a, ButtonElement b)
			{
				b.SetElement(a, CS$<>8__locals1.<>4__this.chara.elements, ButtonElement.Mode.FeatMini);
			};
			callback4.onList = delegate(UIList.SortMode m)
			{
				foreach (Element o in CS$<>8__locals1.<>4__this.chara.elements.ListElements((Element a) => a.source.category == "feat" && !a.HasTag("innate") && a.Value != 0, null))
				{
					CS$<>8__locals1.list.Add(o);
				}
			};
			callback4.onSort = ((Feat a, UIList.SortMode m) => a.GetSortVal(m));
			list4.callbacks = callback4;
			CS$<>8__locals1.list.ChangeSort(UIList.SortMode.ByID);
			CS$<>8__locals1.list.List(false);
		}
		else if (CS$<>8__locals1.idTab == 2)
		{
			if (this.chara.c_upgrades == null)
			{
				this.chara.c_upgrades = new CharaUpgrade();
			}
			CharaUpgrade upgrades = this.chara.c_upgrades;
			CharaGenes genes = this.chara.c_genes;
			int num = upgrades.spent;
			if (genes != null)
			{
				num += genes.GetTotalCost();
			}
			this.note.SetActive(true);
			this.note.AddText("feat_pet".lang(this.chara.feat.ToString() ?? "", num.ToString() ?? "", null, null, null), FontColor.DontChange);
			this.note.AddText("feat_inferior".lang(((genes == null) ? 0 : genes.items.Count).ToString() ?? "", this.chara.MaxGene.ToString() ?? "", ((genes == null) ? 0 : genes.inferior).ToString() ?? "", null, null), FontColor.DontChange);
			this.note.Space(8, 1);
			foreach (CharaUpgrade.Item item in upgrades.items)
			{
				if (!Element.List_MainAttributesMajor.Contains(item.idEle))
				{
					Element element = Element.Create(item.idEle, item.value);
					this.note.AddText("upgraded".lang(element.Name.ToTitleCase(false), item.value.ToString() ?? "", item.cost.ToString() ?? "", null, null), FontColor.DontChange);
				}
			}
			if (this.chara.c_genes != null)
			{
				using (List<DNA>.Enumerator enumerator2 = genes.items.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						DNA g = enumerator2.Current;
						UIItem uiitem = this.note.AddItem("ItemGene");
						uiitem.button1.mainText.SetText(g.GetText());
						uiitem.button1.SetTooltip(delegate(UITooltip t)
						{
							UINote uinote = t.note;
							uinote.Clear();
							uinote.AddHeader(g.GetText(), null);
							g.WriteNote(uinote);
							t.note.Build();
						}, true);
						Action <>9__28;
						uiitem.button2.SetOnClick(delegate
						{
							if (!g.CanRemove())
							{
								SE.Beep();
								return;
							}
							string langDetail = "dialog_removeGene";
							Action actionYes;
							if ((actionYes = <>9__28) == null)
							{
								actionYes = (<>9__28 = delegate()
								{
									genes.inferior--;
									SE.Trash();
									genes.Remove(CS$<>8__locals1.<>4__this.chara, g);
									CS$<>8__locals1.<>4__this.Refresh(CS$<>8__locals1.idTab);
								});
							}
							Dialog.YesNo(langDetail, actionYes, null, "yes", "no");
						});
						uiitem.button2.SetActive(genes != null && genes.inferior > 0);
					}
				}
			}
			if (EClass.debug.enable)
			{
				this.note.AddButton("add 20".lang(), delegate
				{
					CS$<>8__locals1.<>4__this.chara.feat += 20;
					CS$<>8__locals1.<>4__this.Refresh(CS$<>8__locals1.idTab);
				});
				this.note.AddButton("grow 100".lang(), delegate
				{
					CS$<>8__locals1.<>4__this.chara.feat += 100;
					CS$<>8__locals1.<>4__this.chara.TryUpgrade(true);
					CS$<>8__locals1.<>4__this.Refresh(CS$<>8__locals1.idTab);
				});
			}
			if (upgrades.count > 0)
			{
				this.note.AddButton("feat_reset".lang(), delegate
				{
					upgrades.Reset(CS$<>8__locals1.<>4__this.chara);
					SE.Trash();
					CS$<>8__locals1.<>4__this.Refresh(CS$<>8__locals1.idTab);
				});
			}
			this.note.Build();
		}
		else if (CS$<>8__locals1.idTab == 3)
		{
			this.note.SetActive(true);
			WindowChara.RefreshNote(this.chara, this.note, true);
		}
		this.RefreshEquipment();
	}

	public void RefreshEquipment()
	{
		UIList uilist = this.listEq;
		uilist.Clear();
		BaseList baseList = uilist;
		UIList.Callback<BodySlot, UIItem> callback = new UIList.Callback<BodySlot, UIItem>();
		callback.onClick = delegate(BodySlot a, UIItem b)
		{
		};
		callback.onInstantiate = delegate(BodySlot a, UIItem b)
		{
			if (a.elementId == 0)
			{
				b.button1.interactable = false;
				return;
			}
			Thing thing = a.thing;
			b.refObj = thing;
			if (thing != null)
			{
				b.button1.icon.material = this.matItem;
				thing.SetImage(b.button1.icon);
			}
			else
			{
				b.button1.icon.material = null;
				b.button1.icon.sprite = SpriteSheet.Get("Media/Graphics/Icon/Element/", "eq_" + a.element.alias);
				b.button1.icon.color = this.colorUnequipped;
				b.button1.icon.SetNativeSize();
			}
			b.button1.SetTooltip(delegate(UITooltip tt)
			{
				if (a.thing != null)
				{
					a.thing.WriteNote(tt.note, null, IInspect.NoteMode.Default, null);
				}
				else
				{
					tt.note.Clear();
					tt.note.AddHeader(a.name, null);
					tt.note.AddText("noEQ".lang(), FontColor.DontChange);
					if (a.elementId == 35)
					{
						Thing.AddAttackEvaluation(tt.note, this.chara, null);
					}
				}
				tt.note.Build();
			}, true);
		};
		callback.onSort = ((BodySlot a, UIList.SortMode m) => -a.element.sort);
		baseList.callbacks = callback;
		foreach (BodySlot bodySlot in this.chara.body.slots)
		{
			if (bodySlot.elementId != 44)
			{
				uilist.Add(bodySlot);
			}
		}
		uilist.Sort();
		uilist.Refresh(false);
	}

	public Chara chara;

	public UIHeader moldHeader;

	public UIHeader moldHeader2;

	public UIText textName;

	public UIText textInfo1;

	public UIText textInfo2;

	public UIList moldListResist;

	public UIList moldListFeat;

	public Transform contentList;

	public UINote note;

	public UIList listEq;

	public Material matItem;

	public Color colorUnequipped;

	private UIHeader _header;
}
