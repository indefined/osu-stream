using System;
using osum.GameplayElements.Scoring;
using osum.Graphics.Sprites;
using osum.Graphics.Skins;
using osum.Helpers;
using OpenTK;
using OpenTK.Graphics;
using System.Collections;
using System.Collections.Generic;
using osum.Audio;
using osum.GameModes.SongSelect;
using osum.Graphics;
using osum.Online;
using osu_common.Helpers;
using osum.GameplayElements;
namespace osum.GameModes
{
    public class Ranking : GameMode
    {
        internal static Score RankableScore;

        List<pDrawable> fillSprites = new List<pDrawable>();
        List<pDrawable> fallingSprites = new List<pDrawable>();
        List<pDrawable> countSprites = new List<pDrawable>();
        List<pDrawable> resultSprites = new List<pDrawable>();

        const int colour_change_length = 500;
        const int end_bouncing = 600;
        const int time_between_fills = 600;

        const float fill_height = 80;
        float count_height = 80;

        internal override void Initialize()
        {

            pText performance = new pText("Play Performance", 50, new Vector2(0, 0), 1, true, Color4.White);
            performance.Alpha = 0.2f;
            performance.Additive = true;
            performance.Bold = true;
            spriteManager.Add(performance);

            pDrawable fill = pSprite.FullscreenWhitePixel;
            fill.AlignToSprites = true;
            fill.Scale.X *= (float)RankableScore.count300 / RankableScore.totalHits;
            fill.Colour = new Color4(1, 0.63f, 0.01f, 1);
            fillSprites.Add(fill);

            pSpriteText count = new pSpriteText(RankableScore.count300.ToString(), "default", 0, FieldTypes.Standard, OriginTypes.BottomRight, ClockTypes.Mode, new Vector2(0, count_height), 1, true, Color4.White);
            countSprites.Add(count);

            count_height -= 20;

            fill = pSprite.FullscreenWhitePixel;
            fill.AlignToSprites = true;
            fill.Scale.X *= (float)RankableScore.count100 / RankableScore.totalHits;
            fill.Colour = new Color4(0.55f, 0.84f, 0, 1);
            fillSprites.Add(fill);

            count = new pSpriteText(RankableScore.count100.ToString(), "default", 0, FieldTypes.Standard, OriginTypes.BottomRight, ClockTypes.Mode, new Vector2(0, count_height), 1, true, Color4.White);
            countSprites.Add(count);

            count_height -= 20;

            fill = pSprite.FullscreenWhitePixel;
            fill.AlignToSprites = true;
            fill.Scale.X *= (float)RankableScore.count50 / RankableScore.totalHits;
            fill.Colour = new Color4(0.50f, 0.29f, 0.635f, 1);
            fillSprites.Add(fill);

            count = new pSpriteText(RankableScore.count50.ToString(), "default", 0, FieldTypes.Standard, OriginTypes.BottomRight, ClockTypes.Mode, new Vector2(0, count_height), 1, true, Color4.White);
            countSprites.Add(count);

            count_height -= 20;

            fill = pSprite.FullscreenWhitePixel;
            fill.AlignToSprites = true;
            fill.Scale.X *= (float)RankableScore.countMiss / RankableScore.totalHits;
            fill.Colour = new Color4(0.10f, 0.10f, 0.10f, 1);
            fillSprites.Add(fill);

            count = new pSpriteText(RankableScore.countMiss.ToString(), "default", 0, FieldTypes.Standard, OriginTypes.BottomRight, ClockTypes.Mode, new Vector2(0, count_height), 1, true, Color4.White);
            countSprites.Add(count);

            int i = 0;

            foreach (pDrawable p in countSprites)
            {
                p.ScaleScalar = 0.6f;
                p.Alpha = 0;
                p.Additive = true;

                int offset = i++ * time_between_fills;
                p.Transform(new Transformation(TransformationType.Fade, 0, 1 - i * 0.2f, offset, offset + 100));
            }


            i = 0;

            foreach (pDrawable p in fillSprites)
            {
                p.Alpha = 1;
                //p.Additive = true;
                p.DrawDepth = 0.98f;
                p.AlwaysDraw = true;

                int offset = i++ * time_between_fills;

                p.Transform(new Transformation(Color4.Gray, Color4.Gray, 0, end_bouncing + offset));
                p.Transform(new Transformation(Color4.White, p.Colour, end_bouncing + offset, end_bouncing + colour_change_length + offset));
                //force the initial colour to be an ambiguous gray.

                p.Transform(new TransformationBounce(offset, offset + end_bouncing, p.Scale.X, p.Scale.X, 5));
            }

            spriteManager.Add(fillSprites);
            spriteManager.Add(countSprites);

            const float font_size = 40;

            const float header_offset = 0;
            const float number_offset = 0;

            performance = new pText("Total Score", font_size, new Vector2(header_offset, 100), 0.5f, true, Color4.SkyBlue);
            performance.Origin = OriginTypes.TopRight;
            performance.Field = FieldTypes.StandardSnapRight;
            performance.Additive = true;
            resultSprites.Add(performance);

            count = new pSpriteText(RankableScore.totalScore.ToString("#,0"), "score", 0, FieldTypes.Standard, OriginTypes.TopLeft, ClockTypes.Mode, new Vector2(number_offset, 140), 1, true, Color4.White);
            count.Origin = OriginTypes.TopRight;
            count.Field = FieldTypes.StandardSnapRight;
            resultSprites.Add(count);

            performance = new pText("Avg. Timing (" + (RankableScore.hitOffsetMilliseconds > 0 ? "Late" : "Early") + ")", font_size, new Vector2(header_offset, 100), 0.5f, true, Color4.SkyBlue);
            performance.Additive = true;
            resultSprites.Add(performance);

            float avg = (float)RankableScore.hitOffsetMilliseconds / RankableScore.hitOffsetCount;

            count = new pSpriteText(avg.ToString("00.00"), "score", 0, FieldTypes.Standard, OriginTypes.TopLeft, ClockTypes.Mode, new Vector2(number_offset, 140), 1, true, Color4.White);
            resultSprites.Add(count);

            performance = new pText("Accuracy", font_size, new Vector2(header_offset, 200), 0.5f, true, Color4.SkyBlue);
            performance.Origin = OriginTypes.TopRight;
            performance.Field = FieldTypes.StandardSnapRight;
            performance.Additive = true;
            resultSprites.Add(performance);

            count = new pSpriteText((RankableScore.accuracy * 100).ToString("00.00") + "%", "score", 0, FieldTypes.Standard, OriginTypes.TopLeft, ClockTypes.Mode, new Vector2(number_offset, 240), 1, true, Color4.White);
            count.Origin = OriginTypes.TopRight;
            count.Field = FieldTypes.StandardSnapRight;
            resultSprites.Add(count);

            performance = new pText("Max Combo", font_size, new Vector2(header_offset, 300), 0.5f, true, Color4.SkyBlue);
            performance.Origin = OriginTypes.TopRight;
            performance.Field = FieldTypes.StandardSnapRight;
            performance.Additive = true;
            resultSprites.Add(performance);

            count = new pSpriteText(RankableScore.maxCombo.ToString("#,0") + "x", "score", 0, FieldTypes.Standard, OriginTypes.TopLeft, ClockTypes.Mode, new Vector2(number_offset, 340), 1, true, Color4.White);
            count.Origin = OriginTypes.TopRight;
            count.Field = FieldTypes.StandardSnapRight;
            resultSprites.Add(count);

            foreach (pDrawable p in resultSprites)
            {
                p.Alpha = 0;
                spriteManager.Add(p);
            }

            //add a temporary button to allow returning to song select
            s_ButtonBack = new BackButton(returnToSelect);
            s_ButtonBack.Alpha = 0;
            spriteManager.Add(s_ButtonBack);
        }

