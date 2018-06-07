﻿using System;
using LibreLancer.GameData;
using Neo.IronLua;
namespace LibreLancer
{
    public class LuaMenu : GameState
    {
        XmlUIManager ui;
        IntroScene intro;
        Cutscene scene;
        Cursor cur;
        public LuaMenu(FreelancerGame g) : base(g)
        {
            ui = new XmlUIManager(g, "menu", new LuaAPI(this), g.GameData.GetInterfaceXml("mainmenu"));
            ui.Enter();
            g.GameData.PopulateCursors();
            g.CursorKind = CursorKind.None;
            intro = g.GameData.GetIntroScene();
            scene = new Cutscene(intro.Scripts, Game);
            scene.Update(TimeSpan.FromSeconds(1f / 60f)); //Do all the setup events - smoother entrance
            cur = g.ResourceManager.GetCursor("arrow");
            GC.Collect(); //crap
            g.Sound.PlayMusic(intro.Music);
        }

        class LuaAPI
        {
            LuaMenu state;
            public LuaAPI(LuaMenu m) => state = m;
            public void newgame() => state.ui.Leave(() =>
            {
                state.Game.ChangeState(new SpaceGameplay(state.Game, new GameSession(state.Game)));
            });
            public void loadgame() => state.ui.Leave(() =>
            {
                state.Game.ChangeState(new DemoSystemView(state.Game));
            });
            public void multiplayer() {}
            public void exit() => state.ui.Leave(() => state.Game.Exit());
        }

        public override void Draw(TimeSpan delta) 
        {
            scene.Draw();
            ui.Draw(delta);
            Game.Renderer2D.Start(Game.Width, Game.Height);
            cur.Draw(Game.Renderer2D, Game.Mouse);
            Game.Renderer2D.Finish();
        }


        int uframe = 0;
        public override void Update(TimeSpan delta)
        {
            if (uframe < 3)
            { //Allows animations to play correctly
                uframe++;
                ui.Update(TimeSpan.Zero);
            }
            else
            {
                ui.Update(delta);
                scene.Update(delta);
            }
        }

        public override void Unregister()
        {
            ui.Dispose();
            scene.Dispose();
        }
    }
}