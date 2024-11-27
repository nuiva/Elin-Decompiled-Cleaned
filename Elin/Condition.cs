using System;

public class Condition : BaseCondition
{
	public virtual bool IsKilled
	{
		get
		{
			return base.value <= 0;
		}
	}

	public override BaseNotification CreateNotification()
	{
		return new NotificationCondition
		{
			condition = this
		};
	}

	public static T Create<T>(int power = 100, Action<T> onCreate = null) where T : Condition
	{
		return (T)((object)Condition.Create(typeof(T).Name, power, delegate(Condition c)
		{
			onCreate(c as T);
		}));
	}

	public static Condition Create(string alias, int power = 100, Action<Condition> onCreate = null)
	{
		SourceStat.Row row = EClass.sources.stats.alias[alias];
		Condition condition = ClassCache.Create<Condition>(row.type.IsEmpty(alias), "Elin");
		condition.power = power;
		condition.id = row.id;
		condition._source = row;
		if (onCreate != null)
		{
			onCreate(condition);
		}
		return condition;
	}

	public virtual string TextDuration
	{
		get
		{
			string result;
			if (!base.isPerfume)
			{
				if ((result = base.value.ToString()) == null)
				{
					return "";
				}
			}
			else
			{
				result = "";
			}
			return result;
		}
	}

	public virtual void OnStacked(int p)
	{
		base.value += this.EvaluateTurn(p);
		base.SetPhase();
	}

	public Condition SetPerfume(int duration = 3)
	{
		base.isPerfume = true;
		base.value = duration;
		return this;
	}

	public override void Tick()
	{
		base.Mod(-1, false);
	}

	public virtual void OnCalculateFov(Fov fov, ref int radius, ref float power)
	{
	}

	public virtual void OnCreateFov(Fov fov)
	{
	}

	public override void OnValueChanged()
	{
		if (base.value <= 0)
		{
			this.Kill(false);
		}
	}

	public void Kill(bool silent = false)
	{
		base.value = 0;
		this.owner.conditions.Remove(this);
		if (!silent && !this.owner.isDead && !base.source.textEnd.IsEmpty())
		{
			this.owner.Say(base.source.GetText("textEnd", false), this.owner, this.RefString1, null);
		}
		this.PlayEndEffect();
		this.OnRemoved();
		if (this.elements != null)
		{
			this.elements.SetParent(null);
		}
		this.owner.SetDirtySpeed();
		if (this.ShouldRefresh)
		{
			this.owner.Refresh(false);
		}
	}

	public static bool ignoreEffect;
}
