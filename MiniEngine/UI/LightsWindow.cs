using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Systems;
using System.Collections.Generic;

namespace MiniEngine.UI
{
    public class LightsWindow
    {
        private readonly EntityCreator EntityCreator;
        private readonly EntityController EntityController;

        private readonly LightsFactory LightsFactory;
        private readonly EntityLinker EntityLinker;
        private readonly List<Entity> TemporaryEntities;

        private bool show;
        private Vector3 initialPosition;

        public LightsWindow(EntityCreator entityCreator, LightsFactory lightsFactory)
        {
            this.EntityCreator = entityCreator;
            this.LightsFactory = lightsFactory;
        }

        public Vector3 InitialPosition
        {
            get => this.initialPosition;
            set => this.initialPosition = value;
        }

        public void Show()
        {
            if(ImGui.Begin("Lights Creator", ref this.show))
            {
                if(ImGui.Button("Create Point light"))
                {
                    var entity = this.EntityCreator.CreateEntity();
                    this.LightsFactory.PointLightFactory.Construct(entity, this.initialPosition, Color.White, 10.0f, 1.0f);
                }
            }
            ImGui.End();
        }
    }
}
