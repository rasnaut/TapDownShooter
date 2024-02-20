using System;
using UnityEngine;

public class CharacterHealth : CharacterPart
{
  private const string DeathKey = "Death";               // Константа с ключом смерти персонажа

  [SerializeField] private int _startHealthPoints = 100; // Стартовое количество здоровья

  private Animator _animator;     // Аниматор персонажа
  private int      _healthPoints; // Очки здоровья персонажа
  private bool     _isDead;       // Флаг смерти персонажа

  public Action OnDie;            // Событие при смерти

  public void AddHealthPoints(int value) {
    // Если персонаж мёртв
    if (_isDead) { return; } // Выходим из метода
    
    _healthPoints += value;  // Увеличиваем значение здоровья на value

    if (_healthPoints <= 0) { // Если здоровье достигло нуля
      Die();                  // Обрабатываем смерть персонажа
    }
  }

  protected override void Init() {
    _animator     = GetComponentInChildren<Animator>(); // Присваиваем _animator компонент Animator из дочерних объектов
    _healthPoints = _startHealthPoints;                 // Задаём начальное значение здоровья
    _isDead       = false;                              // Ставим флаг в значение «живой»
  }

  private void Die() {
    _isDead = true;                 // Ставим флаг в значение «мёртвый»
    _animator.SetTrigger(DeathKey); // Запускаем анимацию смерти персонажа
    OnDie?.Invoke();                // Вызываем событие OnDie
  }

}
