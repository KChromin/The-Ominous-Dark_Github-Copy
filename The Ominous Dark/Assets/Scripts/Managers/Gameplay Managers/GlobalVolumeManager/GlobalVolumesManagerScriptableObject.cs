using System.Collections.Generic;
using UnityEngine;

namespace NOS.GameplayManagers
{
    [CreateAssetMenu(fileName = "GlobalVolumes", menuName = "ScriptableObjects/Rendering/GlobalVolumes")]
    public class GlobalVolumesManagerScriptableObject : ScriptableObject
    {
        public List<GlobalVolumeProfilesClass> volumesList;
    }
}
