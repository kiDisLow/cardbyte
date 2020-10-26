using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OCRAPITest
{
    class textprocessor
    {
        public string[] nameprocessor(int x, int y, Rootobject rootobject)
        {
            string s1 = getstring(x, y, rootobject);
            string[]  s2 = s1.Split(' ');
            List<string> returnstring = new List<string>();
            

           if (s2[0].Length < 3)
            {  
                returnstring.Add(s2[0]);// rs0 done
              for(int i=1;i<s2.Count();i++)
                {
                    returnstring.Add(s2[i]);
                }


            }
            else
            {
                returnstring.Add(null);
                for (int i = 0; i < s2.Count(); i++)
                {
                    returnstring.Add(s2[i]);
                }           

            }
            for (int i = returnstring.Count(); i < 4; i++)
            {
                returnstring.Add(null);
            }



            return returnstring.ToArray();

        }

        public List<string> adressprocessor(int x,int y,Rootobject rootobject)
        {
            string s1 = getstring(x, y, rootobject);
            List<string> s2 =s1.Split(' ').ToList();
            for(int i=s2.Count();i<13;i++)
            {
                s2.Add(null);
                
            }

            return s2;

        }
        public string emailprocessor(int x, int y, Rootobject rootobject)
        {
            return getWord(x,y,rootobject);

        }

        public string mobileprocessor(int x, int y, Rootobject rootobject)
        {
            string s1= getWord(x, y, rootobject);
           
           

            return s1;

        }


        public string getstring(int x, int y, Rootobject ocrResult) 
        {

            if (ocrResult.ParsedResults != null)
            for (int i = 0; i < ocrResult.ParsedResults.Count(); i++)
            {
                for (int k = 0; k < ocrResult.ParsedResults[i].textOverlay.lines.Count(); k++)
                {
                   

                    for (int j = 0; j < ocrResult.ParsedResults[i].textOverlay.lines[k].words.Count(); j++)
                    {
                        float left = (int)(ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Left);
                        
                        float top = (int)(ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Top );
                       
                        float width = (int)(ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Width);
                        
                      
                        float hight = (int)(ocrResult.ParsedResults[i].textOverlay.lines[k].MaxHeight);
                        if (x>left && x<left+width)
                        {
                            if( y>top &&  y<(top+hight))
                            {
                                return ocrResult.ParsedResults[i].textOverlay.lines[k].LineText;
                            }
                        }

                       
                    }

                }
            }
            return "vaibhav is the best boy in the world and will be the best";
        }

        public List<List<double>> pointfinder(string s1, Rootobject ocrResult,myobject myobject1)
        {
            List<List<double>> points = new List<List<double>>();
            List<double> p1 = new List<double>();
            p1.Add(0);
            p1.Add(0);
            int x = myobject1.mousex;
            int y = myobject1.mousey;

            for (int i = 0; i < ocrResult.ParsedResults.Count(); i++)
            {
                for (int k = 0; k < ocrResult.ParsedResults[i].textOverlay.lines.Count(); k++)
                {

                    for (int j = 0; j < ocrResult.ParsedResults[i].textOverlay.lines[k].words.Count(); j++)
                    {
                       
                        float left = (int)(ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Left);

                        float top = (int)(ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Top);

                        float width = (int)(ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Width);

                        float hight = (int)(ocrResult.ParsedResults[i].textOverlay.lines[k].MaxHeight);

                       // if (x > left && x < left + width)
                        {
                            if (myobject1.label=="others" || y > top && y < (top + hight))
                            {
                                if (ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].WordText == s1)
                                {
                                    double lft1 = ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Left ;
                                    double top1 = ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Top ;
                                    double hight1 = ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Height ;
                                    double width1 = ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Width ;
                                    // p1 = new List<double>();
                                    //  p1[0] = lft;
                                    //  p1[1] = top - hight;
                                    points.Add(new List<double> { lft1, top1 + hight1 });//top point left

                                    // p1[0] = lft;
                                    // p1[1] = top;
                                    // points.Add(p1);
                                    points.Add(new List<double> { lft1, top1 });//bottom point lft

                                    // p1[0] = lft+width;
                                    // p1[1] = top;
                                    // points.Add(p1);
                                    points.Add(new List<double> { lft1 + width1, top1 });//bottom point right

                                    // p1[0] = lft+width;
                                    // p1[1] = top-hight;
                                    // points.Add(p1);
                                    points.Add(new List<double> { lft1 + width1, top1 + hight1 });//top point right
                                    x = (int)(left + width/2);
                                    y = (int)(top + hight / 2);
                                }
                            }
                        }


                    }

                }
            }

          


            return points;
        }

        public string getWord(int x,int y, Rootobject ocrResult)
        {
            for (int i = 0; i < ocrResult.ParsedResults.Count(); i++)
            {
                for (int k = 0; k < ocrResult.ParsedResults[i].textOverlay.lines.Count(); k++)
                {


                    for (int j = 0; j < ocrResult.ParsedResults[i].textOverlay.lines[k].words.Count(); j++)
                    {
                        float left = (int)(ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Left);

                        float top = (int)(ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Top);

                        float width = (int)(ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].Width);

                        float hight = (int)(ocrResult.ParsedResults[i].textOverlay.lines[k].MaxHeight);
                        if (x > left && x < left + width)
                        {
                            if (y > top && y < (top + hight))
                            {
                                return ocrResult.ParsedResults[i].textOverlay.lines[k].words[j].WordText;
                            }
                        }


                    }

                }

            }

                return "";
        }

    }
}
