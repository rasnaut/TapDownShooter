using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
  // Константы с будем использовать их для получения данных ввода с клавиатуры
  private const string MovementHorizontalKey = "Horizontal"; // ключ горизонтального движения
  private const string MovementVerticalKey   = "Vertical"  ; // ключ вертикального движения
  private const string IsGroundedKey         = "IsGrounded"; // ключ приземления

  [SerializeField] private float _gravityMultiplier   = 2f;   // Множитель гравитации
  [SerializeField] private float _movementSpeed       = 6f;   // Скорость движения
  [SerializeField] private float _jumpSpeed           = 30f;  // Скорость прыжка
  [SerializeField] private float _jumpDuration        = 1f;   // Длительность прыжка
  [SerializeField] private float _groundCheckDistance = 0.2f; // Расстояние для приземления
  [SerializeField] private float _groundCheckExtraUp  = 0.2f; // Дополнительная высота проверки земли
  [SerializeField] private float _aimingSpeed         = 10f; // Скорость вращения

  private Animator            _animator;       // Анимация героя
  private CharacterController _characterController; // Контроллер движения
  private Camera              _mainCamera;     // Главная камера
  private Vector3             _groundCheckBox; // Размеры коллайдера для проверки земли
  private bool                _isGrounded;     // Флаг того, что герой на земле
  private bool                _isJumping;      // Флаг того, что герой в прыжке
  private float               _jumpTimer;      // Таймер длительности прыжка

  
  private Transform _aimTransform;       // Трансформа цели игрока
  private RigBuilder _rigBuilder;        // Переменная для анимаций Специального типа RigBuilder
  private WeaponAiming[] _weaponAimings; // Массив объектов типа WeaponAiming

  // Start is called before the first frame update
  void Start()
  {
    Init();
  }

  private void Init()
  {
    _animator            = GetComponentInChildren<Animator>();  // Присваиваем _animator компонент Animator из дочерних объектов
    _characterController = GetComponent<CharacterController>(); // Присваиваем _characterController компонент CharacterController
    _mainCamera          = Camera.main;                         // Присваиваем _mainCamera объект камеры

    // Устанавливаем размеры коллайдера для проверки близости героя к земле
    _groundCheckBox = new Vector3(_characterController.radius, 0.0001f, _characterController.radius); // Пишем там очень маленькое значение 0.0001f

    // НОВОЕ
    _aimTransform  = FindAnyObjectByType<PlayerAim>().transform;  // Находим объект типа PlayerAim Записываем его трансформу в _aimTransform
    _rigBuilder    = GetComponentInChildren<RigBuilder>();        // Получаем RigBuilder из дочерних объектов Записываем его в переменную _rigBuilder
    _weaponAimings = GetComponentsInChildren<WeaponAiming>(true); // Получаем все компоненты WeaponAiming Записываем их в массив _weaponAimings

    InitWeaponAimings(_weaponAimings, _aimTransform);             // Вызываем метод InitWeaponAimings() Передаём туда _weaponAimings и _aimTransform
  }

  private void InitWeaponAimings(WeaponAiming[] weaponAimings, Transform aim)
  {
    // Проходим по всем элементам weaponAimings
    for (int i = 0; i < weaponAimings.Length; i++) { // Вызываем у weaponAimings[i] метод Init()
      weaponAimings[i].Init(aim);                    // И передаём ему компонент цели aim
    }
    _rigBuilder.Build(); // Вызываем у _rigBuilder встроенный метод Build() Чтобы построить скелетную анимацию героя
  }

  void FixedUpdate()
  {
    Gravity (); // Применяем к герою гравитацию
    Movement(); // Двигаем героя клавишами
    Jumping (); // Управляем прыжком героя
    Aiming  (); // Поворачиваем героя для прицела
  }

  private void Gravity()
  {
    // Создаём переменную gravity типа Vector3
    Vector3 gravity = Physics.gravity;                          // Присваиваем ей значение силы гравитации из физического движка Physics
    gravity        *= _gravityMultiplier * Time.fixedDeltaTime; // Умножаем гравитацию на множитель и время кадра
    _characterController.Move(gravity);                         // Придаём гравитацию компоненту CharacterController
  }

  private void Movement()
  {
    Vector3 movement = Vector3.zero;                   // Создаём переменную movement со значением (0, 0, 0)
    movement.x = Input.GetAxis(MovementHorizontalKey); // Задаём movement.x значение горизонтального ввода с клавиатуры (клавиши A и D)
    movement.z = Input.GetAxis(MovementVerticalKey);   // Задаём movement.z значение вертикального ввода с клавиатуры (клавиши W и S)
    movement = GetMovementByCamera(movement);          // Преобразуем вектор перемещения относительно камеры

    // Вычисляем вектор перемещения
    movement *= _movementSpeed * Time.fixedDeltaTime; // Через скорость и время между фиксированными кадрами
    _characterController.Move(movement);              // Придаём движение компоненту CharacterController
    AnimateMovement(movement);                        // Анимируем движение героя
  }

  private Vector3 GetMovementByCamera(Vector3 input)
  {
    Vector3 cameraForward = _mainCamera.transform.forward; // Получаем вектор направления камеры вперёд
    Vector3 cameraRight   = _mainCamera.transform.right;   // Получаем вектор направления камеры вправо
    cameraForward.y = 0f;                                  // Обнуляем значение вектора направления вперёд
    cameraRight.y   = 0f;                                  // Обнуляем значение вектора направления вправо
    cameraForward.Normalize();  // Нормализуем вектор направления вперёд
    cameraRight.Normalize();    // Нормализуем вектор направления вправо
    Vector3 movement = cameraForward * input.z + cameraRight * input.x; // Складываем векторы движения с учётом направления камеры

    return movement; // Возвращаем полученный вектор движения
  }

  private void AnimateMovement(Vector3 movement)
  {
    float relatedX = Vector3.Dot(movement.normalized, transform.right);   // Получаем проекцию вектора движения на ось X
    float relatedY = Vector3.Dot(movement.normalized, transform.forward); // Получаем проекцию вектора движения на ось Y
    _animator.SetFloat(MovementHorizontalKey, relatedX);                  // Устанавливаем значение анимации горизонтального движения
    _animator.SetFloat(MovementVerticalKey  , relatedY);                  // Устанавливаем значение анимации вертикального движения
  }

  private void Jumping()
  {
    RefreshIsGrounded(); // Обновляем данные о приземлении
    if (Input.GetKeyDown(KeyCode.Space) // Если нажата клавиша «пробел»
      &&  _isGrounded                   // И герой находится на земле
      && !_isJumping)                   // И прыжок сейчас не выполняется
    {
      SetIsGrounded(false);  // Убираем состояние «на земле»
      _isJumping = true;     // Ставим флаг «в прыжке»
      _jumpTimer = 0;        // Обнуляем таймер прыжка
    }
    
    if (_isJumping) // Если герой в прыжке
    {
      _jumpTimer += Time.fixedDeltaTime; // Увеличиваем таймер прыжка

      // Рассчитываем вертикальную силу движения вверх с учётом длительности прыжка
      Vector3 motion = Vector3.up * _jumpSpeed * (1 - _jumpTimer / _jumpDuration) * Time.fixedDeltaTime;
      _characterController.Move(motion); // Применяем силу через Character Controller

      if (_jumpTimer >= _jumpDuration  // Если длительность прыжка превышена
        || _isGrounded)  {             // Или если герой приземлился
        _isJumping = false;            // Убираем флаг «в прыжке»
      }
    }
  }

  // Устанавливаем состояние приземления через GroundCheck()
  private void RefreshIsGrounded() { SetIsGrounded(GroundCheck()); }

  private bool GroundCheck()
  {
    Vector3 startCheckPosition = transform.position   + Vector3.up * _groundCheckExtraUp; // Вычисляем позицию начального положения для проверки земли
    float checkDistance        = _groundCheckDistance + _groundCheckExtraUp;              // Вычисляем длину луча для проверки земли

    // Возвращаем результат с расстоянием от начальной позиции до точки ниже героя
    return Physics.BoxCast(startCheckPosition, _groundCheckBox, Vector3.down, transform.rotation, checkDistance);
  }

  private void SetIsGrounded(bool value)
  {
    // Если состояние героя изменяется
    if (value != _isGrounded) { // От «на земле» к «в прыжке» или наоборот
      _animator.SetBool(IsGroundedKey, value); // Обновляем значение в аниматоре
    }
    _isGrounded = value;                       // Присваиваем флагу «на земле» переданное значение
  }

  private void Aiming()
  {
    Vector3 mouseScreenPosition = Input.mousePosition; // Получаем текущую позицию курсора на экране

    // Создаём луч, который будет направлен от главной камеры
    Ray findTargetRay = _mainCamera.ScreenPointToRay(mouseScreenPosition); // В точку на экране, где находится курсор

    if (Physics.Raycast(findTargetRay, out RaycastHit hitInfo)) // Если луч пересекает объекты в игровом мире
    {
      // Вычисляем направление взгляда игрока
      Vector3 lookDirection = (hitInfo.point - transform.position).normalized; // Чтобы смотреть на точку пересечения луча с объектом

      // Обнуляем вертикальную составляющую направления
      // Чтобы игрок не наклонялся вверх или вниз 
      // Позже будем наклонять верхнюю часть туловища по y
      lookDirection.y = 0;

      // Создаём новый поворот игрока
      var newRotation = Quaternion.LookRotation(lookDirection, Vector3.up); // Так, чтобы он смотрел в заданном направлении

      // Плавно поворачиваем игрока с учётом скорости
      transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.fixedDeltaTime * _aimingSpeed);

      // НОВОЕ: Плавно перемещаем цель игрока
      // В точку столкновения с заданной скоростью
      _aimTransform.position = Vector3.Lerp(_aimTransform.position, hitInfo.point, _aimingSpeed * Time.fixedDeltaTime);
    }
  }
}
