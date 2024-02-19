using UnityEngine;

public class CameraController : MonoBehaviour
{
  [SerializeField] private Transform _target;          // Трансформа цели камеры — игрок
  [SerializeField] private Transform _cameraRoot;      // Трансформа объекта, который содержит камеру
  [SerializeField] private Transform _cameraTransform; // Трансформа самой камеры

  [SerializeField] private Vector3 _positionOffset = Vector3.up; // Отступ позиции камеры от цели
  [SerializeField] private float   _moveSpeed      = 5f;         // Скорость перемещения камеры
  [SerializeField] private float   _rotationSpeed  = 65f;        // Скорость вращения камеры
  [SerializeField] private float   _zoomSpeed      = 10f;        // Скорость изменения зума камеры
  [SerializeField] private float   _minZoom        = 3f;         // Минимальное значение зума камеры
  [SerializeField] private float   _maxZoom        = 14f;        // Максимальное значение зума камеры
  [SerializeField] private float   _mouseZoomMultiplier = 3f;    // Множитель изменения зума камеры с помощью мыши
                   
                   private float   _currentZoom;                 // Текущее значение зума камеры

  private void Start()
  {
    
    Init(); // Вызываем метод Init()
  }

  private void Init()
  {
    // Вычисляем текущий зум камеры
    _currentZoom = (_target.position - _cameraTransform.position).magnitude; // Через расстояние между целью и трансформой камеры
  }

  private void LateUpdate()
  {
    MoveCamera();   // Перемещаем камеру
    RotateCamera(); // Вращаем камеру
    ZoomCamera();   // Управляем зумом
  }

  private void MoveCamera()
  {
    // Если цель или корневой объект камеры не заданы
    if (!_target || !_cameraRoot) { return; }
    
    Vector3 targetPosition = _target.position + _positionOffset; // Рассчитываем позицию цели с учётом отступа

    // Плавно перемещаем корневой объект камеры к цели
    _cameraRoot.transform.position = Vector3.Lerp(_cameraRoot.transform.position, targetPosition, _moveSpeed * Time.deltaTime);
  }

  private void RotateCamera()
  {
    // Если корневого объекта камеры нет
    if (!_cameraRoot) { return; } // Выходим из метода
    
    // Заводим переменную для направления поворота
    float direction = 0;
         if (Input.GetKey(KeyCode.Q)) { direction =  1; } // Если нажата клавиша Q - Камера будет поворачиваться по часовой стрелке
    else if (Input.GetKey(KeyCode.E)) { direction = -1; } // Если нажата клавиша E - Камера будет поворачиваться против часовой стрелки
    // Если направление равно 0
    // Не нажата ни клавиша Q, ни клавиша E
    if (Mathf.Approximately(direction, 0)) { return; } // Выходим из метода
    
    // Получаем углы корневого объекта камеры
    Vector3 cameraEuler = _cameraRoot.eulerAngles;

    // Изменяем угол поворота камеры
    // На произведение направления, скорости и времени
    cameraEuler.y += direction * _rotationSpeed * Time.deltaTime;

    _cameraRoot.eulerAngles = cameraEuler; // Присваиваем новые углы корневому объекту камеры
  }

  private void ZoomCamera()
  {
    if (!_cameraTransform) { return; } // Если трансформы главной камеры нет Выходим из метода
    
    float direction = 0; // Заводим переменную для направления поворота

    // Если нажата клавиша Z или клавиша минус
         if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.KeypadMinus)) { direction =  1; } // Камера будет приближаться к карте
    // Иначе, если нажата клавиша X или клавиша плюс
    else if (Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.KeypadPlus))  { direction = -1; } // Камера будет отдаляться от карты
    // Иначе, если прокручено колесо мыши или тачпад
    else if (!Mathf.Approximately(Input.mouseScrollDelta.y, 0)) { 
      direction = -Input.mouseScrollDelta.y * _mouseZoomMultiplier;  // Направление зума будет зависеть от этого значения
    } 
    // Если направление равно 0
    // То есть не было действий, описанных ранее
    if (Mathf.Approximately(direction, 0)) {  return; } // Выходим из метода
    
    _currentZoom += direction * _zoomSpeed * Time.deltaTime;       // Изменяем     текущий зум На произведение направления, скорости и времени
    _currentZoom =  Mathf.Clamp(_currentZoom, _minZoom, _maxZoom); // Ограничиваем текущий зум В пределах минимального и максимального

    // Изменяем позицию трансформы главной камеры
    // Чтобы она была на расстоянии текущего зума от корня
    _cameraTransform.position = _cameraRoot.position - _cameraTransform.forward * _currentZoom;
  }
}
