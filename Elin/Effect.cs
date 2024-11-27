using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class Effect : SceneObject
{
	public static Effect Get(Effect original)
	{
		return UnityEngine.Object.Instantiate<Effect>(original);
	}

	public static Effect Get(string id)
	{
		return Effect.manager.effects.Get(id);
	}

	public static T Get<T>(string id) where T : Effect
	{
		return Effect.manager.effects.Get(id) as T;
	}

	public static EffectManager manager
	{
		get
		{
			return EffectManager.Instance;
		}
	}

	public void Play(float delay, Point from, float fixY = 0f, Point to = null, Sprite sprite = null)
	{
		Point _from = from.Copy();
		Point _to = ((to != null) ? to.Copy() : null) ?? null;
		TweenUtil.Tween(delay, delegate()
		{
			this.Play(_from, fixY, _to, sprite);
		}, null);
	}

	public unsafe Effect Play(Point from, float fixY = 0f, Point to = null, Sprite sprite = null)
	{
		return this._Play(from, *from.Position(), fixY, to, sprite);
	}

	public unsafe Effect _Play(Point from, Vector3 fromV, float fixY = 0f, Point to = null, Sprite sprite = null)
	{
		if (!EMono.core.IsGameStarted)
		{
			this.Kill();
			return null;
		}
		if (sprite)
		{
			this.sr.sprite = sprite;
		}
		if (this.setColor)
		{
			float num = 0.07f * (float)from.cell.light + EMono.core.screen.tileMap._baseBrightness;
			num += ((from.cell.HasRoof || from.cell.isShadowed) ? -0.2f : 0f);
			if (this.sr)
			{
				this.sr.material.SetVector("_Color", new Vector4(num, num, num, 1f));
			}
		}
		fromV += this.posFix;
		fromV.y += fixY;
		if (to == null)
		{
			fromV += this.randomRange.Random();
		}
		this.fromV = fromV;
		this.destPos = (to ?? from);
		base.transform.position = fromV;
		if (this.randomFlip)
		{
			this.sr.flipX = (EMono.rnd(2) == 0);
		}
		if (to != null)
		{
			float num2 = Mathf.Min(this.speed * (float)from.Distance(to), this.duration);
			this.destV = *to.Position() + this.posFix + this.randomRange.Random();
			if (this.rotate)
			{
				base.transform.rotation = from.GetRotation(to);
			}
			this.moveTween = base.transform.DOMove(this.destV, num2, false).SetEase(Ease.Linear).SetDelay(this.startDelay);
		}
		this.Activate();
		this.OnPlay();
		this.OnUpdate();
		return this;
	}

	public Effect Play(Vector3 v)
	{
		if (!EMono.core.IsGameStarted)
		{
			this.Kill();
			return null;
		}
		this.fromV = v;
		base.transform.position = this.fromV + this.posFix;
		this.Activate();
		this.OnPlay();
		this.OnUpdate();
		return this;
	}

	protected void Activate()
	{
		if (this.sprites.Length != 0 && this.type != Effect.Type.Firework)
		{
			this.sr.enabled = false;
		}
		Effect.manager.Add(this);
		this.killTimer = TweenUtil.Tween(this.startDelay + this.duration + 0.01f + (float)this.sprites.Length * 0.01f, null, new Action(this.Kill));
	}

	public Effect Flip(bool x = false, bool y = false)
	{
		if (this.sr)
		{
			this.sr.flipX = (this.sr.flipX ? (!x) : x);
			this.sr.flipY = (this.sr.flipY ? (!y) : y);
		}
		return this;
	}

	public Effect SetStartDelay(float a)
	{
		this.startDelay = a;
		return this;
	}

	public virtual void OnPlay()
	{
	}

	public virtual void OnUpdate()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
		}
		this.timer += (this.randomSpeed ? UnityEngine.Random.Range(Time.deltaTime, Time.deltaTime * 1.5f) : Time.deltaTime);
		if (this.startDelay != 0f)
		{
			if (this.timer < this.startDelay)
			{
				return;
			}
			this.timer = (this.startDelay = 0f);
		}
		if (this.sprites.Length != 0 && this.type != Effect.Type.Firework && this.timer > this.duration / (float)this.sprites.Length)
		{
			this.timer = 0f;
			this.spriteIndex++;
			if (this.spriteIndex >= this.sprites.Length)
			{
				this.Kill();
				return;
			}
			this.sr.enabled = true;
			this.sr.sprite = this.sprites[this.spriteIndex];
		}
	}

	public void Kill()
	{
		TweenUtil.KillTween(ref this.killTimer, false);
		TweenUtil.KillTween(ref this.moveTween, false);
		this.killed = true;
		Effect.manager.Remove(this);
		if (this.pool && Effect.manager.effects.usePool)
		{
			PoolManager.DespawnOrDestroy(base.transform);
			return;
		}
		if (base.gameObject)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void OnDisable()
	{
		if (base.transform.parent && !this.test)
		{
			this.Kill();
		}
	}

	public void OnDestroy()
	{
		if (this.materialsToDestroy != null)
		{
			foreach (Material obj in this.materialsToDestroy)
			{
				UnityEngine.Object.Destroy(obj);
			}
		}
	}

	public Effect Emit(int num)
	{
		ParticleSystem[] array = this.systems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Emit(num);
		}
		return this;
	}

	public Effect SetParticleColor(Color c)
	{
		ParticleSystem[] array = this.systems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].main.startColor = c;
		}
		return this;
	}

	public Effect SetParticleColor(Color color, bool changeMaterial = false, string idCol = "_Color")
	{
		if (changeMaterial)
		{
			this.materialsToDestroy = new List<Material>();
		}
		foreach (ParticleSystem particleSystem in this.systems)
		{
			if (changeMaterial)
			{
				Material material = particleSystem.GetComponent<ParticleSystemRenderer>().material;
				this.materialsToDestroy.Add(material);
				material.SetColor(idCol, color);
			}
			else
			{
				particleSystem.main.startColor = color;
			}
		}
		return this;
	}

	public Effect SetScale(float a)
	{
		base.transform.localScale *= a;
		return this;
	}

	public Effect.Type type;

	public float duration = 1f;

	public float speed;

	public float startDelay;

	public bool lookAtTarget;

	public bool rotate;

	public bool pool;

	public SpriteRenderer sr;

	public ParticleSystem[] systems;

	public Vector3[] dirs;

	public Ease ease;

	public Vector3 posFix;

	public Vector3 randomRange;

	public Tween moveTween;

	public Sprite[] sprites;

	public bool randomFlip;

	public bool randomSpeed;

	public bool test;

	public bool setColor = true;

	public Action onComplete;

	[NonSerialized]
	public int spriteIndex;

	[NonSerialized]
	public float timer;

	[NonSerialized]
	public Vector3 fromV;

	[NonSerialized]
	public Vector3 destV;

	[NonSerialized]
	public bool pooled;

	[NonSerialized]
	public Transform poolParent;

	[NonSerialized]
	public List<Material> materialsToDestroy;

	[NonSerialized]
	public Point destPos;

	protected bool killed;

	[NonSerialized]
	public Tween killTimer;

	public enum Type
	{
		Default,
		Firework
	}
}
