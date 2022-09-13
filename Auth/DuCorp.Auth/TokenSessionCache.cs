using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using System;
using System.Threading;
using System.Web;

namespace DuCorp.Auth
{
   public class TokenSessionCache
   {
      private static ReaderWriterLockSlim SessionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
      string Key = string.Empty;
      string CacheId = string.Empty;
      HttpContext _httpContext = null;
      private readonly MemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
      private readonly DateTimeOffset _cacheDuration = DateTimeOffset.Now.AddHours(12);

      TokenCache _cache = new TokenCache();

      public TokenSessionCache(string key, HttpContext httpcontext = null)
      {
         Key = key;
         CacheId = Key + "_TokenCache";
         _httpContext = httpcontext;
         Load();
      }

      public TokenCache GetTokenCacheInstance()
      {
         _cache.SetBeforeAccess(BeforeAccessNotification);
         _cache.SetAfterAccess(AfterAccessNotification);
         Load();
         return _cache;
      }

      public void SaveUserStateValue(string state)
      {
         SessionLock.EnterWriteLock();

         if (null != _httpContext)
            _httpContext.Session.SetString(CacheId + "_state", state);
         else
            _memoryCache.Set(CacheId + "_state", state, _cacheDuration);

         SessionLock.ExitWriteLock();
      }
      public string ReadUserStateValue()
      {
         string state = string.Empty;
         SessionLock.EnterReadLock();

         if (null != _httpContext)
            state = _httpContext.Session.GetString(CacheId + "_state");
         else
            state = (string)_memoryCache.Get(CacheId + "_state");

         SessionLock.ExitReadLock();

         return state;
      }
      public void Load()
      {
         SessionLock.EnterReadLock();

         byte[] arr = null;

         if (null != _httpContext)
            arr = _httpContext.Session.Get(CacheId);
         else
            arr = (byte[])_memoryCache.Get(CacheId);

         if (null != arr)
            _cache.Deserialize(arr);

         SessionLock.ExitReadLock();
      }

      public void Persist()
      {
         SessionLock.EnterWriteLock();

         if (null != _httpContext)
            _httpContext.Session.Set(CacheId, _cache.Serialize());
         else
            _memoryCache.Set<byte[]>(CacheId, _cache.Serialize(), _cacheDuration);

         SessionLock.ExitWriteLock();
      }

      void BeforeAccessNotification(TokenCacheNotificationArgs args)
      {
         Load();
      }

      void AfterAccessNotification(TokenCacheNotificationArgs args)
      {
         if (args.HasStateChanged)
         {
            Persist();
         }
      }
   }
}
