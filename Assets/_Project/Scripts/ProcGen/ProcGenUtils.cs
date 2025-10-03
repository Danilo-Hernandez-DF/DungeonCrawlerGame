using UnityEditor.Experimental.GraphView;
using UtilsModule;

namespace ProcGen {
    public static class ProcGenUtils {
        public static Dir GetOpposite(this Dir dir) {
            return dir switch {
                Dir.Up => Dir.Down,
                Dir.Down => Dir.Up,
                Dir.Left => Dir.Right,
                Dir.Right => Dir.Left,
                _ => Dir.Up,
            };
        }

        public static Orientation ToOrientation(this Dir dir) {
            return dir switch {
                Dir.Up or Dir.Down => Orientation.Horizontal,
                Dir.Left or Dir.Right => Orientation.Vertical,
                _ => Orientation.Vertical,
            };
        }
    }

    public enum RoomType {NoRoom, StartRoom, PlaceholderRoom, EmptyRoom, HostileRoom, TreasureRoom, ShopRoom, BossRoom, HiddenRoom}
    public enum Orientation {Horizontal, Vertical}
}