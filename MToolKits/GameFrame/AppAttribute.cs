using System;


public class TypeAttribute : Attribute
{
    public int type;
    public TypeAttribute(int type)
    {
        this.type = type;
    }
}
public class HandlerAttribute : TypeAttribute
{
    public HandlerAttribute(int type) : base(type) { }
    
}

public class GameStateAttribute : TypeAttribute
{
    public GameStateAttribute(int type) : base(type) { }
}