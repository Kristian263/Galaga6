using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;
using DIKUArcade.EventBus;
using System;

namespace galaga
{
    public class Player : IGameEventProcessor<object>{
        public Entity Entity {get; private set;}
        private Game game;
        private Image projectileImage;
        public Player(DynamicShape shape, IBaseImage image, Game game) {
            Entity = new Entity(shape, image);
            projectileImage = new Image (System.IO.Path.Combine("Assets", "Images", "BulletRed2.png"));
            this.game = game;
        }

        public void Direction (Vec2F vec){
            Entity.Shape.AsDynamicShape().ChangeDirection(vec);
        }
        public void Move(){
            DynamicShape DShape = Entity.Shape.AsDynamicShape();
            if ((DShape.Position + DShape.Direction).X >= 0.0f &&
                (DShape.Position + DShape.Direction + DShape.Extent).X <= 1.0f){
                    DShape.Move();
                }
        }

        public void Shoot(){
            game.playerShots.AddDynamicEntity(new PlayerShot (((new DynamicShape(
                new Vec2F (Entity.Shape.Position.X +.045f, Entity.Shape.Position.Y +0.1f),
                //Ellers bare entity.Shape.Position pga. så lægge en vektor til
                new Vec2F(0.008f,0.027f)))), projectileImage));
        }

        public void ProcessEvent(GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.PlayerEvent) {
                switch (gameEvent.Message) {
                    case "MOVE_LEFT":
                        Direction(new Vec2F (-0.01f,0.0f));
                        break;
                    case "MOVE_RIGHT":
                        Direction(new Vec2F (0.01f,0.0f));
                        break;
                    case "MOVE_STOP":
                        Direction(new Vec2F (0.0f,0.0f));
                        break;
                    case "SHOOT":
                        Shoot();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}