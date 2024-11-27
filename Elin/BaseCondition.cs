using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class BaseCondition : BaseStats
{
	public int value
	{
		get
		{
			return this._ints[0];
		}
		set
		{
			this._ints[0] = value;
		}
	}

	public int power
	{
		get
		{
			return this._ints[1];
		}
		set
		{
			this._ints[1] = value;
		}
	}

	public int refVal
	{
		get
		{
			return this._ints[2];
		}
		set
		{
			this._ints[2] = value;
		}
	}

	public int refVal2
	{
		get
		{
			return this._ints[3];
		}
		set
		{
			this._ints[3] = value;
		}
	}

	public bool givenByPcParty
	{
		get
		{
			return (this._ints[4] & 2) != 0;
		}
		set
		{
			this._ints[4] = (value ? (this._ints[4] | 2) : (this._ints[4] & -3));
		}
	}

	public bool isPerfume
	{
		get
		{
			return (this._ints[4] & 4) != 0;
		}
		set
		{
			this._ints[4] = (value ? (this._ints[4] | 4) : (this._ints[4] & -5));
		}
	}

	public override Chara Owner
	{
		get
		{
			return this.owner;
		}
	}

	public virtual string Name
	{
		get
		{
			return base.source.GetText("name", false);
		}
	}

	public virtual bool CanStack(Condition c)
	{
		return true;
	}

	public virtual bool IsToggle
	{
		get
		{
			return false;
		}
	}

	public virtual bool WillOverride
	{
		get
		{
			return false;
		}
	}

	public virtual bool AllowMultipleInstance
	{
		get
		{
			return false;
		}
	}

	public virtual bool ConsumeTurn
	{
		get
		{
			return false;
		}
	}

	public virtual bool PreventRegen
	{
		get
		{
			return false;
		}
	}

	public virtual bool ShouldRefresh
	{
		get
		{
			return false;
		}
	}

	public virtual bool CancelAI
	{
		get
		{
			return this.ConsumeTurn;
		}
	}

	public virtual bool TimeBased
	{
		get
		{
			return false;
		}
	}

	public virtual bool SyncRide
	{
		get
		{
			return false;
		}
	}

	public virtual bool TryMove(Point p)
	{
		return true;
	}

	public virtual int GainResistFactor
	{
		get
		{
			return base.source.gainRes;
		}
	}

	public virtual int P2
	{
		get
		{
			return 0;
		}
	}

	public SourceElement.Row sourceElement
	{
		get
		{
			return EClass.sources.elements.map[this.refVal];
		}
	}

	public virtual bool IsElemental
	{
		get
		{
			return false;
		}
	}

	public virtual string RefString1
	{
		get
		{
			if (!this.IsElemental)
			{
				return "";
			}
			return this.sourceElement.GetName().ToLower();
		}
	}

	public void SetElement(int id)
	{
		this.refVal = id;
	}

	public void SetRefVal(int a, int b)
	{
		this.refVal = a;
		this.refVal2 = b;
	}

	public virtual Color GetSpriteColor()
	{
		if (!this.IsElemental)
		{
			return Color.white;
		}
		return EClass.setting.elements[EClass.sources.elements.map[this.refVal].alias].colorSprite;
	}

	public override string ToString()
	{
		return string.Concat(new string[]
		{
			this.Name,
			" ",
			this.value.ToString(),
			" ",
			this.phase.ToString()
		});
	}

	public override string GetText()
	{
		if (!this.IsNullPhase())
		{
			return this.GetPhaseStr();
		}
		return "";
	}

	public override int GetValue()
	{
		return this.value;
	}

	public override Color GetColor(Gradient g)
	{
		return g.Evaluate((base.source.phase.LastItem<int>() == 0) ? 0f : ((float)this.phase / (float)base.source.phase.LastItem<int>()));
	}

	public virtual void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		this.owner = _owner;
		this.phase = this.GetPhase();
		if (this.EmoIcon > this.owner.emoIcon)
		{
			this.owner.emoIcon = this.EmoIcon;
		}
		if (base.source.elements.Length != 0)
		{
			this.elements = new ElementContainer();
			for (int i = 0; i < base.source.elements.Length; i += 2)
			{
				this.elements.SetBase(this.GetElementSource(i).id, base.source.elements[i + 1].Calc(this.power, 0, this.P2), 0);
			}
			this.elements.SetParent(this.owner);
		}
	}

	public SourceElement.Row GetElementSource(int i)
	{
		string text = base.source.elements[i];
		if (text == "res")
		{
			text = this.sourceElement.aliasRef;
		}
		else if (text == "ele")
		{
			text = this.sourceElement.alias;
		}
		return EClass.sources.elements.alias[text];
	}

	public void Start()
	{
		this.OnBeforeStart();
		this.phase = -1;
		this.SetPhase();
		this.OnStart();
		this.OnStartOrStack();
		this.PlayEffect();
		if (base.source.nullify.Length != 0)
		{
			for (int i = this.owner.conditions.Count - 1; i >= 0; i--)
			{
				if (this.TryNullify(this.owner.conditions[i]))
				{
					this.owner.conditions[i].Kill(false);
				}
			}
		}
	}

	public bool TryNullify(Condition c)
	{
		if (base.source.nullify.Length == 0)
		{
			return false;
		}
		foreach (string b in base.source.nullify)
		{
			if (c.source.alias == b)
			{
				this.owner.Say("nullify", this.owner, this.Name.ToLower(), c.Name.ToLower());
				return true;
			}
		}
		return false;
	}

	public virtual void OnBeforeStart()
	{
	}

	public virtual void OnStart()
	{
	}

	public virtual void OnStartOrStack()
	{
	}

	public virtual void PlayEffect()
	{
		if (Condition.ignoreEffect)
		{
			return;
		}
		if (base.source.effect.Length != 0)
		{
			if (!base.source.effect[0].IsEmpty())
			{
				this.owner.PlayEffect(base.source.effect[0], true, 0f, default(Vector3));
			}
			this.owner.PlaySound((base.source.effect.Length >= 2) ? base.source.effect[1] : base.source.effect[0], 1f, true);
		}
	}

	public virtual void PlayEndEffect()
	{
		if (base.source.effect.Length >= 3)
		{
			if (!base.source.effect[2].IsEmpty())
			{
				this.owner.PlayEffect(base.source.effect[2], true, 0f, default(Vector3));
			}
			this.owner.PlaySound((base.source.effect.Length >= 4) ? base.source.effect[3] : base.source.effect[2], 1f, true);
		}
	}

	public virtual void OnRefresh()
	{
	}

	public void SetPhase()
	{
		int num = this.GetPhase();
		if (this.phase != num)
		{
			int num2 = this.phase;
			this.phase = num;
			this.PhaseMsg(num > num2);
			this.OnChangePhase(num2, num);
		}
	}

	public bool IsNullPhase()
	{
		return base.source.strPhase.Length != 0 && base.source.strPhase[this.GetPhase()] == "#";
	}

	public override int GetPhase()
	{
		return base.source.phase[Mathf.Clamp(this.value, 0, 99) / 10];
	}

	public void PhaseMsg(bool inc)
	{
		bool flag = base.source.invert ? (!inc) : inc;
		string[] array = (inc ? base.source.GetText("textPhase", false) : base.source.GetText("textPhase2", false)).Split(Environment.NewLine.ToCharArray());
		if (array.Length <= this.phase || array[this.phase].IsEmpty())
		{
			return;
		}
		if (this.Type == ConditionType.Stance)
		{
			Msg.SetColor("ono");
		}
		else if (!base.source.invert && flag)
		{
			Msg.SetColor("negative");
		}
		else if (base.source.invert && !flag)
		{
			Msg.SetColor("positive");
		}
		base.PopText();
		this.owner.Say(array[this.phase].Split('|', StringSplitOptions.None).RandomItem<string>(), this.owner, this.RefString1, null);
	}

	public override string GetPhaseStr()
	{
		string[] textArray = base.source.GetTextArray("strPhase");
		if (textArray.Length == 0)
		{
			return this.Name;
		}
		return textArray[this.phase].IsEmpty("");
	}

	public virtual void Tick()
	{
	}

	public void Mod(int a, bool force = false)
	{
		if (this.isPerfume && !force)
		{
			return;
		}
		if (this.value == 0)
		{
			return;
		}
		this.value += a;
		this.SetPhase();
		this.OnValueChanged();
	}

	public virtual void OnValueChanged()
	{
	}

	public virtual void OnChangePhase(int lastPhase, int newPhase)
	{
	}

	public virtual void OnRemoved()
	{
	}

	public virtual bool CanManualRemove
	{
		get
		{
			return false;
		}
	}

	public virtual int EvaluatePower(int p)
	{
		return p;
	}

	public virtual int EvaluateTurn(int p)
	{
		return Mathf.Max(1, base.source.duration.Calc(p, 0, this.P2));
	}

	public virtual BaseNotification CreateNotification()
	{
		return new BaseNotification();
	}

	public Element GetDefenseAttribute(Chara c = null)
	{
		if (base.source.defenseAttb.IsEmpty())
		{
			return null;
		}
		if (c == null)
		{
			return Element.Create(base.source.defenseAttb[0], 1);
		}
		return c.elements.GetOrCreateElement(base.source.defenseAttb[0]);
	}

	public override void _WriteNote(UINote n, bool asChild = false)
	{
		List<string> list = new List<string>();
		Element defenseAttribute = this.GetDefenseAttribute(null);
		if (defenseAttribute != null)
		{
			list.Add("hintDefenseAttb".lang(defenseAttribute.Name.ToTitleCase(false), null, null, null, null));
		}
		this.OnWriteNote(list);
		foreach (string key in base.source.nullify)
		{
			list.Add("hintNullify".lang(EClass.sources.stats.alias[key].GetName(), null, null, null, null));
		}
		for (int j = 0; j < base.source.elements.Length; j += 2)
		{
			Element element = Element.Create(this.GetElementSource(j).id, base.source.elements[j + 1].Calc(this.power, 0, this.P2));
			list.Add("modValue".lang(element.Name, ((element.Value < 0) ? "" : "+") + element.Value.ToString(), null, null, null));
		}
		if (list.Count > 0)
		{
			if (!asChild)
			{
				n.Space(8, 1);
			}
			foreach (string str in list)
			{
				n.AddText("_bullet".lang() + str, FontColor.DontChange);
			}
		}
	}

	public virtual void OnWriteNote(List<string> list)
	{
	}

	public virtual RendererReplacer GetRendererReplacer()
	{
		return null;
	}

	[JsonProperty]
	public int[] _ints = new int[5];

	public int phase = -1;

	public ElementContainer elements;

	public Chara owner;
}
