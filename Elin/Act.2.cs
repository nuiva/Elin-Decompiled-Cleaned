using System;
using UnityEngine;
using UnityEngine.UI;

public class Act : Element
{
	public override bool ShowPotential
	{
		get
		{
			return false;
		}
	}

	public override bool UsePotential
	{
		get
		{
			return false;
		}
	}

	public override bool ShowRelativeAttribute
	{
		get
		{
			return true;
		}
	}

	public virtual CursorInfo GetCursorIcon(Card c)
	{
		return this.CursorIcon;
	}

	public virtual CursorInfo CursorIcon
	{
		get
		{
			return CursorSystem.Action;
		}
	}

	public virtual MultiSprite GetStateIcon()
	{
		return null;
	}

	public override Sprite GetSprite()
	{
		return base.source.GetSprite();
	}

	public virtual string ID
	{
		get
		{
			return base.source.alias;
		}
	}

	public virtual bool IsAct
	{
		get
		{
			return true;
		}
	}

	public virtual bool WillEndTurn
	{
		get
		{
			return true;
		}
	}

	public virtual bool CloseLayers
	{
		get
		{
			return true;
		}
	}

	public virtual int LeftHand
	{
		get
		{
			return 0;
		}
	}

	public virtual int RightHand
	{
		get
		{
			return 0;
		}
	}

	public virtual int ElementPowerMod
	{
		get
		{
			return base.source.eleP;
		}
	}

	public virtual bool ShowMapHighlight
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShowMapHighlightBlock
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShowMouseHint(Card c)
	{
		return true;
	}

	public virtual bool PickHeldOnStart
	{
		get
		{
			return false;
		}
	}

	public virtual bool DropHeldOnStart
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanPressRepeat
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanAutofire
	{
		get
		{
			return false;
		}
	}

	public virtual bool ResetAxis
	{
		get
		{
			return false;
		}
	}

	public virtual bool RequireTool
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsHostileAct
	{
		get
		{
			return false;
		}
	}

	public virtual bool HideRightInfo
	{
		get
		{
			return false;
		}
	}

	public virtual bool HaveLongPressAction
	{
		get
		{
			return this.TargetType.CanSelectParty;
		}
	}

	public virtual float Radius
	{
		get
		{
			return base.source.radius;
		}
	}

	public virtual bool IsValidTC(Card c)
	{
		return c.isChara || c.trait.CanBeAttacked;
	}

	public virtual string GetHintText(string str = "")
	{
		return this.GetText(str);
	}

	public virtual string GetText(string str = "")
	{
		if (base.source != null)
		{
			this.id = base.source.id;
		}
		if (this.id != 0)
		{
			return Lang.ParseRaw(base.source.GetName(), str, null, null, null, null);
		}
		if (!Lang.Has(this.ToString()))
		{
			string[] array = new string[5];
			array[0] = this.ToString();
			array[1] = "/";
			int num = 2;
			SourceElement.Row source = base.source;
			array[num] = ((source != null) ? source.ToString() : null);
			array[3] = "/";
			array[4] = this.id.ToString();
			return string.Concat(array);
		}
		return Lang.Get(this.ToString());
	}

	public virtual string GetTextSmall(Card c)
	{
		if (c == null)
		{
			return null;
		}
		return c.Name + c.GetExtraName();
	}

	public virtual int PerformDistance
	{
		get
		{
			return 1;
		}
	}

	public virtual int MaxRadius
	{
		get
		{
			return 99;
		}
	}

