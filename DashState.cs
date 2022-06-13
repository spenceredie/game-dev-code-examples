using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : State
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gracePeriodDur;
    [SerializeField] private float boostDrainRate;
    [SerializeField] private float friction;
    [SerializeField] private Texture rightTexture;
    [SerializeField] private Texture leftTexture;
    [SerializeField] private Texture upTexture;
    [SerializeField] private Material afterimageMat;

    private float gracePeriodTimer;
    public bool jumping = false;

    public override void Init(PlayerController player, Rigidbody2D rb)
    {
        base.Init(player, rb);
    }

    public override void OnEnter(State fromState)
    {
        base.OnEnter(fromState);
        gracePeriodTimer = gracePeriodDur;
        if (player.boostLeft < 0)
        {
            player.boostLeft = 0;
            player.Exhaust();
            return;
        }
        player.ToggleAfterimage(true);
        rb.gravityScale = 0;
        player.SetAnimBool("Dash", true);
    }

    public override void OnExit(State toState)
    {
        rb.gravityScale = 1;
        jumping = false;
        player.ToggleAfterimage(false);
        player.SetAnimBool("Dash", false);
    }

    public override void Tick()
    {
        InputGetter.Inputs inputs = player.inputs;

        CostBoost();
        player.SetAnimFloat("InputDir", inputs.dir);
        UpdateParticleSystem(inputs);
        HandleMovement(inputs);
        HandleInputs(inputs);
    }
    /// <summary>
    /// Performs super jump and exits state without clunk
    /// </summary>
    protected override void Jump()
    {
        base.Jump();
        rb.velocity -= Vector2.up * rb.velocity.y;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumping = true;
        player.Normalize();
    }
    private void HandleInputs(InputGetter.Inputs inputs)
    {
        if (inputs.jump)
        {
            Jump();
            return;
        }
        if (inputs.dive)
        {
            player.Dive();
        }
        else if (inputs.step)
        {
            player.Step();
        }
        else if (inputs.dashUp)
        {
            player.Normalize();
        }
    }
    private void HandleMovement(InputGetter.Inputs inputs)
    {
        if (inputs.up)
        {
            rb.velocity = transform.up * speed;
        }
        else
        {
            if (Mathf.Abs(rb.velocity.x) > speed)
            {
                if (rb.velocity.x * inputs.dir <= 0)
                {
                    rb.velocity = transform.right * inputs.dir * speed;
                }
                else
                {
                    float x = rb.velocity.x - (friction * Time.deltaTime * Mathf.Sign(rb.velocity.x));
                    rb.velocity = transform.right * x;
                }
            }
            else
            {
                rb.velocity = transform.right * inputs.dir * speed;
            }
        }
    }
    private void UpdateParticleSystem(InputGetter.Inputs inputs)
    {
        if (inputs.up)
        {
            afterimageMat.mainTexture = upTexture;
        }
        else if (inputs.dir == 1)
        {
            afterimageMat.mainTexture = rightTexture;
        }
        else if (inputs.dir == -1)
        {
            afterimageMat.mainTexture = leftTexture;
        }

    }
    private void CostBoost()
    {
        if (gracePeriodTimer > 0)
        {
            gracePeriodTimer -= Time.deltaTime;
        }
        else
        {
            player.ConsumeBoost(boostDrainRate * Time.deltaTime);
        }
    }

}
