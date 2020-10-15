using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRAPITest
{
    
        

        public class Root
        {
            public string version { get; set; }
            public Flags flags { get; set; }
            public List<Shape> shapes  { get; set; }
            public string imagePath { get; set; }
            public string imageData { get; set; }
            public int imageHeight { get; set; }
            public int imageWidth { get; set; }
            
    }

        public class Flags2
        {
            public bool Truncated { get; set; }
            public bool Curved { get; set; }
            public bool Slanted { get; set; }
        }
        public class Flags
        {
        }

        public class Shape
        {
            public string label { get; set; }
            public List<List<double>> points { get; set; }
            public string group_id { get; set; }
            public string text_field { get; set; }
            public string shape_type { get; set; }
            public Flags2 flags { get; set; }
        }

       
      
       
       


    }

