public interface IUpdatePhaseAction
{
    void Execute();

}

public interface IUpdateAction : IUpdatePhaseAction { }
public interface ILateUpdateAction : IUpdatePhaseAction { }
public interface IFixedUpdateAction : IUpdatePhaseAction { }