using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LineBot_Twitch.Models
{
    public class LineFlexModel
    {
        public string type { get; set; }
        public string altText { get; set; }
        public FlexContent contents { get; set; }
    }
    public class FlexContent
    {
        public string type { get; set; }
        public List<CarouselContent> contents { get; set; }
    }
    public class CarouselContent
    {
        public string type { get; set; }
        public Hero hero { get; set; }

        public Body body { get; set; }

    }
    public class Hero
    {
        public string type { get; set; }
        public string url { get; set; }
        public string size { get; set; }
        public string aspectRatio { get; set; }
        public string aspectMode { get; set; }
        public LineAction action { get; set; }
    }



    public class LineAction
    {
        public string type { get; set; }
        public string uri { get; set; }
    }


    public class Body
    {
        public string type { get; set; }
        public string layout { get; set; }
        public List<BodyContent> contents { get; set; }
    }
    public class BodyContent
    {
        public string type { get; set; }
    }

    public class BodyContentText: BodyContent
    {
        public string text { get; set; }
        public string weight { get; set; }
        public string size { get; set; }
        public string color { get; set; }
        public int flex { get; set; }
    }
    public class BodyContentBox : BodyContent
    {
        public string layout { get; set; }
        public string margin { get; set; }
        public string spacing { get; set; }
        public List<BodyContent> contents{get;set;}
    }


}
