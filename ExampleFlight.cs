using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Audio;
using Fusion.Content;
using Fusion.Graphics;
using Fusion.Input;
using Fusion.Development;
using Fusion.Mathematics;

namespace ExampleFlight
{
    public class ExampleFlight : Game
    {

        /// <summary>
        /// SceneDemo constructor
        /// </summary>
        

        public ExampleFlight()
            : base()
        {
            //	enable object tracking :
            Parameters.TrackObjects = false;
            Parameters.VSyncInterval = 0;
            Parameters.MsaaLevel = 1;

            //	add services :
            AddService(new SpriteBatch(this), false, false, 0, 0);
            AddService(new DebugStrings(this), true, true, 9999, 9999);
            AddService(new DebugRender(this), true, true, 9998, 9998);
            AddService(new Camera(this), true, false, 1, 1);

            //	load configuration :
            LoadConfiguration();

            //	make configuration saved on exit
            Exiting += FusionGame_Exiting;
            InputDevice.KeyDown += InputDevice_KeyDown;

        }


        ModelOfScene migPlane;
        Server server;
        ConstantBuffer constBuffer;

        /// <summary>
        /// Add services :
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            server = new Server(this, GraphicsDevice);
            server.Init();
            GetService<Camera>().FreeCamPosition = Vector3.Up;


            GetService<Camera>().FreeCamPosition = Vector3.Up * 10;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (constBuffer != null)
                {
                    constBuffer.Dispose();
                }
            }
            base.Dispose(disposing);
        }



        /// <summary>
        /// Handle keys for each demo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void InputDevice_KeyDown(object sender, Fusion.Input.InputDevice.KeyEventArgs e)
        {
            if (e.Key == Keys.F1)
            {
                DevCon.Show(this);
            }

            if (e.Key == Keys.F2)
            {
                GetService<Camera>().ToggleFlyMode();
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
        /// Save configuration on exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FusionGame_Exiting(object sender, EventArgs e)
        {
            SaveConfiguration();
        }



        /// <summary>
        /// 
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

            var cam = GetService<Camera>();
            var dr = GetService<DebugRender>();
            dr.View = cam.GetViewMatrix(StereoEye.Mono);
            dr.Projection = cam.GetProjectionMatrix(StereoEye.Mono);
            server.Update(gameTime, dr, InputDevice);
            dr.DrawGrid(10);
            base.Update(gameTime);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="stereoEye"></param>
        protected override void Draw(GameTime gameTime, StereoEye stereoEye)
        {
           // migPlane.DrawModel();
            GraphicsDevice.ClearBackbuffer(Color.CornflowerBlue, 1, 0);

            server.Update(gameTime);
            server.Draw(gameTime, stereoEye);
            base.Draw(gameTime, stereoEye);
        }
    }
}
