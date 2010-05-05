using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pocket_Tanks_Surface
{
    public class Player
    {
        public Color color {get; set;}
        public string name {get; set;}
        private int _number;
        public int number {get {return _number;}}
        private static int count = 1;
        private static LinkedList<Player> Players = new LinkedList<Player>();
        public Tank tank;

        public Player(Game game,string name,Color color,float rotation, Vector2 position)
        {
            this.name = name;
            this.color = color;
            this._number = count++;
            Players.AddLast(this);
            tank = new Tank(game, this, rotation, position);
        }

        public static Player GetPlayer(int index)
        {
            return Players.ElementAt<Player>(index);
        }

        public bool CanMove()
        {
            return Engine.Instance.InTurn == this;
        }
    }
}
