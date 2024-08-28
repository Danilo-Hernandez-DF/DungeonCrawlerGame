using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;


namespace UtilsModule {
    [Serializable]
    public class WeightedTable<T> {
        private HashSet<WeightedT> items;
        int sumWeights = 0;

        public WeightedTable(List<T> itemList, List<int> weightList = default) {
            if(weightList?.Count < itemList.Count) {
                weightList = new();

                for(int i = 0; i < itemList.Count; i++) {
                    weightList.Add(10);
                }
            }

            items = new HashSet<WeightedT>();

            for (int i = 0; i < itemList.Count; i++) {
                items.Add(new WeightedT {item = itemList[i], weight = weightList[i]});
            }
        }

        public T GetWeightedT(int lowerLimit = 0, uint upperLimit = int.MaxValue, bool seeded = false) {
            foreach(WeightedT item in items) {
                if(items.Count == 1) return item.item;
                if(item.weight < lowerLimit) continue;
                if(item.weight > upperLimit) continue;

                sumWeights += item.weight;
            }

            int rand = seeded? SeededRandom.GetRange(0, sumWeights): Random.Range(0, sumWeights);
            int added = 0;
            foreach(WeightedT item in items) {
                if(item.weight < lowerLimit) continue;
                if(item.weight > upperLimit) continue;

                added += item.weight;
                if(rand < added) return item.item;
            }

            return default;
        }

        [Serializable]
        public struct WeightedT {
            public T item;
            public int weight;
        }
    }
} 