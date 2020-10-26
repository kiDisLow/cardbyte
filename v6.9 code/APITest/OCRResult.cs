using System.Collections.Generic;

namespace OCRAPITest
{

    public class Rootobject
    {
        public List<Parsedresult> ParsedResults { get; set; }
        public int OCRExitCode { get; set; }
        public bool IsErroredOnProcessing { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
    }

    public class Parsedresult
    {
        public object FileParseExitCode { get; set; }
        public string ParsedText { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
        public TextOverlay textOverlay { get; set; }
       
    }

    public class TextOverlay
    {
        public List<Lines> lines { get; set; }
        public bool HasOverlay { get; set; }
        
    }

    public class Lines
    {
        public string LineText { get; set; }
        public List<Words> words { get; set; }
        public double MaxHeight { get; set; }
        public double MinTop { get; set; }
    }

    public class Words
    {
        public string WordText { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

    }

}
