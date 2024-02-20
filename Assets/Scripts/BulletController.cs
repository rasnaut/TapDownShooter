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
    CheckCharacterHit(hit); // Проверяем попадание в персонажа
    
    // Создаём эффект попадания на месте столкновения пули
    Instantiate(_hitPrefab, hit.point, Quaternion.LookRotation(-transform.up, -transform.forward));
    DestroyBullet(); // Пуля пропадает с экрана
  }

  private void DestroyBullet() { Destroy(gameObject); } // Убираем объект пули

  private void CheckCharacterHit(RaycastHit hit)
  {
    // Получаем компонент CharacterHealth
    CharacterHealth hittedHealth = hit.collider.GetComponentInChildren<CharacterHealth>(); // У персонажа, в которого попала пуля

    // Если такой компонент есть
    if (hittedHealth) {                      // То есть пуля попала в персонажа
      int damage = 10;                       // Задаём урон от пули;
      hittedHealth.AddHealthPoints(-damage); // Уменьшаем количество здоровья персонажа
    }
  }
}
