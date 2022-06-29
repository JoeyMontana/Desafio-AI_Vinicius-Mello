using System.Collections.Generic;
using UnityEngine;
using ChallengeAI;

public class AIStates {
    static public string IDLE = "idle";
    static public string WALK = "walk";
    static public string CAPTURE_FLAG = "capture_flag";
    static public string WALK_MY_FLAG = "walk_my_flag";
    static public string WALK_AMMO = "walk_ammo";
    static public string WALK_POWER_UP = "walk_power_up";
    static public string FIND_ENEMY = "find_enemy";
    static public string WALK_RANDOM = "walk_random";

    static public string[] StateNames = new string[] {
        CAPTURE_FLAG,
        WALK_MY_FLAG,
        WALK_AMMO,
        WALK_POWER_UP,
        FIND_ENEMY,
        IDLE
    };
}

public class WalkState : State {
    public WalkState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player,
        changeStateDelegate) {
    }

    public Vector3 Destination { get; set; } = Vector3.zero;

    public override void Enter() {
        Agent.Move(Destination);
        Log($"Destination:{Destination}");
    }

    public override void Exit() {
        Log();
    }

    public override void Update(float deltaTime) {
        
    }
}

public class CaptureFlag : WalkState {
    public CaptureFlag(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player,
        changeStateDelegate) {
    }

    public override void Enter() {
        Destination = (Vector3) Agent.EnemyData[0].FlagPosition;
        Log($"Flag Destination {Destination}");
        base.Enter();
    }

    public override void Update(float deltaTime) {
        if (Agent.Data.Energy <= 5) {
            var nextState = AIStates.IDLE;
            ChangeState(nextState);
            Log($"NextState:{nextState}");
        }
        else if (Agent.Data.Energy <= 40) {
            foreach (var powerUp in Agent.Data.PowerUps) {
                if (Vector3.Distance(Agent.Data.Position, powerUp) <= 15f) {
                    var nextState = AIStates.WALK_POWER_UP;
                    ChangeState(nextState);
                    Log($"NextState:{nextState}");
                }
            }
        }

        if (Agent.Data.HasSightEnemy && Agent.Data.Ammo > 0 && !Agent.Data.IsCooldownFire) {
            Agent.Fire();
            Agent.Move(Destination);
        }
        else if (Agent.Data.Ammo == 0) {
            var nextState = AIStates.WALK_AMMO;
            ChangeState(nextState);
            Log($"NextState:{nextState}");
        }

        if (Agent.Data.HasFlag) {
            var nextState = AIStates.WALK_MY_FLAG;
            ChangeState(nextState);
            Log($"NextState:{nextState}");
        }
    }
}

public class WalkToMyFlag : WalkState {
    public WalkToMyFlag(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player,
        changeStateDelegate) {
    }

    public override void Enter() {
        Destination = (Vector3) Agent.Data.FlagPosition;
        Log($"Flag Destination {Destination}");
        base.Enter();
    }
    
    public override void Update(float deltaTime) {
        if (Agent.Data.Energy <= 5) {
            var nextState = AIStates.IDLE;
            ChangeState(nextState);
            Log($"NextState:{nextState}");
        }
        else if (Agent.Data.Energy <= 40) {
            foreach (var powerUp in Agent.Data.PowerUps) {
                if (Vector3.Distance(Agent.Data.Position, powerUp) <= 15f) {
                    var nextState = AIStates.WALK_POWER_UP;
                    ChangeState(nextState);
                    Log($"NextState:{nextState}");
                }
            }
        }

        if (Agent.Data.HasSightEnemy && Agent.Data.Ammo > 0 && !Agent.Data.IsCooldownFire) {
            Agent.Fire();
            Agent.Move(Destination);
        } else if (Agent.Data.Ammo == 0 && Agent.Data.FlagState == FlagState.Catched) {
            var nextState = AIStates.WALK_AMMO;
            ChangeState(nextState);
            Log($"NextState:{nextState}");
        }

        if (Agent.Data.FlagState == FlagState.Catched) {
            var nextState = AIStates.FIND_ENEMY;
            ChangeState(nextState);
            Log($"NextState:{nextState}");
        }
        
        if (!Agent.Data.HasFlag) {
            var nextState = AIStates.CAPTURE_FLAG;
            ChangeState(nextState);
            Log($"NextState:{nextState}");
        }
    }
}

public class FindEnemy : WalkState {
    public FindEnemy(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player,
        changeStateDelegate) {
    }

    public override void Enter() {
        Destination = Agent.Data.FlagPosition.Value;
        Log($"Enemy position: {Destination}");
        base.Enter();
    }

