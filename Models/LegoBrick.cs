using System;
using System.Drawing;
using static smartFactoryApp.Models.Parameters;
using static smartFactoryApp.Models.ViewParameters;

namespace smartFactoryApp.Models
{
    public class LegoBrick : Beweger
    {
        private LegoColor color;
        private LegoSize size; 
        public LegoColor Color { get {return color;} set {color = value;}}
        public LegoSize Size { get {return size;} set {size = value;}}

        public bool IsDocked { get; set; } = false;
        
        public Pose LegoPose { get; }

        public LegoBrick(Pose pose, LegoColor color, LegoSize size) : base(pose)
        {
            Color = color;
            Size = size;
            LegoPose = pose;
        }
    }
}