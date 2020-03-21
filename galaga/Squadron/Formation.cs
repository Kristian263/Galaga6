using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using System.Collections.Generic;
using DIKUArcade.Math;
using System;

namespace galaga.Squadron
{
    public class Formation : ISquadron
    {
        public EntityContainer <Enemy> Enemies {get;}
        public int MaxEnemies {get;}
        public Formation (int maxEnemies){
            if (maxEnemies > 7 || maxEnemies < 0){
                throw new ArgumentException ("Too many enemies");}
            else {
                MaxEnemies = maxEnemies;
                Enemies = new EntityContainer<Enemy>();
            }
        }
        public void CreateEnemies (List<Image> enemyStrides){
            System.Console.WriteLine(MaxEnemies);
            float j = ((MaxEnemies)*0.1f);
            for (float i = 0.0f; i <= j; i+=0.2f){
                    Enemies.AddDynamicEntity(new Enemy ((new DynamicShape(new Vec2F(0.2f+i,0.8f),
                        new Vec2F(0.1f,0.1f))), new ImageStride(80, enemyStrides)));
                    Enemies.AddDynamicEntity(new Enemy ((new DynamicShape(new Vec2F(0.2f+i,0.6f),
                        new Vec2F(0.1f,0.1f))), new ImageStride(80, enemyStrides)));
                    Enemies.AddDynamicEntity(new Enemy ((new DynamicShape(new Vec2F(0.1f+i,0.7f),
                        new Vec2F(0.1f,0.1f))), new ImageStride(80, enemyStrides)));
            }
        }
    }
}