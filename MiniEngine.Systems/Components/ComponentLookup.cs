using System;
using System.Collections.Generic;

namespace MiniEngine.Systems.Components
{
    public sealed class ComponentLookup
    {
        private readonly Dictionary<Entity, List<EntityComponentRecord>> ComponentsPerEntity;
        private readonly Dictionary<Type, List<EntityComponentRecord>> EntitiesPerComponent;

        private readonly List<EntityComponentRecord> WorkList;
        private readonly List<EntityComponentRecord> EmptyResult;

        public ComponentLookup()
        {
            this.ComponentsPerEntity = new Dictionary<Entity, List<EntityComponentRecord>>();
            this.EntitiesPerComponent = new Dictionary<Type, List<EntityComponentRecord>>();

            this.WorkList = new List<EntityComponentRecord>();
            this.EmptyResult = new List<EntityComponentRecord>(0);
        }

        public IReadOnlyList<EntityComponentRecord> Search(Entity entity)
        {
            if (this.ComponentsPerEntity.TryGetValue(entity, out var records))
            {
                return records;
            }

            return this.EmptyResult;
        }

        public IReadOnlyList<EntityComponentRecord> Search(Type componentType)
        {
            if (this.EntitiesPerComponent.TryGetValue(componentType, out var records))
            {
                return records;
            }

            return this.EmptyResult;
        }

        public void AddComponent(Entity entity, IComponent component)
        {
            var record = new EntityComponentRecord(entity, component);

            if (this.ComponentsPerEntity.TryGetValue(entity, out var entityList))
            {
                entityList.Add(record);
            }
            else
            {
                var newList = new List<EntityComponentRecord>
                {
#pragma warning disable IDE0009 // Member access should be qualified.
                    record
#pragma warning restore IDE0009 // Member access should be qualified.
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
                var newList = new List<EntityComponentRecord>
                {
#pragma warning disable IDE0009 // Member access should be qualified.
                    record
#pragma warning restore IDE0009 // Member access should be qualified.
                };
                this.EntitiesPerComponent.Add(type, newList);
            }
        }

        public void RemoveComponent(Entity entity, Type componentType)
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

        public void RemoveAllComponents(Type type)
        {
            if (this.EntitiesPerComponent.TryGetValue(type, out var entityRecords))
            {
                foreach (var record in entityRecords)
                {
                    this.ComponentsPerEntity[record.Entity].Remove(record);
                }

                this.EntitiesPerComponent.Remove(type);
            }

            this.WorkList.Clear();
        }
    }
}
