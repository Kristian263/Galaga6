using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace galaga
{
    public class Enemy: Entity {
        public Vec2F initialpos {get;}
        public Enemy(DynamicShape shape, IBaseImage image)
            : base(shape, image){
            initialpos = shape.Position;
        }
    }
}