using System;

namespace RP_Server_Scripts.ReusedClasses.Utilities.Functions.ManagerInteraction
{

    public struct MI_RemoveExceptTimeRange : IManagerInteraction
    {

        public DateTime Start;
        public DateTime End;



        public MI_RemoveExceptTimeRange (DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

    }

}
