using System;
using System.Linq;
using UnityEngine;

public class ListPeopleCallReserve : BaseListPeople
{
	public override void OnInstantiate(Chara a, ItemGeneral b)
	{
		ListPeopleCallReserve.<>c__DisplayClass1_0 CS$<>8__locals1 = new ListPeopleCallReserve.<>c__DisplayClass1_0();
		CS$<>8__locals1.a = a;
		CS$<>8__locals1.<>4__this = this;
		b.SetSubText(CS$<>8__locals1.a.job.GetName().ToTitleCase(false), 280, FontColor.Default, TextAnchor.MiddleLeft);
		if (CS$<>8__locals1.a.trait.CanBeBanished)
		{
			b.AddSubButton(EClass.core.refs.icons.trash, delegate
			{
				ListPeopleCallReserve.<>c__DisplayClass1_1 CS$<>8__locals2 = new ListPeopleCallReserve.<>c__DisplayClass1_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				ListPeopleCallReserve.<>c__DisplayClass1_1 CS$<>8__locals3 = CS$<>8__locals2;
				Action func;
				if ((func = CS$<>8__locals1.<>9__1) == null)
				{
					func = (CS$<>8__locals1.<>9__1 = delegate()
					{
						EClass.Home.RemoveReserve(CS$<>8__locals1.a);
						CS$<>8__locals1.a.OnBanish();
						CS$<>8__locals1.<>4__this.list.List(false);
						SE.Trash();
					});
				}
				CS$<>8__locals3.func = func;
				if (CS$<>8__locals1.<>4__this.skipDialog)
				{
					CS$<>8__locals2.func();
					return;
				}
				Dialog.Choice("dialogDeleteRecruit", delegate(Dialog d)
				{
					string text = "yes".lang();
					Action onClick;
					if ((onClick = CS$<>8__locals2.<>9__3) == null)
					{
						onClick = (CS$<>8__locals2.<>9__3 = delegate()
						{
							CS$<>8__locals2.func();
						});
					}
					d.AddButton(text, onClick, true);
					string text2 = "yesAndSkip".lang();
					Action onClick2;
					if ((onClick2 = CS$<>8__locals2.<>9__4) == null)
					{
						onClick2 = (CS$<>8__locals2.<>9__4 = delegate()
						{
							CS$<>8__locals2.func();
							CS$<>8__locals2.CS$<>8__locals1.<>4__this.skipDialog = true;
						});
					}
					d.AddButton(text2, onClick2, true);
					d.AddButton("no".lang(), null, true);
				});
			}, null, null);
		}
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		GameLang.refDrama1 = "";
		c.ShowDialog("_chara", "4-1", "").onKill.AddListener(delegate()
		{
			this.list.List(false);
		});
	}

	public override void OnList()
	{
		foreach (HireInfo hireInfo in EClass.Home.listReserve)
		{
			this.list.Add(hireInfo.chara);
		}
	}

	public HireInfo GetInfo(Chara c)
	{
		return EClass.Home.listReserve.First((HireInfo a) => a.chara == c);
	}

	public bool skipDialog;
}
