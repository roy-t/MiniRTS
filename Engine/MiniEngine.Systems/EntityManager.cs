namespace MiniEngine.Systems
{
    public sealed class EntityManager
    {
        public EntityManager(EntityCreator creator, EntityController controller)
        {
            this.Creator = creator;
            this.Controller = controller;
        }

        public EntityCreator Creator { get; }
        public EntityController Controller { get; }
    }
}
