using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image healthBarSprite;
    [SerializeField] private Image abilityBarSprite;

    [SerializeField] float reduceSpeed = 2;

    private float h_target = 1;
    private float a_target = 1;

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        h_target = currentHealth / maxHealth;
    }

    public void UpdateAbilityBar(float maxAbilityAmt, float currentAbilityAmt)
    {
        a_target = currentAbilityAmt / maxAbilityAmt;
    }

    private void Update()
    {
        healthBarSprite.fillAmount = Mathf.MoveTowards(healthBarSprite.fillAmount, h_target, reduceSpeed * Time.deltaTime);
        abilityBarSprite.fillAmount = Mathf.MoveTowards(abilityBarSprite.fillAmount, a_target, reduceSpeed * Time.deltaTime);
    }

}
