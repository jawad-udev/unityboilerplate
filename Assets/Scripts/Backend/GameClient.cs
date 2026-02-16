using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend
{
	public class GameClient : SingletonMonobehaviour<GameClient>
	{
		/// <summary>
		/// All URLs and tokens are loaded from the GameConfig ScriptableObject.
		/// Edit the asset at Resources/Config/GameConfig to change values.
		/// </summary>
		private GameConfig Config => GameConfig.Instance;

		public string _hostUrl => Config.hostUrl;
		public string _hostAudioUrl => Config.hostAudioUrl;
		public string AppToken => Config.appToken;
		public string cloudfrontUrl => Config.cloudfrontUrl;

		/// <summary>
		/// Runtime access token updated by PlayerService on login/refresh.
		/// API classes read this to attach Bearer auth headers.
		/// </summary>
		public string AccessToken { get; set; }
		[SerializeField]
		private List<RequestMessage> _requests;

		public void DispatchRequest<T>(
			RequestMessage request,
			Action<ResponseMessage<T>> listener,
			bool loadFromCache = false,
			int ttlSeconds = -1,
			CacheType cacheType = CacheType.Session)
		{
			string cacheKey = request._requestPath + "_" + request._body;

			// --------------------------
			// Load from Cache
			// --------------------------
			if (loadFromCache && ApiCacheManager.TryLoad(cacheKey, cacheType, out string cachedJson))
			{
				CachedResponse cached = JsonUtility.FromJson<CachedResponse>(cachedJson);

				ResponseMessage<T> resp = new ResponseMessage<T>();

				// Restore entity
				resp._entity = GameSerializer.Deserialize<T>(cached.entityJson);

				// Restore all server values
				resp._status = cached.status;
				resp._message = cached.message;
				resp._error = cached.error;
				resp._payload = cached.payload;
				resp._statusCode = cached.statusCode;
				resp._access = cached.access;
				resp._refresh = cached.refresh;

				// Restore request (optional)
				resp._request = JsonUtility.FromJson<RequestMessage>(cached.requestJson);

				listener(resp);
				return;
			}

			// --------------------------
			// Server Call
			// --------------------------
			DispatchRequestServer<T>(request, (response) =>
			{
				if (response._status)
				{
					CachedResponse cacheObj = new CachedResponse
					{
						entityJson = GameSerializer.Serialize(response._entity),

						status = response._status,
						message = response._message,
						error = response._error,
						payload = response._payload,
						statusCode = response._statusCode,
						access = response._access,
						refresh = response._refresh,

						requestJson = JsonUtility.ToJson(response._request)
					};

					string json = JsonUtility.ToJson(cacheObj);
					ApiCacheManager.Save(cacheKey, json, cacheType, ttlSeconds);
				}

				listener(response);
			});
		}

		public void DispatchRequestServer<T>(RequestMessage request, Action<ResponseMessage<T>> listener)
		{
			_requests.Add(request);

			GameObject requestMakerObject = new GameObject("web request");
			requestMakerObject.transform.SetParent(transform);
			RequestDispatcher requestMaker = requestMakerObject.AddComponent<RequestDispatcher>();

			requestMaker.Request(request, abjadResponse =>
			{

				listener(GenericResponseFromResponse<T>(abjadResponse));
				RequestMessage req = _requests.Find(areq => areq == abjadResponse._request);
				if (req != null)
					_requests.Remove(req);
			});
		}

		private static ResponseMessage<T> GenericResponseFromResponse<T>(GameResponse response)
		{
			ResponseMessage<T> resp = new ResponseMessage<T>();

			if (response.data == null || response.data.ToString() == "{}")
			{
				resp._entity = Activator.CreateInstance<T>();
			}
			else if (response.data != null)
			{
				if (typeof(T) == typeof(string))
					resp._entity = (T)response.data;
				else
					resp._entity = GameSerializer.Deserialize<T>(response.data.ToString());
			}

			resp._status = response.status;
			resp._message = response.message;
			resp._error = response.error;
			resp._payload = response._payload;
			resp._request = response._request;
			resp._statusCode = response.statusCode;
			resp._access = response.access;
			resp._refresh = response.refresh;
			return resp;
		}


	}
}