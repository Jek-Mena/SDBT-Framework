// Base Attribute Mapper

public abstract class AttributeMapper<TEnum, TAttr>
    where TEnum : System.Enum
    where TAttr : new()
{
    public abstract void MapField(ref TAttr target, TEnum Key, float value);
}