using System.IO;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.GameLogic.Commands
{
    public interface ICommand
    {
        int CommandId { get; }

        void Serialize(Stream stream);

        void Deserialize(Stream stream);

        Entity Execute(Content content, EntityController entityController, Resolver<IComponentFactory> factories);
    }
}
