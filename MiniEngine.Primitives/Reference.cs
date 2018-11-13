namespace MiniEngine.Primitives
{
    public sealed class Reference<T>
    {
        private T instance;
        public Reference()
        {

        }

        public Reference(T instance)
        {
            this.instance = instance;
        }

        public T Get() => this.instance;
        public void Set(T instance) => this.instance = instance;

        public static Reference<T>[] Create(T[] instances)
        {
            var references = new Reference<T>[instances.Length];
            for (var i = 0; i < references.Length; i++)
            {
                references[i] = new Reference<T>(instances[i]);
            }

            return references;
        }

        public static Reference<T>[] CreateEmpty(int length)
        {
            var references = new Reference<T>[length];
            for (var i = 0; i < references.Length; i++)
            {
                references[i] = new Reference<T>();
            }

            return references;
        }
    }
}