	public virtual TargetType TargetType
	{
		get
		{
			if (!base.source.target.IsEmpty())
			{
				string target = base.source.target;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(target);
				if (num <= 1049176909U)
				{
					if (num <= 850565431U)
					{
						if (num != 333403860U)
						{
							if (num == 850565431U)
							{
								if (target == "Party")
								{
									return TargetType.Party;
								}
							}
						}
						else if (target == "Chara")
						{
							return TargetType.Chara;
						}
					}
					else if (num != 1004962635U)
					{
						if (num == 1049176909U)
						{
							if (target == "Select")
							{
								return TargetType.Select;
							}
						}
					}
					else if (target == "Enemy")
					{
						return TargetType.Enemy;
					}
				}
				else if (num <= 2648502281U)
				{
					if (num != 1480824651U)
					{
						if (num == 2648502281U)
						{
							if (target == "SelfParty")
							{
								return TargetType.SelfParty;
							}
						}
					}
					else if (target == "Neighbor")
					{
						return TargetType.SelfAndNeighbor;
					}
				}
				else if (num != 3803529630U)
				{
					if (num == 3834216855U)
					{
						if (target == "Self")
						{
							return TargetType.Self;
						}
					}
				}
				else if (target == "Ground")
				{
					return TargetType.Ground;
				}
			}
			return TargetType.Any;
		}
	}

	public virtual bool LocalAct
	{
		get
		{
			return true;
		}
	}

	public virtual bool CanRapidFire
	{
		get
		{
			return false;
		}
	}

	public virtual float RapidDelay
	{
		get
		{
			return 0.2f;
		}
	}

	public virtual bool ShowAuto
	{
		get
		{
			return false;
		}
	}

	public virtual Color GetActPlanColor()
	{
		if (!this.IsCrime)
		{
			return EClass.Colors.colorAct;
		}
		return EClass.Colors.colorHostileAct;
	}

	public virtual bool IsCrime
	{
		get
		{
			return EClass._zone.IsCrime(EClass.pc, this);
		}
	}

	public virtual bool Perform()
	{
		int id = this.id;
		if ((id == 8230 || id == 8232) && Act.TC.isThing)
		{
			int power = Act.CC.elements.GetOrCreateElement(base.source.id).GetPower(Act.CC) * Act.powerMod / 100;
			ActEffect.Proc(base.source.proc[0].ToEnum(true), power, BlessedState.Normal, Act.CC, Act.TC, default(ActRef));
			return true;
		}
		if (base.source.proc.Length != 0)
		{
			string text = base.source.aliasRef.IsEmpty(Act.CC.MainElement.source.alias);
			string a = base.source.proc[0];
			if (a == "LulwyTrick" || a == "BuffStats")
			{
				text = base.source.proc[1];
			}
			else if (text == "mold")
			{
				text = Act.CC.MainElement.source.alias;
			}
			if (this.TargetType.Range == TargetRange.Self && !Act.forcePt)
			{
				Act.TC = Act.CC;
				Act.TP.Set(Act.CC.pos);
			}
			int power2 = Act.CC.elements.GetOrCreateElement(base.source.id).GetPower(Act.CC) * Act.powerMod / 100;
			ActEffect.ProcAt(base.source.proc[0].ToEnum(true), power2, BlessedState.Normal, Act.CC, Act.TC, Act.TP, base.source.tag.Contains("neg"), new ActRef
			{
				n1 = base.source.proc.TryGet(1, true),
				aliasEle = text,
				act = this
			});
		}
		return true;
	}

	public virtual bool CanPerform()
	{
		return true;
	}

	public bool Perform(Chara _cc, Card _tc = null, Point _tp = null)
	{
		return this.CanPerform(_cc, _tc, _tp) && this.Perform();
	}

