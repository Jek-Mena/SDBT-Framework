[System.Serializable]
public struct NamedFloatField<TEnum> where TEnum : System.Enum
{
    public TEnum Key;
    public float Value;
}