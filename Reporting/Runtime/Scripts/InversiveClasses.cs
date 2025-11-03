using System;
using UnityEngine;

namespace Reporting
{
    public class InversiveClasses
    {
        [Serializable]
        public class ExperienceCreateNotationModel
        {
            [field: SerializeField] public bool IsConform { get; set; }
            [field: SerializeField] public string Comment { get; set; }
            [field: SerializeField] public string InvitationId { get; set; }
            [field: SerializeField] public int ExperienceId { get; set; }
            [field: SerializeField] public DeviceTypeEnum DeviceType { get; set; }
            [field: SerializeField] public DeviceStandaloneTypeEnum TargetDevice { get; set; }
        }

        public enum DeviceTypeEnum
        {
            //Borne
            Terminal = 0,
            //Personnel 
            Personal = 1,
            //Ecran Mural
            WallScreen = 2,
            //Standalone
            Standalone = 3,
            //WebBrowser
            WebBrowser = 4,
        }

        public enum DeviceStandaloneTypeEnum
        {
            None = 0,
            Meta_Quest_2 = 1,
            Pico_4_Enterprise = 2,
            Meta_Quest_3 = 3,
        }
    }
}
