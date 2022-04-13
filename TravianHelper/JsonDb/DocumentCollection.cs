using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TravianHelper.JsonDb
{
    public class DocumentCollection<T>// : IDocumentCollection<T>
    {
        private readonly string _path;
        private readonly string _idField;
        private readonly Lazy<List<T>> _data;
        private readonly Func<string, Func<List<T>, T>, bool, Task<T>> _commit;
        private readonly Func<T, T> _insertConvert;
        private readonly Func<T> _createNewInstance;

        public DocumentCollection(Func<string, Func<List<T>, T>, bool, Task<T>> commit, Lazy<List<T>> data, string path, string idField, Func<T, T> insertConvert, Func<T> createNewInstance)
        {
            _path = path;
            _idField = idField;
            _commit = commit;
            _data = data;
            _insertConvert = insertConvert;
            _createNewInstance = createNewInstance;
        }

        public int Count => _data.Value.Count;

        public IEnumerable<T> AsQueryable() => _data.Value.AsQueryable();

        public IEnumerable<T> Find(Predicate<T> query) => _data.Value.Where(t => query(t));

        public IEnumerable<T> Find(string text, bool caseSensitive = false) => _data.Value.Where(t => ObjectExtensions.FullTextSearch(t, text, caseSensitive));

        public dynamic GetNextIdValue() => GetNextIdValue(_data.Value);

        public T Insert(T item)
        {
            T UpdateAction(List<T> data)
            {
                var itemToInsert = GetItemToInsert(GetNextIdValue(data, item), item, _insertConvert);
                data.Add(itemToInsert);
                return itemToInsert;
            }

            ExecuteLocked(UpdateAction, _data.Value);

            return _commit(_path, UpdateAction, false).Result;
        }

        public bool Replace(Predicate<T> filter, T item, bool upsert = false)
        {
            T UpdateAction(List<T> data)
            {
                var matches = data.Where(e => filter(e));

                if (!matches.Any())
                {
                    if (!upsert)
                        return default;

                    var newItem = _createNewInstance();
                    ObjectExtensions.CopyProperties(item, newItem);
                    var insertItem = _insertConvert(newItem);
                    data.Add(insertItem);
                    return insertItem;
                }

                var index = data.IndexOf(matches.First());
                data[index] = item;

                return item;
            }

            if (ExecuteLocked(UpdateAction, _data.Value) == default)
                return default;

            return _commit(_path, UpdateAction, false).Result;
        }

        public bool Replace(dynamic id, T item, bool upsert = false) => Replace(GetFilterPredicate(id), item, upsert);
        

        public bool Update(Predicate<T> filter, dynamic item)
        {
            bool UpdateAction(List<T> data)
            {
                var matches = data.Where(e => filter(e));

                if (!matches.Any())
                    return false;

                var toUpdate = matches.First();
                ObjectExtensions.CopyProperties(item, toUpdate);

                return true;
            }

            if (!ExecuteLocked(UpdateAction, _data.Value))
                return false;

            return _commit(_path, UpdateAction, false).Result;
        }

        public bool Update(dynamic id, dynamic item) => Update(GetFilterPredicate(id), item);

        public bool Delete(Predicate<T> filter)
        {
            bool UpdateAction(List<T> data)
            {
                var remove = data.FirstOrDefault(e => filter(e));

                if (remove == null)
                    return false;

                return data.Remove(remove);
            }

            if (!ExecuteLocked(UpdateAction, _data.Value))
                return false;

            return _commit(_path, UpdateAction, false).Result;
        }

        public bool Delete(dynamic id) => Delete(GetFilterPredicate(id));

        private T ExecuteLocked(Func<List<T>, T> func, List<T> data)
        {
            lock (data)
            {
                return func(data);
            }
        }

        private dynamic GetNextIdValue(List<T> data, T item = default(T))
        {
            if (!data.Any())
            {
                if (item != null)
                {
                    dynamic primaryKeyValue = GetFieldValue(item, _idField);

                    if (primaryKeyValue != null)
                    {
                        // Without casting dynamic will return Int64 for int
                        if (primaryKeyValue is Int64)
                        {
                            return (int)1;
                        }

                        return primaryKeyValue;
                    }
                }

                return ObjectExtensions.GetDefaultValue<T>(_idField);
            }

            var lastItem = data.Last();

            dynamic keyValue = GetFieldValue(lastItem, _idField);

            if (keyValue == null)
                return null;

            if (keyValue is Int64)
                return (int)keyValue + 1;

            return ParseNextIntegerToKeyValue(keyValue.ToString());
        }

        private dynamic GetFieldValue(T item, string fieldName)
        {
            var expando = JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(item), new ExpandoObjectConverter());
            // Problem here is if we have typed data with upper camel case properties but lower camel case in JSON, so need to use OrdinalIgnoreCase string comparer
            var expandoAsIgnoreCase = new Dictionary<string, dynamic>(expando, StringComparer.OrdinalIgnoreCase);

            if (!expandoAsIgnoreCase.ContainsKey(fieldName))
                return null;

            return expandoAsIgnoreCase[fieldName];
        }

        private string ParseNextIntegerToKeyValue(string input)
        {
            int nextInt = 0;

            if (input == null)
                return $"{nextInt}";

            var chars = input.ToArray().Reverse().TakeWhile(char.IsNumber).Reverse().ToArray();

            if (chars.Count() == 0)
                return $"{input}{nextInt}";

            input = input.Substring(0, input.Length - chars.Count());

            if (int.TryParse(new string(chars), out nextInt))
                nextInt += 1;

            return $"{input}{nextInt}";
        }

        private T GetItemToInsert(dynamic insertId, T item, Func<T, T> insertConvert)
        {
            if (insertId == null)
                return insertConvert(item);

            // If item to be inserted is an anonymous type and it is missing the id-field, then add new id-field
            // If it has an id-field, then we trust that user know what value he wants to insert
            if (ObjectExtensions.IsAnonymousType(item) && ObjectExtensions.HasField(item, _idField) == false)
            {
                var toReturn = insertConvert(item);
                ObjectExtensions.AddDataToField(toReturn, _idField, insertId);
                return toReturn;
            }
            else
            {
                ObjectExtensions.AddDataToField(item, _idField, insertId);
                return insertConvert(item);
            }
        }

        private Predicate<T> GetFilterPredicate(dynamic id) => (e => ObjectExtensions.GetFieldValue(e, _idField) == id);
    }
}