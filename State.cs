using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    protected PlayerController player;
    protected Rigidbody2D rb;
    protected SoundManager sound;

    [SerializeField] protected string soundLabel = "";

    /// <summary>
    /// Initialize the State with references.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="rb"></param>
    public virtual void Init(PlayerController player, Rigidbody2D rb)
    {
        this.player = player;
        this.rb = rb;
        this.sound = player.soundManager;
    }


    /// <summary>
    /// State's behaviour each frame.
    /// </summary>
    public abstract void Tick();


    /// <summary>
    /// Runs when the State is switched to.
    /// </summary>
    /// <param name="fromState">State from which this State is being called.</param>
    public virtual void OnEnter(State fromState)
    {
        sound.Play(soundLabel);
    }


    /// <summary>
    /// Runs when the State is switched out of.
    /// </summary>
    /// <param name="toState">State which will become the active State.</param>
    public abstract void OnExit(State toState);


    /// <summary>
    /// Baseline Jump behaviour; costs boost.
    /// </summary>
    protected virtual void Jump()
    {
        player.ConsumeBoost(player.jumpBoostCost);
        player.SetAnimBool("Grounded", false);
        sound.Play("jump");
    }
}
