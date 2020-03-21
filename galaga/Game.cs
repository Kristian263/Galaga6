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

        private EntityContainer<Enemy> enemies;

        public EntityContainer<PlayerShot> playerShots {get; private set;}

        private List<Image> explosionStrides;
        private MovementStrategies.IMovementStrategy currentMoveStrategy;
        private AnimationContainer explosions;
        private Random rnd = new Random();
        private int explosionLength = 500;
        private Score score;
        private float pct = 1.0f;
        private GameState gameState;
        private GameOverDisPlay GameOverDisPlay;

        public Game() {
            win = new Window ("Mad man", 500,500);
            gameTimer = new GameTimer(60,60);
            score = new Score(new Vec2F(0.05f,0.8f), new Vec2F (0.2f,0.2f));
            gameState = GameState.GamePlaying;
            GameOverDisPlay = new GameOverDisPlay();

            player = new Player(
                new DynamicShape (new Vec2F(0.45f,0.1f), new Vec2F(0.1f,0.1f)),
                new Image(System.IO.Path.Combine("Assets", "Images", "Player.png")), this);

            enemyStrides = ImageStride.CreateStrides (4, System.IO.Path.Combine("Assets", "Images",
                "BlueMonster.png"));

            enemies = new EntityContainer<Enemy>();
            AddEnemies(new Squadron.Formation(6));
            currentMoveStrategy = new MovementStrategies.NoMove();

            playerShots = new EntityContainer<PlayerShot>();

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

            }

        public void AddEnemies(Squadron.ISquadron formation){
            formation.CreateEnemies(enemyStrides);
            enemies = formation.Enemies;
        }
        public void NextMovementStrategy(){
            pct += 0.05f;
            switch (rnd.Next(2)){
                case 0:
                    currentMoveStrategy = new MovementStrategies.Down();
                    break;
                default:
                    currentMoveStrategy = new MovementStrategies.SpacyZigZag();
                    break;
            }
            currentMoveStrategy.IncressSpeed(pct);
        }

        private void UpdateEnemies(){
            if (enemies.CountEntities() <= 0){
                switch (rnd.Next(2)){
                    case 0:
                        AddEnemies(new Squadron.Formation(rnd.Next(4,7)));
                        break;
                    default:
                        AddEnemies(new Squadron.Vformation(rnd.Next(4,9)));
                        break;
                }
            NextMovementStrategy();
            }
        }
        public void IterateShots(){
            void IterateForShoots (PlayerShot shot){
                shot.Shape.Move();
                if (shot.Shape.Position.Y > 1.0f){
                    shot.DeleteEntity();
                } else {
                    void Collision (Enemy enemy){
                        if (CollisionDetection.Aabb(shot.Shape.AsDynamicShape(), enemy.Shape).Collision){
                            shot.DeleteEntity();
                            enemy.DeleteEntity();
                            AddExplosion(enemy.Shape.Position.X,enemy.Shape.Position.Y,
                                enemy.Shape.Extent.X,enemy.Shape.Extent.Y);
                            score.AddPoint();
                                }
                            }
                    enemies.Iterate(Collision);
                    }
                }
            playerShots.Iterate(IterateForShoots);
        }

        public void ExitGameOver(){
            void checkExit(Enemy enemy){
                if(enemy.Shape.Position.Y <= 0.08f){
                    gameState = GameState.GameOver;
                }
            }
            enemies.Iterate(checkExit);
        }
        public void AddExplosion(float posX, float posY, float extentX, float extentY) {
            explosions.AddAnimation(
            new StationaryShape(posX, posY, extentX, extentY), explosionLength,
            new ImageStride(explosionLength / 8, explosionStrides));
            }
        public void GameLoop(){
            while (win.IsRunning()){
                gameTimer.MeasureTime();
                if (gameState == GameState.GamePlaying){
                    while (gameTimer.ShouldUpdate()){
                        eventBus.ProcessEvents();
                        win.PollEvents();
                        UpdateEnemies();
                        player.Move();
                        IterateShots();
                        ExitGameOver();
                        currentMoveStrategy.MoveEnemies(enemies);
                    }
                    if (gameTimer.ShouldRender()){
                    win.Clear();
                    player.Entity.RenderEntity();
                    enemies.RenderEntities();
                    playerShots.RenderEntities();
                    explosions.RenderAnimations();
                    score.RenderScore();
                    win.SwapBuffers();
                        }
                    if (gameTimer.ShouldReset()){
                    win.Title = "Galaga | Ups: " + gameTimer.CapturedUpdates + ", FPS: " + gameTimer.CapturedFrames;
                        }
                } else {
                        win.Clear();
                        GameOverDisPlay.RenderGameOverDisPlay(score.score);
                        win.SwapBuffers();
                        while (gameTimer.ShouldUpdate()) {
                            eventBus.ProcessEvents();
                            win.PollEvents();
                        }
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