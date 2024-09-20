using System;
using UnityEngine;

namespace UtilsModule {
    [CreateAssetMenu(fileName = "New Tag Data", menuName = "Data/Tag/FLoat Tag Data"), Serializable]
    public class FloatTagData : TagData<float> { }
}