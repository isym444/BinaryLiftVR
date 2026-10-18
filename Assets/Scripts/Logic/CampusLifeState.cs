using System;
namespace BinaryLift
{
    public enum CupPlace { Tray, Brewer, Coaster }
    public sealed class CampusLifeState
    {
        public CupPlace Cup { get; private set; }
        public float Fill { get; private set; }
        public bool Brewing { get; private set; }
        public bool CoffeeServed { get; private set; }
        public bool Stamped { get; private set; }
        public bool Submitted { get; private set; }
        public bool DrawerOpen { get; private set; }
        public bool ClueFound { get; private set; }
        public bool PlaceCup(CupPlace place)
        {
            if(Brewing||CoffeeServed||!Enum.IsDefined(typeof(CupPlace),place))return false;
            if(place==CupPlace.Coaster&&Fill<1)return false;
            Cup=place;if(place==CupPlace.Coaster)CoffeeServed=true;return true;
        }
        public bool Brew(){if(Cup!=CupPlace.Brewer||Fill>=1||Brewing)return false;Brewing=true;return true;}
        public bool Advance(float seconds)
        {
            if(!Brewing||seconds<=0||seconds>.25f||float.IsNaN(seconds))return false;
            Fill=Math.Min(1,Fill+seconds/2.5f);if(Fill>=1)Brewing=false;return true;
        }
        public bool Stamp(){if(Stamped||Submitted)return false;Stamped=true;return true;}
        public bool Submit(){if(!Stamped||Submitted)return false;Submitted=true;return true;}
        public bool OpenDrawer(){if(!Submitted||DrawerOpen)return false;DrawerOpen=true;return true;}
        public bool TakeClue(){if(!DrawerOpen||ClueFound)return false;ClueFound=true;return true;}
    }
}
