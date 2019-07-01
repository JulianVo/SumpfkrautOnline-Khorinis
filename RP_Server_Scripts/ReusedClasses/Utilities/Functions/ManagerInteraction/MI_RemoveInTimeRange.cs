using System;

namespace RP_Server_Scripts.ReusedClasses.Utilities.Functions.ManagerInteraction
{

    public struct MI_RemoveInTimeRange : IManagerInteraction
    {

        public DateTime Start;
        public DateTime End;



        public MI_RemoveInTimeRange (DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

    }

}
