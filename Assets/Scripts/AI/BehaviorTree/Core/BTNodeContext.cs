public class BTNodeContext
{
    public UnitCoordinator Coordinator;
    public AttributeContainer Attributes;
    public BlackBoard Blackboard;

    public BTNodeContext(UnitCoordinator coordinator, AttributeContainer attributes /*, BlackBoard blackboard*/)
    {
        Coordinator = coordinator;
        Attributes = attributes;
        //Blackboard = blackboard;
    }
}

/*
Extend this later with:
Transform Self (if needed for proximity or LOS)
ITargetProvider (for target system injection)
IStatusEffectReceiver etc. 
*/