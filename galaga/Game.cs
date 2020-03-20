using System;
using DIKUArcade;
using DIKUArcade.Entities;
using DIKUArcade.Graphics;
using DIKUArcade.Timers;
using DIKUArcade.EventBus;
using DIKUArcade.Math;
using System.Collections.Generic;
using DIKUArcade.Physics;

namespace galaga
{
    public class Game : IGameEventProcessor<object> {

        private Player player;

        private Window win;

        private DIKUArcade.Timers.GameTimer gameTimer;

        private GameEventBus<object> eventBus;

        private List<Image> enemyStrides;

        private List<Enemy> enemies;

        public List<PlayerShot> playerShots {get; private set;}

        private List<Image> explosionStrides;
        private AnimationContainer explosions;

        private int explosionLength = 500;
        private Score score;

        public Game() {
            win = new Window ("Mad man", 500,500);
            gameTimer = new GameTimer(60,60);
            score = new Score(new Vec2F(0.05f,0.8f), new Vec2F (0.2f,0.2f));

            player = new Player(
                new DynamicShape (new Vec2F(0.45f,0.1f), new Vec2F(0.1f,0.1f)),
                new Image(System.IO.Path.Combine("Assets", "Images", "Player.png")), this);

            enemyStrides = ImageStride.CreateStrides (4, System.IO.Path.Combine("Assets", "Images",
                "BlueMonster.png"));

            enemies = new List<Enemy>();

            playerShots = new List<PlayerShot>();

            explosionStrides = ImageStride.CreateStrides(8, System.IO.Path.Combine("Assets", "Images", "Explosion.png"));
            explosions = new AnimationContainer(8);

            eventBus = new GameEventBus<object>();
                eventBus.InitializeEventBus(new List<GameEventType>(){
                    GameEventType.InputEvent,
                    GameEventType.WindowEvent,
                    GameEventType.PlayerEvent,
                });
            win.RegisterEventBus(eventBus);
            eventBus.Subscribe(GameEventType.InputEvent,this);
            eventBus.Subscribe(GameEventType.WindowEvent, this);
            eventBus.Subscribe(GameEventType.PlayerEvent, player);

            AddEnemies();

            }

        public void AddEnemies(){
            for (float i = 0.0f; i < 0.8f; i += 0.1f){
                enemies.Add(new Enemy((new DynamicShape (new Vec2F(0.1f+i,0.8f),
                    new Vec2F(0.1f,0.1f))),
                new ImageStride(80, enemyStrides)));
            }
        }
        public void DrawEnemies(){
            foreach (Enemy enemy in enemies){
                enemy.RenderEntity();
            }
        }
        public void IterateShots(){
            foreach (var shot in playerShots){
                shot.Shape.Move();
                if (shot.Shape.Position.Y > 1.0f){
                    shot.DeleteEntity();
                } else {
                    foreach (var enemy in enemies){
                        if (CollisionDetection.Aabb(shot.Shape.AsDynamicShape(), enemy.Shape).Collision){
                            shot.DeleteEntity();
                            enemy.DeleteEntity();
                            AddExplosion(enemy.Shape.Position.X,enemy.Shape.Position.Y,
                            enemy.Shape.Extent.X,enemy.Shape.Extent.Y);
                            score.AddPoint();
                            }
                        }
                    }
                }
                List <Enemy> newEnemies = new List<Enemy>();
                foreach (Enemy enemy in enemies){
                    if (!enemy.IsDeleted()){newEnemies.Add(enemy);}
                }
                enemies = newEnemies;
            }

        public void AddExplosion(float posX, float posY, float extentX, float extentY) {
            explosions.AddAnimation(
            new StationaryShape(posX, posY, extentX, extentY), explosionLength,
            new ImageStride(explosionLength / 8, explosionStrides));
            }
        public void DrawShots(){
            foreach (PlayerShot shoot in playerShots){
                if (!shoot.IsDeleted()) {shoot.RenderEntity();
                }
            }
        }
        public void GameLoop(){
            while (win.IsRunning()){
                gameTimer.MeasureTime();
                while (gameTimer.ShouldUpdate()){
                    eventBus.ProcessEvents();
                    win.PollEvents();
                    player.Move();
                    IterateShots();
                    }
                if (gameTimer.ShouldRender()){
                win.Clear();
                player.Entity.RenderEntity();
                DrawEnemies();
                DrawShots();
                explosions.RenderAnimations();
                score.RenderScore();
                win.SwapBuffers();
                    }
                if (gameTimer.ShouldReset()){
                win.Title = "Galaga | Ups: " + gameTimer.CapturedUpdates + ", FPS: " + gameTimer.CapturedFrames;
                    }
                }
            }
        public void KeyPress (string key) {
            switch(key){
                case "KEY_ESCAPE":
                    eventBus.RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForAllProcessors(
                        GameEventType.WindowEvent, this, "CLOSE_WINDOW", "", ""));
                        break;
                case "KEY_LEFT":
                    //player.Direction(new Vec2F (-0.01f,0.0f));
                    eventBus.RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForSpecificProcessor(
                            GameEventType.PlayerEvent, this, player, "MOVE_LEFT", "", ""));
                    break;
                case "KEY_RIGHT":
                    eventBus.RegisterEvent(GameEventFactory<object>.CreateGameEventForSpecificProcessor(
                            GameEventType.PlayerEvent, this, player, "MOVE_RIGHT", "", ""));
                    break;
                case "KEY_SPACE":
                    eventBus.RegisterEvent(GameEventFactory<object>.CreateGameEventForSpecificProcessor(
                            GameEventType.PlayerEvent, this, player, "SHOOT", "", ""));
                    break;
                default:
                    break;
            }
        }
        public void KeyRelease (string key) {
            switch(key){
                case "KEY_LEFT":
                    eventBus.RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForSpecificProcessor(
                            GameEventType.PlayerEvent, this, player, "MOVE_STOP", "", ""));
                    break;
                case "KEY_RIGHT":
                    eventBus.RegisterEvent(
                        GameEventFactory<object>.CreateGameEventForSpecificProcessor(
                            GameEventType.PlayerEvent, this, player, "MOVE_STOP", "", ""));
                    break;
            }
        }
        public void ProcessEvent (GameEventType eventType, GameEvent<object> gameEvent) {
            if (eventType == GameEventType.WindowEvent){
                switch (gameEvent.Message){
                    case "CLOSE_WINDOW":
                        win.CloseWindow();
                        break;
                    default:
                        break;
                }
            } else if (eventType == GameEventType.InputEvent){
                switch (gameEvent.Parameter1){
                    case "KEY_PRESS":
                        KeyPress(gameEvent.Message);
                        break;
                    case "KEY_RELEASE":
                        KeyRelease(gameEvent.Message);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}