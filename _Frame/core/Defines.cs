using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum NetLib {
    Telepathy,
    Riptide
}

public enum evt {
    open_door,
}

public enum GlobalState { 
    EmptyState=0,
    LoadingState,
    //MenuState,
   // PlayState,
}

public enum ViewType { 
    LoadingView,
    MenuView,
    ShopView,
}

public enum ViewAnimationType { 
    None,
    FadeIn,
    FadeOut,
    ScaleUp,
    ScaleDown,
    SlideUp,
    SlideDown,
    SlideLeft,
    SlideRight,
}