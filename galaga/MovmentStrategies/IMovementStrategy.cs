using DIKUArcade.Entities;
namespace galaga.MovementStrategies{
    public interface IMovementStrategy{
        void MoveEnemy (Enemy enemy);
        void MoveEnemies(EntityContainer<Enemy> enemies);
        void IncressSpeed(float pct);
    }
}