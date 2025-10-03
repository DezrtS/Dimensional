using FMODUnity;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class FMODReferenceManager : Singleton<FMODReferenceManager>
    {
        [field: Header("Movement SFXs")]
        [field: SerializeField] private EventReference walk;
        [field: SerializeField] private EventReference wallSlide;
        [field: SerializeField] private EventReference balloonJump;
        [field: SerializeField] private EventReference springJump;
        [field: SerializeField] private EventReference parachuteDeploy;
        [field: SerializeField] private EventReference parachuteGlide;
        
        public EventReference Walk => walk;
        public EventReference WallSlide => wallSlide;
        public EventReference BalloonJump => balloonJump;
        public EventReference SpringJump => springJump;
        public EventReference ParachuteDeploy => parachuteDeploy;
        public EventReference ParachuteGlide => parachuteGlide;
    }
}