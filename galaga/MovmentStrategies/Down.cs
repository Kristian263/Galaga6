using DIKUArcade.Entities;
using DIKUArcade.Math;
namespace galaga.MovementStrategies{
    public class Down : IMovementStrategy{
        private float s = -0.0005f;
        public void MoveEnemy (Enemy enemy){
            enemy.Shape.AsDynamicShape().ChangeDirection(new Vec2F (0.0f, s));
            enemy.Shape.Move();
        }
        public void MoveEnemies(EntityContainer<Enemy> enemies){
            enemies.Iterate(MoveEnemy);
        }
    }
}