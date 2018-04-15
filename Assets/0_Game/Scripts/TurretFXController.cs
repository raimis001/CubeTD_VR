using UnityEngine;
using Forge3D;
using System.Collections;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using game.tools;

public class RaycastBullet
{
	public Vector3 point;
	public Vector3 normal;
	public Rigidbody rigidbody;
	public Vector3 forward;
	public Transform transform;

	public RaycastBullet()
	{

	}
	public RaycastBullet(RaycastHit hit)
	{
		Cast(hit);
	}

	private void Cast(RaycastHit hit)
	{
		point = hit.point;
		normal = hit.normal;
		rigidbody = hit.rigidbody;
		transform = hit.transform;
	}
}

[Serializable]
public class TurretPrefabs
{
	public string turretName;
	public F3DFXType turretType;

	[Header("Projectiles")]
	public Transform projectile; // Projectile prefab
	public Transform muzzle; // Muzzle flash prefab  
	public Transform impact; // Impact prefab
	public float offset;
	[Header("Audio")]
	public AudioClip[] audioHit; // Impact prefabs array  
	public AudioClip audioShot; // Shot prefab
	public float audioDistance;
	[Header("Settings")]
	[Range(0, 1f)]
	public float hitpoints;
	[Range(0, 1f)]
	public float burst = 1;
	public float force;
	public float shotDelay;
	public bool loop;

	public AudioClip audioHitRandom
	{
		get { return audioHit.Length == 0 ? null : audioHit[Random.Range(0, audioHit.Length)]; }
	}
}

public class TurretShot
{
	public Transform[] sockets;
	public TurretPrefabs prefab;


	private int currentSoccket = 0;

	private bool isFire;
	private bool stopFire;

	private Transform loopProjectile;

	public void Fire()
	{
		if (isFire) return;
		isFire = true;
		stopFire = false;
		Shot();
	}

	public void Shot()
	{
		if (prefab.muzzle && sockets[currentSoccket])
		{
			PoolManager.Spawn(prefab.muzzle.gameObject, sockets[currentSoccket]);
		}

		if (prefab.projectile)
		{
			Quaternion offsetSphere = Quaternion.Euler(Random.onUnitSphere * prefab.burst);
			GameObject projectile = PoolManager.Spawn(
					prefab.projectile.gameObject,
					sockets[currentSoccket].position + sockets[currentSoccket].forward,
					offsetSphere * sockets[currentSoccket].rotation
			);
			if (projectile)
			{
				Bullet bullet = projectile.GetComponent<Bullet>();
				if (bullet) bullet.ImpactAction = Hit;
			}
			PlaySound(prefab.audioShot, sockets[currentSoccket].position, prefab.loop ? loopProjectile : null, prefab.loop);
		}

		if (prefab.loop && prefab.audioHit.Length > 0)
		{
			PlaySound(prefab.audioHit[0], sockets[currentSoccket].position, null, false);
		}

		++currentSoccket;
		if (currentSoccket >= sockets.Length) currentSoccket = 0;

		if (!prefab.loop) TurretFXController.instance.StartCoroutine(Wait());
	}
	public void Stop()
	{
		if (!isFire) return;

		stopFire = true;

		if (prefab.loop)
		{
			F3DPoolManager.Pools["GeneratedPool"].Despawn(loopProjectile);
			isFire = false;
			if (prefab.audioHit.Length > 1)
			{
				PlaySound(prefab.audioHit[1], sockets[currentSoccket].position, null, false);
			}

		}
	}

	void Hit(RaycastBullet hitPoint)
	{
		Vector3 pos = hitPoint.point + hitPoint.normal * prefab.offset;

		GameObject hit = null;
		if (prefab.impact)
		{
			hit = PoolManager.Spawn(prefab.impact.gameObject, pos, Quaternion.identity);
		}

		if (!prefab.loop && hit)
		{
			PlaySound(prefab.audioHitRandom, pos, null, false);
		}

		if (prefab.force > 0 && hitPoint.rigidbody != null)
			hitPoint.rigidbody.AddForceAtPosition(hitPoint.forward * prefab.force, hitPoint.point, ForceMode.VelocityChange);

		if (!hitPoint.transform) return;

		Enemy enemy = hitPoint.transform.gameObject.GetComponent<Enemy>();
		if (!enemy) return;

		enemy.Hit(prefab.hitpoints);
	}

	IEnumerator Wait()
	{
		yield return new WaitForSeconds(prefab.shotDelay);

		if (!stopFire && !prefab.loop)
		{
			Shot();
		}
		else
		{
			isFire = false;
		}
	}

	void PlaySound(AudioClip clip, Vector3 pos, Transform parent, bool loop)
	{
		if (!clip) return;
		return;

		// Play impact sound effect
		Transform obj = F3DPoolManager.Pools["GeneratedPool"].SpawnAudio(TurretFXController.instance.audioSource, clip, pos, parent);

		if (!obj) return;

		AudioSource aSrc = obj.GetComponent<AudioSource>();

		if (aSrc == null) return;

		aSrc.pitch = Random.Range(0.95f, 1f);
		aSrc.volume = Random.Range(0.6f, 1f);
		aSrc.minDistance = prefab.audioDistance;
		aSrc.loop = loop;
		aSrc.Play();


	}
}

public class TurretFXController : MonoBehaviour
{

	// Singleton instance
	public static TurretFXController instance;

	public Transform audioSource;
	public List<TurretPrefabs> turretsSetup;

	private static readonly List<TurretShot> turrets = new List<TurretShot>();

	void Awake()
	{
		instance = this;
	}

	public int AddTurret(F3DFXType turret, Transform[] socket)
	{
		TurretShot shot = new TurretShot();
		shot.prefab = GetPrefab(turret);
		shot.sockets = socket;

		turrets.Add(shot);
		return turrets.Count - 1;
	}

	public void Fire(int turretID)
	{
		turrets[turretID].Fire();
	}
	public void Stop(int turretID)
	{
		turrets[turretID].Stop();
	}

	TurretPrefabs GetPrefab(F3DFXType turret)
	{
		foreach (TurretPrefabs prefab in turretsSetup)
		{
			if (prefab.turretType != turret) continue;
			return prefab;
		}
		return null;
	}
}