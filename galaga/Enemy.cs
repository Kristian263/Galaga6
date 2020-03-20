using DIKUArcade.Entities;
using DIKUArcade.Graphics;

namespace galaga
{
    public class Enemy: Entity {
        
        public Enemy(DynamicShape shape, IBaseImage image)
            : base(shape, image){

        }
    }
}