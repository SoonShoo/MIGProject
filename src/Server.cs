using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using ExampleFlight.src.Model;
using Fusion;
using Fusion.Audio;
using Fusion.Content;
using Fusion.Graphics;
using Fusion.Input;
using Fusion.Development;
using Microsoft.SqlServer.Server;
using BVector3 = BEPUutilities.Vector3;
using Vector3 = Fusion.Mathematics.Vector3;


namespace ExampleFlight
{
    public class Server : GameService
    {
        GraphicsDevice graphicsDevice;
   
        private Space space;
        private Field field;
        private SphereObject sphere;
        private Car car;
        private Player player;
        private Environment environment;

        private const float scale = 7000;

        public Server(Game game, GraphicsDevice graphicsDevice) : base(game)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public void Init()
        {
            space=new Space();
            space.ForceUpdater.Gravity = new BVector3(0, 0, -9.81f);

            field = new Field(Game, space, graphicsDevice, scale);
            environment = new Environment(Game, graphicsDevice, scale);

            sphere = new SphereObject(Game, space, graphicsDevice);
            
            car = new Car(Game, graphicsDevice, new BVector3(0,0,0));
            car.AddToSpace(space);

            player = new Player(car);
        }

        public void Update(GameTime gameTime, DebugRender dr, InputDevice device)
        {
            space.Update(gameTime.ElapsedSec * 10f);

            field.Update(gameTime, dr);

            sphere.Update(gameTime, dr, device);
            player.Control(gameTime, device);
            car.Update(gameTime, dr, device);
        }

        public override void Draw(GameTime gameTime, StereoEye stereoEye)
        {
            //Drawing
            //...
            car.draw(stereoEye);
            //field.DrawModel(stereoEye);
            environment.draw();
            base.Draw(gameTime, stereoEye);
        }


    }
    
}
