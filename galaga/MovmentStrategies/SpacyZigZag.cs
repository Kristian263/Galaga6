using DIKUArcade.Entities;
using DIKUArcade.Math;
using System;
namespace galaga.MovementStrategies{
    public class SpacyZigZag : IMovementStrategy{
        private float speed;
        private float p;
        private float a;
        public SpacyZigZag(){
            speed = 0.0003f;
            p = 0.1f;
            a = 0.05f;}
        public Vec2F NextPosition(Enemy enemy){
            float y = (enemy.Shape.Position.Y) - speed;
            float x = (float)(enemy.initialpos.X + a * Math.Sin((2.0*Math.PI*(enemy.initialpos.Y-y))/p));
            return new Vec2F (x,y);
        }
        public void MoveEnemy (Enemy enemy){
            enemy.Shape.Position = NextPosition(enemy);
        }
        public void MoveEnemies(EntityContainer<Enemy> enemies){
            enemies.Iterate(MoveEnemy);
        }
    }
}