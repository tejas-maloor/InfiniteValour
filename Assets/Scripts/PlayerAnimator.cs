using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] ThirdPersonController playerController;
    [SerializeField] Animator playerAnim;
    [SerializeField] ComboCharacter comboCharacter;

    public static bool hitWindow = false;

    // Update is called once per frame
    void Update()
    {
        playerAnim.SetFloat("Speed", playerController.Speed, .1f, Time.deltaTime);
        playerAnim.SetBool("IsGrounded", playerController.IsGrounded);        
        playerAnim.SetBool("Roll", playerController.Roll);        
    }

    public void AllowCombo()
    {
        comboCharacter.allowCombo = true;
    }

    public void HitWindowOpen()
    {
        hitWindow = true;
    }

    public void HitWindowClose()
    {
        hitWindow = false;
    }
}
