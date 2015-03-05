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

        Aircraft jetAircraft;
        private Carrier carrier;

        Environment environment;
        World physicsWorld;
        GraphicsDevice graphicsDevice;
        
        private Space space;
        private Field field;
        private SphereObject sphere;
        private Car car;

        public Server(Game game, GraphicsDevice graphicsDevice) : base(game)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public void Init()
        {
            //initialization
            //...
            jetAircraft = new Aircraft(this.Game, physicsWorld, graphicsDevice);

            //environment = new Environment(this.Game, physicsWorld, graphicsDevice);
            
            space=new Space();
            space.ForceUpdater.Gravity = new BVector3(0, 0, -9.81f);
            
            field = new Field(Game, space, graphicsDevice);

            sphere = new SphereObject(Game, space, graphicsDevice);
            
            car = new Car(Game, graphicsDevice, BVector3.Zero);
            car.AddToSpace(space);

            //environment = new Environment(this.Game, physicsWorld, graphicsDevice);
           // carrier = new Carrier(this.Game, physicsWorld, graphicsDevice);
        }

        public void Update(GameTime gameTime, DebugRender dr, InputDevice device)
        {
            //Updating
            //...

            space.Update(gameTime.ElapsedSec * 10f);
            //jetAircraft.Update(gameTime);
            field.Update(gameTime, dr);
            sphere.Update(gameTime, dr, device);
            car.Update(gameTime, dr, device);
            //jetAircraft.Update(gameTime);
            //carrier.Update(gameTime);

        }

        public override void Draw(GameTime gameTime, StereoEye stereoEye)
        {
            //Drawing
            //...
            //field.DrawModel(1.0f, stereoEye);
            //jetAircraft.Draw(gameTime, stereoEye);
            car.draw(stereoEye);
            //environment.Draw(gameTime, stereoEye);
            
            //jetAircraft.Draw(gameTime);
            //carrier.Draw(gameTime);
           // environment.Draw(gameTime);
            base.Draw(gameTime, stereoEye);
        }


    }
    
}
