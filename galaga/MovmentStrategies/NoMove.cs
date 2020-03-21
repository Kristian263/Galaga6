using DIKUArcade.Entities;
using DIKUArcade.Math;
namespace galaga.MovementStrategies{
    public class NoMove : IMovementStrategy{
        public void MoveEnemy (Enemy enemy){
            enemy.Shape.AsDynamicShape().ChangeDirection(new Vec2F (0.0f, 0.0f));
        }
        public void MoveEnemies(EntityContainer<Enemy> enemies){
            enemies.Iterate(MoveEnemy);
        }

        public void IncressSpeed(float pct){
        }
    }
}