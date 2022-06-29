using ChallengeAI;

public class FSMInitExample : FSMInitializer //IFSMInitializer
{
  public override string Name => "Example";
  public override void Init()
  {
    // RegisterState<IdleState>(AIStates.IDLE);
    // RegisterState<WalkState>(AIStates.WALK);
    RegisterState<CaptureFlagTest>(ExampleState.CAPTURE_FLAG);
    RegisterState<WalkToFlagTest>(ExampleState.WALK_FLAG);
    RegisterState<WalkToRandomTest>(ExampleState.WALK_RANDOM);
  }
}
