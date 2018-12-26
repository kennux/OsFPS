using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityTK;

namespace OsFPS
{
    public enum ReticleState
    {
        None,
        Crosshair,
        Interaction
    }

    public class HUD : MonoBehaviour
    {
        public static HUD instance { get { return UnitySingleton<HUD>.Get(); } }

		public string HealthText
		{
			get { return string.Format("{0} / {1}", LocalPlayer.health.ToString("0"), LocalPlayer.maxHealth.ToString("0")); }
		}

		public float HealthFill
		{
			get { return LocalPlayer.health / LocalPlayer.maxHealth; }
		}

		public string AmmoInClipText
		{
			get { return LocalPlayer.GetCurrentWeaponAmmoInClip().ToString(); }
		}

		public string AmmoText
		{
			get { return LocalPlayer.GetCurrentWeaponAmmo().ToString(); }
		}

        [Header("Reticle")]
        public Image reticle;
        public ReticleState reticleState;
        public Sprite crosshairReticle;
        public Sprite interactReticle;

        public void SetReticlePosition(Vector2 pos)
        {
            // Anchored in center
            pos.x = pos.x - (Screen.width / 2f);
            pos.y = pos.y - (Screen.height / 2f);

            this.reticle.rectTransform.anchoredPosition = pos;
        }

        public void Awake()
        {
            UnitySingleton<HUD>.Register(this);
        }

        public void Update()
        {
			// Reticle
			bool reticleActive = true;
            switch (this.reticleState)
            {
                case ReticleState.Crosshair: this.reticle.sprite = this.crosshairReticle; break;
                case ReticleState.Interaction: this.reticle.sprite = this.interactReticle; break;
                case ReticleState.None: reticleActive = false; break;
            }

			if (this.reticle.gameObject.activeSelf != reticleActive)
				this.reticle.gameObject.SetActive(reticleActive);
        }
    }
}