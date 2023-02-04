using FGJ23.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGJ23.Entities.CoordinateEvents
{
    internal class Enemy : Component
    {
        public override void OnAddedToEntity()
        {
            ColliderSystem.RegisterCollider(Entity);
        }

        public override void OnRemovedFromEntity()
        {
            ColliderSystem.UnRegisterCollider(Entity);
        }
    }
}