        void returnToSelect(object sender, EventArgs args)
        {
            Director.ChangeMode(OsuMode.SongSelect);
        }

        public override void Dispose()
        {
            BeatmapInfo bmi = BeatmapDatabase.GetBeatmapInfo(Player.Beatmap, Player.Difficulty);
            if (RankableScore.totalScore > bmi.HighScore)
                bmi.HighScore = RankableScore.totalScore;
            
            AudioEngine.Music.Unload();
            base.Dispose();
        }

        public Ranking()
        {
        }

        bool wasBouncing = true;
        private BackButton s_ButtonBack;
        public override void Update()
        {
            bool bouncing = fillSprites[3].Transformations.Count > 1;

            if (bouncing != wasBouncing)
            {
                pDrawable flash = pSprite.FullscreenWhitePixel;
                flash.Additive = false;
                flash.FadeOutFromOne(500);
                spriteManager.Add(flash);

                resultSprites.ForEach(s => s.Alpha = 1);

                //we should move this to happen earlier but delay the ranking dialog from displaying until after animations are done.
                OnlineHelper.SubmitScore(CryptoHelper.GetMd5String(Player.Beatmap.BeatmapFilename + "-" + Player.Difficulty.ToString()), RankableScore.totalScore);

                s_ButtonBack.FadeIn(150);
            }

            base.Update();

            wasBouncing = bouncing;


            //set the x scale back to the default value (override the bounce transformation).
            float lastPos = 0;

            for (int i = 0; i < fillSprites.Count; i++)
            {
                pDrawable fill = fillSprites[i];
                pDrawable count = countSprites[i];

                fill.Scale.Y = bouncing ? GameBase.BaseSizeFixedWidth.Height : fill_height;

                if (lastPos != 0) fill.Position.X = lastPos;
                lastPos = fill.Position.X + fill.Scale.X + 1;

                count.Position.X = lastPos - 3;
            }

            if (!bouncing)
            {

                float pos = (float)GameBase.Random.NextDouble() * GameBase.BaseSizeFixedWidth.Width;

                pTexture tex = null;
                if (pos < fillSprites[1].Position.X)
                    tex = TextureManager.Load(OsuTexture.hit300);
                else if (pos < fillSprites[2].Position.X)
                    tex = TextureManager.Load(OsuTexture.hit100);
                else if (pos < fillSprites[3].Position.X)
                    tex = TextureManager.Load(OsuTexture.hit50);

                fallingSprites.RemoveAll(p => p.Alpha == 0);
                foreach (pSprite p in fallingSprites)
                {
                    p.Position.Y += (p.Position.Y - p.StartPosition.Y + 1) * 0.05f;
                }


                if (tex != null)
                {
                    pSprite f = new pSprite(tex, FieldTypes.Standard, OriginTypes.TopLeft, ClockTypes.Mode, new Vector2(pos, fill_height - 25), 1f, false, Color4.White);
                    f.ScaleScalar = 0.2f;
                    f.Transform(new Transformation(TransformationType.Fade, 0, 1, Clock.ModeTime, Clock.ModeTime + 150));
                    f.Transform(new Transformation(TransformationType.Fade, 1, 0, Clock.ModeTime + 250, Clock.ModeTime + 1000 + (int)(GameBase.Random.NextDouble() * 1000)));
                    fallingSprites.Add(f);
                    spriteManager.Add(f);
                }
            }

        }

    }
}

