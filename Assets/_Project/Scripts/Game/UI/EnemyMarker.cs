using ProcGen;
using UnityEngine.UI;
using Utils;

namespace Game
{
    public class EnemyMarker: MonoBehaviour
    {
        public Enemy enemy;
        public bool vertical;
        [SerializeField] private Image sprite;
        
        private void Update() {
            if (enemy == null) {
                sprite.enabled = false;
                return;
            }

            sprite.enabled = true;
            var cam = CameraManager.Instance.camera;
            
            if (vertical) {
                var screenPosition = cam.WorldToScreenPoint(new Vector3(enemy.transform.position.x, 0, 0));
                transform.position = new Vector3(screenPosition.x, transform.position.y, transform.position.z);
            } else {
                var screenPosition = cam.WorldToScreenPoint(new Vector3(0, enemy.transform.position.y, 0));
                transform.position = new Vector3(transform.position.x, screenPosition.y, transform.position.z);
            }
        }
        
        public void SetEnemy(Enemy enemy) {
            this.enemy = enemy;
        }

        public void Init(Dir direction) {
            if(direction is Dir.Up or Dir.Down) vertical = true;
            else vertical = false;

            var cam = CameraManager.Instance.camera;
            
            switch (direction) {
                case Dir.Up:
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    transform.position = new Vector3(0, cam.pixelHeight - 50, 0);
                    break;
                case Dir.Down:
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                    transform.position = new Vector3(0, 50, 0);
                    break;
                case Dir.Left:
                    transform.rotation = Quaternion.Euler(0, 0, 270);
                    transform.position = new Vector3(cam.pixelWidth - 50, 0, 0);
                    break;
                case Dir.Right:
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                    transform.position = new Vector3(50, 0, 0);
                    break;
            }
        }
    }
}