using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Audio;
using Fusion.Content;
using Fusion.Graphics;
using Fusion.Input;
using Fusion.Development;
using Microsoft.SqlServer.Server;
using BVector3 = BEPUutilities.Vector3;
using Vector3 = Fusion.Vector3;


namespace ExampleFlight
{
    public class Server : GameService
    {

        Aircraft jetAircraft;
        Environment environment;
        World physicsWorld;
        GraphicsDevice graphicsDevice;

        public Server(Game game, GraphicsDevice graphicsDevice) : base(game)
        {
            //this.game = game;
            this.graphicsDevice = graphicsDevice;

        }

        public void Init()
        {
            //initialization
            //...
            jetAircraft = new Aircraft(this.Game, physicsWorld, graphicsDevice);
            environment = new Environment(this.Game, physicsWorld, graphicsDevice);
        }

        public void Update(GameTime gameTime)
        {
            //Updating
            //...
            jetAircraft.Update(gameTime);


        }

        public void Draw(GameTime gameTime)
        {
            //Drawing
            //...
            jetAircraft.Draw(gameTime);
            environment.Draw(gameTime);

        }


    }
    
}