	public bool CanPerform(Chara _cc, Card _tc = null, Point _tp = null)
	{
		Act.<>c__DisplayClass82_0 CS$<>8__locals1 = new Act.<>c__DisplayClass82_0();
		CS$<>8__locals1.<>4__this = this;
		Act.CC = _cc;
		Act.TC = _tc;
		Point tp = Act.TP;
		Point point = _tp;
		if (_tp == null)
		{
			Card tc = Act.TC;
			point = (((tc != null) ? tc.pos : null) ?? Act.CC.pos);
		}
		tp.Set(point);
		CS$<>8__locals1.tt = this.TargetType;
		if (Act.forcePt && CS$<>8__locals1.tt.Range == TargetRange.Self && !Act.CC.IsPC)
		{
			CS$<>8__locals1.tt = TargetType.Chara;
		}
		if (this.LocalAct && EClass._zone.IsRegion)
		{
			return false;
		}
		if (CS$<>8__locals1.tt.Range == TargetRange.Self)
		{
			Act.TP.Set(Act.CC.pos);
			return this.CanPerform();
		}
		if (Act.CC.IsPC && EClass.core.config.game.shiftToUseNegativeAbilityOnSelf)
		{
			Chara firstChara = Act.TP.FirstChara;
			if (firstChara != null && firstChara.IsPCFactionOrMinion)
			{
				bool flag = base.source.tag.Contains("neg");
				if (base.source.id != 6011 && flag && !EInput.isShiftDown)
				{
					return false;
				}
			}
		}
		if (!(this is ActMelee) && CS$<>8__locals1.tt.Range == TargetRange.Chara && (Act.TC == null || !Act.CC.CanSee(Act.TC)))
		{
			return false;
		}
		CS$<>8__locals1.distCheck = CS$<>8__locals1.<CanPerform>g__DistCheck|0(Act.CC.pos, Act.TP);
		if (!CS$<>8__locals1.distCheck && (Act.CC.IsMultisize || (Act.TC != null && Act.TC.IsMultisize)))
		{
			if (Act.CC.IsMultisize)
			{
				Act.CC.ForeachPoint(delegate(Point p, bool main)
				{
					base.<CanPerform>g__DistCheckMulti|1(p, Act.TC);
				});
			}
			else
			{
				CS$<>8__locals1.<CanPerform>g__DistCheckMulti|1(Act.CC.pos, Act.TC);
			}
		}
		return CS$<>8__locals1.distCheck && this.CanPerform();
	}

	public static void SetReference(Chara _cc, Card _tc = null, Point _tp = null)
	{
		Act.CC = _cc;
		Act.TC = _tc;
		Act.TP.Set(_tp ?? Act.TC.pos);
	}

	public virtual bool IsToolValid()
	{
		return Act.CC == EClass.pc && Act.TOOL != null && Act.TOOL.parent == EClass.pc;
	}

	public static void SetTool(Thing t)
	{
		Act.TOOL = t;
	}

	public new void SetImage(Image image)
	{
		image.sprite = (this.GetSprite() ?? EClass.core.refs.icons.defaultAbility);
		if (!base.source.aliasRef.IsEmpty())
		{
			image.color = EClass.setting.elements[base.source.aliasRef].colorSprite;
		}
		else
		{
			image.color = Color.white;
		}
		image.rectTransform.pivot = new Vector2(0.5f, 0.5f);
		image.SetNativeSize();
	}

	public virtual void OnMarkMapHighlights()
	{
		EClass._map.ForeachSphere(EClass.pc.pos.x, EClass.pc.pos.z, (float)base.act.MaxRadius, delegate(Point p)
		{
			if (!p.HasBlock && this.ShouldMapHighlight(p))
			{
				p.cell.highlight = 8;
				EClass.player.lastMarkedHighlights.Add(p.Copy());
			}
		});
	}

	public virtual bool ShouldMapHighlight(Point p)
	{
		return base.act.TargetType.ShowMapHighlight && base.act.ShowMapHighlight && base.act.CanPerform(EClass.pc, null, p);
	}

	public static Chara CC;

	public static Card TC;

	public static Point TP = new Point();

	public static Thing TOOL;

	public static int powerMod = 100;

	public static bool forcePt;

	public enum CostType
	{
		None,
		MP,
		SP
	}

	public struct Cost
	{
		public int cost;

		public Act.CostType type;
	}
}
