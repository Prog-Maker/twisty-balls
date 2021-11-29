using Code.EcsComponents;
using Kk.LeoQuery;

namespace Code.EcsSystems
{
    public class RestartGameSystem : ISystem
    {
        public void Act(IEntityStorage storage)
        {
            foreach (Entity<RestartGameCommand> command in storage.Query<RestartGameCommand>())
            {
                foreach (Entity<BallBody> entity in storage.Query<BallBody>())
                {
                    entity.Add(new BallDestroyAction());
                }

                command.Destroy();
                
                storage.NewEntity().Add<StartGameCommand>();
            }
        }
    }
}