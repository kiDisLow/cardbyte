using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OCRAPITest
{
    public class diliverymethods
    {
        textprocessor t1 = new textprocessor();
   
        
        public bool addname(Root d1,Rootobject obj, string prefix,string fname,string mname,string lname,myobject myobject1)
        {
           
            if (prefix != "")
            {
                Shape s1 = new Shape();
                s1.label = "NamePrefix";
                s1.shape_type = "Rectangle";
                s1.group_id = "Name";
                s1.text_field = prefix;
                s1.points = new List<List<double>>();
                s1.points.AddRange(t1.pointfinder(prefix, obj,myobject1));

                d1.shapes.Add(s1);

            }
           

            if (fname != "")
            {
                Shape s1 = new Shape();
                s1.label = "FirstName";
                s1.shape_type = "Rectangle";
                s1.group_id = "Name";
                s1.text_field = fname;
                s1.points = new List<List<double>>();
                 foreach(var l1 in t1.pointfinder(fname, obj,myobject1) )
                    {
                    s1.points.Add(l1);
                     }
               // s1.points.Add(t1.pointfinder(fname, obj)[1]);
                  d1.shapes.Add(s1);
              

            }
           



            if (mname != "")
            {
                Shape s1 = new Shape();
                s1.label = "MiddleName";
                s1.shape_type = "Rectangle";
                s1.group_id = "Name";
                s1.text_field = mname;
                s1.points = new List<List<double>>();
                s1.points.AddRange(t1.pointfinder(mname, obj,myobject1));
               
                d1.shapes.Add(s1);
            }
           

            if (lname != "")
            {
                Shape s1 = new Shape();
                s1.label = "LastName";
                s1.shape_type = "Rectangle";
                s1.group_id = "Name";
                s1.text_field = lname;               
                s1.points = new List<List<double>>();
                s1.points.AddRange(t1.pointfinder(lname, obj,myobject1));
                d1.shapes.Add(s1);
            }



            return true;
        }
        public bool addadress(List<myobject> myobj, Root d1, Rootobject obj)
        { 
             foreach(var o1 in myobj)
            {
                Shape  s1 = new Shape();
                if (o1.label == "floore_Field_1" || o1.label == "floore_Field_1" || o1.label == "floore_Field_1" || o1.label == "floore_Field_1")
                {
                    s1.label = o1.label.Replace("floore_", "");
                    s1.text_field = o1.text;
                    s1.shape_type = "Rectangle";
                    s1.group_id = "Floor";
                    s1.points = new List<List<double>>();
                    s1.points.AddRange(t1.pointfinder(o1.text, obj, o1));
                }
                else if (o1.label == "house name _Field_1" || o1.label == "house name _Field_2" || o1.label == "house name _Field_3")
                {
                    s1.label = o1.label.Replace("house name _", "");
                    s1.text_field = o1.text;
                    s1.shape_type = "Rectangle";
                    s1.group_id = "HouseName";
                    s1.points = new List<List<double>>();
                    s1.points.AddRange(t1.pointfinder(o1.text, obj, o1));
                }
                else if (o1.label == "street name  _Field_1" || o1.label == "street name  _Field_2" || o1.label == "street name  _Field_3" || o1.label == "street name  _Field_4" || o1.label == "street name  _Field_5" || o1.label == "street name  _Field_6")
                {
                    s1.label = o1.label.Replace("stree name _", "");
                    s1.text_field = o1.text;
                    s1.shape_type = "Rectangle";
                    s1.group_id = "StreetName";
                    s1.points = new List<List<double>>();
                    s1.points.AddRange(t1.pointfinder(o1.text, obj, o1));
                }
                else if (o1.label == "Locality_Field_1" || o1.label == "Locality_Field_2" || o1.label == "Locality_Field_3" || o1.label == "Locality_Field_4" || o1.label == "Locality_Field_5" || o1.label == "Locality_Field_6" || o1.label == "Locality_Field_7" || o1.label == "Locality_Field_8")
                {
                    s1.label = o1.label.Replace("Locality_", "");
                    s1.text_field = o1.text;
                    s1.shape_type = "Rectangle";
                    s1.group_id = "Locality";
                    s1.points = new List<List<double>>();
                    s1.points.AddRange(t1.pointfinder(o1.text, obj, o1));
                }

                else if (o1.label == "house number")
                {
                    s1.label = "House_number";
                    s1.text_field = o1.text;
                    s1.shape_type = "Rectangle";
                    s1.group_id = "Address";
                    s1.points = new List<List<double>>();
                    s1.points.AddRange(t1.pointfinder(o1.text, obj, o1));
                }


                else if (o1.label != "")
                {
                    s1.label = o1.label;
                    s1.text_field = o1.text;
                    s1.shape_type = "Rectangle";
                    s1.group_id = "Address";
                    s1.points = new List<List<double>>();
                    s1.points.AddRange(t1.pointfinder(o1.text, obj, o1));

                }

                d1.shapes.Add(s1);
            }




            return true;
        }
        public bool addemail(Root d1,Rootobject obj,myobject myobj)
        {
            Shape s1 = new Shape();

            s1.label = "Address";
            s1.shape_type = "Rectangle";
            s1.group_id = "Email";
            s1.text_field = myobj.text;
            s1.points = new List<List<double>>();
            s1.points.AddRange(t1.pointfinder(myobj.searchtext, obj,myobj));
            d1.shapes.Add(s1);


            return true;
        }
        public bool addphone(Root d1, Rootobject obj, myobject myobj)
        {
            Shape s1 = new Shape();

        

                s1.label = "Number";
                s1.shape_type = "Rectangle";
                s1.group_id = myobj.label;
                s1.text_field = myobj.text;
                s1.points = new List<List<double>>();
                s1.points.AddRange(t1.pointfinder(myobj.searchtext, obj, myobj));
                d1.shapes.Add(s1);

            

            return true;
        }

        public bool addcompanyname(Root d1,Rootobject obj,myobject myobj)
        {

            List<string> label = new List<string>() { "Field_1", "Field_2", "Field_3", "Field_4", "Field_5", "Field_6", "Field_7", };
            List<string> text = myobj.text.Split(' ').ToList();
            int i = 0;
            foreach (string str in text)
            {
                Shape s1 = new Shape();
                s1.label = label[i];
                s1.shape_type = "Rectangle";
                s1.group_id = "CompanyName";
                s1.text_field = str;
                s1.points = new List<List<double>>();
                s1.points.AddRange(t1.pointfinder(str, obj,myobj));
                d1.shapes.Add(s1);
                i++;
            }


            return true;
        }
        public bool adddateofbirth(Root d1, Rootobject obj, List<myobject> myobj)
        {
            Shape s1 = new Shape();

            foreach (var o1 in myobj)
            {

                s1.label = o1.label;
                s1.shape_type = "Rectangle";
                s1.group_id = "DateOfBirth";
                s1.text_field = o1.text;
                s1.points = new List<List<double>>();
                s1.points.AddRange(t1.pointfinder(o1.text, obj,o1));
                d1.shapes.Add(s1);

            }

            return true;
        }

        public bool adddesignation(Root d1, Rootobject obj, myobject myobj)
        {
            List<string> label = new List<string>() { "Field_1", "Field_2", "Field_3", "Field_4", "Field_5", "Field_6", "Field_7", };
            List<string> text = myobj.text.Split(' ').ToList();
            int i = 0;
            foreach (string str in text)
            {
                Shape s1 = new Shape();
                s1.label = label[i];
                s1.shape_type = "Rectangle";
                s1.group_id = "Designation";
                s1.text_field = str;
                s1.points = new List<List<double>>();
                s1.points.AddRange(t1.pointfinder(str, obj,myobj));
                d1.shapes.Add(s1);
                i++;
            }


            return true;
        }
        public bool addwebsite(Root d1, Rootobject obj, myobject myobj)
        {
            Shape s1 = new Shape();
            s1.label = "Address";
            s1.shape_type = "Rectangle";
            s1.group_id = "Website";
            s1.text_field = myobj.text;
            s1.points = new List<List<double>>();
            s1.points.AddRange(t1.pointfinder(myobj.searchtext, obj,myobj));
            d1.shapes.Add(s1);



            return true;
        }

        public bool addbusiness(Root d1, Rootobject obj, myobject myobj)
        {
            List<string> label = new List<string>() { "Field_1", "Field_2", "Field_3", "Field_4", "Field_5", "Field_6", "Field_7", "Field_8", "Field_9", "Field_10" };
            List<string> text = myobj.text.Split(' ').ToList();
            int i = 0;
            foreach (string str in text)
            {
                Shape s1 = new Shape();
                s1.label = label[i];
                s1.shape_type = "Rectangle";
                s1.group_id = "BusinessType";
                s1.text_field = str;
                s1.points = new List<List<double>>();
                s1.points.AddRange(t1.pointfinder(str, obj,myobj));
                d1.shapes.Add(s1);
                i++;
            }


            return true;
        }

        public bool addother(Root d1, Rootobject obj, myobject myobj)
        {
            List<string> label = new List<string>() { "Field_1", "Field_2", "Field_3", "Field_4", "Field_5", "Field_6", "Field_7", "Field_8", "Field_9", "Field_10", "Field_11", "Field_12", "Field_13", "Field_14", "Field_15", "Field_16", "Field_17", "Field_18", "Field_19", "Field_20", "Field_21" };
            int i = 0;
            
            for(int j=0;j<d1.shapes.Count();j++)
            {
                if (d1.shapes[j].group_id == "Others")
                    i++;

            }
           
                Shape s1 = new Shape();
                s1.label = label[i];
                s1.shape_type = "Rectangle";
                s1.group_id = "Others";
                s1.text_field = myobj.text;
                s1.points = new List<List<double>>();
                s1.points.AddRange(t1.pointfinder(myobj.text, obj,myobj));
                d1.shapes.Add(s1);
                
            


            return true;
        }



        public bool addcard(Root d1, Rootobject obj, myobject myobj)
        {
            Shape s1 = new Shape();
            s1.label = "CardArea";
            s1.shape_type = "Rectangle";
            s1.group_id = "CARDAREA";
            s1.text_field = "";
            s1.points = new List<List<double>>();
            s1.points.AddRange(t1.pointfinder(myobj.searchtext, obj, myobj));
            d1.shapes.Add(s1);



            return true;
        }





    }
}
