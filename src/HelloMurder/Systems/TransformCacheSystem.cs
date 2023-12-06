using Bang;
using Bang.Entities;
using Bang.Systems;
using HelloMurder3D.Components;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloMurder3D.Systems
{
    [Watch(typeof(Transform3D))]
    internal class TransformCacheSystem : IReactiveSystem
    {
        public void OnAdded(World world, ImmutableArray<Entity> entities)
        {
            UpdateMatrizes(entities);
        }

        private static void UpdateMatrizes(ImmutableArray<Entity> entities)
        {
            foreach (var e in entities)
            {
                if (e.TryGetTransform3D() is Transform3D transform)
                {
                    if (transform.Matrix == null)
                    {
                        e.SetTransform3D(transform.Position, transform.Rotation, transform.Scale);
                    }
                }
            }
        }

        public void OnModified(World world, ImmutableArray<Entity> entities)
        {
            UpdateMatrizes(entities);
        }

        public void OnRemoved(World world, ImmutableArray<Entity> entities)
        {
            
        }
    }
}
