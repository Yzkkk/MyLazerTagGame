using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    public ScoreboardController scoreboard;
    public GunController gunController;
    public GrenadeController grenadeController;
    public ShieldController shieldController;
    public CustomAREffects care;
    public GameObject ScreenDamagePanel;

    private float health;
    private float lerpTimer;

    public float maxHealth = 100f;
    public float chipSpeed = 2f;

    // Shield properties
    public float currentShieldHP = 0;
    private const float maxShieldHP = 30;
    private int currentShields = 3; // Number of shields available to be activated

    public Image frontHealthBar;
    public Image backHealthBar;
    public Image shieldBar; // The bar representing the shield's HP

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        currentShieldHP = Mathf.Clamp(currentShieldHP, 0, maxShieldHP);
        UpdateHealthUI();
        UpdateShieldUI();
    }

    private void UpdateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;

        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        else if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }

    private void UpdateShieldUI()
    {
        float fill = shieldBar.fillAmount;
        float sFraction = currentShieldHP / maxShieldHP;

        if (fill > sFraction)
        {
            // Decrease shield bar smoothly
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;  // Quadratic interpolation for smoother transition
            shieldBar.fillAmount = Mathf.Lerp(fill, sFraction, percentComplete);
        }
        else if (fill < sFraction)
        {
            // Increase shield bar smoothly
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;  // Quadratic interpolation for smoother transition
            shieldBar.fillAmount = Mathf.Lerp(fill, sFraction, percentComplete);
        }
    }

    public void BulletShotReceived()
    {
        StartCoroutine(TakeDamage(10f, 0f));
    }

    public void AttackReceived()
    {
        StartCoroutine(TakeDamage(10f, 1f));
    }

    public void SpearReceived()
    {
        StartCoroutine(TakeDamage(10f, 1f));
    }

    public void GrenadeReceived()
    {
        StartCoroutine(TakeDamage(30f, 2f));
    }

    public void HammerReceived()
    {
        StartCoroutine(TakeDamage(10f, 1f));
    }

    public void PunchReceived()
    {
        StartCoroutine(TakeDamage(10f, 0f));
    }

    public void PortalReceived()
    {
        StartCoroutine(TakeDamage(10f, 0f));
    }

    public IEnumerator TakeDamage(float damage, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (currentShieldHP > 0)
        {
            currentShieldHP -= damage;

            if (currentShieldHP <= 0)
            {
                float overflowDamage = -currentShieldHP;
                health -= overflowDamage;
                currentShieldHP = 0;
                care.RemovePlayerShield();
            }
        }
        else
        {
            health -= damage;
        }

        lerpTimer = 0f;

        StartCoroutine(ShowDamagePanelTemporarily());

        if (health <= 0)
        {
            Respawn();
        }
    }

    /*private void UpdateBloodEffectOpacity(float damage)
    {
        float alphaValue = 0f;

        if (damage >= 30)
            alphaValue = 255 / 255f; // Since Color.a accepts values between 0 and 1
        else if (damage >= 10)
            alphaValue = 150 / 255f;

        Image bloodEffectImage = ScreenDamagePanel.GetComponent<Image>();
        Color tempColor = bloodEffectImage.color;
        tempColor.a = alphaValue;
        bloodEffectImage.color = tempColor;
    }*/


    private IEnumerator ShowDamagePanelTemporarily()
    {
        ScreenDamagePanel.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        ScreenDamagePanel.SetActive(false);
    }


    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

    public void HealthBooster()
    {
        RestoreHealth(20f);  // restore 20 HP with health booster
    }

    public void ActivateShield()
    {
        if (currentShields > 0)
        {
            currentShieldHP = maxShieldHP;
            currentShields--;
        }
    }

    void Respawn()
    {
        health = maxHealth;
        scoreboard.OnPlayerDeath();
        ResetPlayerStats();
    }

    void ResetPlayerStats()
    {
        health = maxHealth;
        gunController.ResetAmmo();
        grenadeController.ResetGrenades();
        shieldController.ResetShields();
    }

    //dummy method to simulate the correction of game state elements from eval server
    public void RandomisePlayerStats()
    {
        // Randomize health between some range, for example between half and full health
        health = Random.Range(0, maxHealth);

        // Randomize bullet count
        gunController.bulletCount = Random.Range(0, 7); // 7 because the max is exclusive
        gunController.UpdateBulletText();

        grenadeController.grenadeCount = Random.Range(0, 3);
        grenadeController.UpdateGrenadeText();

        shieldController.shieldCount = Random.Range(0, 4);
        shieldController.UpdateShieldText();

        StartCoroutine(TakeDamage(0f, 0f));
    }

    public void SetHealth(int x)
    {
        if (health > x)
        {
            StartCoroutine(TakeDamage(health - x, 0f));
        }
        else
        {
            health = x;
        }
    }

    public void SetShieldHp(int x)
    {
        if (currentShieldHP > x)
        {
            StartCoroutine(TakeDamage(currentShieldHP - x, 0f));
        }
        else
        {
            currentShieldHP = x;
        }
    }

}
