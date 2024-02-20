using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : CharacterMovement
{
  private const string MovementHorizontalKey = "Horizontal";  // Константа с ключом горизонтального движения
  private const string MovementVerticalKey   = "Vertical";    // Константа с ключом вертикального движения

  private Animator _animator; // Анимация противника

  // Навигационный агент
  private NavMeshAgent _navMeshAgent; // Управляет навигацией в игре

  private Transform _playerTransform; // Трансформа главного героя Враги будут бежать в его позицию
  private Vector3 _prevPosition;      // Предыдущая позиция врага

  protected override void Init() {
    _animator     = GetComponentInChildren<Animator>(); // Присваиваем _animator компонент Animator из дочерних объектов
    _navMeshAgent = GetComponent<NavMeshAgent>();       // Присваиваем _navMeshAgent компонент NavMeshAgent

    // Присваиваем _playerTransform трансформу героя
    _playerTransform = FindAnyObjectByType<Player>().transform; // FindAnyObjectByType<Player>() ищет игрока по типу Player
    _prevPosition    = transform.position;                      // Присваиваем _prevPosition текущую позицию врага
  }

  protected override void Stop() {
    _navMeshAgent.enabled = false; // Отключаем навигационный агент
    RefreshAnimation();            // Обновляем анимацию врага
  }

  private void Update() {
    if (!IsActive) { // Если враг не активен
      return;        // Выходим из метода
    }
    SetTargetPosition(_playerTransform.position); // Устанавливаем целевую позицию врага
    RefreshAnimation();                           // Обновляем анимацию врага
  }

  private void SetTargetPosition(Vector3 position) { _navMeshAgent.SetDestination(position); } // Устанавливаем целевую позицию врага

  private void RefreshAnimation() {
    Vector3 curPosition = transform.position;          // Получаем текущую позицию врага
    Vector3 deltaMove   = curPosition - _prevPosition; // Вычисляем разницу между текущей и предыдущей позицией

    // Сохраняем текущую позицию в _prevPosition
    _prevPosition = curPosition; // Для использования при следующем обновлении анимации
    deltaMove.Normalize(); // Нормализуем разницу позиций, чтобы она имела длину 1

    float relatedX = Vector3.Dot(deltaMove, transform.right  ); // Вычисляем относительное смещение по оси X
    float relatedY = Vector3.Dot(deltaMove, transform.forward); // Вычисляем относительное смещение по оси Y

    // Устанавливаем значения относительных смещений в аниматоре
    _animator.SetFloat(MovementHorizontalKey, relatedX); // Для смещения по горизонтали
    _animator.SetFloat(MovementVerticalKey  , relatedY); // Для смещения по вертикали
  }
}
