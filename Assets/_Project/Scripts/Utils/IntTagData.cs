using System;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Tag Data", menuName = "Data/Tag/Int Tag Data"), Serializable]
    public class IntTagData : TagData<int> { }
}