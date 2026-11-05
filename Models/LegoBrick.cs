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
        public BrickStatus Status { get; set; } = BrickStatus.Ready;

        // 中央回收线的等待位置编号
        // -1 表示没有占用回收位置
        public int RecyclePlatz { get; set; } = -1;

        public Pose LegoPose { get; }

        // 最初生成这块积木的生产线
        // 回收、抓取和交付时都保留这个来源
        public ProductionLine? OriginLine { get; set; }

        public LegoBrick(Pose pose, LegoColor color, LegoSize size) : base(pose)
        {
            Color = color;
            Size = size;
            LegoPose = pose;
        }
    }

    

}