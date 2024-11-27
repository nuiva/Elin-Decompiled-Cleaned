using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Feat : Element
{
	public override string Name
	{
		get
		{
			return base.source.GetText("name", false).SplitNewline().TryGet(base.Value - 1, -1);
		}
	}

	public override string FullName
	{
		get
		{
			string[] array = base.source.GetText("name", false).SplitNewline();
			return array.TryGet(base.Value - 1, -1) + ((base.source.max > 1 && array.Length == 1 && base.Value > 1) ? (" " + base.Value.ToString()) : "");
		}
	}

	public override bool CanLink(ElementContainer owner)
	{
		return !base.IsGlobalElement;
	}

	public override bool ShowXP
	{
		get
		{
			return false;
		}
	}

	public override bool ShowValue
	{
		get
		{
			return false;
		}
	}

	public override int CostLearn
	{
		get
		{
			return base.source.cost.TryGet(base.Value - 1, -1);
		}
	}

	public override Sprite GetIcon(string suffix = "")
	{
		return SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "ele_Feat");
	}

	public override void OnWriteNote(UINote n, ElementContainer owner)
	{
		this.Apply(base.Value, owner, true);
		foreach (string str in Feat.hints)
		{
			n.AddText("_bullet".lang() + str, FontColor.DontChange);
		}
	}

	public string GetHint(ElementContainer owner)
	{
		string text = "";
		this.Apply(base.Value, owner, true);
		foreach (string str in Feat.hints)
		{
			text = text + str.StripLastPun() + ", ";
		}
		return text.TrimEnd(' ').TrimEnd(',');
	}

	public bool IsAvailable(ElementContainer owner, int a = 1)
	{
		if (base.source.req.Length != 0)
		{
			Element element = owner.GetElement(base.source.req[0]);
			if (element == null || element.ValueWithoutLink < ((base.source.req.Length == 1) ? 1 : base.source.req[Mathf.Clamp(a, 1, base.source.req.Length - 1)].ToInt()))
			{
				return false;
			}
		}
		return true;
	}

	public List<string> Apply(int a, ElementContainer owner, bool hint = false)
	{
		Feat.<>c__DisplayClass17_0 CS$<>8__locals1;
		CS$<>8__locals1.hint = hint;
		CS$<>8__locals1.owner = owner;
		CS$<>8__locals1.a = a;
		CS$<>8__locals1.<>4__this = this;
		if (CS$<>8__locals1.hint)
		{
			Feat.hints.Clear();
		}
		int value = base.Value;
		CS$<>8__locals1.A = Mathf.Abs(CS$<>8__locals1.a);
		CS$<>8__locals1.invert = ((CS$<>8__locals1.a >= 0) ? 1 : -1);
		if (CS$<>8__locals1.hint)
		{
			Feat.featRef[0] = (CS$<>8__locals1.a.ToString() ?? "");
			Feat.featRef[1] = (CS$<>8__locals1.a.ToString() ?? "");
		}
		Chara chara = CS$<>8__locals1.owner.Chara;
		int num = this.vPotential;
		if (!CS$<>8__locals1.hint && CS$<>8__locals1.a > 0 && chara != null)
		{
			num = (this.vPotential = chara.LV);
		}
		int i = this.id;
		if (i <= 1330)
		{
			if (i <= 1305)
			{
				if (i <= 1230)
				{
					switch (i)
					{
					case 1202:
						this.<Apply>g__ModBase|17_2(403, CS$<>8__locals1.a * 20, true, ref CS$<>8__locals1);
						goto IL_12F3;
					case 1203:
					case 1205:
					case 1207:
					case 1208:
					case 1209:
					case 1211:
					case 1214:
					case 1215:
					case 1219:
					case 1220:
					case 1222:
					case 1225:
						goto IL_12F3;
					case 1204:
						this.<Apply>g__ModBase|17_2(64, CS$<>8__locals1.a * 50, true, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(401, CS$<>8__locals1.a, false, ref CS$<>8__locals1);
						goto IL_12F3;
					case 1206:
						this.<Apply>g__ModBase|17_2(78, -CS$<>8__locals1.a * 10, false, ref CS$<>8__locals1);
						goto IL_12F3;
					case 1210:
						this.<Apply>g__ModBase|17_2(955, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(953, CS$<>8__locals1.a * 10, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(958, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(956, CS$<>8__locals1.a * 10, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(950, CS$<>8__locals1.a * -5, false, ref CS$<>8__locals1);
						goto IL_12F3;
					case 1212:
						this.<Apply>g__ModBase|17_2(961, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(953, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(958, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(956, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(954, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(957, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(959, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
						goto IL_12F3;
					case 1213:
						this.<Apply>g__ModBase|17_2(962, CS$<>8__locals1.a * 20, false, ref CS$<>8__locals1);
						goto IL_12F3;
					case 1216:
						this.<Apply>g__ModBase|17_2(6020, CS$<>8__locals1.a, false, ref CS$<>8__locals1);
						goto IL_12F3;
					case 1217:
						this.<Apply>g__ModBase|17_2(952, CS$<>8__locals1.a * -10, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(955, CS$<>8__locals1.a * 20, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(964, CS$<>8__locals1.a * 20, false, ref CS$<>8__locals1);
						goto IL_12F3;
					case 1218:
						this.<Apply>g__ModBase|17_2(950, CS$<>8__locals1.a / 40, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(952, CS$<>8__locals1.a / 40, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(955, CS$<>8__locals1.a / 40, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(953, CS$<>8__locals1.a / 40, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(958, CS$<>8__locals1.a / 40, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(956, CS$<>8__locals1.a / 40, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(954, CS$<>8__locals1.a / 40, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(959, CS$<>8__locals1.a / 40, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(964, CS$<>8__locals1.a / 40, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(961, CS$<>8__locals1.a / 40, false, ref CS$<>8__locals1);
						goto IL_12F3;
					case 1221:
						this.<Apply>g__ModBase|17_2(964, CS$<>8__locals1.a * 20, false, ref CS$<>8__locals1);
						goto IL_12F3;
					case 1223:
						this.<Apply>g__ModBase|17_2(963, CS$<>8__locals1.a * 20, false, ref CS$<>8__locals1);
						goto IL_12F3;
					case 1224:
						this.<Apply>g__ModBase|17_2(951, CS$<>8__locals1.a * 15, false, ref CS$<>8__locals1);
						goto IL_12F3;
					case 1226:
						this.<Apply>g__ModBase|17_2(955, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(953, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
						goto IL_12F3;
					default:
						if (i != 1230)
						{
							goto IL_12F3;
						}
						this.<Apply>g__ModBase|17_2(60, CS$<>8__locals1.A * 2 * CS$<>8__locals1.invert, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(79, CS$<>8__locals1.A * 3 * CS$<>8__locals1.invert, false, ref CS$<>8__locals1);
						goto IL_12F3;
					}
				}
				else
				{
					if (i == 1233)
					{
						this.<Apply>g__ModBase|17_2(954, CS$<>8__locals1.a * 10, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(423, CS$<>8__locals1.a, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(425, CS$<>8__locals1.a, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(424, CS$<>8__locals1.a, false, ref CS$<>8__locals1);
						this.<Apply>g__ModBase|17_2(421, CS$<>8__locals1.a, false, ref CS$<>8__locals1);
						goto IL_12F3;
					}
					if (i != 1300 && i != 1305)
					{
						goto IL_12F3;
					}
				}
			}
			else if (i <= 1315)
			{
				if (i != 1310 && i != 1315)
				{
					goto IL_12F3;
				}
			}
			else if (i != 1320 && i != 1325 && i != 1330)
			{
				goto IL_12F3;
			}
		}
		else if (i <= 1355)
		{
			if (i <= 1340)
			{
				if (i != 1335 && i != 1340)
				{
					goto IL_12F3;
				}
			}
			else if (i != 1345 && i != 1350 && i != 1355)
			{
				goto IL_12F3;
			}
		}
		else if (i <= 1415)
		{
			switch (i)
			{
			case 1400:
				Feat.featRef[0] = ((CS$<>8__locals1.a * 10).ToString() ?? "");
				goto IL_12F3;
			case 1401:
				this.<Apply>g__ModBase|17_2(78, CS$<>8__locals1.a * 15, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1402:
			case 1403:
			case 1405:
				goto IL_12F3;
			case 1404:
				Feat.featRef[0] = ((CS$<>8__locals1.a * 10).ToString() ?? "");
				goto IL_12F3;
			case 1406:
				Feat.featRef[0] = ((CS$<>8__locals1.a * 20).ToString() ?? "");
				Feat.featRef[1] = ((CS$<>8__locals1.a * 5).ToString() ?? "");
				goto IL_12F3;
			default:
				if (i != 1415)
				{
					goto IL_12F3;
				}
				this.<Apply>g__ModBase|17_2(60, CS$<>8__locals1.a * 15, false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(79, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(953, CS$<>8__locals1.a * 3, false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(961, CS$<>8__locals1.a, false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(960, CS$<>8__locals1.a, false, ref CS$<>8__locals1);
				goto IL_12F3;
			}
		}
		else
		{
			if (i == 1419)
			{
				this.<Apply>g__ModPotential|17_3(101, CS$<>8__locals1.a * 50, ref CS$<>8__locals1);
				this.<Apply>g__ModPotential|17_3(111, CS$<>8__locals1.a * 50, ref CS$<>8__locals1);
				this.<Apply>g__ModPotential|17_3(103, CS$<>8__locals1.a * 50, ref CS$<>8__locals1);
				this.<Apply>g__ModPotential|17_3(106, CS$<>8__locals1.a * 50, ref CS$<>8__locals1);
				this.<Apply>g__ModPotential|17_3(122, CS$<>8__locals1.a * 50, ref CS$<>8__locals1);
				this.<Apply>g__ModPotential|17_3(120, CS$<>8__locals1.a * 50, ref CS$<>8__locals1);
				this.<Apply>g__ModPotential|17_3(123, CS$<>8__locals1.a * 50, ref CS$<>8__locals1);
				goto IL_12F3;
			}
			switch (i)
			{
			case 1510:
				this.<Apply>g__ModBase|17_2(65, CS$<>8__locals1.a * 3, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1511:
				this.<Apply>g__ModBase|17_2(65, -CS$<>8__locals1.a * 3, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1512:
				this.<Apply>g__ModBase|17_2(73, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1513:
				this.<Apply>g__ModBase|17_2(73, -CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1514:
				this.<Apply>g__ModBase|17_2(72, CS$<>8__locals1.a * 3, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1515:
				this.<Apply>g__ModBase|17_2(72, -CS$<>8__locals1.a * 3, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1516:
				this.<Apply>g__ModBase|17_2(300, CS$<>8__locals1.a * 4, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1517:
				this.<Apply>g__ModBase|17_2(300, -CS$<>8__locals1.a * 4, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1518:
				this.<Apply>g__ModBase|17_2(79, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1519:
				this.<Apply>g__ModBase|17_2(79, -CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1520:
				this.<Apply>g__ModBase|17_2(70, CS$<>8__locals1.a * 3, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1521:
				this.<Apply>g__ModBase|17_2(70, -CS$<>8__locals1.a * 3, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1522:
				this.<Apply>g__ModBase|17_2(77, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1523:
				this.<Apply>g__ModBase|17_2(77, -CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1524:
				this.<Apply>g__ModBase|17_2(307, CS$<>8__locals1.a * 4, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1525:
				this.<Apply>g__ModBase|17_2(307, -CS$<>8__locals1.a * 4, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1526:
				this.<Apply>g__ModBase|17_2(951, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(950, -CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1527:
				this.<Apply>g__ModBase|17_2(951, -CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(950, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1528:
				this.<Apply>g__ModBase|17_2(952, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1529:
				this.<Apply>g__ModBase|17_2(952, -CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1530:
				this.<Apply>g__ModBase|17_2(960, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(956, -CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1531:
				this.<Apply>g__ModBase|17_2(960, -CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(956, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1532:
			case 1533:
			case 1534:
			case 1535:
			case 1536:
			case 1537:
			case 1538:
			case 1539:
			case 1540:
			case 1541:
			case 1542:
			case 1543:
			case 1544:
			case 1545:
			case 1546:
			case 1547:
			case 1548:
			case 1549:
			case 1558:
			case 1559:
				goto IL_12F3;
			case 1550:
				this.<Apply>g__ModBase|17_2(404, -CS$<>8__locals1.a * 10, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1551:
				this.<Apply>g__ModBase|17_2(77, -CS$<>8__locals1.a * (4 + num / 5), false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1552:
				this.<Apply>g__ModBase|17_2(79, CS$<>8__locals1.a * Mathf.Min(30 + num / 5, 100), false, ref CS$<>8__locals1);
				if (!CS$<>8__locals1.hint && CS$<>8__locals1.a > 0 && chara != null)
				{
					chara.body.UnequipAll(39);
					goto IL_12F3;
				}
				goto IL_12F3;
			case 1553:
				this.<Apply>g__ModBase|17_2(73, CS$<>8__locals1.a * (5 + num / 3), false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(77, -CS$<>8__locals1.a * (5 + num / 3), false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1554:
				this.<Apply>g__ModBase|17_2(404, CS$<>8__locals1.a * 10, false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(401, CS$<>8__locals1.a, false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(79, CS$<>8__locals1.a * (10 + num / 5), false, ref CS$<>8__locals1);
				if (!CS$<>8__locals1.hint && CS$<>8__locals1.a > 0 && chara != null)
				{
					chara.body.UnequipAll(33);
					goto IL_12F3;
				}
				goto IL_12F3;
			case 1555:
				this.<Apply>g__ModBase|17_2(65, CS$<>8__locals1.a * (12 + num), false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(77, -CS$<>8__locals1.a * (5 + num / 5), false, ref CS$<>8__locals1);
				if (!CS$<>8__locals1.hint && CS$<>8__locals1.a > 0 && chara != null)
				{
					chara.body.UnequipAll(31);
					goto IL_12F3;
				}
				goto IL_12F3;
			case 1556:
				this.<Apply>g__ModBase|17_2(64, -CS$<>8__locals1.a * (10 + num), false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(67, CS$<>8__locals1.a * (5 + num / 2), false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1557:
				this.<Apply>g__ModBase|17_2(71, -CS$<>8__locals1.a * (5 + num / 3), false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(72, -CS$<>8__locals1.a * (4 + num / 4), false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(74, CS$<>8__locals1.a * (6 + num / 2), false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(75, CS$<>8__locals1.a * (2 + num / 6), false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1560:
				this.<Apply>g__ModBase|17_2(60, -CS$<>8__locals1.a * 15, false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(70, -CS$<>8__locals1.a * (4 + num / 3), false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1561:
				this.<Apply>g__ModBase|17_2(61, -CS$<>8__locals1.a * 15, false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(76, -CS$<>8__locals1.a * (4 + num / 3), false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1562:
				this.<Apply>g__ModBase|17_2(65, CS$<>8__locals1.a * (15 + num / 2), false, ref CS$<>8__locals1);
				this.<Apply>g__ModBase|17_2(79, -CS$<>8__locals1.a * (10 + num / 5), false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1563:
				this.<Apply>g__ModBase|17_2(77, -CS$<>8__locals1.a * (3 + num / 4), false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1564:
				this.<Apply>g__ModBase|17_2(961, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
				goto IL_12F3;
			case 1565:
				this.<Apply>g__ModBase|17_2(955, CS$<>8__locals1.a * 20, false, ref CS$<>8__locals1);
				goto IL_12F3;
			default:
				switch (i)
				{
				case 1610:
					this.<Apply>g__ModBase|17_2(60, CS$<>8__locals1.a * 4, false, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1611:
					this.<Apply>g__ModBase|17_2(61, CS$<>8__locals1.a * 4, false, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1612:
					this.<Apply>g__ModBase|17_2(62, CS$<>8__locals1.a * 4, false, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1613:
				case 1614:
				case 1615:
				case 1616:
				case 1617:
				case 1618:
				case 1619:
				case 1637:
				case 1638:
				case 1639:
				case 1641:
					goto IL_12F3;
				case 1620:
					this.<Apply>g__ModAttribute|17_4(70, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1621:
					this.<Apply>g__ModAttribute|17_4(72, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1622:
					this.<Apply>g__ModAttribute|17_4(71, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1623:
					this.<Apply>g__ModAttribute|17_4(73, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1624:
					this.<Apply>g__ModAttribute|17_4(74, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1625:
					this.<Apply>g__ModAttribute|17_4(76, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1626:
					this.<Apply>g__ModAttribute|17_4(75, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1627:
					this.<Apply>g__ModAttribute|17_4(77, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1628:
					this.<Apply>g__ModBase|17_2(78, CS$<>8__locals1.a * 2, false, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1629:
					this.<Apply>g__ModBase|17_2(79, CS$<>8__locals1.a * 5, false, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1630:
					this.<Apply>g__ModBase|17_2(65, ((CS$<>8__locals1.A == 1) ? 2 : ((CS$<>8__locals1.A == 2) ? 5 : 10)) * CS$<>8__locals1.invert, false, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1631:
					this.<Apply>g__ModBase|17_2(64, ((CS$<>8__locals1.A == 1) ? 2 : ((CS$<>8__locals1.A == 2) ? 5 : 10)) * CS$<>8__locals1.invert, false, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1632:
					this.<Apply>g__ModBase|17_2(150, CS$<>8__locals1.a * 2, false, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1633:
					this.<Apply>g__ModBase|17_2(210, CS$<>8__locals1.a * 3, false, ref CS$<>8__locals1);
					this.<Apply>g__ModBase|17_2(402, ((CS$<>8__locals1.A == 3) ? 1 : 0) * CS$<>8__locals1.invert, false, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1634:
					this.<Apply>g__ModBase|17_2(291, CS$<>8__locals1.a * 4, false, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1635:
					this.<Apply>g__ModBase|17_2(100, CS$<>8__locals1.a * 3, false, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1636:
					this.<Apply>g__ModBase|17_2(306, CS$<>8__locals1.a * 4, false, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1640:
					this.<Apply>g__ModBase|17_2(77, CS$<>8__locals1.a * 2, false, ref CS$<>8__locals1);
					goto IL_12F3;
				case 1642:
					Feat.featRef[0] = (((CS$<>8__locals1.a == 1) ? 10 : ((CS$<>8__locals1.a == 2) ? 20 : 30)).ToString() ?? "");
					goto IL_12F3;
				case 1643:
					Feat.featRef[0] = (((CS$<>8__locals1.a == 1) ? 1 : ((CS$<>8__locals1.a == 2) ? 3 : 5)).ToString() ?? "");
					goto IL_12F3;
				case 1644:
					Feat.featRef[0] = (CS$<>8__locals1.a.ToString() ?? "");
					if (!CS$<>8__locals1.hint && CS$<>8__locals1.a > 0)
					{
						CS$<>8__locals1.owner.Chara.AddRandomBodyPart(CS$<>8__locals1.owner.Chara.IsPC);
						if (CS$<>8__locals1.owner.Chara.IsPC && WidgetEquip.Instance)
						{
							WidgetEquip.Instance.Rebuild();
						}
					}
					this.<Apply>g__ModBase|17_2(60, (CS$<>8__locals1.A > 5) ? ((CS$<>8__locals1.A - 5) * -3 * CS$<>8__locals1.invert) : 0, false, ref CS$<>8__locals1);
					this.<Apply>g__ModBase|17_2(79, (CS$<>8__locals1.A > 5) ? ((CS$<>8__locals1.A - 5) * -5 * CS$<>8__locals1.invert) : 0, false, ref CS$<>8__locals1);
					this.<Apply>g__ModBase|17_2(77, (CS$<>8__locals1.A > 5) ? ((CS$<>8__locals1.A - 5) * -3 * CS$<>8__locals1.invert) : 0, false, ref CS$<>8__locals1);
					goto IL_12F3;
				default:
					goto IL_12F3;
				}
				break;
			}
		}
		this.<Apply>g__GodHint|17_5(ref CS$<>8__locals1);
		IL_12F3:
		if (CS$<>8__locals1.hint)
		{
			string text = base.source.GetText("textExtra", false);
			if (!text.IsEmpty())
			{
				string text2 = text.SplitNewline().TryGet(value - 1, 99);
				if (!text2.IsEmpty())
				{
					int num2 = 0;
					string[] array = text2.Split(',', StringSplitOptions.None);
					for (i = 0; i < array.Length; i++)
					{
						string item = array[i].Replace("#1", Feat.featRef[num2]);
						Feat.hints.Add(item);
						num2++;
					}
				}
			}
		}
		return Feat.hints;
	}

	[CompilerGenerated]
	private void <Apply>g__Note|17_0(string s, ref Feat.<>c__DisplayClass17_0 A_2)
	{
		if (!A_2.hint)
		{
			return;
		}
		Feat.hints.Add(s);
	}

	[CompilerGenerated]
	private void <Apply>g__NoteElement|17_1(int ele, int a, ref Feat.<>c__DisplayClass17_0 A_3)
	{
		SourceElement.Row row = EClass.sources.elements.map[ele];
		if (row.category == "ability")
		{
			this.<Apply>g__Note|17_0("hintLearnAbility".lang(row.GetName().ToTitleCase(false), null, null, null, null), ref A_3);
			return;
		}
		if (row.tag.Contains("flag"))
		{
			this.<Apply>g__Note|17_0(row.GetName(), ref A_3);
			return;
		}
		string @ref = ((a < 0) ? "" : "+") + a.ToString();
		if (row.category == "resist")
		{
			int num = 0;
			@ref = ((a > 0) ? "+" : "-").Repeat(Mathf.Clamp(a / 5 + num, 1, 5));
			this.<Apply>g__Note|17_0("modValueRes".lang(row.GetName(), @ref, null, null, null), ref A_3);
			return;
		}
		this.<Apply>g__Note|17_0("modValue".lang(row.GetName(), @ref, null, null, null), ref A_3);
	}

	[CompilerGenerated]
	private void <Apply>g__ModBase|17_2(int ele, int _v, bool hide, ref Feat.<>c__DisplayClass17_0 A_4)
	{
		if (!A_4.hint)
		{
			A_4.owner.ModBase(ele, _v);
		}
		if (!hide && _v != 0)
		{
			this.<Apply>g__NoteElement|17_1(ele, _v, ref A_4);
		}
	}

	[CompilerGenerated]
	private void <Apply>g__ModPotential|17_3(int ele, int _v, ref Feat.<>c__DisplayClass17_0 A_3)
	{
		if (!A_3.hint)
		{
			A_3.owner.ModPotential(ele, _v);
		}
		this.<Apply>g__Note|17_0("modPotential".lang(EClass.sources.elements.map[ele].GetName(), "+" + _v.ToString() + "%", null, null, null), ref A_3);
	}

	[CompilerGenerated]
	private void <Apply>g__ModAttribute|17_4(int ele, ref Feat.<>c__DisplayClass17_0 A_2)
	{
		if (!A_2.hint)
		{
			Debug.Log(A_2.A);
			Debug.Log(((A_2.A == 1) ? 2 : ((A_2.A == 2) ? 4 : 5)) * A_2.invert);
		}
		this.<Apply>g__ModBase|17_2(ele, ((A_2.A == 1) ? 2 : ((A_2.A == 2) ? 4 : 5)) * A_2.invert, false, ref A_2);
		this.<Apply>g__ModPotential|17_3(ele, A_2.a * 10, ref A_2);
	}

	[CompilerGenerated]
	private void <Apply>g__GodHint|17_5(ref Feat.<>c__DisplayClass17_0 A_1)
	{
		if (!A_1.hint)
		{
			return;
		}
		foreach (Element element in A_1.owner.Card.Chara.faithElements.dict.Values)
		{
			if (element.source.id != this.id)
			{
				this.<Apply>g__NoteElement|17_1(element.id, element.Value, ref A_1);
			}
		}
	}

	public static List<string> hints = new List<string>();

	public static string[] featRef = new string[5];
}
