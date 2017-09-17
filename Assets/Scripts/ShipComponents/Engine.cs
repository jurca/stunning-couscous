using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipComponents
{
	public class Engine : MonoBehaviour
	{
		public ParticleSystem exhaust;
		public float minEmissionRate;
		public float maxEmissionRate;
		public float minParticleLifetime;
		public float maxParticleLifetime;
		public float minSpeed;
		public float maxSpeed;

		[HideInInspector] public float power;

		ParticleSystem.MainModule mainParticleModule;
		ParticleSystem.EmissionModule emissionModule;

		void Awake ()
		{
			mainParticleModule = exhaust.main;
			emissionModule = exhaust.emission;
		}

		void Update ()
		{
			emissionModule.rateOverTime = power * (maxEmissionRate - minEmissionRate) + minEmissionRate;
			mainParticleModule.startLifetime = power * (maxParticleLifetime - minParticleLifetime) + minParticleLifetime;
			mainParticleModule.startSpeed = power * (maxSpeed - minSpeed) + minSpeed;
		}
	}
}
