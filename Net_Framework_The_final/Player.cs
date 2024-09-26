using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace English_for_kids
{
    [Serializable]
    public class Player:IComparable
    {
        public string Nickname { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public int Rating { get; set; }
        public int Number { get; set; }
        public Player()  { }
        public override string ToString()
        {
            return $" {Number}) {Nickname} {Age} лет {Rating} очков";
        }
        public int CompareTo(object obj) 
        {
            if (obj is Player)
                return Rating.CompareTo((obj as Player).Rating); 
            throw new NotImplementedException();
        }
    }
}
