using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAiming : CharacterAiming
{
  [SerializeField] private float _aimingSpeed = 10f; // Скорость вращения

  private Transform _aimTransform;       // Трансформа цели игрока
  private RigBuilder _rigBuilder;        // Переменная для анимаций Специального типа RigBuilder
  private WeaponAiming[] _weaponAimings; // Массив объектов типа WeaponAiming

  private Camera _mainCamera;     // Главная камера

  //Переопределили метод Init()
  protected override void Init() {
    _mainCamera    = Camera.main;                                 // Присваиваем _mainCamera объект камеры
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

  void FixedUpdate() {
    if (!IsActive) { return; } // Если герой не активен Выходим из метода
    Aiming();                  // Поворачиваем героя для прицела
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

      // Плавно перемещаем цель игрока
      // В точку столкновения с заданной скоростью
      _aimTransform.position = Vector3.Lerp(_aimTransform.position, hitInfo.point, _aimingSpeed * Time.fixedDeltaTime);
    }
  }
}
