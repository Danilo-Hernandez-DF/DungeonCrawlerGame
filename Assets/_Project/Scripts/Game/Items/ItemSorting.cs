using System.Linq;

namespace Game {
	public static class ItemSorting
	{
		public static List<ItemData> Sort(this List<ItemData> items, ItemSortType criteria , bool ascending = true) {
			var ordered = new List<ItemData>();
			
			switch (criteria) {
				case ItemSortType.Rarity:
					ordered = items.OrderBy(x => x.rarity).ThenBy(x => x.name).ToList();
					break;
				case ItemSortType.Name:
					ordered = items.OrderBy(x => x.name).ThenBy(x => x.rarity).ToList();
					break;
			}
			
			if(!ascending) ordered.Reverse();
			return ordered;
		}
	}
	
	public enum ItemSortType {
		Rarity,
		Name,
		Amount,
		Price
	}
}