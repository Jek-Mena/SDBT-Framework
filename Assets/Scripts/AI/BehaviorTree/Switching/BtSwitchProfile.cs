using System;
using System.Collections.Generic;

[Serializable]
[System.Obsolete]
public class BtSwitchProfile
{
    // To support named switch profiles;
    public Dictionary<string, SwitchCondition> SwitchProfiles;
}