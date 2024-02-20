using UnityEngine;

public class BulletController : MonoBehaviour
{
  [SerializeField] private GameObject _hitPrefab; // Префаб, который будет появляться при попадании пули
  
  [SerializeField] private float _speed    = 30f;    // Скорость пули
  [SerializeField] private float _lifeTime = 2f;     // Время отображения пули на экране

  // Update is called once per frame
  void Update() {
    ReduceLifeTime(); // Уменьшаем время отображения пули на экране
    CheckHit();       // Проверяем попадание в объект
    Move();           // Перемещаем пулю
  }

  private void ReduceLifeTime() {
    _lifeTime -= Time.deltaTime; // Сокращаем время отображения пули на время, прошедшее с последнего кадра
    if (_lifeTime <= 0) {        // Если время отображения пули истекло
      DestroyBullet();           // Пуля пропадает с экрана
    }
  }

  private void CheckHit() {
    // Если пуля столкнулась с чем-то
    if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, _speed * Time.deltaTime)) {
      Hit(hit); // Обрабатываем попадание
    }
  }

  // Меняем позицию пули через изменения скорости и времени
  private void Move() { transform.position += transform.forward * _speed * Time.deltaTime; }

  private void Hit(RaycastHit hit) {
    // Создаём эффект попадания на месте столкновения пули
    Instantiate(_hitPrefab, hit.point, Quaternion.LookRotation(-transform.up, -transform.forward));
    DestroyBullet(); // Пуля пропадает с экрана
  }

  // Убираем объект пули
  private void DestroyBullet() { Destroy(gameObject); }
}
