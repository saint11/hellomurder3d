using Bang.Contexts;
using Bang.Entities;
using Bang.Systems;
using Murder.Core.Geometry;
using Murder.Components;
using Murder;
using Bang.StateMachines;
using Bang.Components;
using Murder.Helpers;
using HelloMurder.Components;
using HelloMurder.Messages;
using HelloMurder.Core;
using HelloMurder.Core.Input;
using Bang.Interactions;
using System.Numerics;
using Murder.Utilities;

namespace HelloMurder.Systems
{
    [Filter(kind: ContextAccessorKind.Read, typeof(PlayerComponent))]
    public class PlayerInputSystem : IUpdateSystem, IFixedUpdateSystem
    {
        private Vector2 _cachedInputAxis = Vector2.Zero;
        private bool _interacted = false;
        
        public void FixedUpdate(Context context)
        {
            foreach (Entity entity in context.Entities)
            {
                PlayerComponent player = entity.GetComponent<PlayerComponent>();
                
                bool moved = _cachedInputAxis.HasValue();
                
                if (_interacted)
                {
                    entity.SendMessage<InteractorMessage>();
                }
                
                if (moved)
                {
                    Direction direction = DirectionHelper.FromVector(_cachedInputAxis);
                    entity.SetAgentImpulse(_cachedInputAxis, direction);
                }
                _interacted = false;
            }
        }

        public void Update(Context context)
        {
            _cachedInputAxis = Game.Input.GetAxis(InputAxis.Movement).Value;
        }
    }
}
