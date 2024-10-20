using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected CharacterMovement character;

    public State(CharacterMovement character)
    {
        this.character = character;
    }

    public virtual void Enter() { }
    public virtual void HandleInput() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
    public virtual void CustomerHit() { }
}
