using System;
using Newtonsoft.Json;
using UnityEngine;

public class TraitEffect : TraitItem
{
	public float Interval
	{
		get
		{
			if (this.data.interval != 0f)
			{
				return this.data.interval;
			}
			return 3f;
		}
	}

	public int id
	{
		get
		{
			return this.owner.refVal;
		}
		set
		{
			this.owner.refVal = value;
		}
	}

	public Effect Effect
	{
		get
		{
			return EClass.core.refs.fireworks[this.id % EClass.core.refs.fireworks.Count];
		}
	}

	public virtual string Path
	{
		get
		{
			return "";
		}
	}

	public override bool UseExtra
	{
		get
		{
			return this.owner.isOn;
		}
	}

	public TraitEffect.Data data
	{
		get
		{
			if (this._data == null)
			{
				this._data = this.owner.GetObj<TraitEffect.Data>(7);
				if (this._data == null)
				{
					this._data = new TraitEffect.Data();
					this.owner.SetObj(7, this._data);
				}
			}
			return this._data;
		}
		set
		{
			this.owner.SetObj(7, value);
		}
	}

	public void Proc(Vector3 v = default(Vector3))
	{
		Effect effect = Effect.Get(this.Path);
		if (this.data.color.Get().a != 0f)
		{
			effect.SetParticleColor(this.data.color.Get());
		}
		if (this.data.sprite != 0)
		{
			Sprite sprite = effect.sprites[this.data.sprite % effect.sprites.Length];
			ParticleSystem[] componentsInChildren = effect.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ParticleSystem.ShapeModule shape = componentsInChildren[i].shape;
				if (shape.shapeType == ParticleSystemShapeType.Sprite && shape.sprite)
				{
					shape.sprite = sprite;
					shape.texture = sprite.texture;
				}
			}
		}
		if (effect.dirs.Length != 0)
		{
			effect.transform.localEulerAngles = effect.dirs[this.owner.dir % effect.dirs.Length];
		}
		effect.Play((v == default(Vector3)) ? ((this.owner.parent == EClass._zone) ? this.owner.renderer.position : EClass.pc.renderer.position) : v);
	}

	public override void TrySetAct(ActPlan p)
	{
		base.TrySetAct(p);
		if (p.input != ActInput.AllAction)
		{
			return;
		}
		if (this.Effect.sprites.Length != 0)
		{
			p.TrySetAct("actChangeSymbol", delegate()
			{
				UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
				uicontextMenu.AddSlider("adjustment", (float a) => ((int)a).ToString() ?? "", (float)this.data.sprite, delegate(float b)
				{
					this.data.sprite = (int)b;
				}, 0f, (float)(this.Effect.sprites.Length - 1), true, false, false);
				uicontextMenu.Show();
				return false;
			}, this.owner, null, 1, false, true, false);
		}
		if (this.Effect.systems.Length != 0)
		{
			p.TrySetAct("actChangeColor", delegate()
			{
				EClass.ui.AddLayer<LayerColorPicker>().SetColor(this.data.color.Get(), new Color(0f, 0f, 0f, 0f), delegate(PickerState state, Color _c)
				{
					this.data.color.Set(_c);
				});
				return false;
			}, this.owner, null, 1, false, true, false);
		}
		p.TrySetAct("actChangeInterval", delegate()
		{
			UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
			uicontextMenu.AddSlider("adjustment", (float a) => (0.01f * (float)((int)(a * 10f))).ToString() ?? "", this.data.interval * 10f, delegate(float b)
			{
				this.data.interval = b * 0.1f;
			}, 0f, 200f, true, false, false);
			uicontextMenu.Show();
			return false;
		}, this.owner, null, 1, false, true, false);
		p.TrySetAct("actChangeDelay", delegate()
		{
			UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
			uicontextMenu.AddSlider("adjustment", (float a) => (0.01f * (float)((int)(a * 10f))).ToString() ?? "", this.data.delay * 10f, delegate(float b)
			{
				this.data.delay = b * 0.1f;
			}, 0f, 200f, true, false, false);
			uicontextMenu.Show();
			return false;
		}, this.owner, null, 1, false, true, false);
		if (EClass.debug.enable)
		{
			p.TrySetAct("actChangeType", delegate()
			{
				UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
				uicontextMenu.AddSlider("adjustment", (float a) => a.ToString() ?? "", (float)this.id, delegate(float b)
				{
					this.id = (int)b;
					this.Proc(default(Vector3));
				}, 0f, (float)(EClass.core.refs.fireworks.Count - 1), true, false, false);
				uicontextMenu.Show();
				return false;
			}, this.owner, null, 1, false, true, false);
		}
	}

	public override int CompareTo(Card b)
	{
		return b.refVal - this.owner.refVal;
	}

	public float timer;

	public TraitEffect.Data _data;

	public class Data
	{
		[JsonProperty]
		public int sprite;

		[JsonProperty]
		public float interval;

		[JsonProperty]
		public float delay;

		[JsonProperty]
		public SerializableColor color = new SerializableColor(0, 0, 0, 0);
	}
}
