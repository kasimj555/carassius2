using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using PNUnfolding.Model;

namespace PNUnfolding
{
    /// <summary>
    /// Условный словарь для узлов. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class VDictionary<T> where T : PetriNetNode
    {
        /// <summary>
        /// Настоящий словарь.
        /// </summary>
        private Dictionary<T, T> PetriDictionary;

        /// <summary>
        /// Добавление нового созданного ключа (новый узел по старому узлу) и старого узла в словарь.
        /// </summary>
        /// <param name="node"></param>
        public void Add(T node)
        {
            T netNew;
            if (typeof(T) == typeof(VPlace))
            {
                netNew = VPlace.Create(75 * Unfolding.Table, 75 * Unfolding.Row) as T;

            }
            else
            {
                netNew = VTransition.Create(75 * Unfolding.Table, 75 * Unfolding.Row) as T;
            }
            netNew.Label = node.Id;
            PetriDictionary.Add(netNew, node);
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        public VDictionary()
        {
            PetriDictionary = new Dictionary<T, T>();
        }

        /// <summary>
        /// Метод возвращает последный элемент в словаре.
        /// </summary>
        /// <returns></returns>
        public T Last()
        {
            return PetriDictionary.Last().Key;
        }

        /// <summary>
        /// Индексатор для словаря.
        /// </summary>
        /// <param name="New"></param>
        /// <returns></returns>
        public T this[T New]
        {
            get { return PetriDictionary[New]; }
        }

        /// <summary>
        /// Свойство возвращающее ключи словаря.
        /// </summary>
        public Dictionary<T, T>.KeyCollection Keys
        {
            get { return PetriDictionary.Keys; }
        }

        /// <summary>
        /// Метод определяющий содержится данный узел в значениях словаря.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Exist(T node)
        {
            return PetriDictionary.ContainsValue(node);
        }

        /// <summary>
        /// Метод возвращающий первый ключ значение которого равно данному.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T GetKey(T value)
        {
            foreach(var pair in PetriDictionary)
            {
                if (pair.Value == value)
                    return pair.Key;
            }
            throw new Exception("Something wrong");
        }

        /// <summary>
        /// Метод возвращающий список ключей значения которых равны данному.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<T> GetKeys(T value)
        {
            List<T> list=new List<T>();
            foreach (var pair in PetriDictionary)
            {
                if (pair.Value == value)
                    list.Add(pair.Key);
            }
            return list;
        }

        /// <summary>
        /// Метод для очистки словаря.
        /// </summary>
        public void Clear()
        {
            PetriDictionary.Clear();
        }
    }
}
