using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

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

        public Player(string name,Color color)
        {
            this.name = name;
            this.color = color;
            this._number = count++;
            Players.AddLast(this);
        }

        public static Player GetPlayer(int index)
        {
            return Players.ElementAt<Player>(index);
        }
    }
}
