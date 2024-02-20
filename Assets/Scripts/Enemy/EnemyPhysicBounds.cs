using UnityEngine;

public class EnemyPhysicBounds : CharacterPhysicBounds
{
  private Collider _bodyCollider; // Коллайдер врага

  protected override void Init() { // Инициализируем переменные
    _bodyCollider = GetComponentInChildren<Collider>(); // Присваиваем _bodyCollider компонент Collider из дочерних объектов
  } 
  
  protected override void Stop() { // Останавливаем врага
    _bodyCollider.enabled = false; // Выключаем коллайдер
  }
}
