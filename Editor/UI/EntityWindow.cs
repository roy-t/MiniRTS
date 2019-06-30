using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImGuiNET;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;
using MiniEngine.UI.State;
using MiniEngine.UI.Utilities;

namespace MiniEngine.UI
{
    public sealed class EntityWindow
    {
        private readonly Editors Editors;
        private readonly EntityController EntityController;
        private readonly ComponentSearcher ComponentSearcher;
        private readonly Dictionary<Type, int> ComponentCounter;

        private readonly List<IComponent> Components;


        public EntityWindow(Editors editors, EntityController entityController, ComponentSearcher componentSearcher)
        {
            this.Editors = editors;
            this.EntityController = entityController;
            this.ComponentSearcher = componentSearcher;
            this.ComponentCounter = new Dictionary<Type, int>();
            this.Components = new List<IComponent>();
        }

        public UIState State { get; set; }
        public EntityState EntityState => this.State.EntityState;
        
        public void Render()
        {
            if (ImGui.Begin("Entity Details", ref this.EntityState.ShowEntityWindow, ImGuiWindowFlags.NoCollapse))
            {
                ImGui.Text($"{this.EntityState.SelectedEntity}");

                this.ComponentCounter.Clear();

                this.Components.Clear();
                this.ComponentSearcher.GetComponents(this.EntityState.SelectedEntity, this.Components);

                for (var i = 0; i < this.Components.Count; i++)
                {
                    var component = this.Components[i];
                    // ImGui requires a unique name for every node, so for each component we add
                    // check how many of that component we've already added and use that in the name
                    var count = this.Count(component);

                    var name = GetName(component);
                    if (ImGui.TreeNode(name + " #" + count.ToString("00")))
                    {
                        this.CreateEditors(component);

                        if (ImGui.Button("Remove Component"))
                        {
                            var container = this.ComponentSearcher.GetContainer(component);
                            container.Remove(component);
                        }
                        ImGui.TreePop();
                    }
                }

                if (this.EntityController.GetAllEntities().Contains(this.EntityState.SelectedEntity))
                {
                    if (ImGui.Button("Destroy Entity"))
                    {
                        this.EntityController.DestroyEntity(this.EntityState.SelectedEntity);
                        this.EntityState.ShowEntityWindow = false;
                        var entities = this.EntityController.GetAllEntities();
                        this.EntityState.SelectedEntity = entities.Any() ? entities.First() : new Entity(-1);
                    }
                }

                ImGui.End();
            }
        }

        private void CreateEditors(IComponent component)
        {
            var componentType = component.GetType();

            var properties = componentType.GetProperties();
            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];

                var attributes = property.GetCustomAttributes(typeof(EditorAttribute), false);
                for (var a = 0; a < attributes.Length; a++)
                {
                    if (attributes[a] is EditorAttribute attribute)
                    {
                        var getter = GetGetter(property, component, componentType);
                        var setter = GetSetter(attribute.Setter, component, componentType) ?? GetSetter(property, component, componentType);

                        var index = 0;
                        if (!string.IsNullOrEmpty(attribute.IndexProperty))
                        {
                            index = (int)component.GetType().GetProperty(attribute.IndexProperty).GetGetMethod().Invoke(component, null);
                        }

                        this.Editors.Create(attribute.Name, getter(), attribute.MinMax, setter, index);
                    }
                }
            }            
        }

        private static string GetName(IComponent component) 
            => component.GetType().Name;

        private static Func<object> GetGetter(PropertyInfo property, IComponent component, Type componentType)
        {            
            if (property != null)
            {
                return () => property.GetGetMethod().Invoke(component, null);
            }

            return null;
        }

        private static Action<object> GetSetter(string name, IComponent component, Type componentType)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
           
            var method = componentType.GetMethod(name);
            if(method != null)
            {
                return o => method.Invoke(component, new object[] { o });
            }

            return null;
        }

        private static Action<object> GetSetter(PropertyInfo property, IComponent component, Type componentType)
        {
            if(property != null && property.GetSetMethod() != null)
            {
                return o => property.GetSetMethod().Invoke(component, new object[] { o });
            }

            return null;
        }

        private int Count(IComponent component)
        {
            if (this.ComponentCounter.ContainsKey(component.GetType()))
            {
                this.ComponentCounter[component.GetType()] += 1;
            }
            else
            {
                this.ComponentCounter.Add(component.GetType(), 0);
            }

            var id = this.ComponentCounter[component.GetType()];
            return id;
        }
    }
}
