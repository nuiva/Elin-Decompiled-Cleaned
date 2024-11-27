using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HoardActor : EMono
{
	public Hoard hoard
	{
		get
		{
			return EMono.player.hoard;
		}
	}

	public HoardActor.BG bg
	{
		get
		{
			return this.bgs[this.hoard.bg];
		}
	}

	public void Activate()
	{
		this.passShadow.Init();
		this.hoard.score = 0;
		this.RefreshBG();
		this.RefreshZoom();
		RigidUpdate.leftX = -5f;
		RigidUpdate.rightX = 5f;
		if (this.actors.Count > 0)
		{
			this.Clear();
		}
		this.angryEhe = (EMono.rnd(2) == 0);
		Vector3 zero = Vector3.zero;
		int num = 0;
		foreach (Hoard.Item item in this.hoard.items.Values)
		{
			num += item.show;
		}
		IEnumerable<Hoard.Item> enumerable = new List<Hoard.Item>(this.hoard.items.Values).Shuffle<Hoard.Item>();
		string b = this.hoard.mode.ToString();
		foreach (Hoard.Item item2 in enumerable)
		{
			string filter = item2.Source.filter;
			if (this.hoard.mode == Hoard.Mode.all || filter.IsEmpty() || !(filter != b))
			{
				bool flag = item2.Source.tag.Contains("god");
				int num2 = item2.show;
				if (item2.random)
				{
					num2 = Rand.rnd(num2 + 1);
					if (num2 > 2 && num2 < 50)
					{
						num2 = Rand.rnd(num2 + 1) + 1;
					}
				}
				if (num2 != 0)
				{
					GameObject actor = this.hoard.GetActor(item2.id);
					CollectibleActor component = actor.GetComponent<CollectibleActor>();
					SpriteRenderer component2 = actor.GetComponent<SpriteRenderer>();
					Vector3 extents = component2.bounds.extents;
					int num3 = (int)(extents.x * extents.y * 10f);
					if (num3 > 5)
					{
						num3 = 5;
					}
					float y = extents.y;
					float z = flag ? -0.1f : 0f;
					bool hasSound = !item2.Source.sound.IsEmpty();
					if (this.debug)
					{
						string id = item2.id;
						string str = "/";
						Vector3 vector = extents;
						Debug.Log(id + str + vector.ToString());
					}
					float num4 = Mathf.Sqrt(actor.GetComponent<Rigidbody2D>().mass);
					if (num4 < 0.7f)
					{
						num4 = 0.7f;
					}
					BoxCollider2D component3 = actor.GetComponent<BoxCollider2D>();
					if (component3)
					{
						component3.size = extents * this.sizeMod;
					}
					else
					{
						actor.GetComponent<CircleCollider2D>().radius = (extents.x + extents.y) * this.sizeModCircle * this.sizeModCircle;
					}
					string id2 = item2.id;
					uint num5 = <PrivateImplementationDetails>.ComputeStringHash(id2);
					if (num5 <= 2580362002U)
					{
						if (num5 <= 1166849410U)
						{
							if (num5 <= 622086627U)
							{
								if (num5 != 595338019U)
								{
									if (num5 != 622086627U)
									{
										goto IL_687;
									}
									if (!(id2 == "renton"))
									{
										goto IL_687;
									}
									this.renton = component;
									if (!this.angryEhe)
									{
										Util.Instantiate<SpriteRenderer>("UI/Layer/Hoard/Effect/sister", component);
										component.paired = true;
										this.MakeSteady(component);
										goto IL_687;
									}
									goto IL_687;
								}
								else if (!(id2 == "mine_dog"))
								{
									goto IL_687;
								}
							}
							else if (num5 != 1111712579U)
							{
								if (num5 != 1166849410U)
								{
									goto IL_687;
								}
								if (!(id2 == "hat_magic"))
								{
									goto IL_687;
								}
								goto IL_67F;
							}
							else
							{
								if (!(id2 == "tentacle"))
								{
									goto IL_687;
								}
								goto IL_676;
							}
						}
						else if (num5 <= 2122968502U)
						{
							if (num5 != 1891070888U)
							{
								if (num5 != 2122968502U)
								{
									goto IL_687;
								}
								if (!(id2 == "mine"))
								{
									goto IL_687;
								}
							}
							else
							{
								if (!(id2 == "bat"))
								{
									goto IL_687;
								}
								goto IL_67F;
							}
						}
						else if (num5 != 2273286343U)
						{
							if (num5 != 2580362002U)
							{
								goto IL_687;
							}
							if (!(id2 == "mine_rock"))
							{
								goto IL_687;
							}
						}
						else
						{
							if (!(id2 == "ehe"))
							{
								goto IL_687;
							}
							this.ehe = component;
							goto IL_687;
						}
						actor.AddComponent<RigidExplode>().particle = this.psExplode;
					}
					else if (num5 <= 3274701990U)
					{
						if (num5 <= 3222636880U)
						{
							if (num5 != 2973714812U)
							{
								if (num5 == 3222636880U)
								{
									if (id2 == "tentacle3")
									{
										goto IL_676;
									}
								}
							}
							else if (id2 == "pet_mani")
							{
								RigidExplode rigidExplode = actor.AddComponent<RigidExplode>();
								rigidExplode.particle = this.psExplode;
								rigidExplode.chance = 0.1f;
								rigidExplode.intervalMin *= 2f;
							}
						}
						else if (num5 != 3239414499U)
						{
							if (num5 == 3274701990U)
							{
								if (id2 == "sis")
								{
									if (!this.hoard.voice)
									{
										hasSound = false;
									}
								}
							}
						}
						else if (id2 == "tentacle2")
						{
							goto IL_676;
						}
					}
					else if (num5 <= 3516364402U)
					{
						if (num5 != 3340080213U)
						{
							if (num5 == 3516364402U)
							{
								if (id2 == "mani")
								{
									if (EMono.rnd(2) == 0)
									{
										actor.AddComponent<RigidAngle>();
									}
									if (EMono.rnd(2) == 0)
									{
										component2.material = this.matMani;
										UnityEngine.Object.DestroyImmediate(actor.GetComponent<RigidExplode>());
									}
									else
									{
										RigidExplode rigidExplode2 = actor.AddComponent<RigidExplode>();
										rigidExplode2.particle = this.psExplode;
										rigidExplode2.radius *= 10f;
										rigidExplode2.force *= 10f;
										rigidExplode2.chance = 0.1f;
										rigidExplode2.intervalMin *= 2f;
									}
								}
							}
						}
						else if (id2 == "tentacle4")
						{
							goto IL_676;
						}
					}
					else if (num5 != 3610891294U)
					{
						if (num5 == 4049731356U)
						{
							if (id2 == "chicken")
							{
								actor.AddComponent<RigidUpdateSound>();
							}
						}
					}
					else if (id2 == "hat_feather")
					{
						goto IL_67F;
					}
					IL_687:
					if (item2.floating)
					{
						actor.AddComponent<RigidFloat>();
					}
					component.updates = actor.GetComponents<RigidUpdate>();
					for (int i = 0; i < num2; i++)
					{
						object obj = (i == 0) ? actor : UnityEngine.Object.Instantiate<GameObject>(actor);
						float num6 = this.radiusX;
						zero.x = this.posCenter.x + UnityEngine.Random.Range(-num6, num6);
						zero.y = this.posCenter.y + UnityEngine.Random.Range(0f, this.radiusY + 0.1f * (float)num) / num4;
						zero.z = z;
						object obj2 = obj;
						obj2.transform.position = zero;
						CollectibleActor component4 = obj2.GetComponent<CollectibleActor>();
						Rigidbody2D component5 = obj2.GetComponent<Rigidbody2D>();
						component4.item = item2;
						component4.hasSound = hasSound;
						component4.rb = component5;
						this.actors.Add(component4);
						if (component4.updates.Length != 0)
						{
							foreach (RigidUpdate rigidUpdate in component4.updates)
							{
								rigidUpdate.rb = component5;
								this.updates.Add(rigidUpdate);
							}
						}
						component4.shadow = num3;
						component4.shadowY = y;
					}
					continue;
					IL_676:
					z = -0.05f;
					goto IL_687;
					IL_67F:
					actor.AddComponent<RigidAngle>();
					goto IL_687;
				}
			}
		}
		this.actors.Shuffle<CollectibleActor>();
		IEnumerable<CollectibleActor> enumerable2 = from a in this.actors
		where a.item.Source.tag.Contains("god")
		select a;
		IEnumerable<CollectibleActor> enumerable3 = from a in this.actors
		where a.item.Source.tag.Contains("chara")
		select a;
		foreach (CollectibleActor collectibleActor in this.actors)
		{
			if (!collectibleActor.paired)
			{
				string id2 = collectibleActor.item.id;
				uint num5 = <PrivateImplementationDetails>.ComputeStringHash(id2);
				if (num5 <= 2988676833U)
				{
					if (num5 <= 1150570096U)
					{
						if (num5 != 460777894U)
						{
							if (num5 != 1111712579U)
							{
								if (num5 != 1150570096U)
								{
									goto IL_D33;
								}
								if (!(id2 == "qulu"))
								{
									goto IL_D33;
								}
								if (this.<Activate>g__FindPair|25_2(collectibleActor, "dragon", 0.3f))
								{
									this.MakeSteady(collectibleActor);
									continue;
								}
								goto IL_D33;
							}
							else if (!(id2 == "tentacle"))
							{
								goto IL_D33;
							}
						}
						else
						{
							if (!(id2 == "vesda"))
							{
								goto IL_D33;
							}
							HoardActor.<Activate>g__DisableParticle|25_3(collectibleActor, 50);
							using (IEnumerator<CollectibleActor> enumerator4 = enumerable3.GetEnumerator())
							{
								while (enumerator4.MoveNext())
								{
									CollectibleActor collectibleActor2 = enumerator4.Current;
									if (!collectibleActor2.paired && !(collectibleActor2 == collectibleActor))
									{
										this.MakePair(collectibleActor, collectibleActor2, new Vector3(0f, -0.25f, -0.01f));
										collectibleActor2.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
										collectibleActor2.GetComponent<SpriteRenderer>().flipX = false;
										break;
									}
								}
								continue;
							}
							goto IL_D2A;
						}
					}
					else if (num5 != 1382054340U)
					{
						if (num5 != 2281206494U)
						{
							if (num5 != 2988676833U)
							{
								goto IL_D33;
							}
							if (!(id2 == "jure"))
							{
								goto IL_D33;
							}
							if (this.<Activate>g__FindPair|25_2(collectibleActor, "odina", 0.3f))
							{
								this.MakeSteady(collectibleActor);
								continue;
							}
							goto IL_D33;
						}
						else
						{
							if (!(id2 == "itz"))
							{
								goto IL_D33;
							}
							if (EMono.rnd(2) == 0)
							{
								foreach (CollectibleActor collectibleActor3 in enumerable3)
								{
									if (!collectibleActor3.paired && !(collectibleActor3 == collectibleActor) && collectibleActor3.item.Source.tag.Contains("fem"))
									{
										this.MakePair(collectibleActor, collectibleActor3, new Vector3(0.02f, 0f, -0.01f));
										collectibleActor3.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
										collectibleActor3.GetComponent<SpriteRenderer>().flipX = true;
										this.MakeSteady(collectibleActor);
										break;
									}
								}
								if (collectibleActor.paired)
								{
									continue;
								}
							}
							collectibleActor.GetComponentInChildren<ParticleSystem>().SetActive(false);
							goto IL_D33;
						}
					}
					else
					{
						if (!(id2 == "lar"))
						{
							goto IL_D33;
						}
						goto IL_C47;
					}
				}
				else if (num5 <= 3340080213U)
				{
					if (num5 != 3222636880U)
					{
						if (num5 != 3239414499U)
						{
							if (num5 != 3340080213U)
							{
								goto IL_D33;
							}
							if (!(id2 == "tentacle4"))
							{
								goto IL_D33;
							}
						}
						else if (!(id2 == "tentacle2"))
						{
							goto IL_D33;
						}
					}
					else if (!(id2 == "tentacle3"))
					{
						goto IL_D33;
					}
				}
				else if (num5 != 3842579285U)
				{
					if (num5 != 3907705998U)
					{
						if (num5 != 4161811998U)
						{
							goto IL_D33;
						}
						if (!(id2 == "dragon"))
						{
							goto IL_D33;
						}
						if (this.<Activate>g__FindPair|25_2(collectibleActor, "qulu", 0.3f))
						{
							this.MakeSteady(collectibleActor);
							continue;
						}
						goto IL_D33;
					}
					else
					{
						if (!(id2 == "lomi"))
						{
							goto IL_D33;
						}
						if (this.<Activate>g__FindPair|25_2(collectibleActor, "lar", 0.15f))
						{
							this.MakeSteady(collectibleActor);
							continue;
						}
						continue;
					}
				}
				else
				{
					if (!(id2 == "lulu"))
					{
						goto IL_D33;
					}
					goto IL_D2A;
				}
				using (IEnumerator<CollectibleActor> enumerator4 = enumerable3.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						CollectibleActor collectibleActor4 = enumerator4.Current;
						if (!collectibleActor4.paired && !(collectibleActor4 == collectibleActor))
						{
							this.MakePair(collectibleActor, collectibleActor4, new Vector3(0f, 0.25f, -0.01f));
							collectibleActor.GetComponent<SpriteAnimation>().link = collectibleActor4.transform;
							if (EMono.rnd(2) == 0)
							{
								collectibleActor4.GetComponent<SpriteRenderer>().flipX = true;
							}
							if (EMono.rnd(2) == 0)
							{
								collectibleActor.transform.SetLocalScaleX(collectibleActor.transform.localScale.x * -1f);
								break;
							}
							break;
						}
					}
					continue;
				}
				IL_C47:
				if (this.<Activate>g__FindPair|25_2(collectibleActor, "lomi", 0.15f))
				{
					this.MakeSteady(collectibleActor);
					continue;
				}
				continue;
				IL_D33:
				if (EMono.rnd(2) != 0 && collectibleActor.item.Source.tag.Contains("pair"))
				{
					foreach (CollectibleActor collectibleActor5 in enumerable2)
					{
						if (!collectibleActor5.paired && !(collectibleActor5 == collectibleActor))
						{
							this.MakePair(collectibleActor, collectibleActor5, 0.26f);
							break;
						}
					}
					continue;
				}
				continue;
				IL_D2A:
				HoardActor.<Activate>g__DisableParticle|25_3(collectibleActor, 50);
				goto IL_D33;
			}
		}
		foreach (CollectibleActor collectibleActor6 in this.actors)
		{
			if (!collectibleActor6.paired && EMono.rnd(3) != 0)
			{
				collectibleActor6.GetComponent<SpriteRenderer>().flipX = true;
			}
		}
	}

	public void RefreshZoom()
	{
		if (this.hoard.pixelPerfect)
		{
			this.cam.orthographicSize = (float)Screen.height * 0.5f * 0.01f * 2f;
			return;
		}
		this.cam.orthographicSize = 5f;
	}

	public void MakeSteady(CollectibleActor a)
	{
		(a.rb ?? a.GetComponent<Rigidbody2D>()).freezeRotation = true;
	}

	public void MakePair(CollectibleActor a, CollectibleActor b, float dist = 0.26f)
	{
		float z = 0.01f;
		if (a.item.id != "lulu")
		{
			z = ((Rand.rnd(2) == 0) ? 0.01f : -0.01f);
		}
		this.MakePair(a, b, new Vector3(dist, 0f, z));
	}

	public void MakePair(CollectibleActor a, CollectibleActor b, Vector3 v)
	{
		b.transform.SetParent(a.transform, true);
		b.transform.localPosition = v;
		b.GetComponent<SpriteRenderer>().flipX = true;
		b.rb.isKinematic = true;
		b.GetComponent<Collider2D>().enabled = false;
		a.paired = (b.paired = true);
	}

	public void RefreshBG()
	{
		this.srBG.sprite = (this.srReflection.sprite = this.bg.sprite);
		this.matReflection.SetColor("_GrabColor", new Color(1f, 1f, 1f, this.bg.reflection * (0.01f * (float)this.hoard.reflection)));
		this.srReflection.SetActive(this.hoard.reflection > 0);
	}

	public void Clear()
	{
		foreach (CollectibleActor collectibleActor in this.actors)
		{
			if (collectibleActor && collectibleActor.gameObject)
			{
				UnityEngine.Object.DestroyImmediate(collectibleActor.gameObject);
			}
		}
		this.actors.Clear();
		this.updates.Clear();
	}

	private void Update()
	{
		this.RefreshZoom();
		this.matMani.SetFloat("_Space", this.curveMani.Evaluate(Time.timeSinceLevelLoad % 10f / 10f));
		float value = (this.cam.orthographicSize <= 5f) ? 0f : ((this.cam.orthographicSize - 5f) * this.reflectMod);
		this.matReflection.SetFloat("_Offset", value);
		EMono.scene.transAudio.position = new Vector3(0f, this.groundY, 0f);
		if (!this.hoard.shadow)
		{
			return;
		}
		float shadow = this.bg.shadow;
		foreach (CollectibleActor collectibleActor in this.actors)
		{
			if (collectibleActor.active)
			{
				float num = (collectibleActor.transform.position.y - this.alphaFix1) * this.alphaFix2;
				num = 1f - num * num * this.alphaFix4;
				if (num >= 0f)
				{
					if (num > this.maxAplha)
					{
						num = this.maxAplha;
					}
					num *= shadow;
					float num2 = (collectibleActor.transform.position.y + this.groundY) * 0.05f;
					if (num2 > this.shdowLimitY)
					{
						num2 = this.shdowLimitY;
					}
					this.passShadow.Add(collectibleActor.transform.position.x, collectibleActor.shadowY * this.sizeFixY + this.groundY + num2, 0.8f, (float)((int)(num * 1000f) * 1000 + collectibleActor.shadow), 0f);
				}
			}
		}
		this.passShadow.Draw();
	}

	private void FixedUpdate()
	{
		float num = RigidUpdate.delta = Time.fixedDeltaTime;
		if (this.angryEhe && this.renton && this.ehe && !this.ehe.rb.isKinematic && (Mathf.Abs(this.renton.transform.position.x - this.ehe.transform.position.x) > 0.6f || this.renton.transform.position.y > this.ehe.transform.position.y))
		{
			this.eheTimer += num;
			if (this.eheTimer > 2f)
			{
				this.ehe.rb.position = new Vector3(this.renton.transform.position.x, this.renton.transform.position.y + 4f, this.ehe.transform.position.z);
				this.ehe.PlaySound("teleport");
				this.eheTimer = 0f;
				this.eheCount++;
				if (this.eheCount > 5)
				{
					this.renton.rb.constraints = RigidbodyConstraints2D.FreezePositionX;
					this.renton.rb.gravityScale = 3f;
				}
			}
		}
		else
		{
			this.eheTimer = 0f;
		}
		foreach (RigidUpdate rigidUpdate in this.updates)
		{
			if (rigidUpdate.active)
			{
				rigidUpdate.OnFixedUpdate();
			}
		}
	}

	[CompilerGenerated]
	private bool <Activate>g__FindPair|25_2(CollectibleActor taker, string id, float dist)
	{
		foreach (CollectibleActor collectibleActor in this.actors)
		{
			if (collectibleActor.item.id == id && !collectibleActor.paired)
			{
				this.MakePair(taker, collectibleActor, dist);
				return true;
			}
		}
		return false;
	}

	[CompilerGenerated]
	internal static void <Activate>g__DisableParticle|25_3(CollectibleActor a, int chance)
	{
		if (EMono.rnd(100) < chance)
		{
			ParticleSystem[] componentsInChildren = a.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetActive(false);
			}
		}
	}

	public Camera cam;

	public Vector3 posCenter;

	public float radiusX;

	public float radiusY;

	public float sizeMod;

	public float sizeModCircle;

	public float reflectMod;

	public new bool debug;

	public List<CollectibleActor> actors;

	public List<RigidUpdate> updates;

	public MeshPass passShadow;

	public Material matReflection;

	public Material matMani;

	public AnimationCurve curveMani;

	public SpriteRenderer srBG;

	public SpriteRenderer srReflection;

	public ParticleSystem psExplode;

	public HoardActor.BG[] bgs;

	private CollectibleActor renton;

	private CollectibleActor ehe;

	private bool angryEhe;

	private float eheTimer = -5f;

	private int eheCount;

	public float groundY = -4f;

	public float shdowLimitY = 1f;

	public float alphaFix1 = 0.3f;

	public float alphaFix2 = 0.5f;

	public float alphaFix4 = 0.15f;

	public float maxAplha = 0.6f;

	public float sizeFixY = -0.1f;

	[Serializable]
	public class BG
	{
		public Sprite sprite;

		public float reflection;

		public float shadow;
	}
}
