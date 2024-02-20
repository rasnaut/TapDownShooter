using UnityEngine;

public class PlayerShooting : CharacterShooting
{
  [SerializeField] private BulletController _bulletPrefab;        // Префаб пули
  [SerializeField] private float            _bulletDelay = 0.05f; // Задержка между выстрелами

  private Transform _bulletSpawnPoint; // Точка появления пули
  private float     _bulletTimer;      // Счётчик времени между выстрелами

  public override void Init()
  {
    _bulletSpawnPoint = GetComponentInChildren<BulletSpawnPoint>().transform; // Получаем компонент Transform для точки вылета пули
    _bulletTimer = 0;                                                         // Обнуляем таймер выстрела
  }

  private void Update() { Shooting(); } // Обрабатываем выстрел игрока

  private void Shooting()
  {
    
    if (Input.GetMouseButton(0)) // Если нажата левая кнопка мыши
    {
      _bulletTimer += Time.deltaTime; // Увеличиваем таймер выстрела На время, прошедшее с предыдущего кадра

      if (_bulletTimer >= _bulletDelay) { // Если достигнуто значение задержки
        _bulletTimer = 0;                 // Обнуляем таймер выстрела
        SpawnBullet();                    // Делаем новую пулю
      }
    }
  }

  // Создаём экземпляр префаба пули
  private void SpawnBullet() {
    // В точке появления с теми же параметрами
    Instantiate(_bulletPrefab, _bulletSpawnPoint.position, _bulletSpawnPoint.rotation);
  }
}
