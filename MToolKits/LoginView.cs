using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SPA;
using UnityEngine.UI;

public class LoginView : UIView
{
    [HideInInspector]
    [FindChildComponent("LoginButton")]
    public Button LoginBtn;

    [HideInInspector]
    [FindChildComponent("RegisterButton")]
    public Button RegisterButton;
}
