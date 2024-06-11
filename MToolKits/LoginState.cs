using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[GameState(0)]
public class LoginState : GameState
{
    public override void OnEnter()
    {
        app.EmitMessage(8, "");
    }
}
