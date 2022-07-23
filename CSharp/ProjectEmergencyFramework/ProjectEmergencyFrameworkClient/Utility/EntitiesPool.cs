using CitizenFX.Core.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Utility
{
    public abstract class EntitiesPool<T> : IEnumerable<T>, IEnumerable
    {
        private Hash FindFirst { get; set; }
        private Hash FindNext { get; set; }
        private Hash EndFind { get; set; }

        public EntitiesPool(uint findFirst, uint findNext, uint endFind)
        {
            FindFirst = (Hash)findFirst;
            FindNext = (Hash)findNext;
            EndFind = (Hash)endFind;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<T> GetEnumerator()
        {
            var foundEntity = new OutputArgument();

            int handle = Function.Call<int>(FindFirst, foundEntity);

            if (handle == -1)
            {
                yield break;
            }

            int entityHandle;
            var hasMore = true;

            while (hasMore)
            {
                entityHandle = foundEntity.GetResult<int>();

                if (entityHandle == -1)
                {
                    continue;
                }

                if (CastSilently(entityHandle, out T entity))
                {
                    yield return entity;
                }

                hasMore = Function.Call<bool>(FindNext, handle, foundEntity);
            }

            Function.Call(EndFind, handle);
        }

        protected bool CastSilently(int handle, out T entity)
        {
            try
            {
                entity = Cast(handle);
                return true;
            }
            catch (Exception e)
            {
                entity = default(T);
                return false;
            }
        }

        protected abstract T Cast(int hanlde);
    }
}
