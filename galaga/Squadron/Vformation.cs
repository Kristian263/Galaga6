using System.Collections.Generic;
using System;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace galaga.Squadron {
    /// <summary>
    ///Arranges a number of enemies' position, so that they are arranged like a V.
    /// </summary>
    public class Vformation : ISquadron {

        public EntityContainer<Enemy> Enemies {get;}
        public int MaxEnemies {get;}
        /// <summary>
        ///Sets the initial amount of enemies in the Squadron.
        /// </summary>
        /// <param name="maxEnemies">the amount of enemies in the Squadron</param>
        public Vformation(int maxEnemies) {
            if (maxEnemies > 8 || maxEnemies < 0) {
                throw new ArgumentException ("This formation cannot take more than 8 enemies");
            }
            MaxEnemies = maxEnemies;
            Enemies = new EntityContainer<Enemy>(14);
        }
        /// <summary>
        ///Through a for loop, arranges enemies in a V-shape.
        /// </summary>
        /// <param name="enemyStrides">the images the enemy animation should consist of</param>
        public void CreateEnemies (List<Image> enemyStrides) {
            float j = ((MaxEnemies)/2.0f)*0.1f;
            for (float i = 0.0f; i < j; i+=0.1f){
                Enemies.AddDynamicEntity(new Enemy(new DynamicShape(
                    new Vec2F((0.15f+i), 0.85f-i),new Vec2F(0.1f,0.1f)),
                    new ImageStride(80,enemyStrides)));
                if ((0.15+i, 0.85-i) != (0.55,0.55))
                {Enemies.AddDynamicEntity(new Enemy(new DynamicShape(
                    new Vec2F((0.75f-i), 0.85f-i),new Vec2F(0.1f,0.1f)),
                    new ImageStride(80,enemyStrides)));
                }
            }
        }
    }
}