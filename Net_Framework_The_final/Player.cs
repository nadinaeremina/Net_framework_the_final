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
        public Player()
        {
        }

        public string First_name { get; set; }
        public string Last_name { get; set; }
        public int Age { get; set; }
        public string Rating { get; set; }
        public int Number { get; set; }
        public override string ToString()
        {
            return $" {Number}) {First_name} {Last_name} {Age} лет {Rating} очков";
        }
        public int CompareTo(object obj) 
        {
            if (obj is Player)
                return Rating.CompareTo((obj as Player).Rating); 
            throw new NotImplementedException();
        }
    }
}
