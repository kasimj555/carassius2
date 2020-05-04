using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;
using PNUnfolding.Model;

namespace PNUnfolding
{
    public class VDictionary<T> where T : PetriNetNode
    {
        private Dictionary<T, T> PetriDictionary;

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
            PetriDictionary.Add(netNew, node);
        }

        public VDictionary()
        {
            PetriDictionary = new Dictionary<T, T>();
        }

        public T Last()
        {
            return PetriDictionary.Last().Key;
        }

        public T this[T New]
        {
            get { return PetriDictionary[New]; }
        }

        public Dictionary<T, T>.KeyCollection Keys
        {
            get { return PetriDictionary.Keys; }
        }

        public bool Exist(T node)
        {
            return PetriDictionary.ContainsValue(node);
        }

        public T GetKey(T value)
        {
            foreach(var pair in PetriDictionary)
            {
                if (pair.Value == value)
                    return pair.Key;
            }
            throw new Exception("Something wrong");
        }

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

        public void Clear()
        {
            PetriDictionary.Clear();
        }
    }
}
