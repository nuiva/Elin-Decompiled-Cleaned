using System;
using DG.Tweening;
using UnityEngine;

public class SpriteBasedLaser : MonoBehaviour
{
	public delegate void LaserHitTriggerHandler(RaycastHit2D hitInfo);

	public Material mat;

	public Vector3 dest;

	public Vector3 fixTip;

	public Vector3 dest2;

	public Vector3 fix;

	public Vector3 fix2;

	public float arcNum;

	public float arcNum2;

	public DOTweenAnimation[] anime;

	public LineRenderer laserLineRendererArc;

	public int laserArcSegments = 20;

	public RandomPositionMover laserOscillationPositionerScript;

	public bool oscillateLaser;

	public float maxLaserLength = 20f;

	public float oscillationSpeed = 1f;

	public GameObject targetGo;

	public ParticleSystem hitSparkParticleSystem;

	public float laserArcMax;

	public float maxLaserRaycastDistance;

	public bool laserRotationEnabled;

	public bool lerpLaserRotation;

	public float turningRate = 3f;

	public float collisionTriggerInterval = 0.25f;

	public LayerMask mask;

	public bool useArc;

	public float oscillationThreshold = 0.2f;

	private GameObject gameObjectCached;

	private float laserAngle;

	private float lerpYValue;

	private float startLaserLength;

	public Transform trail;

	public GameObject startGoPiece;

	public GameObject middleGoPiece;

	public GameObject endGoPiece;

	private float startSpriteWidth;

	private bool waitingForTriggerTime;

	private ParticleSystem.EmissionModule hitSparkEmission;

	public event LaserHitTriggerHandler OnLaserHitTriggered;

	public void Play(Vector3 to)
	{
		base.transform.position = base.transform.position.SetZ(0f);
		dest2 = (dest = to.SetZ(0f) + fix + new Vector3(0.2f, 0.2f, 0f).Random());
		hitSparkEmission = hitSparkParticleSystem.emission;
		gameObjectCached = base.gameObject;
		if (laserLineRendererArc != null)
		{
			laserLineRendererArc.SetVertexCount(laserArcSegments);
		}
		startLaserLength = maxLaserLength;
		if (laserOscillationPositionerScript != null)
		{
			laserOscillationPositionerScript.radius = oscillationThreshold;
		}
		Update();
		Update();
		trail.localScale = middleGoPiece.transform.localScale;
		trail.rotation = middleGoPiece.transform.rotation;
		trail.position = middleGoPiece.transform.position;
		DOTweenAnimation[] array = anime;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DOPlay();
		}
	}

	private void OscillateLaserParts(float currentLaserDistance)
	{
		if (!(laserOscillationPositionerScript == null))
		{
			lerpYValue = Mathf.Lerp(middleGoPiece.transform.localPosition.y, laserOscillationPositionerScript.randomPointInCircle.y, Time.deltaTime * oscillationSpeed);
			if (startGoPiece != null && middleGoPiece != null)
			{
				Vector2 vector = Vector2.Lerp(b: new Vector2(startGoPiece.transform.localPosition.x, laserOscillationPositionerScript.randomPointInCircle.y), a: startGoPiece.transform.localPosition, t: Time.deltaTime * oscillationSpeed);
				startGoPiece.transform.localPosition = vector;
				Vector2 vector2 = new Vector2(currentLaserDistance / 2f + startSpriteWidth / 4f, lerpYValue);
				middleGoPiece.transform.localPosition = vector2;
			}
			if (endGoPiece != null)
			{
				Vector2 vector3 = new Vector2(currentLaserDistance + startSpriteWidth / 2f, lerpYValue);
				endGoPiece.transform.localPosition = vector3;
			}
		}
	}

	private void SetLaserArcVertices(float distancePoint, bool useHitPoint)
	{
		for (int i = 1; i < laserArcSegments; i++)
		{
			float y = Mathf.Clamp(Mathf.Sin((float)i + Time.time * UnityEngine.Random.Range(0.5f, 1.3f)), 0f - laserArcMax + UnityEngine.Random.RandomRange(0f, laserArcMax * 0.5f), laserArcMax + UnityEngine.Random.RandomRange(0f, laserArcMax * 0.5f));
			Vector2 vector = new Vector2((float)i * 1.2f * arcNum2, y);
			if (useHitPoint && i == laserArcSegments - 1)
			{
				laserLineRendererArc.SetPosition(i, new Vector2(distancePoint, 0f));
			}
			else
			{
				laserLineRendererArc.SetPosition(i, vector);
			}
		}
	}

	private void Update()
	{
		if (!(gameObjectCached != null))
		{
			return;
		}
		middleGoPiece.transform.localScale = new Vector3(maxLaserLength - startSpriteWidth + 0.2f, middleGoPiece.transform.localScale.y, middleGoPiece.transform.localScale.z);
		if (oscillateLaser)
		{
			OscillateLaserParts(maxLaserLength);
		}
		else
		{
			if (middleGoPiece != null)
			{
				middleGoPiece.transform.localPosition = new Vector2(maxLaserLength / 2f + startSpriteWidth / 4f, lerpYValue);
			}
			if (endGoPiece != null)
			{
				endGoPiece.transform.localPosition = new Vector3(maxLaserLength + startSpriteWidth / 2f, 0f, 0f) + fixTip;
			}
		}
		if (laserRotationEnabled)
		{
			Vector3 vector = dest2 - gameObjectCached.transform.position;
			laserAngle = Mathf.Atan2(vector.y, vector.x);
			if (laserAngle < 0f)
			{
				laserAngle = MathF.PI * 2f + laserAngle;
			}
			float angle = laserAngle * 57.29578f;
			if (lerpLaserRotation)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.AngleAxis(angle, base.transform.forward), Time.deltaTime * turningRate);
			}
			else
			{
				base.transform.rotation = Quaternion.AngleAxis(angle, base.transform.forward);
			}
		}
		maxLaserLength = Vector2.Distance(dest, base.transform.position) + startSpriteWidth / 4f + fix2.x;
		if (hitSparkParticleSystem != null)
		{
			hitSparkParticleSystem.transform.position = dest;
			hitSparkEmission.enabled = true;
		}
		if (useArc)
		{
			if (!laserLineRendererArc.enabled)
			{
				laserLineRendererArc.enabled = true;
			}
			SetLaserArcVertices(maxLaserLength, useHitPoint: true);
			SetLaserArcSegmentLength();
		}
		else if (laserLineRendererArc.enabled)
		{
			laserLineRendererArc.enabled = false;
		}
	}

	private void SetLaserArcSegmentLength()
	{
		int vertexCount = Mathf.Abs((int)maxLaserLength) * (int)arcNum;
		laserLineRendererArc.SetVertexCount(vertexCount);
		laserArcSegments = vertexCount;
	}
}
