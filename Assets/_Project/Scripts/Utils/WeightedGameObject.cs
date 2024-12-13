using System;
using System.Collections.Generic;
using UnityEngine;

namespace UtilsModule {
    [Serializable]
    public class WeightedGameObject : WeightedObject<GameObject> { 
        public static WeightedGameObject GetFromBase(WeightedObject<GameObject> toConvert) {
            return new WeightedGameObject {item = toConvert.item, weight = toConvert.weight};
        }

        public static WeightedObject<GameObject> GetBase(WeightedGameObject toConvert) {
            return new WeightedObject<GameObject> {item = toConvert.item, weight = toConvert.weight};
        }

        public static List<WeightedGameObject> GetListFromBase(List<WeightedObject<GameObject>> toConvert) {
            List<WeightedGameObject> toReturn = new List<WeightedGameObject> ();

            foreach(WeightedObject<GameObject> item in toConvert) {
                toReturn.Add(GetFromBase(item));
            }

            return toReturn;
        }

        public static List<WeightedObject<GameObject>> GetListOfBase(List<WeightedGameObject> toConvert) {
            List<WeightedObject<GameObject>> toReturn = new List<WeightedObject<GameObject>> ();

            foreach(WeightedGameObject item in toConvert) {
                toReturn.Add(GetBase(item));
            }

            return toReturn;
        }
    }
}