using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace gInkPlus
{
    public class TextObject
    {
        public string Text { get; set; }
        public Point Location { get; set; }
        public Font Font { get; set; }
        public Color Color { get; set; }
        public int Transparency { get; set; }
    }

}
