using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace galaga{

    public class GameOverDisPlay{
        private Text display;

        public GameOverDisPlay(){
            display = new Text("GameOver", new Vec2F(0.5f, 0.5f),
                new Vec2F (0.4f,0.4f));
        }

        public void RenderGameOverDisPlay(int score) {
            display.SetText(string.Format("GameOver \n {0}", score.ToString()));
            display.SetColor(new Vec3I(255,0,0));
            display.RenderText();
        }
    }
}