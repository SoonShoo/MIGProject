using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using Fusion;
using Fusion.Audio;
using Fusion.Content;
using Fusion.Graphics;
using Fusion.Input;
using System.Runtime.InteropServices;
using GraphicsDevice = Fusion.Graphics.GraphicsDevice;
using Matrix = Fusion.Matrix;
using SpriteBatch = Fusion.Graphics.SpriteBatch;
using Vector4 = Fusion.Vector4;
//using Vector3BEPU = BEPUutilities.Vector3;
using Fusion.Development;


namespace MIGProject
{
    public class MIGProject : Game
    {


        ModelsOfScene migPlane;
        Game game;
      //  ConstData cbData;
        Camera cam;
        GraphicsDevice graphicDevice;
        private GameTime gameTimeGlobal;


        /// <summary>
        /// MIGProject constructor
        /// </summary>
        public MIGProject()
            : base()
        {
            //	enable object tracking :
            Parameters.TrackObjects = false;
            Parameters.VSyncInterval	=	0;
            Parameters.MsaaLevel = 1;

            //	uncomment to enable debug graphics device:
            //	(MS Platform SDK must be installed)
            //	Parameters.UseDebugDevice	=	true;

            //	add services :
            AddService(new SpriteBatch(this), false, false, 0, 0);
            AddService(new DebugStrings(this), true, true, 9999, 9999);
            AddService(new DebugRender(this), true, true, 9998, 9998);
            AddService(new Camera(this), true, false, 1, 1);


            //	add here additional services :

            //	load configuration for each service :
            LoadConfiguration();

            //	make configuration saved on exit :
            Exiting += Game_Exiting;
            InputDevice.KeyDown += InputDevice_KeyDown;
        }


        /// <summary>
        /// Initializes game :
        /// </summary>
        protected override void Initialize()
        {
            //	initialize services :
            base.Initialize();
           
            //GetService<Camera>().FreeCamPosition = Fusion.Vector3.Up * 10;
            var device = GraphicsDevice;
            

            migPlane = new ModelsOfScene(this, GraphicsDevice, "mig29.fbx", "render.hlsl", "ice.jpg");
            migPlane.LoadContent();
            Reloading += (s, e) => migPlane.LoadContent();
            

            //	add keyboard handler :
            InputDevice.KeyDown += InputDevice_KeyDown;
            Log.Message("Hello world1");
            migPlane.SetPosition(1.0f, 1.0f, 1.0f);
            GetService<Camera>().FreeCamPosition = Fusion.Vector3.Up * 10;
            //	load content & create graphics and audio resources here:
        }



        /// <summary>
        /// Disposes game
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //	dispose disposable stuff here
                //	Do NOT dispose objects loaded using ContentManager.
            }
            base.Dispose(disposing);
        }



        /// <summary>
        /// Handle keys
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InputDevice_KeyDown(object sender, Fusion.Input.InputDevice.KeyEventArgs e)
        {
            if (e.Key == Keys.F1)
            {
                DevCon.Show(this);
            }

            if (e.Key == Keys.F5)
            {
                Reload();
            }

            if (e.Key == Keys.F12)
            {
                GraphicsDevice.Screenshot();
            }

            if (e.Key == Keys.Escape)
            {
                Exit();
            }
        }



        /// <summary>
        /// Saves configuration on exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Game_Exiting(object sender, EventArgs e)
        {
            SaveConfiguration();
        }



        /// <summary>
        /// Updates game
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            var ds = GetService<DebugStrings>();

            ds.Add(Color.Orange, "FPS {0}", gameTime.Fps);
            ds.Add("F1   - show developer console");
            ds.Add("F5   - build content and reload textures");
            ds.Add("F12  - make screenshot");
            ds.Add("ESC  - exit");



            gameTimeGlobal = gameTime;
            var cam = GetService<Camera>();
            var dr = GetService<DebugRender>();
            dr.View = cam.ViewMatrix;
            dr.Projection = cam.ProjMatrix;
            dr.DrawGrid(10);


            base.Update(gameTime);

            //	Update stuff here :
        }



        /// <summary>
        /// Draws game
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="stereoEye"></param>
        protected override void Draw(GameTime gameTime, StereoEye stereoEye)
        {
            //var sb = GetService<SpriteBatch>();
            GraphicsDevice.ClearBackbuffer(new Color4(0, 0, 0, 0));
            migPlane.DrawModel(this, gameTime, 0.0003f);

            base.Draw(gameTime, stereoEye);
            
            //	Draw stuff here :
        }
    }
}
