using MiniEngine.Systems.Components;
using System;
using System.Collections.Generic;

namespace MiniEngine.Systems
{
    public sealed class ComponentLookup
    {
        private readonly Dictionary<Entity, List<Record>> ComponentsPerEntity;
        private readonly Dictionary<Type, List<Record>> EntitiesPerComponent;

        private readonly List<Record> WorkList;
        private readonly List<Record> EmptyResult;

        public ComponentLookup()
        {
            this.ComponentsPerEntity = new Dictionary<Entity, List<Record>>();
            this.EntitiesPerComponent = new Dictionary<Type, List<Record>>();

            this.WorkList = new List<Record>();
            this.EmptyResult = new List<Record>(0);
        }

        public IReadOnlyList<Record> Search(Entity entity)
        {
            if(this.ComponentsPerEntity.TryGetValue(entity, out var records))
            {
                return records;
            }

            return this.EmptyResult;
        }
        
        public IReadOnlyList<Record> Search(Type componentType)
        {
            if(this.EntitiesPerComponent.TryGetValue(componentType, out var records))
            {
                return records;
            }

            return this.EmptyResult;
        }

        public void AddComponent(Entity entity, IComponent component)
        {
            var record = new Record(entity, component);
            
            if(this.ComponentsPerEntity.TryGetValue(entity, out var entityList))
            {
                entityList.Add(record);
            }
            else
            {
                var newList = new List<Record>
                {
                    record
                };
                this.ComponentsPerEntity.Add(entity, newList);
            }

            var type = component.GetType();
            if (this.EntitiesPerComponent.TryGetValue(type, out var typeList))
            {
                typeList.Add(record);
            }
            else
            {
                var newList = new List<Record>
                {
                    record
                };
                this.EntitiesPerComponent.Add(type, newList);
            }
        }

        public void Remove(Entity entity, Type componentType)
        {
            if (this.ComponentsPerEntity.TryGetValue(entity, out var componentRecords))
            {
                foreach (var record in componentRecords)
                {
                    if (record.Component.GetType() == componentType)
                    {
                        this.WorkList.Add(record);
                    }
                }

                foreach (var record in this.WorkList)
                {
                    componentRecords.Remove(record);
                }
            }

            this.WorkList.Clear();

            if (this.EntitiesPerComponent.TryGetValue(componentType, out var entityRecords))
            {
                foreach (var record in entityRecords)
                {
                    if (record.Entity == entity)
                    {
                        this.WorkList.Add(record);
                    }
                }

                foreach (var record in this.WorkList)
                {
                    entityRecords.Remove(record);
                }
            }

            this.WorkList.Clear();
        }
      
        public class Record
        {
            public Record(Entity entity, IComponent component)
            {
                this.Entity = entity;
                this.Component = component;
            }

            public Entity Entity { get; }
            public IComponent Component { get; }
        }
    }
}