    public override void Update(float deltaTime) {
        if (Agent.Data.Energy <= 5) {
            var nextState = AIStates.IDLE;
            Log($"NextState:{nextState}");
        }
        else if (Agent.Data.Energy <= 40) {
            foreach (var powerUp in Agent.Data.PowerUps) {
                if (Vector3.Distance(Agent.Data.Position, powerUp) <= 15f) {
                    var nextState = AIStates.WALK_POWER_UP;
                    ChangeState(nextState);
                    Log($"NextState:{nextState}");
                }
            }
        }
        
        if (!Agent.Data.HasFlag) {
            var nextState = AIStates.CAPTURE_FLAG;
            ChangeState(nextState);
            Log($"NextState:{nextState}");
        } else if (Agent.Data.FlagState == FlagState.StartPosition) {
            var nextState = AIStates.WALK_MY_FLAG;
            ChangeState(nextState);
            Log($"NextState:{nextState}");
        }
        
        if (Agent.Data.HasSightEnemy && Agent.Data.Ammo > 0 && !Agent.Data.IsCooldownFire) {
            Agent.Fire();
            Agent.Move(Destination);
        } else if (Agent.Data.Ammo == 0) {
            var nextState = AIStates.WALK_AMMO;
            ChangeState(nextState);
            Log($"NextState:{nextState}");
        }

        

        Destination = Agent.Data.FlagPosition.Value;
        Agent.Move(Destination);
    }
}

public class WalkToAmmo : WalkState {
    public WalkToAmmo(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) {
    }

    public override void Enter() {
        if (Agent.Data.AmmoRefill == null) {
            var nextState = AIStates.IDLE;
            ChangeState(nextState);
        }
        Destination = (Vector3) Agent.Data.AmmoRefill[0];
        Log($"Ammo destination: {Destination}");
        base.Enter();
    }
    
    public override void Update(float deltaTime) {
        if (Agent.Data.Ammo == 2) {
            if (Agent.Data.HasFlag) {
                var nextState = AIStates.WALK_MY_FLAG;
                ChangeState(nextState);
                Log($"NextState:{nextState}");
            }
            else {
                var nextState = AIStates.CAPTURE_FLAG;
                ChangeState(nextState);
                Log($"NextState:{nextState}");
            }
        }
        
        if (Agent.Data.Energy <= 5) {
            var nextState = AIStates.IDLE;
            ChangeState(nextState);
            Log($"NextState:{nextState}");
        }
        else if (Agent.Data.Energy <= 40) {
            foreach (var powerUp in Agent.Data.PowerUps) {
                if (Vector3.Distance(Agent.Data.Position, powerUp) <= 15f) {
                    var nextState = AIStates.WALK_POWER_UP;
                    ChangeState(nextState);
                    Log($"NextState:{nextState}");
                }
            }
        }
    }
}

public class WalkToPowerUp : WalkState {
    public WalkToPowerUp(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player, changeStateDelegate) {
    }

    private float currentDistance;
    private List<float> powerUpsDistances;
    private int closestPowerUp;

    public override void Enter() {
        if (Agent.Data.Position.x <= 0) {
            Destination = (Vector3) Agent.Data.PowerUps[1];    
        }
        else {
            Destination = (Vector3) Agent.Data.PowerUps[0];
        }
        
        Log($"Power Up Destination {Destination}");
        base.Enter();
    }

    public override void Update(float deltaTime) {
        if (Agent.Data.RemainingDistance <= 0.05f) {
            if (Agent.Data.HasFlag) {
                var nextState = AIStates.WALK_MY_FLAG;
                ChangeState(nextState);
                Log($"NextState:{nextState}");
            }
            else {
                var nextState = AIStates.CAPTURE_FLAG;
                ChangeState(nextState);
                Log($"NextState:{nextState}");
            }
        }

        if (Agent.Data.HasSightEnemy && Agent.Data.Ammo > 0 && !Agent.Data.IsCooldownFire) {
            Agent.Fire();
            Agent.Move(Destination);
        }
    }
}

public class IdleState : State {
    public IdleState(string name, IPlayer player, FSMChangeState changeStateDelegate) : base(name, player,
        changeStateDelegate) {
    }

    private float angle;
    
    public override void Enter() {
        Agent.Stop();
        Log($"Is Idle");
    }

    public override void Exit() {
        Log();
    }

    public override void Update(float deltaTime) {
        if (Agent.Data.Energy >= 65f && Agent.Data.Energy < 90) {
            foreach (var powerUp in Agent.Data.PowerUps) {
                if (Vector3.Distance(Agent.Data.Position, powerUp) <= 15f) {
                    var nextState = AIStates.WALK_POWER_UP;
                    ChangeState(nextState);
                    Log($"NextState:{nextState}");
                }
            }
        }
        
        if (Agent.Data.Energy >= 90f) {
            if (Agent.Data.HasFlag) {
                var nextState = AIStates.WALK_MY_FLAG;
                ChangeState(nextState);
                Log($"NextState:{nextState}");
            }
            else {
                var nextState = AIStates.CAPTURE_FLAG;
                ChangeState(nextState);
                Log($"NextState:{nextState}");
            }
        }

        if (Agent.Data.HasSightEnemy && Agent.Data.Ammo > 0 && !Agent.Data.IsCooldownFire) {
            Agent.Fire();
        }

        angle += deltaTime;
        Agent.Rotate(angle);
    }
}