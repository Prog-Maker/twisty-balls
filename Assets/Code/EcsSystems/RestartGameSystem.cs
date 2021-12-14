using Code.EcsComponents;
using Code.Phases;
using Kk.BusyEcs;

namespace Code.EcsSystems
{
    [EcsSystem]
    public class RestartGameSystem
    {
        [Inject]
        public IEnv env;
        
        [Update]
        public void Act(Entity command, RestartGameCommand _)
        {
            env.Query((Entity entity, ref BallType _) =>
            {
                if (!entity.Has<BallDestroyAction>())
                {
                    entity.Add(new BallDestroyAction());
                }
            });

            command.DelEntity();

            env.NewEntity(new StartGameCommand());
        }
    }
}