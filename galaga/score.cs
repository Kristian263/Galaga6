using DIKUArcade.Graphics;
using DIKUArcade.Math;

namespace galaga{

    public class Score{
        public int score {get; private set;}
        private Text display;

        public Score(Vec2F position, Vec2F extent){
            score = 0;
            display = new Text(score.ToString(), position, extent);
        }

        public void AddPoint(){
            score += 1;
        }

        public void RenderScore() {
            display.SetText(string.Format("Score: {0}", score.ToString()));
            display.SetColor(new Vec3I(255,0,0));
            display.RenderText();
        }
    }
}