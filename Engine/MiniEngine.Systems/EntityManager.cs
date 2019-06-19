namespace MiniEngine.Systems
{
    public sealed class EntityManager
    {
        public EntityManager(EntityCreator creator, EntityController controller, EntityLinker linker)
        {
            this.Creator = creator;
            this.Controller = controller;
            this.Linker = linker;
        }

        public EntityCreator Creator { get; }
        public EntityController Controller { get; }
        public EntityLinker Linker { get; }
    }
}
