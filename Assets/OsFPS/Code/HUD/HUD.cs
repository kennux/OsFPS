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

        [Header("Health")]
        public Text healthText;
        public Image healthFill;

        [Header("Ammo")]
        public Text ammoInClipText;
        public Text ammoText;

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
            // Health
            this.healthText.text = LocalPlayer.health.ToString("0.0") + " / " + LocalPlayer.maxHealth.ToString();
            this.healthFill.fillAmount = LocalPlayer.health / LocalPlayer.maxHealth;

            // Ammo
            this.ammoInClipText.text = LocalPlayer.GetCurrentWeaponAmmoInClip().ToString();
            this.ammoText.text = LocalPlayer.GetCurrentWeaponAmmo().ToString();

            // Reticle
            this.reticle.gameObject.SetActive(true);
            switch (this.reticleState)
            {
                case ReticleState.Crosshair: this.reticle.sprite = this.crosshairReticle; break;
                case ReticleState.Interaction: this.reticle.sprite = this.interactReticle; break;
                case ReticleState.None: this.reticle.gameObject.SetActive(false); break;
            }
        }
    }
}