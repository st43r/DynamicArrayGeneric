using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Norbit.Crm.Protasov
{
	/// <summary>
	/// Динамический массив.
	/// </summary>
	/// <typeparam name="T">Тип элементов массива.</typeparam>
	public class DynamicArray<T> : IEnumerable<T>, IEnumerator<T>, IComparable<T>
	{
		#region

		/// <summary>
		/// Размер дополнительного места в массиве.
		/// </summary>
		private const int _additionalSize = 8;

		/// <summary>
		/// Длина пустого массива.
		/// </summary>
		private const int _defaultLength = 0;

		#endregion

		#region Приватные поля.
		/// <summary>
		/// Длина массива.
		/// </summary>
		private int _length = 0;

		/// <summary>
		/// Позиция текущего элемента.
		/// </summary>
		private int _position = 0;

		/// <summary>
		/// Массив.
		/// </summary>
		private T[] array;

		#endregion


		#region Геттеры.
		/// <summary>
		/// Получает длину.
		/// </summary>
		public int GetLength
		{
			get => _length;
		}

		/// <summary>
		/// Получает ёмкость.
		/// </summary>
		public int GetCapacity
		{
			get => array.Length;
		}
		#endregion


		#region Конструкторы.
		/// <summary>
		/// Создает пустой массив ёмкостью 8.
		/// </summary>
		public DynamicArray()
			: this(_defaultLength)
		{

		}

		/// <summary>
		/// Создает пустой массив ёмкостью <paramref name="size"/>+8.
		/// </summary>
		/// <param name="size">Размер массива.</param>
		public DynamicArray(int size)
		{
			ValidationHelper.CheckRangeValue(size, 0, true);

			array =
				size % 2 == 0
				? new T[size + _additionalSize]
				: new T[size + _additionalSize + 1];
		}

		/// <summary>
		/// Создает массив, заполняя его элементами из <paramref name="collection"/>.
		/// </summary>
		/// <param name="collection">Набор элементов.</param>
		public DynamicArray(IEnumerable<T> collection)
			: this(_defaultLength)
		{
			AddRange(collection);
		}
		#endregion


		#region Методы.
		/// <summary>
		/// Добавляет в конец массива <paramref name="value"/>.
		/// </summary>
		/// <param name="value">Добавляемое значение.</param>
		public void Add(T value)
		{
			Validation.CheckNull(value);

			AddRange(new List<T> { value });
		}

		/// <summary>
		/// Добавляет в конец массива <paramref name="collection"/>.
		/// </summary>
		/// <param name="collection">Набор элементов.</param>
		public void AddRange(IEnumerable<T> collection)
		{
			Validation.CheckNull(collection);
			ValidationHelper.CheckRangeValue(collection.Count(), 1, true);

			foreach (var item in collection)
			{
				AddCapacity();

				array[_length++] = item;
			}
		}


		/// <summary>
		/// Вставляет в массив <paramref name="value"/> на <paramref name="index"/>.
		/// </summary>
		/// <param name="index">Место вставки.</param>
		/// <param name="value">Вставляемое значение.</param>
		public void Insert(int index, T value)
		{
			ValidationHelper.CheckRangeValue(index, 0, _length);
			Validation.CheckNull(value);

			AddCapacity();

			for (var i = _length; i > index - 1; --i)
			{
				array[i] = array[i - 1];
			}

			array[index - 1] = value;
			_length++;
		}


		/// <summary>
		/// Удаляет элемент, равный <paramref name="item"/> из массива.
		/// </summary>
		/// <param name="item">Удаляемый элемент.</param>
		/// <returns>True - если удалось удалить. False в обратном случае.</returns>
		/// <exception cref="ArgumentNullException">Пустое значение.</exception>
		public bool Remove(T item)
		{
			Validation.CheckNull(item);

			for (var i = 0; i < _length; i++)
			{
				if (Equals(array[i], item))
				{
					for (var j = i; j < _length - 1; j++)
					{
						array[j] = array[j + 1];
					}

					array[_length--] = default;

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Копирование <paramref name="array"/> в новый, с ёмкостью в 2 раза больше.
		/// </summary>
		/// <param name="array">Копируемый массив.</param>
		/// <returns>Массив.</returns>
		private T[] Copy()
		{
			var temp = new T[array.Length * 2];

			for (var i = 0; i < _length; ++i)
			{
				temp[i] = array[i];
			}

			return temp;
		}

		/// <summary>
		/// Добавляет ёмкость в <paramref name="array"/>, если он заполнился.
		/// </summary>
		/// <param name="array">Проверяемый массив.</param>
		private void AddCapacity()
		{
			if (_length == array.Length)
			{
				array = Copy();
			}
		}

		/// <summary>
		/// Сравнивает на равенство с <paramref name="objectTwo"/>.
		/// </summary>
		/// <param name="objectTwo">Другой объект.</param>
		/// <returns>True - если объекты равны. False - в обратном случае.</returns>
		public bool Equals(T objectTwo) => GetHashCode() == objectTwo.GetHashCode();

		/// <summary>
		/// Сравнивает на равенство с <paramref name="objectTwo"/>.
		/// </summary>
		/// <param name="objectTwo">Другой объект.</param>
		/// <returns>True - если объекты равны. False - в обратном случае.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			var arrayObject = obj as DynamicArray<T>;

			if (arrayObject == null)
			{
				return false;
			}

			return Equals(arrayObject);
		}

		/// <summary>
		/// Сравнивает динамический массив с <paramref name="array"/>.
		/// </summary>
		/// <param name="array">Другой динамический массив.</param>
		/// <returns>True - если объекты равны. False - в обратном случае.</returns>
		public bool Equals(DynamicArray<T> array)
		{
			if (_length != array.GetLength)
			{
				return false;
			}

			for (var i = 0; i < _length; i++)
			{
				if (this.array[i].GetHashCode() != array[i].GetHashCode())
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Получение хэш-кода массива.
		/// </summary>
		/// <returns>Хэш-код.</returns>
		public override int GetHashCode()
		{
			return array.GetHashCode();
		}

		/// <summary>
		/// Получает строковое представление элементов массива.
		/// </summary>
		/// <returns>Строку.</returns>
		public override string ToString()
		{
			if (_length == 0)
			{
				return "Array is empty.\n";
			}

			var info = new StringBuilder();

			for (var i = 0; i < _length - 1; i++)
			{
				info.Append($"{array[i]} ");
			}

			info.Append($"{array[_length - 1]}.\n");

			return info.ToString();
		}
		#endregion


		#region Операторы.

		/// <summary>
		/// Оператор сравнения равенства <paramref name="objectOne"/> с <paramref name="objectTwo"/>.
		/// </summary>
		/// <param name="objectOne">Объект динамического массива.</param>
		/// <param name="objectTwo">Объект, с которым сравниваем.</param>
		/// <returns>True - если объекты равны. False - в обратном случае.</returns>
		public static bool operator ==(DynamicArray<T> objectOne, T objectTwo) => objectOne.GetHashCode() == objectTwo.GetHashCode();

		/// <summary>
		/// Оператор сравнения равенства <paramref name="objectOne"/> с <paramref name="objectTwo"/>.
		/// </summary>
		/// <param name="objectOne">Объект динамического массива.</param>
		/// <param name="objectTwo">Объект, с которым сравниваем.</param>
		/// <returns>True - если объекты не равны. False - в обратном случае.</returns>
		public static bool operator !=(DynamicArray<T> objectOne, T objectTwo) => objectOne.GetHashCode() != objectTwo.GetHashCode();

		/// <summary>
		/// Оператор сравнения равенства <paramref name="objectOne"/> с <paramref name="objectTwo"/>.
		/// </summary>
		/// <param name="objectOne">Объект динамического массива.</param>
		/// <param name="objectTwo">Другой динамический массив.</param>
		/// <returns>True - если объекты равны. False - в обратном случае.</returns>
		public static bool operator ==(DynamicArray<T> objectOne, DynamicArray<T> objectTwo) => objectOne.Equals(objectTwo);

		/// <summary>
		/// Оператор сравнения равенства <paramref name="objectOne"/> с <paramref name="objectTwo"/>.
		/// </summary>
		/// <param name="objectOne">Объект динамического массива.</param>
		/// <param name="objectTwo">Другой динамический массив.</param>
		/// <returns>True - если объекты не равны. False - в обратном случае.</returns>
		public static bool operator !=(DynamicArray<T> objectOne, DynamicArray<T> objectTwo) => !objectOne.Equals(objectTwo);

		/// <summary>
		/// Индексатор.
		/// </summary>
		/// <param name="i">Индекс.</param>
		/// <returns>Элемент массива.</returns>
		public T this[int i]
		{
			get
			{
				ValidationHelper.CheckRangeValue(i, 0, _length - 1);
				return array[i];
			}
			set
			{
				ValidationHelper.CheckRangeValue(i, 0, _length - 1);
				array[i] = value;
			}
		}

		#endregion

		#region Интерфейсы.
		/// <summary>
		/// Создает набор элементов из <paramref name="array"/>.
		/// </summary>
		/// <param name="array">Получаемый массив.</param>
		/// <returns>Коллекцию.</returns>
		public IEnumerable<T> GetEnumerator()
		{
			for (var i = 0; i < _length; ++i)
			{
				yield return array[i];
			}
		}

		/// <summary>
		/// Реализация интерфейса.
		/// </summary>
		/// <returns>Коллекцию.</returns>
		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return (IEnumerator<T>)GetEnumerator();
		}

		/// <summary>
		/// Реализация интерфейса.
		/// </summary>
		/// <returns>Коллекцию.</returns>
		/// <exception cref="NotImplementedException">Интерфейс не имплементирован.</exception>
		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Устанавливает позицию индекса на следующий элемент.
		/// </summary>
		/// <returns>True - если получилось переместить, False - в обратном случае.</returns>
		public bool MoveNext()
		{
			_position++;

			return _position < _length;
		}

		/// <summary>
		/// Устанавливает позицию на начало.
		/// </summary>
		public void Reset()
		{
			_position = -1;
		}

		/// <summary>
		/// Имплементация интерфейса.
		/// </summary>
		void IDisposable.Dispose()
		{
		}

		/// <summary>
		/// Имплементация интерфейса.
		/// </summary>
		T IEnumerator<T>.Current => array[_position];

		/// <summary>
		/// Имплементация интерфейса.
		/// </summary>
		public DynamicArray<T> Current
		{
			get
			{
				ValidationHelper.CheckRangeValue(Convert.ToInt32(Current), 0, _length - 1);

				return Current;
			}
		}

		/// <summary>
		/// Имплементация интерфейса.
		/// </summary>
		object IEnumerator.Current => Current;

		/// <summary>
		/// Сравнивает с <paramref name="obj"/>.
		/// </summary>
		/// <param name="obj">Другой объект.</param>
		/// <returns>Разницу.</returns>
		/// <exception cref="ArgumentException">Неверный тип объекта.</exception>
		public int CompareTo(T obj)
		{
			if (obj == null)
			{
				return 1;
			}

			var otherObject = obj as DynamicArray<T>;

			if (otherObject != null)
			{
				return this.GetHashCode().CompareTo(otherObject.GetHashCode());
			}

			throw new ArgumentException("Неверный тип объекта");
		}


		#endregion
	}
}
