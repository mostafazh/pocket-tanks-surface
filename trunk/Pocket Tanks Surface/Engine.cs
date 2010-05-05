using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace Pocket_Tanks_Surface
{
    public class Engine
    {
        private static Engine instance;

        private static Game game;
        private Player _p1;
        public Player P1 { get { return _p1; } }
        
        private Player _p2;
        public Player P2 { get { return _p2; } }

        private Player _InTurn;
        public Player InTurn { get { return _InTurn; } }

        private Engine(Game game)
        {
            _p1 = new Player(game, "Player 1", Color.Red ,180,new Vector2(1024-50,609));
            _p2 = new Player(game, "Player 2", Color.Blue, 0, new Vector2(0, 609));
            _InTurn = P1;
            P1.tank.moveEnded += new moveEndedEvent(tank_moveEnded);
            P2.tank.moveEnded += new moveEndedEvent(tank_moveEnded);
        }

        public static void SetGame(Game g)
        {
            if (game == null)
                game = g;
        }

        [MethodImplAttribute(MethodImplOptions.Synchronized)] 
        public static Engine GetInstance()
        {
            if(instance == null)
                instance = new Engine(game);
            return instance;
        }

        private void NextTurn()
        {
            _InTurn = _InTurn == P1 ? P2 : P1;
        }

        [MethodImplAttribute(MethodImplOptions.Synchronized)] 
        public void Move(Vector2 newPosition)
        {
            InTurn.tank.Move(newPosition);
        }

        private void tank_moveEnded()
        {
            Console.WriteLine("Move Ended");
            NextTurn();
        }
    }
}
