using ChallengeAI;

public class FSMInit : FSMInitializer //IFSMInitializer
{
  public override string Name => "AI Vinicius";
  public override void Init()
  {
    RegisterState<CaptureFlag>(AIStates.CAPTURE_FLAG);
    RegisterState<WalkToMyFlag>(AIStates.WALK_MY_FLAG);
    RegisterState<WalkToAmmo>(AIStates.WALK_AMMO);
    RegisterState<WalkToPowerUp>(AIStates.WALK_POWER_UP);
    RegisterState<FindEnemy>(AIStates.FIND_ENEMY);
    RegisterState<IdleState>(AIStates.IDLE);
  }
}
